using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using DarkTunnel;

namespace DarkTCP
{
    public partial class Form1 : Form
    {
        DarkTunnel.Server Darktunnel = new DarkTunnel.Server("none", "GIP6TEE", "GIPSALTEDPASS", "MD5", 1, "Something to Ini", 256, "Client", true, 500, 1000, 0, 100, 5000); //Server

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            
            //Server events
            DarkTunnel.Server.ServerStatus += new ServerStatusHandler(server_onServerStatusChanged);
            DarkTunnel.Server.ClientConnect += new onClientConnectHandler(server_onClientConnect);
            DarkTunnel.Server.ReceivedData += new onReceivedDataHandler(server_onReceivedData);
            DarkTunnel.Server.ClientDisconnected += new onClientDisconnectHandler(server_onClientDisconnected);
            DarkTunnel.Server.ClientError += new onErrorHandler(server_onError);
            DarkTunnel.Server.ServerStartup += new onServerStartupHandler(server_onServerStartup);
            DarkTunnel.Server.ServerSendData += new onServerSendDataHandler(server_onSendData);
            DarkTunnel.Server.ServerFloodProtector += new onServerFloodHandler(server_onFloodProtector);
        }

        #region Server events
        MemoryStream _ServerStream;

        public void server_onServerStatusChanged(object o, ServerStatusEventArgs e)
        {
            label2.Text = "Server Status: " + e.Status;
        }

        public void server_onClientConnect(object o, onClientConnectEventArgs e)
        {
            Server_Log("New client connected Remote IP Address: " + MiscFunctions.SplitString(e.RemoteIP, ':', 0) + "\tGUID: " + e.GUID.ToString() + "\tNickname: " + e.Nickname);
            label4.Text = "Connected clients: " + e.Connections.ToString();
            comboBox2.Items.Add(e.Nickname);
        }

        public void server_onReceivedData(object o, onReceivedDataEventArgs e)
        {
            label5.Text = "Total received bytes: " + e.TotalReceivedData.ToString();
            label20.Text = "Total received packets: " + e.TotalReceivedPackets.ToString();
            Server_Log("Received data from: " + e.RemoteIP + "\tlength: " + e.Data.Length + "\tData: " + e.Data + "\tGuid: " + e.GUID.ToString() + "\tNickname: " + e.Nickname);

            label5.Refresh();
            label20.Refresh();
        }

        public void server_onClientDisconnected(object o, onClientDisconnectedEventArgs e)
        {
            Server_Log("Client Disconnected: " + e.RemoteIP + "\tGUID: " + e.GUID + "\tNickname: " + e.Nickname);
            label4.Text = "Connected clients: " + e.Connections.ToString();

            for (int i = 0; i < comboBox2.Items.Count; i++)
            {
                if (comboBox2.Items[i].ToString() == e.Nickname)
                    comboBox2.Items.RemoveAt(i);
            }
        }

        public void server_onError(object o, onErrorEventArgs e)
        {
            Server_Log("Error reason: " + e.Reason + "\tfrom remote ip: " + e.RemoteIP + "\tGUID: " + e.GUID.ToString() + "\tNickname: " + e.Nickname);
        }

        public void server_onServerStartup(object o, onServerStartupEventArgs e)
        {
            if (e.success)
                Server_Log("Server successfully started at port " + e.Port.ToString());
            else
                Server_Log("server is unsuccessfully started, reason: " + e.Error);
        }

        public void server_onSendData(object o, onServerSendDataEventArgs e)
        {
            label8.Text = "Total sended bytes: " + e.TotalSendedData.ToString();
            label21.Text = "Total sended Packets: " + e.TotalSendedPackets.ToString();
            Server_Log("Sending data to: " + e.TargetIP + ":" + e.Port.ToString() + "\tGUID: " + e.TargetGUID.ToString() + "\tNickname: " + e.TargetNickname + "\tPacket Length: " + e.PacketLength.ToString() + "\tData: " + e.Data);
            label8.Refresh();
            label21.Refresh();
        }
       
        public void server_onFloodProtector(object o, onServerFloodEventArgs e)
        {
            Server_Log("Detected flood attack. GUID: " + e.GUID.ToString() + "\tIP:" + e.RemoteIP);
        }
        #endregion

        #region Buttons/Functions
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Darktunnel.IsServerRunning)
                Darktunnel.Stop();
        }

        private void Server_Log(string log)
        {
            textBox1.AppendText(log + "\r\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Darktunnel.listen(IPAddress.Any.ToString(), (int)numericUpDown1.Value);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (Darktunnel.IsServerRunning)
                Darktunnel.Stop();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Darktunnel.broadcast(textBox4.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Darktunnel.SendDataToNickname(textBox4.Text, comboBox2.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Darktunnel.DisconnectAllClients();
        }

        #endregion
    }
}