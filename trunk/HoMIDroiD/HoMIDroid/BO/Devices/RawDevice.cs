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
    public class RawDevice : Device
    {
        public RawDevice()
        {
            this.DeviceType = BO.DeviceType.Other;
            this.DisplayType = BO.DisplayType.NoValue;
        }
    }
}