using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using InternetGame.Library;

namespace InternetGame.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Run();
        }
    }
}
