//---------------------------------------------------------------------
// File: SOAPHTTPRequestResponseStepEx.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c)BizUnit Extensions CodePlex
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit.Extensions
{

    /// <summary>
    /// The SOAPHTTPRequestResponseStep test step may be used to call a Web Service and optionally validate it's response.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep AssemblyPath="" TypeName="BizUnit.Extensions.SoapHttpRequestResponseStepEx">
    ///		<Url>http://machine/virdir/StockQuoteService.aspx?wsdl</Url>
    ///     <SoapAction>GetQuote</SoapAction>
    ///		<InputFile>c:\temp\stockinputrequest.xml</InputFile>
    ///		<OutputFile>c:\temp\stockserviceoutput.xml</OutputFile>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Url</term>
    ///			<description>The Url where the WSDL maybe obtained</description>
    ///		</item>
    ///		<item>
    ///			<term>SoapAction</term>
    ///			<description>the Soap Action corresponding to the web method/operation being invoked</description>
    ///		</item>
    ///		<item>
    ///			<term>WebMethod</term>
    ///			<description>The Web Method (opperation) to invoke.</description>
    ///		</item>
    ///		<item>
    ///			<term>InputFile</term>
    ///			<description>The full path to the file containing the soap Message to be posted without the "Envelope" stuff(</description>
    ///		</item>
    ///		<item>
    ///			<term>OutputFile</term>
    ///			<description>The full path to the file where the service response is to be stored. if not provided a temporary file name will be generated and used </description>
    ///		</item>
    ///		<item>
    ///			<term>ValidationStep</term>
    ///			<description>Optional validation step.</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	
    public class SoapHttpRequestResponseStepEx : TestStepBase
    {
        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        private string soapAction;

        public string SoapAction
        {
            get { return soapAction; }
            set { soapAction = value; }
        }
        private string inputFile;

        public string InputFile
        {
            get { return inputFile; }
            set { inputFile = value; }
        }
        private string outputFile;

        public string OutputFile
        {
            get { return outputFile; }
            set { outputFile = value; }
        }

        public override void Execute(Context context)
        {
            Validate(context);
            string postData = MakePostData(inputFile, context);
            HttpWebRequest webRequest = MakeHttpRequest(postData, url, soapAction, context);
            bool ret = SaveResponse(webRequest, context);

        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(url, "Url");
            ArgumentValidation.CheckForEmptyString(soapAction, "SoapAction");
            ArgumentValidation.CheckForEmptyString(inputFile, "InputFile");
            //We dont need to check for the output file. - if there isnt one we just ignore
            if (outputFile.Length == 0)
                outputFile = Path.GetTempFileName();
        }

        #region "Private helper methods"
        private string MakePostData(string inputFile, Context context)
        {
            string soapHeader = "<?xml version=\"1.0\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body>";
            string soapFooter = "</soap:Body></soap:Envelope>";
            string soapBody = "";
            // Read soapBody from file
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(inputFile);
            soapBody = xDoc.DocumentElement.OuterXml;
            string postData = soapHeader + soapBody + soapFooter;
            context.LogInfo("PostData has been created");
            return (postData);
        }
        private HttpWebRequest MakeHttpRequest(string postData, string uriString, string soapAction, Context context)
        {
            HttpWebRequest httpRequest = null;
            Uri uri = new Uri(uriString);
            httpRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpRequest.Headers.Add("SOAPAction", soapAction);
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            httpRequest.ContentType = "text/xml";
            httpRequest.Method = "POST";
            httpRequest.ContentLength = postData.Length;

            Stream requestStream = httpRequest.GetRequestStream();
            context.LogInfo("making a Http request");
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            return (httpRequest);
        }
        private bool SaveResponse(HttpWebRequest httpRequest, Context context)
        {
            HttpWebResponse response;
            byte[] responseBytes;
            StreamReader responseReader;
            string strResponse;
            context.LogInfo("Writing response to file");
            try
            {
                response = (HttpWebResponse)httpRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();

                responseReader = new StreamReader(responseStream, Encoding.UTF8);
                responseBytes = Encoding.UTF8.GetBytes(responseReader.ReadToEnd());
                strResponse = System.Text.Encoding.UTF8.GetString(responseBytes, 0, responseBytes.Length);

                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(strResponse);
                XmlNode myNode = xDoc.SelectSingleNode("/*[local-name()='Envelope']/*[local-name()='Body']");
                string msgResponse = myNode.InnerXml;
                using (StreamWriter sw = new StreamWriter(outputFile, false))
                {
                    // Add some text to the file.
                    sw.Write(msgResponse);
                }
            }
            catch (WebException webEx)
            {
                LogSoapException(outputFile, webEx, out responseBytes, out responseReader, context);

            }
            return (true);



        }
        private static void LogSoapException(string outputFile, WebException webEx, out byte[] responseBytes, out StreamReader responseReader, Context context)
        {
            Stream exceptionStream = webEx.Response.GetResponseStream();
            responseReader = new StreamReader(exceptionStream, Encoding.UTF8);
            responseBytes = Encoding.UTF8.GetBytes(responseReader.ReadToEnd());

            File.WriteAllBytes(outputFile + ".err", responseBytes);

            System.Text.StringBuilder message = new StringBuilder();

            message.AppendLine("Exception caught.");
            message.AppendLine(webEx.Message);

            if (webEx.InnerException != null)
            {
                message.AppendLine("Inner Exception Message is : ");
                message.AppendLine(webEx.InnerException.Message);
            }
            context.LogError(message.ToString());
            context.LogException(webEx);
        }


        #endregion
    }

}
