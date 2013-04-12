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
    public class MixedServer : IHmdServer
    {
        private bool useMock;
        MockServer MockServer { get; set; }
        BaseServer RealServer { get; set; }
        IHmdServer Server
        {
            get
            {
                return this.useMock ? this.MockServer : this.RealServer;
            }
        }

        public MixedServer(MockServer mock, BaseServer real)
        {
            this.MockServer = mock;
            this.RealServer = real;
        }


        public void Connect(string serverID, string host, int port)
        {
            this.useMock = host == "demo";
            this.Server.Connect(serverID, host, port);
        }

        public BO.Device GetDevice(string id)
        {
            return this.Server.GetDevice(id);
        }

        public List<BO.Device> GetDevices()
        {
            return this.Server.GetDevices();
        }

        public List<BO.Device> GetDevicesInZone(BO.Zone zone)
        {
            return this.Server.GetDevicesInZone(zone);
        }

        public List<BO.Group<BO.Device>> GetDevicesByCategory()
        {
            return this.Server.GetDevicesByCategory();
        }

        public List<BO.Zone> GetZones()
        {
            return this.Server.GetZones();
        }

        public List<BO.Macro> GetMacros()
        {
            return this.Server.GetMacros();
        }

        public BO.Zone GetZone(string id)
        {
            return this.Server.GetZone(id);
        }

        public bool ExecuteAction(BO.Device device, BO.DeviceAction action)
        {
            return this.Server.ExecuteAction(device, action);
        }

        public bool ExecuteMacro(BO.Macro macro)
        {
            return this.Server.ExecuteMacro(macro);
        }

        public void Clear()
        {
            this.Server.Clear();
        }

        public void Refresh()
        {
            this.Server.Refresh();
        }

        public bool RefreshDevice(BO.Device device)
        {
            return this.Server.RefreshDevice(device);
        }


    }
}