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
    public class DeviceController : NamedController<Device>, IDisposable
    {
        public View View { get; private set; }
        public Device Device { get { return this.Item; } }

        public DeviceController(Context context, Device device)
            : base(context, device)
        {
            device.ValueChanged += new EventHandler(device_ValueChanged);
        }


        public override View GetListItemView(int viewId)
        {
            //return base.GetListItemView(viewId);

            var layoutInflater = (LayoutInflater)this.Context.GetSystemService(Context.LayoutInflaterService);
            this.View = layoutInflater.Inflate(this.getViewLayout(this.Device), null);
            this.View.Id = viewId;

            // Configure the view
            this.updateView(this.View);
            this.registerEvents();

            // return
            return this.View;
        }

        public override bool Click()
        {
            var activityType = this.getActivityTypeForDetailedView();

            if (activityType != null)
            {
                this.startActivity(activityType, this.Device.Id);
                return true;
            }
            else if (this.Device.DefautAction != null)
            {
                return this.Device.DefautAction.Visit(this.Device);
            }

            Toast.MakeText(this.Context, "Pas de vue détaillée pour cet appareil.", ToastLength.Long);

            return false;
        }

        private Type getActivityTypeForDetailedView()
        {
            switch (this.Device.DeviceType)
            {
                case DeviceType.OnOff:
                    return typeof(DeviceOnOff);
                case DeviceType.Dim:
                    return typeof(DeviceDim);
            }

            //if (    this.Device.DeviceCategory == DeviceCategory.Light
            //    ||  this.Device.DeviceCategory == DeviceCategory.Switch
            //    ||  this.Device.DeviceCategory == DeviceCategory.Energy)
            //{
                
            //}
            //else
            //{

            //}

            return null; // No view available
        }

        private int getViewLayout(Device device)
        {
            switch (device.DeviceType)
            {
                case DeviceType.OnOff:
                    return Resource.Layout.ListItemOnOffDevice;
                default:
                    return Resource.Layout.ListItemBasicDevice;
            }
        }

        private void registerEvents()
        {
            if (this.Device.DeviceType == DeviceType.OnOff)
            {
                var btnView = this.View.FindViewById(Resource.Id.action) as ToggleButton;
                if (btnView != null)
                {
                    btnView.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
                    {
                        if (e.IsChecked)
                            this.Device.ExecuteAction<OnAction>();
                        else
                            this.Device.ExecuteAction<OffAction>();
                    };
                }
            }
        }

        private void updateView(View view)
        {
            if (view == null)
                return;

            var textView = view.FindViewById<TextView>(Resource.Id.name);
            if (textView != null)
            {
                textView.SetText(this.Device.Name, TextView.BufferType.Normal);
            }

            if (this.Device.DeviceType == DeviceType.OnOff)
            {
                var btnView = view.FindViewById(Resource.Id.action) as ToggleButton;
                if (btnView != null)
                    btnView.Checked = this.Device.NumericValue > 0;
            }
            else
            {
                var valueView = view.FindViewById(Resource.Id.valueText) as TextView;
                if (valueView != null)
                    valueView.SetText(this.Device.ValueFormatted, TextView.BufferType.Normal);
            }
        }

        private void startActivity(Type type, string id)
        {
            Bundle bundle = new Bundle();
            bundle.PutString(DeviceOnOff.PARAMS_ID, id);

            Intent intent = new Intent(this.Context, type);
            intent.PutExtras(bundle);

            this.Context.StartActivity(intent);
        }

        private void device_ValueChanged(object sender, EventArgs e)
        {
            if (this.View != null)
                this.updateView(this.View);
        }

        public void Dispose()
        {
            if (this.Device != null)
                this.Device.ValueChanged -= new EventHandler(device_ValueChanged);
        }
    }
}