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
using HoMIDroid.BO.Devices;
using HoMIDroid.BO;

namespace HoMIDroid.Server
{
    public class RealServer : BaseServer
    {
        #region Declarations
        private HmdService.Server _server; 
        #endregion

        #region Properties
        public string Url { get; set; }

        public HmdService.Server Server
        {
            get
            {
                if (_server == null)
                {
                    _server = new HmdService.Server();
                    _server.Url = this.Url;
                }

                return _server;
            }
        }

        public string ServerID { get; private set; }
        #endregion

        #region CTor
        public RealServer()
            : this("123456789", "10.0.0.105")
        {
        }

        public RealServer(string serverID, string hostName, int port = 8000)
        {
            this.configure(hostName, port);
            this.ServerID = serverID;
        } 
        #endregion

        public override void Connect(string serverID, string hostName, int port)
        {
            this.ServerID = serverID;
            this.configure(hostName, port);

            // Try to connect
            this.Server.GetIdServer(serverID);
            
        }

        public override BO.Device GetDevice(string id)
        {
            return this.getDevice(this.Server.ReturnDeviceByID(this.ServerID, id));
        }

        public override List<BO.Device> GetDevices()
        {
            return this.getDevices(this.Server.GetAllDevices(this.ServerID));
        }

        public override List<BO.Device> GetDevicesInZone(BO.Zone zone)
        {
            return this.getDevices(this.Server.GetDeviceInZone(this.ServerID, zone.Reference));
        }

        public override List<BO.Zone> GetZones()
        {
            var zones = this.Server.GetAllZones(this.ServerID);
            if (zones == null)
                return new List<BO.Zone>();


            var list = new List<BO.Zone>(zones.Length);
            foreach (var z in zones)
            {
                list.Add(this.getZone(z));
            }

            return list;
        }

        public override BO.Zone GetZone(string id)
        {
            var zone = this.GetZones().FirstOrDefault(z => z.Id == id);

            if (zone.Devices == null || zone.Devices.Count == 0)
                zone.Devices = new List<Device>(this.GetDevicesInZone(zone));

            return zone;
        }

        public override bool ExecuteAction(HoMIDroid.BO.Device device, HoMIDroid.BO.DeviceAction action)
        {
            try
            {
                this.Server.ExecuteDeviceCommand(this.ServerID, device.Id, action.ToServerAction(device));

                //var updated = this.GetDevice(device.Id);
                //if (updated != null)
                //    device.Value = updated.Value;

                return true;
            }
            catch (Exception exc)
            {
                Android.Util.Log.Error("HoMIDroid.Server.RealServer", exc.ToString());
                return false;
            }
        }

        #region Private methods

        private void configure(string hostName, int port = 8000)
        {
            this._server = null; // reset current instance
            this.Url = string.Format("http://{0}:{1}/ServiceModelSamples/service", hostName, 8000);
        }
        private List<BO.Device> getDevices(HmdService.TemplateDevice[] list)
        {
            if (list == null)
                return new List<BO.Device>();

            var devices = new List<BO.Device>(list.Length);
            foreach (var d in list)
            {
                var device = this.getDevice(d);
                if (device != null)
                    devices.Add(device);
            }
            return devices;
        }
        private BO.Device getDevice(HmdService.TemplateDevice d)
        {
            var isDim = false;
            if (d._DeviceAction != null && d._DeviceAction.Length > 0)
            {// Search if an action is a DIM
                isDim = (d._DeviceAction.Count(a => (a.Nom == "DIM")) > 0);
            }

            switch (d._Type)
            {
                case HmdService.DeviceListeDevices.LAMPE:
                    if (isDim)
                        return this.getDimDevice(d, DeviceCategory.Light);
                    return this.getOnOffDevice(d, DeviceCategory.Light);

                case HmdService.DeviceListeDevices.SWITCH:
                    if (isDim)
                        return this.getDimDevice(d, DeviceCategory.Switch);
                    return this.getOnOffDevice(d, DeviceCategory.Switch);
            }
            return this.getRawDevice(d);
        }
        private BO.Zone getZone(HmdService.Zone z)
        {
            var zone = new BO.Zone(z._Id)
            {
                Name = z._Name,
                Reference = z._Id
            };

            //if (z._ListElement != null && z._ListElement.Length > 0)
            //{
            //    zone.SubZones = new List<Zone>(z._ListElement.Length);
            //    foreach (var subZone in z._ListElement)
            //    {
            //        subZone.
            //        zone.SubZones.Add(this.getZone(subZone));
            //    }
            //}
            return zone;
        }
        private List<BO.DeviceAction> getAction(HmdService.DeviceAction action)
        {
            var list = new List<BO.DeviceAction>(3);
            switch (action.Nom)
            {
                case "ON":
                    list.Add(DeviceAction.Get<OnAction>());
                    break;
                case "OFF":
                    list.Add(DeviceAction.Get<OffAction>());
                    break;

                case "DIM":
                    list.Add(DeviceAction.Get<UpAction>());
                    list.Add(DeviceAction.Get<DownAction>());
                    break;

                case "Read":
                    break;
                default:
                    list.Add(new RawAction(action));
                    break;
            }
            return null;
        }

