using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace WpfApplication1
{
    public static class Server
    {
        public static Hashtable clientsList = new Hashtable();
        public static Dictionary<string, string> clientData = new Dictionary<string, string>();

        public static  void StartServer()
        {
            TcpListener serverSocket = new TcpListener(8888);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            //Console.WriteLine("Chat Server Started ....");
            counter = 0;
            while ((true))
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                clientSocket.ReceiveBufferSize = 8192;

                byte[] bytesFrom = new byte[10025];
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                if (dataFromClient.Contains("7d2abf2d0fa7c3a0c13236910f30bc43"))
                {
                    // registration
                    var nick = dataFromClient.Substring(dataFromClient.IndexOf("7d2abf2d0fa7c3a0c13236910f30bc43")+ 32);
                    var password = dataFromClient.Substring(0, dataFromClient.IndexOf("7d2abf2d0fa7c3a0c13236910f30bc43"));
                    if (!clientData.ContainsKey(nick))
                        {
                        clientsList.Add(nick, clientSocket);
                        clientData.Add(nick, password);
                        broadcast(nick + " registered to chat ", nick, false);
                        handleClinet client = new handleClinet();
                        client.startClient(clientSocket, nick, clientsList);
                    } 
                }
                if (dataFromClient.Contains("d56b699830e77ba53855679cb1d252da"))
                {
                    // login
                    var nick = dataFromClient.Substring(dataFromClient.IndexOf("d56b699830e77ba53855679cb1d252da") + 32);
                    var password = dataFromClient.Substring(0, dataFromClient.IndexOf("d56b699830e77ba53855679cb1d252da"));
                    if (clientData.ContainsKey(nick))
                    {
                        clientsList.Add(nick, clientSocket);
                        broadcast(nick + " joined to chat ", nick, false);
                        handleClinet client = new handleClinet();
                        client.startClient(clientSocket, nick, clientsList);
                    }
                }
                /*
                if (dataFromClient.Contains("ddbb706f8d37c145a044d931ab097eac"))
                {
                    // is authorized
                    var nick = dataFromClient.Replace("ddbb706f8d37c145a044d931ab097eac", "");
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)clientSocket;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    Byte[] broadcastBytes = null;
                    broadcastBytes = Encoding.ASCII.GetBytes(clientsList.Contains(nick).ToString());
                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                }*/

            }
            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        public static void broadcast(string msg, string uName, bool flag)
        {
            if(msg == "1a505b46f54bdd130ab920910e33428b")
            {
                // quite
                clientsList.Remove(uName);
                msg = string.Format("{0} left the chat", uName);
                flag = false;
            }

            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
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


    public class handleClinet
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
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                    rCount = Convert.ToString(requestCount);

                    Server.broadcast(dataFromClient, clNo, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

    }
}
