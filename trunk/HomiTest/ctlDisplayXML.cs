using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Xsl;

namespace HomiTest
{
    public partial class ctlDisplayXML : UserControl
    {
        XmlDocument _doc;

        [DefaultValue(null)]
        public XmlDocument DocumentXml
        {
            get
            {
                return _doc;
            }
            set
            {
                _doc = value;
                if (_doc == null)
                {
                    this.webBrowser1.DocumentText = string.Empty;
                }
                else
                {
                    Stream s = this.GetType().Assembly.GetManifestResourceStream("HomiTest.defaultss.xsl");

                    XmlReader xr = XmlReader.Create(s);
                    XslCompiledTransform xct = new XslCompiledTransform();
                    xct.Load(xr);

                    StringBuilder sb = new StringBuilder();
                    XmlWriter xw = XmlWriter.Create(sb);
                    xct.Transform(_doc, xw);

                    this.webBrowser1.DocumentText = sb.ToString();
                }
            }
        }

        public ctlDisplayXML()
        {
            InitializeComponent();
        }
    }
}
