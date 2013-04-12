using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using HoMIDroid.Activities;

namespace HoMIDroid
{
    [Activity(Label = "HoMIDroid", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class MainActivity : TabActivity
    {
        public const string TAB_ZONE = "zone";
        public const string TAB_DEVICE = "device";
        public const string TAB_MACRO = "macro";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.createScreen();
        }

        public override bool  OnCreateOptionsMenu(IMenu menu)
        {
            var refreshMenuItem = menu.Add(0, 1, 1, Resource.String.Refresh);
            var quitMenuItem = menu.Add(0, 1, 1, Resource.String.Quit);
            refreshMenuItem.SetIcon(Resource.Drawable.refresh);
            quitMenuItem.SetIcon(Resource.Drawable.logout);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 1:
                    var app = TinyIoC.TinyIoCContainer.Current.Resolve<HmdApp>();
                    app.ThrowRefreshData();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        
        private void createScreen()
        {
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.main);

            Intent intent;
            TabHost.TabSpec spec;

            intent = new Android.Content.Intent().SetClass(this, typeof(ListZone));
            spec = this.TabHost.NewTabSpec(TAB_ZONE)
                .SetIndicator("Zones", Resources.GetDrawable(Resource.Drawable.zone))
                .SetContent(intent);
            this.TabHost.AddTab(spec);

            intent = new Android.Content.Intent().SetClass(this, typeof(ListGroupDevice));
            spec = this.TabHost.NewTabSpec(TAB_DEVICE)
                .SetIndicator("Composants", Resources.GetDrawable(Resource.Drawable.composant))
                .SetContent(intent);
            this.TabHost.AddTab(spec);

            intent = new Android.Content.Intent().SetClass(this, typeof(ListMacro));
            spec = this.TabHost.NewTabSpec(TAB_MACRO)
                .SetIndicator("Macros", Resources.GetDrawable(Resource.Drawable.macro))
                .SetContent(intent);
            this.TabHost.AddTab(spec);
        }

        
    }
}

