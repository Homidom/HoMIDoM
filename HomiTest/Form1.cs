using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HomiTest.HmdService;

namespace HomiTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            WebServicesTraceExtension.MessageTraced += delegate(object sender, WebServicesTraceExtension.MessageTraceEventArgs e)
            {
                tb_xml.Text = e.Message;
                this.updateWebBrowser(e.Message);
                this.updateWebBrowser(e.Message);
            };
        }

        private HmdService.Server getServer()
        {
            var server = new HmdService.Server();
            server.Url = string.Format("http://{0}:{1}/ServiceModelSamples/service", this.tb_hostName.Text, this.nud_port.Value);
            return server;
        }

        private List<TemplateDevice> getDevices()
        {
            using (var server = this.getServer())
            {
                try
                {
                    return new List<TemplateDevice>(server.GetAllDevices(this.tb_key.Text));
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Error retrieving all devices", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        private void updateWebBrowser(string xml)
        {
            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);
            this.ctlDisplayXML1.DocumentXml = doc;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var devices = this.getDevices();
            if (devices != null)
            {
                this.lv_devices.Items.Clear();
                foreach (var device in devices)
                {
                    this.lv_devices.Items.Add(new ListViewItem(device._Name)
                    {
                        Tag = device._ID
                    });
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.updateWebBrowser(this.tb_xml.Text);
            this.tabControl1.SelectedTab = this.tabControl1.TabPages[0];
        }
    }
}
