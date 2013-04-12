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
using HoMIDroid.Server;
using HoMIDroid.BO;

namespace HoMIDroid.Activities
{
    [Activity(Label = "HoMIDroid - View device", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class DeviceOnOff : Activity
    {
        public const string PARAMS_ID = "id";
        public Device CurrentDevice { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.deviceOnOff);

            // Retrieve the device to display
            var server = TinyIoC.TinyIoCContainer.Current.Resolve<IHmdServer>();
            var deviceId = this.Intent.GetStringExtra(PARAMS_ID);
            if (!string.IsNullOrEmpty(deviceId))
                this.CurrentDevice = server.GetDevice(deviceId);

            // Handle button click events
            this.RegisterEvents();

            // Fill screen
            this.FillScreenData();
        }

        protected virtual void RegisterEvents()
        {
            ImageButton btnAction = (ImageButton)this.FindViewById(Resource.Id.deviceOnOff_btnAction);
            if (btnAction != null)
                btnAction.Click += new EventHandler(btnAction_Click);
        }

        protected virtual void FillScreenData()
        {
            this.FindViewById<TextView>(Resource.Id.deviceOnOff_txtName).Text = this.CurrentDevice.Name;
            ImageButton btnAction = (ImageButton)this.FindViewById(Resource.Id.deviceOnOff_btnAction);
            if (btnAction != null)
            {
                switch (this.CurrentDevice.DeviceCategory)
                {
                    case DeviceCategory.Light:
                        if (this.CurrentDevice.NumericValue == 0)
                            btnAction.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.light_bulb_off));
                        else
                            btnAction.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.light_bulb_on));
                        break;
                    default:
                        if (this.CurrentDevice.NumericValue == 0)
                            btnAction.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.switch_OFF));
                        else
                            btnAction.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.switch_ON));
                        break;
                }
            }
        }


        private void btnAction_Click(object sender, EventArgs e)
        {
            if (this.CurrentDevice == null)
                Toast.MakeText(this, "Unknow device", ToastLength.Long);
            else if (this.CurrentDevice.ExecuteDefaultAction())
                this.FillScreenData();
        }
    }
}