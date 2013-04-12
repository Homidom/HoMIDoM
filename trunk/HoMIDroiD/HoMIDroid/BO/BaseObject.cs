using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using HoMIDroid.Controllers;

namespace HoMIDroid.BO
{
    public abstract class BaseObject : Java.Lang.Object
    {
        /// <summary>
        /// Get/Set the name of this item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get if a click on this item must be confirmed by the user
        /// </summary>
        public virtual bool ConfirmClick { get { return false; } }

        /// <summary>
        /// Retrieve the controller that will handle any action on this item
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract BaseController GetController(Context context);
    }
}