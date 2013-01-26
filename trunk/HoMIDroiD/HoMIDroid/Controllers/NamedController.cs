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

namespace HoMIDroid.Controllers
{
    public class NamedController<T> : BaseController where T: INamedObject
    {
        public T Item { get; private set; }

        public bool HasContextMenu { get; set; }

        public NamedController(Context context, T item)
            : base(context)
        {
            this.Item = item;
        }

        public override View GetView()
        {
            return this.GetListItemView(0);
        }

        public override View GetListItemView(int viewId)
        {
            TextView textView = new TextView(this.Context);
            textView.Text = this.Item.Name;
            textView.Id = viewId;
            textView.Gravity = GravityFlags.CenterVertical;

            textView.SetMinimumHeight(80);
            textView.TextSize = 25;
            return textView;
        }

        public override View GetContextMenuView()
        {
            return null;
        }
    }
}