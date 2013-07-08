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
using HoMIDroid.BO;
using HoMIDroid.Controllers;

namespace HoMIDroid.Adapters
{
    public class ZoneContentExpandableGroupAdapter : BaseExpandableGroupAdapter<BaseObject>
    {
        public Zone Zone { get; private set; }

        public ZoneContentExpandableGroupAdapter(Context context, Zone zone)
            : base(context, getZoneGroups(zone))
        {
            this.Zone = zone;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            var item = this.GetChild(groupPosition, childPosition) as BaseObject;
            var controller = item.GetController(this.Context);

            return controller.CreateListItemView(groupPosition * childPosition);

        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            var group= this.GetGroup(groupPosition) as Group<BaseObject>;
            var controller = group.GetController(this.Context);
            var view = controller.CreateListItemView(groupPosition);
            view.SetPadding(60, 0, 0, 0);
            return view;
        }

        
        private static List<Group<BaseObject>> getZoneGroups(Zone zone)
        {
            var list = new List<Group<BaseObject>>(10);

            if (zone.Devices != null)
            {
                var deviceGroup = new Group<BaseObject>(zone.Devices.Count + " Composant(s)");
                foreach (var d in zone.Devices)
                    deviceGroup.Elements.Add(d);
                list.Add(deviceGroup);
            }

            if (zone.Macros != null)
            {
                var macroGroup = new Group<BaseObject>(zone.Macros.Count + " Macro(s)");
                foreach (var m in zone.Macros)
                    macroGroup.Elements.Add(m);
                list.Add(macroGroup);
            }

            if (zone.SubZones != null)
            {
                var subZoneGroup = new Group<BaseObject>(zone.SubZones.Count + " Zone(s)");
                foreach (var z in zone.SubZones)
                    subZoneGroup.Elements.Add(z);
                list.Add(subZoneGroup);
            }

            return list;
        }

    }
}