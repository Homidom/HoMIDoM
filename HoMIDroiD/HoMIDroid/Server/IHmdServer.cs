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
using HoMIDroid.Server;
using HoMIDroid.BO;

namespace HoMIDroid.Server
{
    public interface IHmdServer
    {
        void Connect(string serverID, string host, int port);
        Device GetDevice(string id);
        List<Device> GetDevices();
        List<Device> GetDevicesInZone(Zone zone);
        List<Group<Device>> GetDevicesByCategory();
        List<Zone> GetZones();
        Zone GetZone(string id);
        bool ExecuteAction(Device device, DeviceAction action);

        void Clear();

        void Refresh();
    }
}