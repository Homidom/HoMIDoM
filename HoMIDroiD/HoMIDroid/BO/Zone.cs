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
    public class Zone : BaseObject, INamedObject
    {
        /// <summary>
        /// Gets the internal unique identifier of this zone
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get
            {
                return string.Format(
                    "{0} appareils, {1} macros & {2} sous-zone",
                    this.Devices.Count,
                    this.Macros.Count,
                    this.SubZones.Count
                );
            }
        }

        /// <summary>
        /// Gets or sets the zone reference.
        /// </summary>
        /// <value>
        /// The reference.
        /// </value>
        public string Reference { get; set; }

        /// <summary>
        /// Gets the devices that belong to current zone.
        /// </summary>
        public List<Device> Devices { get; set; }

        /// <summary>
        /// Gets the sub-zones that belong to current zone.
        /// </summary>
        public List<Zone> SubZones { get; set; }

        /// <summary>
        /// Gets the macros that belong to current zone.
        /// </summary>
        public List<Macro> Macros { get; set; }


        public Zone()
            : this(Guid.NewGuid().ToString())
        {
        }
        public Zone(string id)
        {
            this.Id = id;
            this.Devices = new List<Device>();
            this.Macros = new List<Macro>();
        }

        #region Overriden methods
        public override bool Equals(object obj)
        {
            if (obj is Zone)
                return ((Zone)obj).Id == this.Id;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Id.ToString() + " - " + this.Name;
        } 
        #endregion


        public override Controllers.BaseController GetController(Context context)
        {
            return new ZoneController(context, this);
        }
    }
}