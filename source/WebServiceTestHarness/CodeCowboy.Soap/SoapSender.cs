using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Net;

namespace CodeCowboy.Soap
{
    public class SoapSender
    {
        //Private
        private Object thisLock = new Object();
        private string m_soapContentType = "application/soap+xml; charset=utf-8";
        private string m_soapAction = string.Empty; //"http://www.server.com/rd/2011/03/IService/ServiceMethod";
        private List<string[]> m_parameters = new List<string[]>();


        //Constructor
        public SoapSender()
        {

        }


        public string WcfEndPoint { get; set; }
        public string SoapMessageTemplateFullPath { get; set; }
        public string SoapParameterFullPath { get; set; }
        public string SoapResponseFullPath { get; set; }
        public string SoapMessageTemplate { get; set; }


        public string SoapContentType
        {
            get { return m_soapContentType; }
            set { m_soapContentType = value; }
        }


        public string SoapAction
        {
            get { return m_soapAction; }
            set { m_soapAction = value; }
        }


        private void LoadSoapTemplate()
        {
            SoapMessageTemplate = File.ReadAllText(Path.Combine(SoapMessageTemplateFullPath));
        }


        private void LoadParameterFile()
        {
            m_parameters.Clear();
            foreach (string line in File.ReadLines(SoapParameterFullPath))
            {
                if (line != null)
                {
                    m_parameters.Add(line.Split(new char[] { ',' }));

                }
            }
        }


        private string CreateSoapMessage(string[] param)
        {
            StringBuilder sb = new StringBuilder(SoapMessageTemplate);
            sb.Replace("$(messageid)", Guid.NewGuid().ToString());
            int i = 1;
            foreach (string s in param)
            {
                string token = "$(" + i.ToString() + ")";
                sb.Replace(token, s);
                i++;
            }
            return sb.ToString();
        }

        private void Send(WebClient client, string soapMessage)
        {
            string response = String.Empty;
            try
            {
                response = client.UploadString(WcfEndPoint, soapMessage);
            }
            catch (Exception ex)
            {
                response = "<Exception>" + ex.Message + "</Exception>";
            }

            lock (thisLock)
            {
                using (StreamWriter sw = File.AppendText(SoapResponseFullPath))
                {
                    sw.WriteLine(response);
                }
            }
        }


        public void SendAll(int delayMilliSeconds)
        {
            LoadSoapTemplate();
            LoadParameterFile();
            //Statement below allows untrusted sites for WCF or HTTPClient
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            foreach (string[] param in m_parameters)
            {
                string soapMessage = CreateSoapMessage(param);
                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type", SoapContentType);
                    client.Headers.Add("SOAPAction", SoapAction);
                    Send(client, soapMessage);
                }
                Console.WriteLine(delayMilliSeconds); 
                Thread.Sleep(delayMilliSeconds);
            }
        }


        public void SendAll(int delayMinMilliSeconds, int delayMaxMilliseconds)
        {
            LoadSoapTemplate();
            LoadParameterFile();
            //Statement below allows untrusted sites for WCF or HTTPClient
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            foreach (string[] param in m_parameters)
            {
                string soapMessage = CreateSoapMessage(param);
                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type", SoapContentType);
                    client.Headers.Add("SOAPAction", SoapAction);
                    Send(client, soapMessage);
                }
                Random r = new Random(Guid.NewGuid().GetHashCode());
                int n = r.Next(delayMinMilliSeconds, delayMaxMilliseconds);
                Console.WriteLine(n);
                Thread.Sleep(n);
            }

        }


        public void SendAllMutiThreaded(int numThreads)
        {
            LoadSoapTemplate();
            LoadParameterFile();
            //Statement below allows untrusted sites for WCF or HTTPClient
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            try
            {
                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = numThreads };
                Parallel.ForEach(m_parameters, parallelOptions, param =>
                    {
                        string soapMessage = CreateSoapMessage(param);
                        using (var client = new WebClient())
                        {
                            client.Headers.Add("Content-Type", SoapContentType);
                            client.Headers.Add("SOAPAction", SoapAction);
                            Send(client, soapMessage);
                        }
                    }

                );
            }
            catch (AggregateException)
            {
                throw;
            }              

        }
    }


}
