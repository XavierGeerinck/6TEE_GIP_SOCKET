using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace Server.User
{
    class DoChat
    {
        public static void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;

                    //Grijp de stream die verzonden is door de client
                    NetworkStream networkStream = Server.Network.handleClinet.clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)Server.Network.handleClinet.clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine("From client - " + Server.Network.handleClinet.clNo + " : " + dataFromClient);
                    rCount = Convert.ToString(requestCount);
                    Server.User.BroadCast.broadcast(dataFromClient, Server.Network.handleClinet.clNo, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
