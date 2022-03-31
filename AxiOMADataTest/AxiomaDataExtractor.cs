using PcNcCommClient;
using PcNcCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AxiOMADataTest
{
    public class AxiomaDataExtractor
    {
        // NC-client
        private INcClient iNcClient = null;
        private AbstractCommClientEx client = null;
        private bool isClientConnected = false;

        private System.Threading.Timer timer;
        private System.Threading.Timer endTimer;

        private int _activeChanIndex = 0;

        public List<AxiomaData> AxiomaData = new List<AxiomaData>();

        public DateTime StartAt { get; set; }
        public DateTime StopAt { get; set; }

        private void client_ChangeServerState(object sender, AbstractServer.ChangeServerStateArgs args)
        {
            if (this.client.ConnectState != ConnectStates.Connected)
            {
                this.isClientConnected = false;
                if (timer != null)
                    timer.Change(Timeout.Infinite, 2000);
                timer = null;
            }
            else
            {
                this.isClientConnected = true;
                timer = new System.Threading.Timer(Timer_Tick, this, 0, 50);
            }
        }

        private void Timer_Tick(object o)
        {
            var thisItem = ((AxiomaDataExtractor)o);

            double posX,posY,posZ;
            int filepos, level, subprog_version;
            string filename;

            thisItem.client.GetAxisDrPosValue(0, out posX);
            thisItem.client.GetAxisDrPosValue(1, out posY);
            thisItem.client.GetAxisDrPosValue(2, out posZ);

            thisItem.client.GetFilePosition(1, out filename, out filepos, out level, out subprog_version);
            var data = new AxiomaData()
            {
                Filename = filename,
                Level = level,
                FilePosition = filepos,
                SubprogVersion = subprog_version,
                Coords = new Coords(posX, posY, posZ)
            };
            this.AxiomaData.Add(data);
        }

        public bool Connect(string targetName)
        {
            if (this.isClientConnected)
                return false;

            if (this.iNcClient == null)
                this.iNcClient = new AbstractCommTcpClient();
            if (this.iNcClient == null)
            {
                MessageBox.Show("Не создан приёмопередатчик");
                return false;
            }

            if (this.client == null)
                this.client = new AbstractCommClientEx(this.iNcClient);

            if (this.client == null)
            {
                MessageBox.Show("Не создан терминальный клиент");
                return false;
            }

            if (string.IsNullOrEmpty(targetName))
                targetName = PcNcConstants.TargetName;

            this.client.ChangeServerState += new AbstractServer.OnChangeServerState(client_ChangeServerState);


            ErrorCodes error = ErrorCodes.NoError;
            error = this.client.InitServer(targetName);
            if (error != ErrorCodes.NoError)
            {
                MessageBox.Show("Ошибка при инициализации терминального клиента или ошибка соединения");
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
            }

            UInt32 this_client_index = 0;
            error = this.client.NcFunctions.GetClientIndex(out this_client_index);
            string descr = "AxiOMA Test Data Collector " + this_client_index.ToString();
            error = this.client.NcFunctions.SetClientDescr(this_client_index, descr);
            error = this.client.NcFunctions.SetClientType(this_client_index, NcClientTypes.Debug);
            error = this.client.NcFunctions.SetClientState(this_client_index, NcClientStates.Active);

            return true;
        }
        public void Disconnect()
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

            timer.Change(Timeout.Infinite, 2000);
            //Disconnect();
        }

        private void OneTimeCallback2(Object o)
        {
            var thisItem = ((AxiomaDataExtractor)o);
            thisItem.Disconnect();
            thisItem.StopAt = DateTime.UtcNow;
            var dis = thisItem.AxiomaData;
        }

        internal void Process(string address, int minutes)
        {
            this.Connect(address);
            this.StartAt = DateTime.UtcNow;
            endTimer = new System.Threading.Timer(OneTimeCallback2, this, (minutes *60 *1000), 0);
        }
    }
}
