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
using HoMIDroid.Server;

namespace HoMIDroid.Controllers
{
    public class MacroController : NamedController<Macro>
    {
        public View View { get; private set; }
        public Macro Macro { get { return this.Item; } }

        public MacroController(Context context, Macro macro)
            : base(context, macro)
        {
        }

        public override bool Click()
        {
            return this.Macro.Execute();
        }
    }
}