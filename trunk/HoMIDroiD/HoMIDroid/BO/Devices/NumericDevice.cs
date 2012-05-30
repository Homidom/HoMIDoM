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

namespace HoMIDroid.BO.Devices
{
    public class NumericDevice : Device
    {
        public override DeviceAction DefautAction
        {
            get
            {
                if (this.NumericValue == 0)
                    return this.Actions[2]; // ON
                return this.Actions[3]; // OFF
            }
        }

        public NumericDevice()
        {
            this.DeviceType = BO.DeviceType.Numeric;
            this.DisplayType = BO.DisplayType.Numeric;

            this.Actions = new List<DeviceAction>()
            {
                DeviceAction.Get<UpAction>(),
                DeviceAction.Get<DownAction>(),
                DeviceAction.Get<OnAction>(),
                DeviceAction.Get<OffAction>(),
                DeviceAction.Get<DimAction>()
            };
        }
    }
}