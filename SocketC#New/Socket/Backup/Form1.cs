using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using DarkTunnel;

namespace DarkTCP
{
    public partial class Form1 : Form
    {
        /*******************************************************************************
         * The DarkTunnel and the DarkTCP projects are created by DragonHunter         *
         *                                                                             *
         * Encryption can be AES or none if you do not want a encryption               *
         * If you set the encryption to "none" the server can't detect invalid data    *
         *                                                                             *
         * AES is able to detect invalid data and when invalid data is found           *
         * The client who sended it will be disconnected from the server               *
         * This is against packet modifications and server attackers                   *
         *                                                                             *
         * The AES encryption takes from 0 till 15 miliseconds to encrypt data         *
         * So it's not even worth to set the encryption to none                        *
         *                                                                             *
         *  Consturctor                                                                *
         *  - Encryptions: AES, none (default: AES)                                    *
         *  - Password to encrypt with                                                 *
         *  - Salt to encrypt with                                                     *
         *  - HashAlgorithm Can be SHA1, SHA256, SHA384, SHA512 or MD5                 *
         *  - PasswordIterations Number of iterations to do (default: 1)               *
         *  - InitialVector Needs to be 16 ASCII characters long                       *
         *  - bits Can be 128, 192, or 256 (default: 256)                              *
         *  - Nickname, A nickname is something for the server that will hold all      *
         *    Clients with a name, As default the nickname will be as: Client1         *
         *    And when more clients will join it will be Client2, Client3...           *
         *  - Show Error: You can disable to show errors (Client-Side only)            *
         *  - Flood Protector: Against DOS Attacks                                     *
         *  - Max IP Connections: How many connections can the server get from the     *
         *                        Same ip address, 0 = unlimited connections from      *
         *                        same ip                                              *
         *  - Accept Connection Delay: Against Syn-Floods and such                     *
         *                                                                             *
         *  You can also use some misc functions in the class DarkTunnel.MiscFunctions *
         *  All functions in this class are static                                     *
         *******************************************************************************/

        DarkTunnel.Server Darktunnel = new DarkTunnel.Server("none", "Your own password here", "Ur Own Salt Password", "MD5", 1, "Something to Ini", 256, "Client", true, 500, 1000, 0, 100, 5000); //Server
        DarkTunnel.Client DarktunnelClient = new DarkTunnel.Client("none", "Your own password here", "Ur Own Salt Password", "MD5", 1, "Something to Ini", 256, true);

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
            DarkTunnel.Server.ServerReceivedFile += new onServerReceivedFileHandler(server_onReceivedFile);
            DarkTunnel.Server.ServerUploadingFile += new onServerUploadingFileHandler(server_onUploadingFile);
            DarkTunnel.Server.ServerFloodProtector += new onServerFloodHandler(server_onFloodProtector);

            //Client events
            DarkTunnel.Client.ClientStatus += new ClientStatusHandler(client_onClientStatusChanged);
            DarkTunnel.Client.ClientConnect += new ClientConnectHandler(client_onClientConnected);
            DarkTunnel.Client.ReceivedData += new ClientReceivedDataHandler(client_onClientReceivedData);
            DarkTunnel.Client.ClientDisconnected += new ClientDisconnectHandler(client_onClientDisconnect);
            DarkTunnel.Client.ClientSendData += new ClientSendDataHandler(client_onClientSendData);
            DarkTunnel.Client.ClientReceivedFile += new ClientReceivedFileHandler(client_onReceivedFile);
            DarkTunnel.Client.ClientUploadingFile += new ClientUploadingFileHandler(client_onUploadingFile);
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
        
        public void server_onReceivedFile(object o, onServerReceivedFileEventArgs e)
        {
            label40.Text = "Total received files: " + e.TotalReceivedFiles.ToString();
            label41.Text = "Total sended files: " + e.TotalSendedFiles.ToString();
            Server_Log("received file\tFile size: " + e.memoryStream.Length + "\tDescription: " + e.Description + "\tRemote IP: " + e.RemoteIP + "\tNickname: " + e.Nickname + "\tGUID: " + e.GUID);

            if (e.Description == "image")
                _ServerStream = e.memoryStream;
            else
                File.WriteAllBytes("C:\\Server_Received File.rar", e.FileBytes);
        }

