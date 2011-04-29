using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace Server.Network
{
    class handleClinet
    {
        public static TcpClient clientSocket;
        public static string clNo;
        Hashtable clientsList;

        public void startClient(TcpClient inClientSocket, string clineNo, Hashtable cList)
        {
            clientSocket = inClientSocket;
            clNo = clineNo;
            clientsList = cList;
            Thread ctThread = new Thread(Server.User.DoChat.doChat);
            ctThread.Start();
        }
    }
}
