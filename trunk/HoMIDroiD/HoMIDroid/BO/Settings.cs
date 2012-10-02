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

namespace HoMIDroid.BO
{
    public class Settings
    {
        const string APP_SHARED_PREFS = "net.colsup.homidroid.connections_preferences"; //  Name of the file -.xml

        public Activity Activity { get; private set; }

        public Settings(Activity activity)
        {
            this.Activity = activity;
        }

        public List<HmdPreferences> GetConnections()
        {
            var preferences = this.Activity.GetSharedPreferences(APP_SHARED_PREFS, FileCreationMode.Private);

            var list = new List<HmdPreferences>();

            string server;
            string serverID;
            string portStr;
            int port;
            for (int i = 0; i < 100; i++)
            {
                server = preferences.GetString("Server" + i, string.Empty);
                serverID = preferences.GetString("ServerID" + i, string.Empty);
                portStr = preferences.GetString("Port" + i, string.Empty);

                if (!int.TryParse(portStr, out port))
                    port = 8000;

                if (!string.IsNullOrEmpty(server) && !string.IsNullOrEmpty(serverID))
                    list.Add(new HmdPreferences() { Server = server, ServerID = serverID, Port = port });
                else
                    break;
            }

            return list;
        }

        public bool SaveConnections(List<HmdPreferences> connections)
        {
            var preferences = this.Activity.GetSharedPreferences(APP_SHARED_PREFS, FileCreationMode.Private);
            var editor = preferences.Edit();

            // remove all existing connection
            editor.Clear();

            // Add the new connections
            for (int i = 0; i < connections.Count; i++)
            {
                editor.PutString("Server" + i, connections[i].Server);
                editor.PutString("ServerID" + i, connections[i].ServerID);
                editor.PutString("Port" + i, connections[i].Port.ToString());
            }

            return editor.Commit();
        }
       
    }

    public struct HmdPreferences
    {
        public string Server { get; set; }
        public string ServerID { get; set; }
        public int Port { get; set; }
    }
}