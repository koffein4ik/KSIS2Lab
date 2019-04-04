using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServer;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string request = null;
            Console.WriteLine("Enter your request");
            request = Console.ReadLine();
            Client client1 = new Client("127.0.0.1", 11000, "D:\\Client\\");
            while (request != "quit")
            {
                client1.SendRequest(request);
                Console.WriteLine(client1.getAnswer());
                Console.WriteLine("Enter your request");
                request = Console.ReadLine();
            }
        }
    }
}
