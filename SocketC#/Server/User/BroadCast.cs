using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace Server.User
{
    class BroadCast
    {
        public static void broadcast(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in Server.Network.Listener.clientsList)
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
}
