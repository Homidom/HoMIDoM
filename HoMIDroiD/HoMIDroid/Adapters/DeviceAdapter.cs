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
    public class DeviceAdapter : ArrayAdapter<Device>
    {
        public List<Device> Items { get; private set; }

        public DeviceAdapter(Context context, int textViewResourceId, List<Device> items)
            : base(context, textViewResourceId)
        {
            this.Items = items;
            foreach (var item in this.Items)
                this.Add(item);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var device = this.GetItem(position);
            var view = convertView;

            if (view == null)
                return device.GetController(this.Context).GetListItemView(position);

            return view;
        }
    }
}