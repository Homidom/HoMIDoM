using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HoMIDroid.Adapters;
using HoMIDroid.Server;
using TinyIoC;

namespace HoMIDroid
{
    [Activity(Label = "HoMIDroid - Device Info", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class ListDevice2 : ListActivity
    {
        private ProgressDialog progressDialog = null;

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