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

namespace HoMIDroid.BO
{
    public abstract class DeviceAction
    {
        public virtual bool RequireInput { get; protected set; }
        public virtual string Caption { get; protected set; }
        public virtual string Key { get; protected set; }

        #region CTor
        public DeviceAction(string key)
        {
            this.Key = key;
        }

        public DeviceAction(string key, string caption)
            : this(key)
        {
            this.Caption = caption;
        } 
        #endregion

        public virtual bool Visit(Device device)
        {
            var app = TinyIoC.TinyIoCContainer.Current.Resolve<Application>();
            var server = TinyIoC.TinyIoCContainer.Current.Resolve<HoMIDroid.Server.IHmdServer>();
            if (server.ExecuteAction(device, this))
            {
                Toast.MakeText(app, string.Format("{0}: {1} ({2})", device.Name, this.Caption, device.NumericValue), ToastLength.Short).Show();
                Android.Util.Log.Debug("HoMIDroid.BO.DeviceAction", "device new value: {0}", device.NumericValue);
                return true;
            }
            return false;
        }

        public virtual HmdService.DeviceAction ToServerAction(Device device)
        {
            var action = new HmdService.DeviceAction();

            action.Nom = this.Key;
            action.Parametres = new HmdService.DeviceActionParametre[0];

            return action;
        }

        #region Static Actions
        public static T Get<T>() where T : DeviceAction, new()
        {
            var item = TinyIoC.TinyIoCContainer.Current.Resolve<T>();
            if (item == null)
            {
                item = new T();
                TinyIoC.TinyIoCContainer.Current.Register(item);
            }
            return item;
        }  
        #endregion
    }

    #region Action classes
    public class DimAction : DeviceAction
    {
        #region CTor
        public DimAction()
            : base("DIM")
        {
            this.Caption = "Modifier";
            this.RequireInput = true;
        }

        public DimAction(string caption)
            : base("DIM", caption)
        {
        } 
        #endregion

        public override bool Visit(Device device)
        {
            return base.Visit(device);
        }

        public override HmdService.DeviceAction ToServerAction(Device device)
        {
            var action = base.ToServerAction(device);

            action.Parametres = new HmdService.DeviceActionParametre[]
            {
                new HmdService.DeviceActionParametre() 
                {
                    Nom = "Variation",
                    Value = System.Convert.ToInt32(device.NumericValue)
                }
            };

            return action;
        }
    }

    public class UpAction : DimAction
    {
        #region CTor
        public UpAction()
            : base("Augmenter")
        {
        }

        public UpAction(string caption)
            : base(caption)
        {
        } 
        #endregion

        public override bool Visit(Device device)
        {
            device.NumericValue += 5;
            if (device.NumericValue > 100)
                device.NumericValue = 100;

            return base.Visit(device);
        }

        public override HmdService.DeviceAction ToServerAction(Device device)
        {
            var action = base.ToServerAction(device);
            var newValue = System.Convert.ToInt32(device.NumericValue);
            action.Parametres[0].Value = newValue;
            device.NumericValue = newValue;

            return action;
        }
    }

    public class DownAction : DimAction
    {
        #region CTor
        public DownAction()
            : base("Diminuer")
        {
        }

        public DownAction(string caption)
            : base(caption)
        {
        } 
        #endregion

        public override bool Visit(Device device)
        {
            device.NumericValue -= 5;
            if (device.NumericValue < 0)
                device.NumericValue = 0;

            return base.Visit(device);
        }

        public override HmdService.DeviceAction ToServerAction(Device device)
        {
            var action = base.ToServerAction(device);
            var newValue = System.Convert.ToInt32(device.NumericValue);
            action.Parametres[0].Value = newValue;
            device.NumericValue = newValue;

            return action;
        }
    }

    public class OnAction : DeviceAction
    {
        #region CTor
        public OnAction()
            : base("ON", "Allumer")
        {
        }

        public OnAction(string caption)
            : base("ON", caption)
        {
        } 
        #endregion

        public override bool Visit(Device device)
        {
            device.NumericValue = 100;
            return base.Visit(device);
        }
    }

    public class OffAction : DeviceAction
    {
        #region CTor
        public OffAction()
            : base("OFF", "Eteindre")
        {
        }

        public OffAction(string caption)
            : base("OFF", caption)
        {
        } 
        #endregion

        public override bool Visit(Device device)
        {
            device.NumericValue = 0;
            return base.Visit(device);
        }
    }

    public class RawAction : DeviceAction
    {
        public HmdService.DeviceAction Raw { get; private set; }

        #region CTor
        public RawAction(HmdService.DeviceAction action)
            : base(action == null ? "RAW" : action.Nom, action == null ? "RAW" : action.Nom)
        {
            this.Raw = action;
        }

        public RawAction(HmdService.DeviceAction action, string caption)
            : base(action == null ? "RAW" : action.Nom, caption)
        {
            this.Raw = action;
        }
        #endregion

        public override HmdService.DeviceAction ToServerAction(Device device)
        {
            return this.Raw;
        }
    }
    #endregion
}