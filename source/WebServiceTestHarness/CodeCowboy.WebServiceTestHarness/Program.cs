using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCowboy.Soap;

namespace CodeCowboy.WebServiceTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            SoapSender ss = new SoapSender();
            ss.WcfEndPoint = @"http://www.someserver.com/NumberService/NumberService.svc";
            ss.SoapAction = @"http://tempuri.org/INumberService/Add";
            ss.SoapContentType = @"text/xml; charset=utf-8";
            ss.SoapMessageTemplateFullPath = @"..\..\Samples\NumberServiceTestTemplate-01.xml";
            ss.SoapParameterFullPath = @"..\..\Samples\NumberServiceTestParameters-01.txt";
            ss.SoapResponseFullPath = @"..\..\Samples\NumberServiceTestResults-01.txt";
            //ss.SendAll(500); //delay 500 milliseconds
            //ss.SendAll(500, 5000);  //delay randomly between each request
            ss.SendAllMutiThreaded(5); //send request with multiple threads
        }
    }
}
