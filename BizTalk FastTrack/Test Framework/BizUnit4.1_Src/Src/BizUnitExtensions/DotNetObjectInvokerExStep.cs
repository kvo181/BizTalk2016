//---------------------------------------------------------------------
// File: DotNetObjectInvokerExStep.cs
// 
// Summary: 
//
// Copyright (c) http://bizunitextensions.codeplex.com. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using BizUnit.Common;
using BizUnit.Xaml;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.Extensions
{
    /// <summary>
    /// The DotNetObjectInvokerStep is a sort of extension mechansim. It is used to call out to any .NET component that can accept
    /// requests and return responses. This allows developers to invoke their components from BizUnit and do stuff that is not 
    /// possible in BizUnit itself. The EXTENSION now provides the facility to use the context while setting the parameter values
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// In this example we are invoking a method called TestEndRequest3 in a component named ServiceLevelTracking
    /// and giving the invoker the fully qualified type name. The method TestEndRequest3 takes 2 Parameters, a string and an integer
    /// and we explicitly indicate this to the invoker.
    /// 
    /// <code escaped="true">
    ///	<TestStep AssemblyPath="" TypeName="BizUnit.Extensions.DotNetObjectInvokerStep">
    ///		<TypeName>WoodgroveBank.ServiceLevelTracking.ServiceLevelTracking, WoodgroveBank.ServiceLevelTracking, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a1054514fc67bded</TypeName>	
    ///		<AssemblyPath></AssemblyPath>
    ///		<MethodToInvoke>TestEndRequest3</MethodToInvoke>
    ///		<Parameter><string>fooBar</string></Parameter>
    ///		<Parameter><int>123</int></Parameter>
    ///		<ReturnParameter><int>barfoo</int></ReturnParameter>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>TypeName</term>
    ///			<description>The name of the type of the .Net object to invoke the method on. Note, if the type is GAC'd then the assembly name, version, public key etc need to be specified</description>
    ///		</item>
    ///		<item>
    ///			<term>AssemblyPath</term>
    ///			<description>The path to the assembly <para>(optional)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>MethodToInvoke</term>
    ///			<description>The name of the method to invoke</description>
    ///		</item>
    ///		<item>
    ///			<term>Parameter</term>
    ///			<description>The value for the parameter to pass into the method. Note: the format should be the serialised .Net type <para>(optional | multiple)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>ReturnParameter</term>
    ///			<description>The value returned from the method <para>(optional)</para></description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class DotNetObjectInvokerExStep : TestStepBase
    {

        //private methods
        private string typeName;

        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }
        private string assemblyPath;

        public string AssemblyPath
        {
            get { return assemblyPath; }
            set { assemblyPath = value; }
        }
        private string methodToInvoke;

        public string MethodToInvoke
        {
            get { return methodToInvoke; }
            set { methodToInvoke = value; }
        }
        private string returnParameter;

        public string ReturnParameter
        {
            get { return returnParameter; }
            set { returnParameter = value; }
        }
        private ArrayList parameters = new ArrayList();

        public ArrayList Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        static private object CreateObject(string typeName, string assemblyPath, Context context)
        {
            object comp = null;
            Type ty = null;
            context.LogInfo("About to create the folowing .Net type: {0}", typeName);

            try
            {


                if (assemblyPath != null && assemblyPath.Length != 0)
                {
                    context.LogInfo("Loading assembly form path: {0}", assemblyPath);

                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    ty = assembly.GetType(typeName, true, false);
                }
                else
                {
                    ty = Type.GetType(typeName);
                }

                if (ty != null)
                {
                    comp = Activator.CreateInstance(ty);
                }

                return comp;
            }
            catch (Exception ex)
            {
                context.LogException(ex);

                throw;
            }
        }

        public override void Execute(Context context)
        {
            //first validate
            Validate(context);

            object obj = CreateObject(typeName, AssemblyPath, context);

            MethodInfo mi = obj.GetType().GetMethod(MethodToInvoke);

            //find all the parameters of that method
            ParameterInfo[] pi = mi.GetParameters();
            //Now setup an array list containing all the parameters we need to pass to the method
            //We take the value from the parameter collection we built up - either from the XmlNodes
            //(which in turn may have been read from the context) or passed in directly as an ArrayList
            //but when placing them into the parameter list we convert them to the required type
            //We know the type by looking at the corresponding indexed item in the ParameterInfo[] array


            ArrayList parameterList = new ArrayList();
            try
            {
                for (int c = 0; c < pi.Length; c++)
                {
                    Type t = pi[c].ParameterType;
                    XmlSerializer xs = new XmlSerializer(t);
                    // WS/Benjy added test for < and > 
                    // WS removed deserializer. Read Parameters[c] as InnerText rather than InnerXML, so that strings can contain XML if needed.
                    parameterList.Add(Convert.ChangeType(Parameters[c], t));
                    //
                    //				parameterList.Add( xs.Deserialize(new XmlTextReader(StreamHelper.LoadMemoryStream(Parameters[c].InnerXml))) );
                }
            }
            catch (Exception e)
            {

                string exMsg = "There was an error when attempting to deserialize your Parameters. Please check they can be converted to the correct type for the method you are trying to call." + e.Message;
                context.LogException(new Exception(exMsg, e));
            }


            object[] paramsForCall = new object[parameterList.Count];
            for (int c = 0; c < parameterList.Count; c++)
            {
                paramsForCall[c] = parameterList[c];
            }

            context.LogInfo("About to call the method: {0}() on the type {1}", MethodToInvoke, typeName);
            // Call the .Net Object...
            object returnValue = mi.Invoke(obj, paramsForCall);
            context.LogInfo("Return value: {0}", returnValue);

            if ((ReturnParameter != null) && (ReturnParameter.Length > 0))
            {
                XmlSerializer xsRet = new XmlSerializer(returnValue.GetType());

                MemoryStream rs = new MemoryStream();
                xsRet.Serialize(new StreamWriter(rs), returnValue);
                MemoryStream es = StreamHelper.LoadMemoryStream(ReturnParameter);

                rs.Seek(0, SeekOrigin.Begin);
                es.Seek(0, SeekOrigin.Begin);
                StreamHelper.CompareXmlDocs(rs, es, context);
            }
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "TypeName");
            ArgumentValidation.CheckForEmptyString(methodToInvoke, "MethodToInvoke");
            ArgumentValidation.CheckForNullReference(parameters, "Parameters");
        }
    }
}
