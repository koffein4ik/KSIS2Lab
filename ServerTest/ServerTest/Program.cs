using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServer;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Server serv1 = new Server("127.0.0.1", 11000, "D:\\Server\\");
            serv1.Start();
        }
    }
}
