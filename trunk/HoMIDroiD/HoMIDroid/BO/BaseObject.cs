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
using HoMIDroid.Controllers;

namespace HoMIDroid.BO
{
    public abstract class BaseObject : Java.Lang.Object
    {
        public abstract BaseController GetController(Context context);
    }
}