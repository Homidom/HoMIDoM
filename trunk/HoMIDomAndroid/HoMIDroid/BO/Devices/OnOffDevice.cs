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
    public class OnOffDevice : Device
    {
        public override DeviceAction DefautAction
        {
            get
            {
                if (this.NumericValue == 0)
                    return this.Actions[0]; // ON
                return this.Actions[1]; // OFF
            }
        }

        public OnOffDevice()
        {
            this.DeviceType = BO.DeviceType.OnOff;
            this.DisplayType = BO.DisplayType.Boolean;

            this.Actions = new List<DeviceAction>()
            {
                DeviceAction.Get<OnAction>(),
                DeviceAction.Get<OffAction>()
            };
        }
    }
}