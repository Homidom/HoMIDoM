using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using HoMIDroid.Controllers;
using Android.Widget;
using Android.App;

namespace HoMIDroid.BO
{
    public class Macro : BaseObject, INamedObject
    {
        /// <summary>
        /// Gets the internal unique identifier of this zone
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the zone reference.
        /// </summary>
        /// <value>
        /// The reference.
        /// </value>
        public string Reference { get; set; }

        /// <summary>
        /// Get if a click on this item must be confirmed by the user
        /// </summary>
        public override bool ConfirmClick
        {
            get
            {
                return true;
            }
        }

        
        public Macro()
            : this(Guid.NewGuid().ToString())
        {
        }
        public Macro(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Retrieve the controller that will handle any action on this item
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Controllers.BaseController GetController(Context context)
        {
            return new MacroController(context, this);
        }


        public bool Execute()
        {
            var app = TinyIoC.TinyIoCContainer.Current.Resolve<Application>();
            var server = TinyIoC.TinyIoCContainer.Current.Resolve<HoMIDroid.Server.IHmdServer>();
            if (server.ExecuteMacro(this))
            {
                Toast.MakeText(app, string.Format("Macro '{0}' executée.", this.Name), ToastLength.Short).Show();
                return true;
            }
            else
            {
                Toast.MakeText(app, string.Format("Impossible d'executer la macro '{0}'.", this.Name), ToastLength.Short).Show();
                return false;
            }
        }
    }
}