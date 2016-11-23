//---------------------------------------------------------------------
// File: RestHTTPRequestResponseStepEx.cs
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
    /// The RestHTTPRequestResponseStep test step may be used to call a REST Web Service and optionally validate it's response.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep AssemblyPath="" TypeName="BizUnit.Extensions.SoapHttpRequestResponseStepEx">
    ///		<Url>http://machine/virdir/StockQuoteService.aspx?wsdl</Url>
    ///     <Method>POST</Method>
    ///     <ContentType>application/json</ContentType>
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
    ///			<term>Method</term>
    ///			<description>the HTTP verb</description>
    ///		</item>
    ///		<item>
    ///			<term>ContentType</term>
    ///			<description>The HTTP request content type</description>
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
    public class RestHttpRequestResponseStepEx : TestStepBase
    {
        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        private string method;

        public string Method
        {
            get { return method; }
            set { method = value; }
        }

        private string contentType;

        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
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
            HttpWebRequest webRequest = MakeHttpRequest(postData, url, method, contentType, context);
            bool ret = SaveResponse(webRequest, context);
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(url, "Url");
            ArgumentValidation.CheckForEmptyString(method, "Method");
            ArgumentValidation.CheckForEmptyString(contentType, "ContentType");
            ArgumentValidation.CheckForEmptyString(inputFile, "InputFile");
            //We dont need to check for the output file. - if there isnt one we just ignore
            if (outputFile.Length == 0)
                outputFile = Path.GetTempFileName();
        }

        #region "Private helper methods"
        private string MakePostData(string inputFile, Context context)
        {
            string postData = string.Empty;
            using (StreamReader reader = new StreamReader(inputFile))
            {
                postData = reader.ReadToEnd();
            }
            context.LogInfo("PostData has been created");
            return (postData);
        }
        private HttpWebRequest MakeHttpRequest(string postData, string uriString, string method, string contentType, Context context)
        {
            HttpWebRequest httpRequest = null;
            Uri uri = new Uri(uriString);
            httpRequest = (HttpWebRequest)WebRequest.Create(uri);

            byte[] bytes = Encoding.UTF8.GetBytes(postData);

            httpRequest.Method = method;
            httpRequest.ContentType = contentType;
            
            if (method != "GET")
            {
                httpRequest.ContentLength = postData.Length;
                Stream requestStream = httpRequest.GetRequestStream();
                context.LogInfo("making a Http request");
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
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

                using (StreamWriter sw = new StreamWriter(outputFile, false))
                {
                    // Add some text to the file.
                    sw.Write(strResponse);
                }
            }
            catch (WebException webEx)
            {
                LogRestException(outputFile, webEx, out responseBytes, out responseReader, context);

            }
            return (true);



        }
        private static void LogRestException(string outputFile, WebException webEx, out byte[] responseBytes, out StreamReader responseReader, Context context)
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
