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
        List<Group<Device>> data;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.ExpandableListView.Clickable = true;
            this.ExpandableListView.ChildClick += (sender, e) => { this.childClick(e.GroupPosition, e.ChildPosition); };
            
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

        protected override void OnResume()
        {
            if (this.data != null)
                this.data.ForEach(g => g.Elements.ForEach(e => e.TriggerValueChanged()));
            base.OnResume();
        }

        private void app_RefreshData(object sender, EventArgs e)
        {
            this.refresh();
        }
        
        private void refresh()
        {
            var server = TinyIoC.TinyIoCContainer.Current.Resolve<IHmdServer>();
            this.data = server.GetDevicesByCategory();
            this.SetListAdapter(new DeviceExpandableGroupAdapter(this, this.data));
        }

        private bool childClick(int groupPosition, int childPosition)
        {
            var item = this.ExpandableListAdapter.GetChild(groupPosition, childPosition) as BaseObject;
            if (item != null)
                return item.GetController(this).Click();
            return false;
        }
    }
}