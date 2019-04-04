using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ClientServer
{
    public class Client
    {
        public IPAddress ipAddr;
        public IPEndPoint ipEndPoint;
        public static string storagepath;
        public Socket sender;

        public Client(string ip, int ipendpoint, string path)
        {
            ipAddr = IPAddress.Parse(ip);
            ipEndPoint = new IPEndPoint(ipAddr, ipendpoint);
            storagepath = path;
        }

        public void SendRequest(string request)
        {
            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(ipEndPoint);
            byte[] msg = Encoding.UTF8.GetBytes(request);
            sender.Send(msg);
        }

        public string getAnswer()
        {
            byte[] bytes = new byte[1024];
            string answer = "";
            int recbytes = 0;
            do
            {
                recbytes = sender.Receive(bytes);
                answer += Encoding.UTF8.GetString(bytes, 0, recbytes);
            }
            while (sender.Available > 0);

            //answer = Encoding.UTF8.GetString(bytes);
            string answ = answer;
            if (answer.Contains("Yes"))
            {
                answer = answer.TrimEnd('\0');
                answer = answer.Remove(answer.Length - 4);
                if (answer.Contains("Super"))
                {
                    answer = answer.Insert(6, "Resweight ");
                }
                else
                {
                    answer = answer.Insert(0, "Resweight ");
                }
                using (FileStream fstream = new FileStream(storagepath + answer + ".csv", FileMode.OpenOrCreate))
                {
                    int bytesRecieved = 0;
                    var arr = new byte[1024];
                    do
                    {
                        bytesRecieved = sender.Receive(arr, arr.Length, 0);
                        fstream.Write(arr, 0, bytesRecieved);
                    }
                    while (sender.Available > 0);
                }
                //fstream.Close();
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return answ;
            }
            else
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return answer;
            }

        }
    }
}
