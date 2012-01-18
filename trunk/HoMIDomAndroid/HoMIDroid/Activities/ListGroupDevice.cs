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
    [Activity(Label = "HoMIDroid - Devices")]
    public class ListGroupDevice : ExpandableListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.ExpandableListView.Clickable = true;
            this.ExpandableListView.ChildClick = new ExpandableListView.ChildClickHandler(this.childClick);

            this.refresh();
        }

        //protected override void OnResume()
        //{
        //    base.OnResume();
        //    this.refresh();
        //}

        private void refresh()
        {
            var server = TinyIoC.TinyIoCContainer.Current.Resolve<IHmdServer>();
            this.SetListAdapter(new DeviceExpandableGroupAdapter(this, server.GetDevicesByCategory()));

        }

        private bool childClick(ExpandableListView parent, View v, int groupPosition, int childPosition, long id)
        {
            var item = this.ExpandableListAdapter.GetChild(groupPosition, childPosition) as BaseObject;
            if (item != null)
                return item.GetController(this).Click();
            return false;
        }
    }
}