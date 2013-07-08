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
using HoMIDroid;
using HoMIDroid.BO;
using HoMIDroid.Controllers;

namespace HoMIDroid.BO
{
    public class Device : BaseObject, INamedObject
    {
        #region Declarations
        private double? valueNumeric;
        private object value; 
        #endregion

        public event EventHandler ValueChanged;

        public string           ImageKey        { get; set; }
        public DeviceType       DeviceType      { get; set; }
        public DeviceCategory   DeviceCategory  { get; set; }
        public DisplayType      DisplayType     { get; set; }
        public string           Id              { get; set; }
        public bool             IsReadOnly      
        {
            get
            {
                return this.Name.StartsWith("HOMI_");
            }
        }
        public double?          NumericValue    
        {
            get { return this.valueNumeric; }
            set
            {
                if (this.valueNumeric != value)
                {
                    this.valueNumeric = value;
                    this.value = value;
                    this.OnValueChanged(EventArgs.Empty);
                }
            }
        }
        public object           Value           
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.valueNumeric = this.value as double?;
                this.OnValueChanged(EventArgs.Empty);
            }
        }
        public int              DeviceImage     { get; set; }

        public virtual List<DeviceAction> Actions   { get; protected set; }
        public virtual DeviceAction DefautAction    { get; protected set; }

        public string ValueFormatted
        {
            get
            {
                var numericValue = this.NumericValue;
                if (!numericValue.HasValue)
                    numericValue = 0;

                switch (this.DisplayType)
                {
                    case DisplayType.Boolean:
                        return numericValue.Value > 0 ? "On" : "Off";
                    case DisplayType.Integer:
                        return System.Convert.ToInt32(this.NumericValue.Value).ToString();
                    case DisplayType.Numeric:
                        return numericValue.Value.ToString();
                    case DisplayType.Percentage:
                        return numericValue.Value.ToString("#0.#") + "%";
                    case DisplayType.Temperature:
                        return numericValue.Value.ToString("#0.#°");
                    case DisplayType.NoValue:
                        return string.Empty;
                    case DisplayType.Text:
                        return this.Value == null ? string.Empty : this.Value.ToString();
                    default:
                        return System.Convert.ToInt32(numericValue.Value).ToString();
                }
            }
        }

        public Device()
        {
            this.Actions = new List<DeviceAction>();
        }

        public bool ExecuteAction<T>() where T: DeviceAction
        {
            var action = this.retrieveAction<T>();
            if (action != null)
                return action.Visit(this);
            return false;
        }
        public bool ExecuteDefaultAction()
        {
            if (this.DefautAction != null)
                return this.DefautAction.Visit(this);
            return false;
        }

        public bool On()
        {
            var action = this.retrieveAction<OpenAction>();
            if (action == null)
                action = this.retrieveAction<OnAction>();

            if (action != null)
                return action.Visit(this);
            return false;
        }

        public bool Off()
        {
            var action = this.retrieveAction<CloseAction>();
            if (action == null)
                action = this.retrieveAction<OffAction>();

            if (action != null)
                return action.Visit(this);
            return false;
        }
        
        public override Controllers.BaseController GetController(Context context)
        {
            return new DeviceController(context, this);
        }

        public void TriggerValueChanged()
        {
            this.OnValueChanged(EventArgs.Empty);
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (this.ValueChanged != null)
                this.ValueChanged(this, e);
        }

        #region Private methods
        private DeviceAction retrieveAction<T>() where T: DeviceAction
        {
            var q = from a in this.Actions
                    where a.GetType() == typeof(T)
                    select a;

            return q.FirstOrDefault();
        }
        #endregion
    }
}