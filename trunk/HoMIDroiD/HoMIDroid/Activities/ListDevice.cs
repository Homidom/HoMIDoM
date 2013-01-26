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
    [Activity(Label = "HoMIDroid - Devices")]
    public class ListDevice : ListActivity
    {
        private ProgressDialog progressDialog = null;
        //private List<Device> devices;
        //private DeviceAdapter adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var server = TinyIoC.TinyIoCContainer.Current.Resolve<IHmdServer>();
            var devices = server.GetDevices();

            this.ListAdapter = new DeviceAdapter(this, Resource.Layout.testItem, devices);

            this.ListView.TextFilterEnabled = true;
            this.ListView.Clickable = true;
            
        }

        //protected override void OnListItemClick(ListView l, View v, int position, long id)
        //{
        //    base.OnListItemClick(l, v, position, id);
        //    Toast.MakeText(this, "OnListItemClick!", ToastLength.Short).Show();
        //}

        //void showToast(object sender, ItemEventArgs e)
        //{
        //    Toast.MakeText(this, "Hello", ToastLength.Short).Show();
        //}
        //private ListView getListView()
        //{
        //    return this.FindViewById<ListView>(Resource.Id.listView);
        //}

        //private void loadSync()
        //{
        //    var server = TinyIoCContainer.Current.Resolve<IHmdServer>();

        //    this.devices = server.GetDevices();
        //    this.adapter = new DeviceAdapter(this, Resource.Layout.OnOffDevice, this.devices);
        //    var listView = this.getListView();
        //    listView.Adapter = this.adapter;
        //}

        //private void loadAsync()
        //{
        //    //this.devices = new List<Device>();

        //    Thread t = new Thread(new ParameterizedThreadStart(delegate
        //    {
        //        var server = TinyIoCContainer.Current.Resolve<IHmdServer>();
        //        var dList = server.GetDevices();

        //        this.RunOnUiThread(delegate
        //        {
        //            //foreach (var device in dList)
        //            //    this.devices.Add(device);

        //            //this.adapter = new DeviceAdapter(this, Resource.Layout.OnOffDevice, this.devices);
        //            var adapter = new DeviceAdapter(this, Resource.Layout.OnOffDevice, dList);
        //            this.ListAdapter = adapter;
        //            this.ListView.TextFilterEnabled = true;
        //            this.ListView.Clickable = true;
        //            adapter.NotifyDataSetChanged();

        //            this.progressDialog.Dismiss();
        //        });
        //    }));

        //    t.Start();
        //    this.progressDialog = ProgressDialog.Show(this, "Please wait...", "Retrieving data ...", true);
        //}

    }
}