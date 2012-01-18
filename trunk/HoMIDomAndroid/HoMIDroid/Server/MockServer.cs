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
using System.Threading;
using HoMIDroid.BO.Devices;

namespace HoMIDroid.Server
{
    public class MockServer : BaseServer
    {
        List<Device> listDevices;
        List<Zone> listZones;
        
        public MockServer()
        {
            this.listDevices = this.createDevice();
            this.listZones = this.createZones();
        }
        #region IHmdServer Members

        public override Device GetDevice(string id)
        {
            return this.GetDevices().FirstOrDefault(d => d.Id == id);
        }

        public override List<Device> GetDevices()
        {
            return this.listDevices;
        }

        public override List<Device> GetDevicesInZone(Zone zone)
        {
            if (zone.Devices != null && zone.Devices.Count > 0)
                return zone.Devices;

            return new List<Device>();
        }

        public override List<Zone> GetZones()
        {
            return this.listZones;
        }

        public override Zone GetZone(string id)
        {
            return this.GetZones().FirstOrDefault(z => z.Id == id);
        }

        public override bool ExecuteAction(Device device, DeviceAction action)
        {
            return true;
        }
        #endregion


        private List<Zone> createZones() 
        {

            var list = new List<Zone>();

            list.Add(new Zone("e0be93af-b6dd-4519-9feb-6ba48f3fda09")
            {
                Name = "Zone #1",
                Devices = new List<Device>() 
                {
                    new OnOffDevice() { Name = "Ma Lampe #1 - Zone #1", DeviceType = DeviceType.OnOff, DisplayType = DisplayType.Boolean, NumericValue = 0, Id="1" },
                    new OnOffDevice() { Name = "Ma Lampe #2 - Zone #1", DeviceType = DeviceType.OnOff, DisplayType = DisplayType.Boolean, NumericValue = 1, Id="2" },
                    new NumericDevice() { Name = "Ma Lampe DIM - Zone #1", DeviceType = DeviceType.Dim, DisplayType = DisplayType.Percentage, NumericValue = 75, Id="3" },
                    new NumericDevice() { Name = "Ma Températur - Zone #1", DeviceType = DeviceType.Numeric, DisplayType = DisplayType.Boolean, NumericValue = 20.5, Id="4" }
                },
                SubZones = new List<Zone>()
                {
                    new Zone() 
                    {
                        Name = "Ma sous-zone",
                        Devices = new List<Device>() 
                        {
                            new OnOffDevice() { Name = "Ma Lampe #1 - SUB ZONE", DeviceType = DeviceType.OnOff, DisplayType = DisplayType.Boolean, NumericValue = 0, Id="5" }
                        }
                    }
                },
                Macros = new List<Macro>() 
                { 
                    new Macro() { Name = "Ma macro" }
                }
            });

            list.Add(new Zone("db951b15-f389-4d3d-96b2-965620093eaa")
            {
                Name = "Zone #2",
                Devices = new List<Device>() 
                {
                    new OnOffDevice() { Name = "Ma Lampe #1 - Zone #2", DeviceType = DeviceType.OnOff, DisplayType = DisplayType.Boolean, NumericValue = 1, Id="6" },
                    new OnOffDevice() { Name = "Ma Lampe #2 - Zone #2", DeviceType = DeviceType.OnOff, DisplayType = DisplayType.Boolean, NumericValue = 0, Id="7" },
                    new NumericDevice() { Name = "Ma Lampe DIM - Zone #2", DeviceType = DeviceType.Dim, DisplayType = DisplayType.Boolean, NumericValue = 25, Id="8" },
                    new NumericDevice() { Name = "Ma Température - Zone #2", DeviceType = DeviceType.Numeric, DisplayType = DisplayType.Boolean, NumericValue = 22, Id="9" }
                }
            });

            return list;
        }

        private List<Device> createDevice()
        {

            var list = new List<Device>();

            list.Add(new OnOffDevice()
            {
                Name = "Ma Lampe #1",
                DeviceType = DeviceType.OnOff,
                DisplayType = DisplayType.Boolean,
                NumericValue = 0,
                DeviceCategory = DeviceCategory.Light,
                Id = "1"
            });

            list.Add(new OnOffDevice()
            {
                Name = "Ma Lampe #2",
                DeviceType = DeviceType.OnOff,
                DisplayType = DisplayType.Boolean,
                NumericValue = 1,
                DeviceCategory = DeviceCategory.Light,
                Id = "2"
            });

            list.Add(new OnOffDevice()
            {
                Name = "Mon interrupteur #1",
                DeviceType = DeviceType.OnOff,
                DisplayType = DisplayType.Boolean,
                NumericValue = 1,
                DeviceCategory = DeviceCategory.Switch,
                Id = "3"
            });

            list.Add(new NumericDevice()
            {
                Name = "Ma Lampe DIM #1",
                DeviceType = DeviceType.Dim,
                DisplayType = DisplayType.Percentage,
                NumericValue = 0,
                DeviceCategory = DeviceCategory.Light,
                Id = "4"
            });

            list.Add(new NumericDevice()
            {
                Name = "Ma Lampe DIM #2",
                DeviceType = DeviceType.Dim,
                DisplayType = DisplayType.Percentage,
                NumericValue = 45,
                DeviceCategory = DeviceCategory.Light,
                Id = "5"
            });

            list.Add(new NumericDevice()
            {
                Name = "Ma Température",
                DeviceType = DeviceType.Numeric,
                DisplayType = DisplayType.Temperature,
                NumericValue = 20.5,
                DeviceCategory = DeviceCategory.Meteo,
                Id = "6"
            });


            list.Add(new OnOffDevice()
            {
                Name = "Mon interrupteur #2",
                DeviceType = DeviceType.OnOff,
                DisplayType = DisplayType.Boolean,
                NumericValue = 0,
                DeviceCategory = DeviceCategory.Switch,
                Id = "7"
            });


            list.Add(new NumericDevice()
            {
                Name = "Système audio XY",
                DeviceType = DeviceType.Dim,
                DisplayType = DisplayType.Numeric,
                NumericValue = 20,
                DeviceCategory = DeviceCategory.Multimedia,
                Id = "8"
            });

            list.Add(new NumericDevice()
            {
                Name = "Device à nom très long oui vraiment très long",
                DeviceType = DeviceType.Numeric,
                DisplayType = DisplayType.Numeric,
                NumericValue = 1102,
                DeviceCategory = DeviceCategory.Other,
                Id = "9"
            });

            return list;
        }
    }
}