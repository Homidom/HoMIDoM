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
using HoMIDroid.Controllers;

namespace HoMIDroid.BO
{
    public class Macro : BaseObject, INamedObject
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }


        public override Controllers.BaseController GetController(Context context)
        {
            return new NamedController<Macro>(context, this);
        }
    }
}