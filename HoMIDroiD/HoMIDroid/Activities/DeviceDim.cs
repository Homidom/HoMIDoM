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
using HoMIDroid.Controllers;
using HoMIDroid.Server;

namespace HoMIDroid.Activities
{
    [Activity(Label = "HoMIDroid - View DIM Device")]
    public class DeviceDim : Activity
    {
        public const string PARAMS_ID = "id";
        public Device CurrentDevice { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.deviceDim);

            // Retrieve the device to display
            var server = TinyIoC.TinyIoCContainer.Current.Resolve<IHmdServer>();
            var deviceId = this.Intent.GetStringExtra(PARAMS_ID);
            if (!string.IsNullOrEmpty(deviceId))
                this.CurrentDevice = server.GetDevice(deviceId);


            this.FillScreenData(true);
            // Handle button click events
            this.RegisterEvents();
        }

        protected void RegisterEvents()
        {
            ImageButton btnPlus = (ImageButton)this.FindViewById(Resource.Id.deviceDIM_btn_plus);
            if (btnPlus != null)
                btnPlus.Click += new EventHandler(btnPlus_Click);

            ImageButton btnMinus = (ImageButton)this.FindViewById(Resource.Id.deviceDIM_btn_minus);
            if (btnMinus != null)
                btnMinus.Click += new EventHandler(btnMinus_Click);

            SeekBar seekBar = (SeekBar)this.FindViewById(Resource.Id.deviceDIM_seekBar);
            if (seekBar != null)
            {
                var listener = new CustomSeekBarChangeListener();
                listener.StopTrackingTouch += new EventHandler(listener_StopTrackingTouch);
                seekBar.SetOnSeekBarChangeListener(listener);
            }

            ImageButton btnAction = (ImageButton)this.FindViewById(Resource.Id.deviceDim_btnAction);
            if (btnAction != null)
                btnAction.Click += new EventHandler(btnAction_Click);
        }

        void listener_StopTrackingTouch(object sender, EventArgs e)
        {
            SeekBar seekBar = (SeekBar)this.FindViewById(Resource.Id.deviceDIM_seekBar);
            this.CurrentDevice.NumericValue = seekBar.Progress;
            if (this.CurrentDevice.ExecuteAction<DimAction>())
                this.FillScreenData(false);
        }

        protected void FillScreenData(bool updateSeekBar)
        {
            // Set device name & description
            this.FindViewById<TextView>(Resource.Id.deviceDIM_txtName).Text = this.CurrentDevice.Name;

            // Set image button
            ImageButton btnAction = (ImageButton)this.FindViewById(Resource.Id.deviceDim_btnAction);
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

            // Set current value
            this.FindViewById<TextView>(Resource.Id.deviceDIM_txtPercent).Text = this.CurrentDevice.ValueFormatted;

            if (updateSeekBar) 
            {
                var seekBar = this.FindViewById<SeekBar>(Resource.Id.deviceDIM_seekBar);
                seekBar.Max = 100;
                seekBar.Progress = Convert.ToInt32(this.CurrentDevice.NumericValue);
            }
        }

        void btnPlus_Click(object sender, EventArgs e)
        {
            if (this.CurrentDevice == null)
                Toast.MakeText(this, "Unknow device", ToastLength.Long);
            else if (this.CurrentDevice.ExecuteAction<UpAction>())
                this.FillScreenData(true);
        }

        void btnMinus_Click(object sender, EventArgs e)
        {
            if (this.CurrentDevice == null)
                Toast.MakeText(this, "Unknow device", ToastLength.Long);
            else if (this.CurrentDevice.ExecuteAction<DownAction>())
                this.FillScreenData(true);

        }

        void btnAction_Click(object sender, EventArgs e)
        {
            if (this.CurrentDevice == null)
                Toast.MakeText(this, "Unknow device", ToastLength.Long);
            else if (this.CurrentDevice.ExecuteDefaultAction())
                this.FillScreenData(true);
        }


        #region private class CustomSeekBarChangeListener
        private class CustomSeekBarChangeListener : Java.Lang.Object, SeekBar.IOnSeekBarChangeListener
        {
            public event EventHandler ProgressChanged;
            public event EventHandler StartTrackingTouch;
            public event EventHandler StopTrackingTouch;

            public CustomSeekBarChangeListener()
            {
            }

            public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
            {
                if (this.ProgressChanged != null)
                    this.ProgressChanged(this, EventArgs.Empty);
            }

            public void OnStartTrackingTouch(SeekBar seekBar)
            {
                if (this.StartTrackingTouch != null)
                    this.StartTrackingTouch(this, EventArgs.Empty);
            }

            public void OnStopTrackingTouch(SeekBar seekBar)
            {
                if (this.StopTrackingTouch != null)
                    this.StopTrackingTouch(this, EventArgs.Empty);
            }
        } 
        #endregion
    }
}