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
using System.Threading;
using TinyIoC;
using HoMIDroid.BO;

namespace HoMIDroid.Activities
{
    [Activity(Label = "HoMIDroid", MainLauncher = true, Icon = "@drawable/Icon", NoHistory=true)]
    public class Login : Activity
    {
        private ProgressDialog progressDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.Login);

            // Handle button click event
            Button loginBtn = (Button)this.FindViewById(Resource.Id.login_btn_login);
            if (loginBtn != null)
                loginBtn.Click += new EventHandler(loginBtn_Click);

            // Handle Enter key press on edit text
            EditText loginEditText = (EditText)this.FindViewById(Resource.Id.login_et_server);
            if (loginEditText != null)
            {
                loginEditText.KeyPress = (v, k, e) =>
                {
                    if (e.Action == KeyEventActions.Down && ((Keycode)k) == Keycode.Enter)
                    {
                        this.loginAsync();
                        return true;
                    }
                    return false;
                };
            }

            // Handle Enter key press on port
            EditText portEditText = (EditText)this.FindViewById(Resource.Id.login_et_server);
            if (loginEditText != null)
            {
                portEditText.KeyPress = (v, k, e) =>
                {
                    if (e.Action == KeyEventActions.Down && ((Keycode)k) == Keycode.Enter)
                    {
                        this.loginAsync();
                        return true;
                    }
                    return false;
                };
            }

            this.showPreferencePopup();
        }

        void loginBtn_Click(object sender, EventArgs e)
        {
            this.loginAsync();
        }

        private void loginAsync()
        {
            // this gets the resources in the xml file and assigns it to a local variable of type EditText
            var server = this.FindViewById<EditText>(Resource.Id.login_et_server).Text;
            var serverID = this.FindViewById<EditText>(Resource.Id.login_et_id).Text;
            var portString = this.FindViewById<EditText>(Resource.Id.login_et_port).Text;

            int port;
            if (string.IsNullOrEmpty(portString) || !int.TryParse(portString, out port))
                port = 8000;

            this.loginAsync(serverID, server, port);
        }

        private void loginAsync(string serverID, string server, int port)
        {
            // Save the login in preferences
            var settings = new Settings(this);
            var connections = settings.GetConnections();
            var existingPreference = connections.FirstOrDefault(p => p.Server == server);

            if (string.IsNullOrEmpty(existingPreference.Server))
            {
                connections.Add(new HmdPreferences()
                {
                    Server = server,
                    ServerID = serverID,
                    Port = port
                });
            }
            else
            {
                existingPreference.ServerID = serverID;
                existingPreference.Port = port;
            }

            settings.SaveConnections(connections);


            // Start connection
            Thread t = new Thread(new ParameterizedThreadStart(delegate
            {
                var hmdServer = TinyIoCContainer.Current.Resolve<IHmdServer>();
                var connectionState = true;
                var connectionError = string.Empty;
                try
                {
                    hmdServer.Connect(serverID, server, port);
                }
                catch (Exception exc)
                {
                    connectionError = exc.Message;
                    connectionState = false;
                }

                this.RunOnUiThread(delegate
                {
                    this.progressDialog.Dismiss();
                    if (!connectionState)
                    {
                        // Display the errror message to the user
                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        builder.SetTitle("Erreur de connexion");
                        builder.SetMessage(connectionError);
                        builder.SetCancelable(false);
                        builder.SetNeutralButton("Fermer", delegate(object sender, DialogClickEventArgs e)
                        {
                            
                        });
                        builder.Show();
                    }
                    else
                    {
                        // Start the main activity
                        //Intent intent = new Intent(this, typeof(MainActivity));
                        //this.StartActivity(intent);
                        this.refreshAsync();
                    }
                });
            }));

            this.progressDialog = ProgressDialog.Show(this, "Veuillez patienter", "Connexion...", true);
            t.Start();
        }

        private void refreshAsync()
        {
            Thread t = new Thread(new ParameterizedThreadStart(delegate
            {
                var hmdServer = TinyIoCContainer.Current.Resolve<IHmdServer>();
                var refreshState = true;
                var refreshError = string.Empty;
                try
                {
                    hmdServer.Refresh();
                }
                catch (Exception exc)
                {
                    refreshError = exc.Message;
                    refreshState = false;
                }

                this.RunOnUiThread(delegate
                {
                    this.progressDialog.Dismiss();
                    if (!refreshState)
                    {
                        // Display the errror message to the user
                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        builder.SetTitle("Erreur");
                        builder.SetMessage("Erreur lors de la récupération des données: " + refreshError);
                        builder.SetCancelable(false);
                        builder.SetNeutralButton("Fermer", delegate(object sender, DialogClickEventArgs e)
                        {

                        });
                        builder.Show();
                    }
                    else
                    {
                        // Start the main activity
                        Intent intent = new Intent(this, typeof(MainActivity));
                        this.StartActivity(intent);
                    }
                });
            }));

            this.progressDialog = ProgressDialog.Show(this, "Veuillez patienter", "Chargement...", true);
            t.Start();
        }

        private void showPreferencePopup()
        {
            var settings = new Settings(this);
            var connections = settings.GetConnections();

            if (connections.Count == 0)
                return; // not yet any items

            string[] items = connections.Select(p => p.Server + ":" + p.Port).ToArray();
            
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetTitle("Choix du serveur");
            builder.SetPositiveButton("Ajouter", delegate(object sender, DialogClickEventArgs e)
            {
                // Let the popup close and show the normal login form
            });
            builder.SetNegativeButton("Gérer", delegate(object sender, DialogClickEventArgs e)
            {
                this.openConnectionManager();
            });
            builder.SetItems(items, delegate(object sender, DialogClickEventArgs e)
            {
                var index     = (int)e.Which;
                var connection = connections[index];
                this.loginAsync(connection.ServerID, connection.Server, connection.Port);
            });

            var alert = builder.Create();
            alert.Show();
        }

        private void openConnectionManager()
        {
            Intent intent = new Intent(this, typeof(ConnectionManager));
            this.StartActivity(intent);
        }
    }
}