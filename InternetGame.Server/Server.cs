using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InternetGame.Library;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InternetGame.Server
{
    class Server
    {
        private static List<Player> players;
        private NetPeerConfiguration config;
        private NetServer server;
        int intPlayer = 0;
        int startPosX = 0;
        int startPosY = 0;

        public Server()
        {
            players = new List<Player>();
            config = new NetPeerConfiguration("networkGame") { Port = 1337 };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            server = new NetServer(config);
        }

        public void Run()
        {
            server.Start();
            Console.WriteLine("Server started...");
            while (true)
            {
                NetIncomingMessage inc;
                //If no message is to be handled, continue
                if ((inc = server.ReadMessage()) == null) continue;

                //If there is a message to be handled. This happens
                switch (inc.MessageType)
                {
                    //Ex, a disconnect or connect
                    case NetIncomingMessageType.StatusChanged:
                        break;

                        //Checks if joining player has login approval
                    case NetIncomingMessageType.ConnectionApproval:
                            ConnectionApproval(inc);
                        break;

                        //If the message has the type.Data this happens it sends the inc message to the Data method
                    case NetIncomingMessageType.Data:
                        Data(inc);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();

                }
            }
        }

        //When an inc comes, this checks if it's 
        private void Data(NetIncomingMessage inc)
        {
            //Inc here is a keypress/input sent in a packettype with a byte.
            var packetType = (PacketType)inc.ReadByte();

            switch (packetType)
            {
                case PacketType.Input:
                    Input(inc);
                    //Sends the input to Method input, input is in the inc message.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();

            }
        }
        
        //Recieves the input
        private void Input(NetIncomingMessage inc)
        {
            Console.WriteLine("Recieved new input");

            var key = (Keys)inc.ReadByte();
            var name = inc.ReadString();
            var player = players.FirstOrDefault(p => p.Name == name);
            if (player == null)
            {
                Console.WriteLine("Could not find player with name {0}", name);
                return;
            }
                    switch(key)
                    {
                        case Keys.Down:
                    if(player.YPosition <= 480-20)
                            player.YPosition += 5;
                            break;
                        case Keys.Up:
                    if (player.YPosition >= 0)
                    {
                        player.YPosition -= 5;
                    }
                            break;
                        case Keys.Left:
                    if (player.XPosition >= 0)
                    {
                        player.XPosition -= 5;
                    }
                            break;
                        case Keys.Right:
                    if (player.XPosition <= 800 - 20)
                    {
                        player.XPosition += 5;
                    }
                            break;
                case Keys.Space:
                    player.XPosition += 2;
                    break;
                    }
            //
            SendPlayerPosition(player, inc);
            }
        //Here it checks what byte the inc has. If it's PacketType.Login ->
        //then it creates a new player witht the information in inc.
        //Then it sends an approval of the connection to new connection
        //Some stuff
        //It counts all the players already in the game and begins to write all the properties of them in the outmsg
        //When all players are counted it sends the message with the info to the new player
        //Then it calls sendplayerinfo and sends player, inc to it.
        private void ConnectionApproval(NetIncomingMessage inc)
        {
            Console.WriteLine("New connection...");
            var data = inc.ReadByte();
            if (data == (byte)PacketType.Login)
            {
                Console.WriteLine("..connection accepted.");
                var player = CreatePlayer(inc);
                inc.SenderConnection.Approve();
                var outmsg = server.CreateMessage();
                outmsg.Write((byte)PacketType.Login);
                System.Threading.Thread.Sleep(1000);
                outmsg.Write(true);
                outmsg.Write(players.Count);
                for (int n = 0; n < players.Count; n++)
                {
                    outmsg.WriteAllProperties(players[n]);
                }
                server.SendMessage(outmsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                SendPlayerPosition(player, inc);
            }
            else
            {
                inc.SenderConnection.Deny("Diddn't send correct information");
            }
        }

        //A new player is created and all things that the player needs is given here
        //A connection, a name and start positions, then it adds that player to the list and returns player.
        private Player CreatePlayer(NetIncomingMessage inc)
        {
            var random = new Random();
            var player = new Player
            {
                Connection = inc.SenderConnection,
                Name = inc.ReadString(),
                XPosition = startPosX,
                YPosition = startPosY
            };
            players.Add(player);
            return player;
        }

        private void SendPlayerPosition(Player player, NetIncomingMessage inc)
        {
            //Creates a message containing the new playerposition and all properties of player.
            //Then sends it all to every player so that they know where that player is.
            Console.WriteLine("Sending out new player position");
            var outmessage = server.CreateMessage();
            outmessage.Write((byte)PacketType.PlayerPosition);
            outmessage.WriteAllProperties(player);
            server.SendToAll(outmessage, NetDeliveryMethod.ReliableOrdered);
        }

        private void SendFullPlayerList()
        {
            Console.WriteLine("Sending out full player list");
            var outmessage = server.CreateMessage();
            outmessage.Write((byte)PacketType.AllPlayers);
            outmessage.Write(players.Count);
            foreach(var player in players)
            {
                outmessage.WriteAllProperties(player);
            }
            server.SendToAll(outmessage, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
