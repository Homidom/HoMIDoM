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
    [Activity(Label = "HoMIDroid - Zone")]
    class ZoneContent : ExpandableListActivity
    {
        public const string PARAMS_ID = "id";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            var server = TinyIoC.TinyIoCContainer.Current.Resolve<IHmdServer>();

            Zone zone = null;

            var zoneId = this.Intent.GetStringExtra(PARAMS_ID);
            if (!string.IsNullOrEmpty(zoneId))
                zone = server.GetZone(zoneId);

            this.ExpandableListView.Clickable = true;
            this.ExpandableListView.ChildClick = new ExpandableListView.ChildClickHandler(this.childClick);

            if (zone != null)
                this.SetListAdapter(new ZoneContentExpandableGroupAdapter(this, zone));
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