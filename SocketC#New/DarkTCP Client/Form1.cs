using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using DarkTunnel;

namespace DarkTCP_Client
{
    public partial class Form1 : Form
    {
        DarkTunnel.Client DarktunnelClient = new DarkTunnel.Client("none", "GIP6TEE", "GIPSALTEDPASS", "MD5", 1, "Something to Ini", 256, true);

        public Form1()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
            //Client events
            DarkTunnel.Client.ClientStatus += new ClientStatusHandler(client_onClientStatusChanged);
            DarkTunnel.Client.ClientConnect += new ClientConnectHandler(client_onClientConnected);
            DarkTunnel.Client.ReceivedData += new ClientReceivedDataHandler(client_onClientReceivedData);
            DarkTunnel.Client.ClientDisconnected += new ClientDisconnectHandler(client_onClientDisconnect);
            DarkTunnel.Client.ClientSendData += new ClientSendDataHandler(client_onClientSendData);
        }

        #region Client Events
        MemoryStream _ClientStream;
        public void client_onClientStatusChanged(object o, ClientStatusEventArgs e)
        {
            label6.Text = "Client Status: " + e.Status;
        }

        public void client_onClientReceivedData(object o, ClientReceivedDataEventArgs e)
        {
            label7.Text = "Total received bytes: " + e.TotalReceivedData.ToString();
            label24.Text = "Total received packets: " + e.TotalReceivedPackets.ToString();
            Client_Log("Received data from: " + e.RemoteIP + "\tLength: " + e.Data.Length + "\tData: " + e.Data);
            label7.Refresh();
            label24.Refresh();
        }

        public void client_onClientConnected(object o, ClientConnectEventArgs e)
        {
            Client_Log("Client is connected to: " + e.RemoteIP + ":" + e.Port.ToString());
        }

        public void client_onClientDisconnect(object o, ClientDisconnectEventArgs e)
        {
            Client_Log("Client is disconnected from " + e.RemoteIP + ":" + e.Port.ToString());
        }

        public void client_onClientSendData(object o, ClientSendDataEventArgs e)
        {
            Client_Log("Sended data to: " + e.TargetIP + ":" + e.Port + "\tLength: " + e.PacketLength + "\tData: " + e.Data);
            label22.Text = "Total sended bytes: " + e.TotalSendedData.ToString();
            label23.Text = "Total sended packets: " + e.TotalSendedPackets.ToString();
            label22.Refresh();
            label23.Refresh();
        }

        #endregion

        #region Buttons/Functions
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DarktunnelClient.IsConnected)
                DarktunnelClient.Close();
        }

        private void Client_Log(string log)
        {
            textBox2.AppendText(log + "\r\n");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!DarktunnelClient.IsConnected)
                DarktunnelClient.Connect(textBox3.Text, (short)numericUpDown2.Value);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DarktunnelClient.Send(textBox5.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (DarktunnelClient.IsConnected)
                DarktunnelClient.Close();
        }

        #endregion
    }
}
