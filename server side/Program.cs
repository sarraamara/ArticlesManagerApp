using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace ServerChatProgram
{
    class Program
    {
        public static Hashtable clientsList = new Hashtable(); 

            static TcpListener serverSocket ;
            static TcpClient clientSocket;
        static void Main(string[] args)
        {
            try
            {
                 serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
                 clientSocket = default(TcpClient);
                int counter = 0;
                serverSocket.Start();
                Console.WriteLine("Chat Server Started ....");
                while ((true))
                {
                    clientSocket = serverSocket.AcceptTcpClient();
                    counter += 1;
                    
                    string dataFromClient = null;

                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[(int)clientSocket.ReceiveBufferSize];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    clientsList.Add(dataFromClient, clientSocket);

                    broadcast(dataFromClient + " vient de rejoindre la conversation.", dataFromClient, false);
                    Console.WriteLine(dataFromClient + " Joined chat room ");
                    handleClient client = new handleClient();
                    client.startClient(clientSocket, dataFromClient, clientsList);
                }
                
                clientSocket.Close();
                serverSocket.Stop();
                Console.WriteLine("exit");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                clientSocket.Close();
                serverSocket.Stop();
                Console.WriteLine("exit");
                Console.ReadLine();
                
            }

        }
        public static void broadcast(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " dit : " + msg);
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);
                }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        } 
    }
    public class handleClient
    {
        TcpClient clientSocket;
        string clNo;
        Hashtable clientsList;

        public void startClient(TcpClient inClientSocket, string clineNo, Hashtable cList)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            this.clientsList = cList;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }

        private void doChat()
        {
            int requestCount = 0;
            
            string dataFromClient = null;
            string rCount = null;
            bool end = true;
            NetworkStream networkStream = null;
            while ((end))
            {
                try
                {
                    requestCount++;
                    networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[(int)clientSocket.ReceiveBufferSize];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                    rCount = Convert.ToString(requestCount);

                    if (dataFromClient.Equals("END"))
                    {
                        end = false;
                        clientsList.Remove(clNo);
                        Program.broadcast(clNo+" vient de quitter la conversation.", clNo, false);

                    }
                    else
                    {
                        Program.broadcast(dataFromClient, clNo, true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            networkStream.Close();
            clientSocket.Close() ;

        }
    } 
}