        public void server_onUploadingFile(object o, onServerUploadingFileEventArgs e)
        {
            progressBar2.Maximum = (int)e.OutputFileSize;
            progressBar2.Value = (int)e.TotalUploadedBytes;
        }

        public void server_onFloodProtector(object o, onServerFloodEventArgs e)
        {
            Server_Log("Detected flood attack. GUID: " + e.GUID.ToString() + "\tIP:" + e.RemoteIP);
        }
        #endregion
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

        public void client_onReceivedFile(object o, ClientReceivedFileEventArgs e)
        {
            label43.Text = "Total received files: " + e.TotalReceivedFiles.ToString();
            label42.Text = "Total sended files: " + e.TotalSendedFiles.ToString();
            Client_Log("received file\tFile size: " + e.memoryStream.Length + "\tDescription: " + e.Description + "\tRemote IP: " + e.RemoteIP + ":" + e.Port);
            if (e.Description == "image")
                _ClientStream = e.memoryStream;
            else
                File.WriteAllBytes("C:\\Client_Received File.rar", e.FileBytes);
        }

        public void client_onUploadingFile(object o, ClientUploadingFileEventArgs e)
        {
            progressBar1.Maximum = (int)e.OutputFileSize;
            progressBar1.Value = (int)e.TotalUploadedBytes;
        }
        #endregion
        #region Buttons/Functions
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Darktunnel.IsServerRunning)
                Darktunnel.Stop();

            if (DarktunnelClient.IsConnected)
                DarktunnelClient.Close();
        }

        private void Server_Log(string log)
        {
            textBox1.AppendText(log + "\r\n");
        }
        private void Client_Log(string log)
        {
            textBox2.AppendText(log + "\r\n");
        }

        private void ShowImage(MemoryStream stream)
        {
            new ImgPreview { Stream = stream }.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Darktunnel.listen(IPAddress.Any.ToString(), (int)numericUpDown1.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(!DarktunnelClient.IsConnected)
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

        private void button9_Click(object sender, EventArgs e)
        {
            textBox7.Text = DragonEncryption.Encrypt(textBox6.Text, textBox8.Text, (short)numericUpDown3.Value);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox10.Text = DragonEncryption.Decrypt(textBox11.Text, textBox9.Text, (short)numericUpDown4.Value);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Multiselect = false;
                dialog.Title = "Select file you want to send";
                if (dialog.ShowDialog() == DialogResult.OK)
                    DarktunnelClient.SendFile(dialog.FileName, textBox12.Text);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox17.Text = AesEncryption.Encrypt(textBox13.Text, textBox14.Text, textBox15.Text, comboBox3.Text, (int)numericUpDown5.Value, textBox16.Text, Convert.ToInt32(comboBox1.Text));
        }

        private void button13_Click(object sender, EventArgs e)
        {
            textBox18.Text = AesEncryption.Decrypt(textBox22.Text, textBox21.Text, textBox20.Text, comboBox4.Text, (int)numericUpDown6.Value, textBox19.Text, Convert.ToInt32(comboBox5.Text));
        }
        private void button14_Click(object sender, EventArgs e)
        {
            ShowImage(_ServerStream);
        }
        private void button16_Click(object sender, EventArgs e)
        {
            ShowImage(_ClientStream);
        }
        private void button15_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Multiselect = false;
                dialog.Title = "Select file you want to send";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (MessageBox.Show("Do you want to broadcast this file ?", "Broadcast File ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        Darktunnel.Broadcast_File(dialog.FileName, textBox23.Text);
                    else
                        Darktunnel.BroadcastNickname_File(dialog.FileName, textBox23.Text, comboBox2.Text);
                }
            }
        }
        #endregion
    }
}