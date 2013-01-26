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
using HoMIDroid.Activities;

namespace HoMIDroid.Controllers
{
    public class ZoneController : NamedController<Zone>
    {
        public Zone Zone { get { return this.Item; } }

        public ZoneController(Context context, Zone zone)
            : base(context, zone)
        {

        }


        public override bool Click()
        {
            Bundle bundle = new Bundle();
            bundle.PutString(ZoneContent.PARAMS_ID, this.Zone.Id);

            // Start a new intend to display content of current Zone
            Intent intent = new Intent(this.Context, typeof(ZoneContent));
            intent.PutExtras(bundle);

            this.Context.StartActivity(intent);
            return true;
        }
    }
}