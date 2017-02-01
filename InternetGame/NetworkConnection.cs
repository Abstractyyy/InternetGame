using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using InternetGame.Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InternetGame
{
    class NetworkConnection
    {

        private NetClient client;

        public List<Player> Players { get; set; }

        public string Username { get; set; }

        public bool Active { get; set; }

        public NetworkConnection()
        {
            Players = new List<Player>();
        }

        public bool Start()
        {
            var random = new Random();
            client = new NetClient(new NetPeerConfiguration("networkGame"));
            client.Start();
            Username = "name_" + random.Next(0, 100);
            var outmsg = client.CreateMessage();                        
            outmsg.Write((byte)PacketType.Login);
            outmsg.Write(Username);
            client.Connect("192.168.248.92", 1337, outmsg);
            return EstablishInfo();
        }

        public bool EstablishInfo()
        {
            var time = DateTime.Now;
            NetIncomingMessage inc;

            while (true)
            {
                if (DateTime.Now.Subtract(time).Seconds > 5)
                {
                    return false;
                }
                if ((inc = client.ReadMessage()) == null) continue;

                switch(inc.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        var data = inc.ReadByte();
                        if(data == (byte)PacketType.Login)
                        {
                            Active = inc.ReadBoolean();
                            if (Active)
                            {
                                RecieveAllPlayers(inc);
                                return true;
                            }
                                return false;   
                        }
                            return false;
                        
                }

            }
        }

        public void Update()
        {
            NetIncomingMessage inc;
            while((inc = client.ReadMessage()) != null)
            {
                if (inc.MessageType != NetIncomingMessageType.Data) continue;
                var packageType = (PacketType)inc.ReadByte();
                switch(packageType)
                {
                    case PacketType.PlayerPosition:
                        ReadPlayer(inc);
                        break;

                    case PacketType.AllPlayers:
                        RecieveAllPlayers(inc);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }
        private void RecieveAllPlayers(NetIncomingMessage inc)
        {
            var count = inc.ReadInt32();
            for(int n = 0; n < count; n++)
            {
                ReadPlayer(inc);
            }
        }

        private void ReadPlayer(NetIncomingMessage inc)
        {
            var player = new Player();
            inc.ReadAllProperties(player);
            if (Players.Any(p => p.Name == player.Name))
            {
                var oldPlayer = Players.FirstOrDefault(p => p.Name == player.Name);
                oldPlayer.XPosition = player.XPosition;
                oldPlayer.YPosition = player.YPosition;
            }
            else
            {
                Players.Add(player);
            }
        }

        public void SendInput(Keys key)
        {
            var outmessage = client.CreateMessage();
            outmessage.Write((byte)PacketType.Input);
            outmessage.Write((byte)key);
            outmessage.Write(Username);
            client.SendMessage(outmessage, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
