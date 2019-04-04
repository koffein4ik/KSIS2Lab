using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;

namespace ClientServer
{
    public class Server
    {
        public IPAddress ipAddr;
        public IPEndPoint ipEndPoint;
        static string storagepath;

        public Server(string ip, int ipendpoint, string path)
        {
            ipAddr = IPAddress.Parse(ip);
            ipEndPoint = new IPEndPoint(ipAddr, ipendpoint);
            storagepath = path;
        }

        public void Start()
        {
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);
                while (true)
                {
                    Socket handler = sListener.Accept();
                    string request = "";
                    string reply = "";
                    byte[] bytes = new byte[1024];
                    int bytesRecieved = handler.Receive(bytes);
                    string answer = "";
                    request = Encoding.UTF8.GetString(bytes, 0, bytesRecieved);
                    answer = getanswer(request);
                    if (request.Contains("Super"))
                    {
                        answer = "Super " + answer;
                    }
                    if (answer != "Incorrect command")
                    {

                        if (System.IO.File.Exists(storagepath + answer + ".csv"))
                        {
                            //byte[] msg1 = File.ReadAllBytes(storagepath + request + ".csv");
                            reply = "";
                            reply = answer;
                            reply = answer + " Yes";
                            bytes = Encoding.UTF8.GetBytes(reply);
                            handler.Send(bytes);
                            handler.SendFile(storagepath + answer + ".csv");
                        }
                        else
                        {
                            reply = request + " No";
                            bytes = Encoding.UTF8.GetBytes(reply);
                            handler.Send(bytes);
                        }
                    }
                    else
                    {
                        reply = "Incorrect command";
                        bytes = Encoding.UTF8.GetBytes(reply);
                        handler.Send(bytes);
                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception EX)
            {

            }
        }

        public static string getanswer(string request)
        {
            string answer = request;
            char[] charsToTrim = { ' ' };
            answer.Trim(charsToTrim);
            Regex template = new Regex(@"(Super)?Resweight\s([0-9]{4}-[0-9]{2}-[0-9]{2})|(End)$", RegexOptions.IgnoreCase);

            if (template.IsMatch(answer))
            {
                string str;
                str = answer.ToLower();
                if (str.Contains("end"))
                {
                    if (str.Contains("super"))
                    {
                        answer = GetLastFile(true);
                    }
                    else
                    {
                        answer = GetLastFile(false);
                    }
                    Regex datetime = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{2}");
                    Match firstmatch = datetime.Match(answer);
                    answer = firstmatch.ToString();
                    return answer;
                }
                else
                {
                    Regex datetime = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{2}");
                    Match firstmatch = datetime.Match(answer);
                    answer = firstmatch.ToString();
                    return answer;
                }
            }
            else
            {
                return "Incorrect command";
            }

        }

        public static string GetLastFile(Boolean super)
        {
            string[] files = Directory.GetFiles(storagepath);
            Regex datetime = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{2}");
            string currdate;
            string maxdate;
            Match firstmatch = datetime.Match(files[0]);
            maxdate = firstmatch.ToString();
            DateTime latestdate;
            DateTime currentdate;
            latestdate = DateTime.Parse(maxdate);
            if (super)
            {

                for (int i = 1; i < files.Length; i++)
                {
                    if (files[i].Contains("Super"))
                    {
                        firstmatch = datetime.Match(files[i]);
                        currdate = firstmatch.ToString();
                        currentdate = DateTime.Parse(currdate);
                        if (DateTime.Compare(latestdate, currentdate) < 0)
                        {
                            latestdate = currentdate;
                            maxdate = files[i];
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i < files.Length; i++)
                {
                    if (!(files[i].Contains("Super")))
                    {
                        firstmatch = datetime.Match(files[i]);
                        currdate = firstmatch.ToString();
                        currentdate = DateTime.Parse(currdate);
                        if (DateTime.Compare(latestdate, currentdate) < 0)
                        {
                            latestdate = currentdate;
                            maxdate = files[i];
                        }

                    }
                }
            }
            return maxdate;
        }
    }
}
