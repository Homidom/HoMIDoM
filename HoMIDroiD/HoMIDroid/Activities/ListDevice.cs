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
using TinyIoC;
using HoMIDroid.Server;
using HoMIDroid.Adapters;
using System.Threading;

namespace HoMIDroid
{
    [Activity(Label = "HoMIDroid - Devices", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class ListDevice : ListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var server = TinyIoC.TinyIoCContainer.Current.Resolve<IHmdServer>();
            var devices = server.GetDevices();

            this.ListAdapter = new DeviceAdapter(this, Resource.Layout.testItem, devices);

            this.ListView.TextFilterEnabled = true;
            this.ListView.Clickable = true;
            
        }
    }
}