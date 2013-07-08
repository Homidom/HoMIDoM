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
    public class MacroAdapter : ArrayAdapter<Macro>
    {
        public List<Macro> Items { get; private set; }

        public MacroAdapter(Context context, int textViewResourceId, List<Macro> items)
            : base(context, textViewResourceId)
        {
            this.Items = items;
            foreach (var item in this.Items)
                this.Add(item);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var macro = this.GetItem(position);
            var view = convertView;
            var controller = macro.GetController(this.Context);

            if (view == null)
                return controller.CreateListItemView(position);
            else
                controller.UpdateListItemView(position, view);

            return view;
        }
    }
}