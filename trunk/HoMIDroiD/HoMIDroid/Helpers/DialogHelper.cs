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

namespace HoMIDroid.Helpers
{
    public class DialogHelper
    {
        public static void ShowConfirmationDialog(string title, string message, BaseObject item, Context context, Action<BaseObject> onAccept)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);

            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton("Oui", (sender, e) => { onAccept(item); });
            builder.SetNegativeButton("Non", (sender, e) => { /* close popup */ });

            var alert = builder.Create();
            alert.Show();
        }
    }
}