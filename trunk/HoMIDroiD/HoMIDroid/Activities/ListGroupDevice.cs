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
    [Activity(Label = "HoMIDroid - Devices", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation)]
    public class ListGroupDevice : ExpandableListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.ExpandableListView.Clickable = true;
            this.ExpandableListView.ChildClick += (sender, e) => { this.childClick(e.Parent, e.ClickedView, e.GroupPosition, e.ChildPosition, e.Id); };
            
            var app = TinyIoC.TinyIoCContainer.Current.Resolve<HmdApp>();
            app.RefreshData += app_RefreshData;

            this.refresh();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var app = TinyIoC.TinyIoCContainer.Current.Resolve<HmdApp>();
            app.RefreshData -= app_RefreshData;
        }

        private void app_RefreshData(object sender, EventArgs e)
        {
            this.refresh();
        }
        
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