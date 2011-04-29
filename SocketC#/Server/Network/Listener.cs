using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace Server.Network
{
    class Listener
    {
        //definieer user table
        public static Hashtable clientsList = new Hashtable();

        public static void startlistening()
        {
            //aanaf zodat we de server god kunnen stoppen.
            bool aanaf = true;

            //Definieer Sockets voor client en server + Listening IP en poort (in dit geval ip 0.0.0.0 en poort 13000)
            TcpListener serverSocket = new TcpListener(System.Net.IPAddress.Any, 13000);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            //Socket start
            serverSocket.Start();
            Server.Logger.addlog("Chat Server Started ....", Server.Logger.LogType.Socket);

            //reset counter
            counter = 0;

            //main loop
            while (aanaf == true)
            {
                try
                {
                    counter += 1;
                    clientSocket = serverSocket.AcceptTcpClient();

                    byte[] bytesFrom = new byte[10025];
                    string dataFromClient = null;

                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    clientsList.Add(dataFromClient, clientSocket);

                    Server.User.BroadCast.broadcast(dataFromClient + " Joined ", dataFromClient, false);
                    Console.WriteLine();
                    Server.Logger.addlog(dataFromClient + " Joined chat room ", Server.Logger.LogType.User);
                    handleClinet client = new handleClinet();
                    client.startClient(clientSocket, dataFromClient, clientsList);
                }
                catch
                {
                    Server.Logger.addlog("An error occured in Linstener.cs!", Logger.LogType.Error);
                    aanaf = false;

                    //Zet de ClientSocket en de ServerSocket uit!
                    clientSocket.Close();
                    serverSocket.Stop();

                    //Zend log door
                    Server.Logger.addlog("exit", Server.Logger.LogType.Error);
                }
            }
        }
    }
}
