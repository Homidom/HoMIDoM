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
        private List<BO.Device> alldevices;
        private List<BO.Device> devices;
        private List<BO.Zone> zones;
        private List<BO.Macro> macros;

        #region CTor
        public RealServerWithCache()
            : base()
        {
            var app = TinyIoC.TinyIoCContainer.Current.Resolve<HmdApp>();
            app.RefreshData += new EventHandler<EventArgs>(app_RefreshData);
            this.alldevices = new List<BO.Device>();
        }

        public RealServerWithCache(string serverID, string hostName, int port = 8000)
            : base(serverID, hostName, port)
        {
        } 
        #endregion

        public override BO.Device GetDevice(string id)
        {
            if (this.devices == null || this.devices.Count == 0)
                this.devices = this.ensureUniqueDevices(base.GetDevices());
            return this.devices.FirstOrDefault(d => d.Id == id);
        }

        public override List<BO.Device> GetDevices()
        {
            if (this.devices == null || this.devices.Count == 0)
                this.devices = this.ensureUniqueDevices(base.GetDevices());
            return this.devices;
        }

        public override List<BO.Zone> GetZones()
        {
            if (this.zones == null || this.zones.Count == 0)
                this.zones = this.ensureUniqueDevice(base.GetZones());
            return this.zones;
        }

        public override List<BO.Macro> GetMacros()
        {
            if (this.macros == null || this.macros.Count == 0)
                this.macros = base.GetMacros();
            return this.macros;
        }

        public override void Refresh()
        {
            this.Clear();
            this.GetDevices();
            this.GetZones();
            this.GetMacros();
        }

        public override void Clear()
        {
            this.devices = null;
            this.zones = null;
            this.macros = null;
            this.alldevices.Clear();
        }

        List<BO.Device> ensureUniqueDevices(List<BO.Device> devices)
        {
            var list = new List<BO.Device>();
            foreach (var d in devices)
                list.Add(this.ensureUniqueDevice(d));
            return list;
        }

        BO.Device ensureUniqueDevice(BO.Device device)
        {
            var original = this.alldevices.FirstOrDefault(d => d.Id == device.Id);
            if (original != null)
                return original;
            this.alldevices.Add(device);
            return device;
        }

        List<BO.Zone> ensureUniqueDevice(List<BO.Zone> zones)
        {
            var list = new List<BO.Zone>();
            foreach (var z in zones)
                list.Add(this.ensureUniqueDevice(z));
            return list;
        }

        BO.Zone ensureUniqueDevice(BO.Zone zone)
        {
            var devices = this.ensureUniqueDevices(zone.Devices);

            zone.Devices.Clear();
            zone.Devices.AddRange(devices);

            return zone;
        }

        void app_RefreshData(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}