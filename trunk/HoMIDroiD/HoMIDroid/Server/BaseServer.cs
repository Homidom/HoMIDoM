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
    public abstract class BaseServer : IHmdServer
    {
        #region Abstract methods

        public abstract Device GetDevice(string id);

        public abstract List<Device> GetDevices();

        public abstract List<Device> GetDevicesInZone(Zone zone);

        public abstract List<Zone> GetZones();

        public abstract List<Macro> GetMacros();

        public abstract Zone GetZone(string id);

        public abstract bool ExecuteAction(Device device, DeviceAction action);

        public abstract bool ExecuteMacro(Macro macro);

        #endregion


        public virtual List<Group<Device>> GetDevicesByCategory()
        {
            var groups = new Dictionary<DeviceCategory, Group<Device>>();
            var devices = this.GetDevices();

            Group<Device> group;
            foreach (var device in devices)
            {
                if (!groups.ContainsKey(device.DeviceCategory))
                    groups.Add(device.DeviceCategory, new Group<Device>(device.DeviceCategory));

                group= groups[device.DeviceCategory];
                group.Elements.Add(device);
            }

            // Order the group on the DeviceCategory then returns
            return groups
                .OrderBy(g => g.Value.Name)
                .Select(g => g.Value)
                .ToList();
        }

        public virtual void Connect(string serverID, string host, int port)
        {
            
        }

        public virtual void Clear()
        {

        }

        public virtual void Refresh()
        {

        }

        public virtual bool RefreshDevice(Device device)
        {
            return false;
        }
    }
}