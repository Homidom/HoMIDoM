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
using HoMIDroid.Adapters;
using HoMIDroid.BO;

namespace HoMIDroid.Activities
{
    [Activity(Label = "HoMIDroid - Zones")]
    public class ListZone : ListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var server = TinyIoC.TinyIoCContainer.Current.Resolve<IHmdServer>();
            var zones = server.GetZones();

            this.ListAdapter = new ZoneAdapter(this, Resource.Layout.TestItem, zones);

            this.ListView.TextFilterEnabled = true;
            this.ListView.Clickable = true;

            this.ListView.ItemClick += delegate(object sender, Android.Widget.AdapterView.ItemClickEventArgs args)
            {
                var zone = this.ListAdapter.GetItem(args.Position) as Zone;
                if (zone != null)
                    zone.GetController(this).Click();
            };
        }
    }
}