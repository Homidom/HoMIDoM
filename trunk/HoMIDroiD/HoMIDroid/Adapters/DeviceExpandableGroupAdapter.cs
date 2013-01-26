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
using Android.Util;

namespace HoMIDroid.Adapters
{
    public class DeviceExpandableGroupAdapter : BaseExpandableGroupAdapter<Device>
    {
        public DeviceExpandableGroupAdapter(Context context, List<Group<Device>> groups)
            : base(context, groups.OrderBy(g => g.Name).ToList())
        {
            
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            var device = this.GetChild(groupPosition, childPosition) as Device;
            var deviceView = new DeviceController(this.Context, device);

            return deviceView.GetListItemView(groupPosition * childPosition);

        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            var group= this.GetGroup(groupPosition) as Group<Device>;
            var view = group.GetController(this.Context).GetListItemView(groupPosition);

            view.SetPadding(60, 0, 0, 0);
            return view;
        }
    }
}