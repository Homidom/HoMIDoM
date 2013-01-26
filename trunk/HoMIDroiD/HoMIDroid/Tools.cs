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

namespace HoMIDroid
{
    public class ObjectRef<T> : Java.Lang.Object
    {
        public ObjectRef(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }
}