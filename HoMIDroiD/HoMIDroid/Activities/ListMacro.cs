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
using HoMIDroid.Adapters;
using HoMIDroid.Helpers;

namespace HoMIDroid.Activities
{
    [Activity(Label = "HoMIDroid - Macros", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class ListMacro : ListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var server = TinyIoC.TinyIoCContainer.Current.Resolve<IHmdServer>();
            var macros = server.GetMacros();

            this.ListAdapter = new MacroAdapter(this, Resource.Layout.testItem, macros);

            this.ListView.TextFilterEnabled = true;
            this.ListView.Clickable = true;

            this.ListView.ItemClick += (sender, args) =>
            {
                var macro = this.ListAdapter.GetItem(args.Position) as Macro;
                if (macro != null)
                {
                    if (macro.ConfirmClick)
                        this.askForConfirmation(macro, (m) => { m.GetController(this).Click(); });
                    else
                        macro.GetController(this).Click();
                }
            };
        }

        private void askForConfirmation(Macro item, Action<BaseObject> onAccept)
        {
            DialogHelper.ShowConfirmationDialog(
                "Veuillez confirmer",
                string.Format("Êtes-vous sûr de vouloir lancer la macro '{0}' ?", item.Name),
                item, this, onAccept
            );
        }
    }
}