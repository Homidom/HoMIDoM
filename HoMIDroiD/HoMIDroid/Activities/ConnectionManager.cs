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
    [Activity(Label = "HoMIDroid - Connections", NoHistory = true)]
    public class ConnectionManager : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.SetContentView(Resource.Layout.Login);
            this.showDeletePreferencesPopup();
        }


        private void showDeletePreferencesPopup()
        {
            var settings = new Settings(this);

            var connections = settings.GetConnections();
            string[] items = connections.Select(p => p.Server + ":" + p.Port).ToArray();
            List<int> selected = new List<int>();
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetTitle("Supprimer des connexions");
            builder.SetPositiveButton("Annuler", delegate(object sender, DialogClickEventArgs e)
            {
                this.returnToLoginScreen();
            });
            builder.SetNegativeButton("Supprimer la selection", delegate(object sender, DialogClickEventArgs e)
            {
                if (selected.Count > 0)
                {
                    foreach (var index in selected.OrderByDescending(p => p))
                    {
                        connections.RemoveAt(index);
                    }
                    selected.Clear();
                    settings.SaveConnections(connections);
                }
                this.returnToLoginScreen();
            });

            var itemsSelected = new bool[items.Length];
            for (int i = 0; i < itemsSelected.Length; i++)
                itemsSelected[0] = false;
            builder.SetMultiChoiceItems(items, itemsSelected, delegate(object sender, DialogMultiChoiceClickEventArgs e)
            {
                var index = (int)e.Which;
                if (e.IsChecked && !selected.Contains(index))
                    selected.Add(index);
                else if (!e.IsChecked && selected.Contains(index))
                    selected.Remove(index);
            });

            var alert = builder.Create();
            alert.Show();
        }

        private void returnToLoginScreen()
        {
            Intent intent = new Intent(this, typeof(Login));
            this.StartActivity(intent);
        }
    }
}