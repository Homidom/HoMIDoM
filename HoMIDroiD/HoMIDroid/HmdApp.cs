using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using TinyIoC;
using HoMIDroid.Server;

namespace HoMIDroid
{
    [Application]
    public class HmdApp : Application
    {
        #region CTor
        public HmdApp()
            : base()
        {

        }
        public HmdApp(IntPtr handle, Android.Runtime.JniHandleOwnership jniHandleOwnership)
            : base(handle, jniHandleOwnership)
        {
        } 
        #endregion

        public override void OnCreate()
        {
            base.OnCreate();

            initTinyIoC();
        }

        private void initTinyIoC()
        {
            TinyIoCContainer.Current.Register<Application>(this);
            TinyIoCContainer.Current.Register<HmdApp>(this);
            TinyIoCContainer.Current.Register<IHmdServer>(new MixedServer(new MockServer(), new RealServer()));
        }


        public event EventHandler<StartActivityEventArgs> StartActivityRequest;
        public event EventHandler<EventArgs> RefreshData;

        public class StartActivityEventArgs : EventArgs
        {
            public string Tab { get; set; }
            public Android.Content.Intent Intent { get; set; }
        }

        //public void RequestActivity(string tab, Android.Content.Intent intent)
        //{
        //    if (this.StartActivityRequest != null)
        //        this.StartActivityRequest(this, new StartActivityEventArgs() { Tab = tab, Intent = intent });
        //}
        public void ThrowRefreshData()
        {
            if (this.RefreshData != null)
                this.RefreshData(this, EventArgs.Empty);
        }
    }
}
