using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace HomiTest
{
    public class WebServicesTraceExtension : SoapExtension
    {
        Stream oldStream;
        Stream newStream;
        string filename;

        #region class MessageTraceEventArgs
		public class MessageTraceEventArgs : EventArgs 
        {
            public string Message {get; private set;}

            public MessageTraceEventArgs(string msg) 
            {
                this.Message = msg;
            }
        } 
	    #endregion

        public static event EventHandler<MessageTraceEventArgs> MessageTraced;


        // Save the Stream representing the SOAP request or SOAP response into
        // a local memory buffer.
        public override Stream ChainStream(Stream stream)
        {
            oldStream = stream;
            newStream = new MemoryStream();
            return newStream;
        }

        // When the SOAP extension is accessed for the first time, the XML Web
        // service method it is applied to is accessed to store the file
        // name passed in, using the corresponding SoapExtensionAttribute.   
        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return ((TraceExtensionAttribute)attribute).Filename;
        }

        // The SOAP extension was configured to run using a configuration file
        // instead of an attribute applied to a specific XML Web service
        // method.
        public override object GetInitializer(Type WebServiceType)
        {
            // Return a file name to log the trace information to, based on the
            // type.
            return "C:\\" + WebServiceType.FullName + ".log";
        }

        // Receive the file name stored by GetInitializer and store it in a
        // member variable for this specific instance.
        public override void Initialize(object initializer)
        {
            filename = (string)initializer;
        }

        //  If the SoapMessageStage is such that the SoapRequest or
        //  SoapResponse is still in the SOAP format to be sent or received,
        //  save it out to a file.
        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeSerialize:
                    break;
                case SoapMessageStage.AfterSerialize:
                    WriteOutput(message);
                    break;
                case SoapMessageStage.BeforeDeserialize:
                    WriteInput(message);
                    break;
                case SoapMessageStage.AfterDeserialize:
                    break;
                default:
                    throw new Exception("invalid stage");
            }
        }

        public void WriteOutput(SoapMessage message)
        {
            //this.LatestXML = new StringBuilder();

            newStream.Position = 0;
            MemoryStream ms = new MemoryStream();
            StreamWriter w = new StreamWriter(ms);

            //string soapString = (message is SoapServerMessage) ? "SoapResponse" : "SoapRequest";
            //w.WriteLine("-----" + soapString + " at " + DateTime.Now);
            //w.Flush();
            Copy(newStream, ms);
            w.Flush();

            newStream.Position = 0;
            Copy(newStream, oldStream);

            ms.Position = 0;
            var sr = new StreamReader(ms);
            this.OnMessageTraced(new MessageTraceEventArgs(sr.ReadToEnd()));

            w.Close();
        }

        public void WriteInput(SoapMessage message)
        {
            Copy(oldStream, newStream);
            MemoryStream ms = new MemoryStream();
            StreamWriter w = new StreamWriter(ms);

            //string soapString = (message is SoapServerMessage) ? "SoapRequest" : "SoapResponse";
            //w.WriteLine("-----" + soapString +
            //            " at " + DateTime.Now);
            //w.Flush();
            newStream.Position = 0;
            Copy(newStream, ms);

            ms.Position = 0;
            var sr = new StreamReader(ms);
            this.OnMessageTraced(new MessageTraceEventArgs(sr.ReadToEnd()));

            w.Close();
            newStream.Position = 0;
        }

        void Copy(Stream from, Stream to)
        {
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }

        protected virtual void OnMessageTraced(MessageTraceEventArgs e)
        {
            if (MessageTraced != null)
                MessageTraced(this, e);
        }
    }



    // Create a SoapExtensionAttribute for the SOAP Extension that can be
    // applied to an XML Web service method.
    [AttributeUsage(AttributeTargets.Method)]
    public class TraceExtensionAttribute : SoapExtensionAttribute
    {

        private string filename = "c:\\log.txt";
        private int priority;

        public override Type ExtensionType
        {
            get { return typeof(WebServicesTraceExtension); }
        }

        public override int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
            }
        }
    }
}