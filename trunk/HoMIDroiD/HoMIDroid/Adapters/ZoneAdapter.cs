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
    public class ZoneAdapter : ArrayAdapter<Zone>
    {
        public List<Zone> Items { get; private set; }

        public ZoneAdapter(Context context, int textViewResourceId, List<Zone> items)
            : base(context, textViewResourceId)
        {
            this.Items = items;
            foreach (var item in this.Items)
                this.Add(item);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var zone = this.GetItem(position);
            var view = convertView;

            if (view == null)
            {
                var dv = zone.GetController(this.Context);
                return dv.GetListItemView(position);
            }

            return view;
        }
    }
}