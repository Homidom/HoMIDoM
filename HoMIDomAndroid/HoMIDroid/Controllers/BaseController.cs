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

namespace HoMIDroid.Controllers
{
    public abstract class BaseController
    {
        public Context Context { get; private set; }

        public BaseController(Context context)
        {
            this.Context = context;
        }

        public abstract View GetView();
        public abstract View GetListItemView(int viewId);
        public abstract View GetContextMenuView();

        public virtual bool Click() 
        {
            return false;
        }
    }
}