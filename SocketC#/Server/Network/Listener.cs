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
            while ((true))
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

            //stop de ClientSocket en de ServerSocket (Probleem want ik zit in een while true loop :P
            clientSocket.Close();
            serverSocket.Stop();
            Server.Logger.addlog("exit", Server.Logger.LogType.Error);
            Console.ReadLine();
        }
    }
}
