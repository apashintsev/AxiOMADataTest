using PcNcCommClient;
using PcNcCommon;
using System;
using System.Timers;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var a = new axio();
            a.Connect("localhost");
        }
    }
    class axio { 
        // NC-client
        private INcClient iNcClient = null;
        private AbstractCommClientEx client = null;
        private bool isClientConnected = false;

        private Timer timer;

        private int _activeChanIndex = 0;

        private void client_ChangeServerState(object sender, AbstractServer.ChangeServerStateArgs args)
        {
            if (this.client.ConnectState != ConnectStates.Connected)
            {
                this.isClientConnected = false;
                if (timer != null)
                    timer.Stop();
                timer = null;
            }
            else
            {
                this.isClientConnected = true;

                timer = new Timer();
                timer.Interval = 50;
                //timer.Tick += Timer_Tick;
                timer.Start();
            }
            //UpdateControls();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double pos;
            int filepos, level, subprog_version;
            string filename;

            this.client.GetAxisDrPosValue(0, out pos);
           // this.textBox1.Text = pos.ToString();
            this.client.GetAxisDrPosValue(1, out pos);
            //this.textBox2.Text = pos.ToString();
            this.client.GetAxisDrPosValue(2, out pos);
            //this.textBox3.Text = pos.ToString();

            this.client.GetFilePosition(1, out filename, out filepos, out level, out subprog_version);
            var filePosTextBoxText = "Channel number: " + 1 + Environment.NewLine + "FileName: " + filename + Environment.NewLine + "File position: " + filepos + Environment.NewLine + "Level: " + level + Environment.NewLine + "Subprog version: " + subprog_version;
            Console.WriteLine(filePosTextBoxText);
        }

        //private void UpdateControls()
        //{
        //    if (this.isClientConnected)
        //    {
        //        button1.Text = "Disconnect";
        //        button1.Enabled = true;
        //    }
        //    else
        //    {
        //        button1.Text = "Connect";
        //        button1.Enabled = true;
        //    }
        //}

        public bool Connect(string targetName)
        {
            if (this.isClientConnected)
                return false;

            //this.button1.Enabled = false;

            if (this.iNcClient == null)
                this.iNcClient = new AbstractCommTcpClient();
            if (this.iNcClient == null)
            {
                Console.WriteLine("Не создан приёмопередатчик");
                return false;
            }

            if (this.client == null)
                this.client = new AbstractCommClientEx(this.iNcClient);

            if (this.client == null)
            {
                Console.WriteLine("Не создан терминальный клиент");
                return false;
            }

            if (string.IsNullOrEmpty(targetName))
                targetName = PcNcConstants.TargetName;

            //this.Server = this.client;

            //if (this.client.EwiHandler != null)
            //  this.client.EwiHandler.EwiMessage += new OnEwiEvent(client_EwiMessage);
            this.client.ChangeServerState += new AbstractServer.OnChangeServerState(client_ChangeServerState);


            ErrorCodes error = ErrorCodes.NoError;
            error = this.client.InitServer(targetName);
            if (error != ErrorCodes.NoError)
            {
                Console.WriteLine("Ошибка при инициализации терминального клиента или ошибка соединения");
                this.client.ChangeServerState -= new AbstractServer.OnChangeServerState(client_ChangeServerState);

                if (error == ErrorCodes.ExceptionError)
                    return false;

                return false;
            }

            AbstractChannel channel = this.client.ChanList.GetAt(this._activeChanIndex);
            if (channel == null)
            {
                this._activeChanIndex = 0;
                channel = this.client.ChanList.GetAt(this._activeChanIndex);
                if (channel == null)
                {
                    this._activeChanIndex = -1;
                }

                //try
                //{
                //  this.numericUpDown_channel.Value = this._activeChanIndex + 1;
                //}
                //catch
                //{
                //}
            }

            //if (channel != null)
            //{
            //  if ((channel.Msg != null)
            //      && (channel.Msg.Length > 0))
            //    AddChannelMsg(channel.Number, channel.Msg.Filepos, channel.Msg.FileIndex, channel.Msg.StackLevel,
            //      channel.Msg.Version, channel.Msg.Length, channel.Msg.Data);
            //}

            //if (channel != null)
            //{
            //  if ((channel.MdiString != null)
            //      && (!string.IsNullOrEmpty(channel.MdiString.MdiString)))
            //    AddChannelMdi(channel.Number, channel.MdiString.Version, channel.MdiString.MdiString);
            //}

            UInt32 this_client_index = 0;
            error = this.client.NcFunctions.GetClientIndex(out this_client_index);
            string descr = "AxiOMA Test Data Collector " + this_client_index.ToString();
            error = this.client.NcFunctions.SetClientDescr(this_client_index, descr);
            error = this.client.NcFunctions.SetClientType(this_client_index, NcClientTypes.Debug);
            error = this.client.NcFunctions.SetClientState(this_client_index, NcClientStates.Active);

            //AdviseChannelEvents(channel);
            //UpdateControls();
            return true;
        }

        private void Disconnect()
        {
            if ((!this.isClientConnected)
                || (this.client == null))
                return;

            //this.Server = null;

            this.isClientConnected = false;

            //if (this.client.EwiHandler != null)
            //  this.client.EwiHandler.EwiMessage -= new OnEwiEvent(client_EwiMessage);
            this.client.ChangeServerState -= new AbstractServer.OnChangeServerState(client_ChangeServerState);

            AbstractChannel channel = null;
            if (this.client.ChanList != null)
                channel = this.client.ChanList.GetAt(this._activeChanIndex);

            //UnAdviseChannelEvents(channel);

            if (this.client.NcClient != null)
            {
                this.client.NcClient.DestroyData();
            }

            if (this.client != null)
            {
                this.client.DestroyServer();
            }

            if (this.iNcClient != null)
            {
                this.iNcClient.DestroyData();
                this.iNcClient = null;
            }

            this.client = null;
            //UpdateControls();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.isClientConnected)
                Connect("localhost");
            else
                Disconnect();
        }

        //private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    timer.Stop();
        //    Disconnect();
        //}
    }
}
