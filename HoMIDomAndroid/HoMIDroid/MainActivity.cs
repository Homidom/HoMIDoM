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
    [Activity(Label = "HoMIDroid")]
    public class MainActivity : TabActivity
    {
        public const string TAB_ZONE = "zone";
        public const string TAB_DEVICE = "device";

        TabHost.TabSpec zoneSpec;
        TabHost.TabSpec deviceSpec;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.createScreen();
        }
        
        private void createScreen()
        {
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Intent intent;
            TabHost.TabSpec spec;

            intent = new Android.Content.Intent().SetClass(this, typeof(ListZone));
            spec = this.TabHost.NewTabSpec(TAB_ZONE)
                .SetIndicator("Zone", Resources.GetDrawable(Resource.Drawable.ic_tab_zone))
                .SetContent(intent);
            this.TabHost.AddTab(spec);

            intent = new Android.Content.Intent().SetClass(this, typeof(ListGroupDevice));
            spec = this.TabHost.NewTabSpec(TAB_DEVICE)
                .SetIndicator("Device", Resources.GetDrawable(Resource.Drawable.ic_tab_zone))
                .SetContent(intent);
            this.TabHost.AddTab(spec);
        }

        
    }
}

