using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using TextRpg_packet;

namespace Server
{
    class Program
    {
        
        
        static void Main(string[] args)
        {
            Server server = new Server();
            server.ServerStart();
            Console.WriteLine("Hello World!");
        }



    }
    class Server
    {
        const int PORT = 8066;
        private TcpListener m_listener;
        public void ServerStart()
        {
            try
            {
                m_listener = new TcpListener(PORT);
                m_listener.Start();

                while(true)
                {
                    TcpClient hClient = m_listener.AcceptTcpClient();

                    if(hClient.Connected)
                    {
                        Handler handle = new Handler();
                        Thread client=new Thread(handle.handling);
                        client.Start(hClient);
                    }

                }
            }
            catch
            {
                Console.WriteLine("Server Error");
            }
        }
    }
    class Handler
    {
        public NetworkStream m_Stream;
        public bool m_bStop = false;

        private byte[] sendBuffer=new byte[256];
        private byte[] readBuffer=new byte[256];

        TcpClient client_socket;

        public Boss m_boss;
        public Pattern m_pattern;
        

        public void handling(object client_socket)
        {
            this.client_socket = (TcpClient)client_socket;
            m_Stream=this.client_socket.GetStream();
            m_bStop = true;
            int n_read = 0;

            while(m_bStop)
            {
                try
                {
                    n_read = 0;
                    n_read=this.m_Stream.Read(readBuffer,0,256);
                }
                catch
                {
                    this.m_bStop = false;
                    this.m_Stream= null;
                    return;
                }
                try
                {
                    Packet packet = (Packet)Packet.Desserialize(this.readBuffer);

                    switch ((int)packet.Typee)
                    {
                        case (int)Packet_Type.보스:
                            this.m_boss = (Boss)Packet.Desserialize(this.readBuffer);
                            m_boss.value = BossOut(m_boss.value);
                            Packet.Serialize(m_boss).CopyTo(this.sendBuffer, 0);
                            Send();
                            break;
                        case (int)Packet_Type.패턴:
                            this.m_pattern = (Pattern)Packet.Desserialize(this.readBuffer);
                            m_pattern.pattern = PatternOut(m_pattern.chapter, m_pattern.pattern);
                            Packet.Serialize(m_pattern).CopyTo(this.sendBuffer, 0);
                            Send();
                            break;
                        case (int)Packet_Type.종료:
                            this.m_bStop = false;
                            this.m_Stream = null;
                            Console.WriteLine("{0} 종료",client_socket);
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("Error Null");
                    Packet error = new Packet();
                    error.Typee = (int)Packet_Type.에러;
                    Packet.Serialize(error).CopyTo(this.sendBuffer, 0);
                    Send();
                }
            }
        }

        public void Send()
        {
            this.m_Stream.Write(this.sendBuffer,0,this.sendBuffer.Length);
            this.m_Stream.Flush();
            for (int i=0;i<256;i++)
            {
                this.sendBuffer[i]=0;
            }
        }

        private int PatternOut(int chapter,int user_pattern)
        {
            Random random = new Random();
            int looking = random.Next(0, 100);
            if(looking<=20*chapter)
            {
                switch(looking)
                {
                    case 0:
                        return 1;
                    case 1:
                        return 0;
                    case 2:
                        return 1;
                    default:
                        return 0;
                }
            }
            else
            {
                return random.Next(0, 3);
            }
        }

        private int BossOut(int value)
        {
            Random random = new Random();
            return random.Next(0, value);
        }

    }
}