        private RawDevice getRawDevice(HmdService.TemplateDevice d)
        {
            var device = new RawDevice();

            device.Id = d._ID;
            device.Name = d._Name;
            device.NumericValue = d._ValueDef;
            device.DeviceCategory = DeviceCategory.Other;
            device.DeviceType = DeviceType.Other;
            device.DisplayType = DisplayType.Text;

            if (d._DeviceAction != null && d._DeviceAction.Length > 0)
            {// Search if there is a DIM action, which means the device support DIM
                if (d._DeviceAction.Count(a => (a.Nom == "DIM")) > 0)
                {
                    device.DeviceType = DeviceType.Dim;
                }
                else if (d._DeviceAction.Count(a => (a.Nom == "ON")) > 0 && d._DeviceAction.Count(a => (a.Nom == "OFF")) > 0)
                {
                    device.DeviceType = DeviceType.OnOff;
                }
            }
            
            if (d._DeviceAction != null && d._DeviceAction.Length > 0)
            {
                foreach (var action in d._DeviceAction)
                {
                    device.Actions.Add(new RawAction(action));
                }
            }

            switch (d._Type)
            {
                //case HmdService.DeviceListeDevices.APPAREIL:
                //case HmdService.DeviceListeDevices.AUDIO:
                //case HmdService.DeviceListeDevices.BAROMETRE:
                //case HmdService.DeviceListeDevices.BATTERIE:
                //case HmdService.DeviceListeDevices.COMPTEUR:
                //case HmdService.DeviceListeDevices.CONTACT:
                //    break;
                case HmdService.DeviceListeDevices.DETECTEUR:
                    device.DisplayType = DisplayType.Boolean;
                    break;
                //case HmdService.DeviceListeDevices.DIRECTIONVENT:
                //    break;
                case HmdService.DeviceListeDevices.ENERGIEINSTANTANEE:
                case HmdService.DeviceListeDevices.ENERGIETOTALE:
                    device.DisplayType = DisplayType.Numeric;
                    break;
                //case HmdService.DeviceListeDevices.FREEBOX:
                //    break;
                case HmdService.DeviceListeDevices.GENERIQUEBOOLEEN:
                    device.DisplayType = DisplayType.Boolean;
                    break;
                //case HmdService.DeviceListeDevices.GENERIQUESTRING:
                //    break;
                case HmdService.DeviceListeDevices.GENERIQUEVALUE:
                    device.DisplayType = DisplayType.Numeric;
                    break;
                case HmdService.DeviceListeDevices.HUMIDITE:
                    device.DisplayType = DisplayType.Percentage;
                    break;
                //case HmdService.DeviceListeDevices.METEO:
                //case HmdService.DeviceListeDevices.MULTIMEDIA:
                //    break;
                case HmdService.DeviceListeDevices.PLUIECOURANT:
                case HmdService.DeviceListeDevices.PLUIETOTAL:
                    device.DisplayType = DisplayType.Numeric;
                    break;
                //case HmdService.DeviceListeDevices.TELECOMMANDE:
                //    break;
                case HmdService.DeviceListeDevices.TEMPERATURE:
                case HmdService.DeviceListeDevices.TEMPERATURECONSIGNE:
                    device.DisplayType = DisplayType.Temperature;
                    break;
                //case HmdService.DeviceListeDevices.UV:
                //case HmdService.DeviceListeDevices.VITESSEVENT:
                //case HmdService.DeviceListeDevices.VOLET:
                //    break;
            }

            return device;
        }
        private OnOffDevice getOnOffDevice(HmdService.TemplateDevice d, DeviceCategory deviceCategory)
        {
            return new OnOffDevice()
            {
                Id = d._ID,
                Name = d._Name,
                NumericValue = this.getValue(d._Value),
                DeviceCategory = deviceCategory
            };
        }
        private NumericDevice getDimDevice(HmdService.TemplateDevice d, DeviceCategory deviceCategory)
        {
            return new NumericDevice()
            {
                Id = d._ID,
                Name = d._Name,
                NumericValue = this.getValue(d._Value),
                DeviceCategory = deviceCategory,
                DeviceType = DeviceType.Dim,
                DisplayType = DisplayType.Percentage
            };
        }
        private double? getValue(object value)
        {
            if (value != null)
            {
                if (value is double)
                    return (double)value;
                if (value is int)
                    return Convert.ToDouble((int)value);

                double d;
                if (double.TryParse(value.ToString(), out d))
                    return d;
            }
            return null;
        }
        #endregion
    }
}