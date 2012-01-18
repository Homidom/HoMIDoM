using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HoMIDroid.Server
{
    public class RealServerWithCache : RealServer
    {
        private List<BO.Device> devices;
        private List<BO.Zone> zones;

        #region CTor
        public RealServerWithCache()
            : base()
        {
        }

        public RealServerWithCache(string serverID, string hostName, int port = 8000)
            : base(serverID, hostName, port)
        {
        } 
        #endregion

        public override List<BO.Device> GetDevices()
        {
            if (this.devices == null || this.devices.Count == 0)
                this.devices = base.GetDevices();
            return this.devices;
        }

        public override List<BO.Zone> GetZones()
        {
            if (this.zones == null || this.zones.Count == 0)
                this.zones = base.GetZones();
            return this.zones;
        }

        public void Refresh()
        {
            this.Clear();
            this.GetDevices();
            this.GetZones();
        }

        public void Clear()
        {
            this.devices = null;
            this.zones = null;
        }
    }
}