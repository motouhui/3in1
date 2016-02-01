using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO;
using System.Net.Sockets;
using TestFGameServer;
using System.Diagnostics;

public class NewServer : MonoBehaviour
{


     public enum Command {
            MESSAGE = 1,
            JOININ = 2,
            LEAVE = 3,
            DOACTION = 4
        }

        public enum Status
        {
            IDLE = 0,
            PASS = 1,
            ATTACK = 2,
            CRITICAL_ATTACK = 3,
            DEFEND = 4,
            DODGE = 5
        }

        TcpClient tcp = null;
        string host = null;
        int port = 0;
        
        public bool Connected = false;

        public NewServer(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        private void Connect(string host, int port)
        {
            tcp = new TcpClient(host, port);
        }

        private void SendCommand(Command command, Status status)
        {
            var stream = tcp.GetStream();

            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write((Int32)1);
            writer.Write((Int32)1);
            writer.Write((Int32)command);
            if (command == Command.DOACTION)
            {
                writer.Write((Int32)4);
                writer.Write((Int32)status);
            }
            else
            {
                writer.Write((Int32)0);
            }
        }

        private MemoryStream buffer = new MemoryStream();
        private byte[] b = new byte[2048];

        private void ReceiveMessage(IAsyncResult ar)
        {
            int bytesRead;
            try
            {
                bytesRead = tcp.GetStream().EndRead(ar);
                buffer.Write(b, 0, bytesRead);
                buffer.Seek(0, SeekOrigin.Begin);
                while (buffer.Length > 4 * 4)
                {
                    BinaryReader reader = new BinaryReader(buffer);
                    reader.ReadInt32(); // magic
                    reader.ReadInt32(); // id
                    reader.ReadInt32(); // command
                    int length = reader.ReadInt32(); // length

                    if (buffer.Length - buffer.Position >= length)
                    {
                        buffer.Seek(-4 * 4, SeekOrigin.Current);
                        Game game = Game.Parse(buffer);
                        action(game);
                        //buffer.CopyTo(t);
                        byte[] tempData;
                        if (buffer.Length != buffer.Position)
                        {
                            tempData = new byte[buffer.Length - buffer.Position];
                            buffer.Read(tempData, 0, tempData.Length);
                            buffer = new MemoryStream(tempData);
                            buffer.Seek(0, SeekOrigin.Begin);
                        }
                        else
                        {
                            buffer = new MemoryStream();
                        }
                    }
                    else
                    {
                        buffer.Seek(-4 * 4, SeekOrigin.Current);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            tcp.GetStream().BeginRead(b, 0, 2048, ReceiveMessage, null);
        }
        
        public delegate void GameServerAction(Game game);
        public delegate void GameServerConnectedAction(bool connected);

        private GameServerAction action;
        private GameServerConnectedAction connectedaction;
        private Thread thread;

        public void AddGameServerAction(GameServerAction action)
        {
            this.action = action;
        }

        public void AddGameServerConnectedAction(GameServerConnectedAction action)
        {
            this.connectedaction = action;
        }

        public void Start()
        {
            this.thread = new Thread(() =>
            {
                Connect(host, port);
                Connected = true;
                connectedaction(true);

                tcp.GetStream().BeginRead(b, 0, 2048, ReceiveMessage, null);
            });
            this.thread.Start();
        }

        public void Close()
        {
            if (tcp != null)
            {
                tcp.Close();
            }
        }

        public bool Send(Command cmd, Status status)
        {
            if (Connected)
            {
                new Thread(() =>
                {
                    SendCommand(cmd, status);
                }).Start();
                return true;
            }
            return false;
        }
}