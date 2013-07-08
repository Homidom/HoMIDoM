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
            return this.CreateListItemView(0);
        }

        public override View CreateListItemView(int viewId)
        {
            TextView textView = new TextView(this.Context);
            this.updateView(viewId, textView);
            return textView;
        }

        public override void UpdateListItemView(int viewId, View view)
        {
            base.UpdateListItemView(viewId, view);
            this.updateView(viewId, (TextView)view);
        }

        public override View GetContextMenuView()
        {
            return null;
        }


        private void updateView(int viewId, TextView textView)
        {
            textView.Text = this.Item.Name;
            textView.Id = viewId;
            textView.Gravity = GravityFlags.CenterVertical;

            textView.SetMinimumHeight(80);
            textView.TextSize = 25;
        }
    }
}