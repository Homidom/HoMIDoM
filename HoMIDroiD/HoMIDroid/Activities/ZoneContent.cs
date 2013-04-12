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
using HoMIDroid.Helpers;

namespace HoMIDroid.Activities
{
    [Activity(Label = "HoMIDroid - Zone", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class ZoneContent : ExpandableListActivity
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
            this.ExpandableListView.ChildClick += (sender, e) => { this.childClick(e.GroupPosition, e.ChildPosition); };

            if (zone != null)
                this.SetListAdapter(new ZoneContentExpandableGroupAdapter(this, zone));
        }

        private bool childClick(int groupPosition, int childPosition)
        {
            var item = this.ExpandableListAdapter.GetChild(groupPosition, childPosition) as BaseObject;
            if (item != null)
            {
                if (item.ConfirmClick)
                {
                    this.askForConfirmation(item, (obj) => { obj.GetController(this).Click(); });
                    return false;
                }
                else
                {
                    return item.GetController(this).Click();
                }
            }
            return false;
        }

        private void askForConfirmation(BaseObject item, Action<BaseObject> onAccept)
        {
            DialogHelper.ShowConfirmationDialog(
                "Veuillez confirmer",
                string.Format("Êtes-vous sûr de vouloir lancer la macro '{0}' ?", item.Name),
                item, this, onAccept
            );
        }
    }
}