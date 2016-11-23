using System.Security.Principal;
using System.ServiceModel;
using BizUnit.BizTalkServices.Tests.BizUnitServiceReference;
using BizUnitWcfServiceLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BizUnit.TestSteps.BizTalk.Port;
using IBizUnitService = BizUnit.BizTalkServices.Tests.BizUnitServiceReference.IBizUnitService;

namespace BizUnit.BizTalkServices.Tests
{


    /// <summary>
    ///This is a test class for ReceivePortConductorStepTest and is intended
    ///to contain all ReceivePortConductorStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReceivePortConductorStepTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void GetDataTest()
        {
            var client = new BizUnitServiceClient();
            if (client.ClientCredentials != null)
                client.ClientCredentials.Windows.ClientCredential =
                    System.Net.CredentialCache.DefaultNetworkCredentials;
            Console.WriteLine(client.Endpoint.Address.Uri);
            Console.WriteLine(client.GetData(5));
        }

        /// <summary>
        ///A test for ReceivePortConductorStep Constructor
        ///</summary>
        [TestMethod()]
        public void ReceivePortConductorStepConstructorTest()
        {
            var client = GetBizUnitService();
            var step = new BizUnitServiceReference.ReceivePortConductorStep
            {
                               ReceivePortName = "HS_Horizon_StagingTables",
                               ReceiveLocationName = "HS_Horizon_C10_SQL",
                               Action = ReceivePortConductorStepReceivePortAction.Disable
                           };
            client.ReceivePortConductorStep(step);
        }

        static IBizUnitService GetBizUnitService()
        {
            var factory = new ChannelFactory<IBizUnitService>("bizUnitTcpEndPoint");
            if (factory.Credentials != null)
            {
                factory.Credentials.Windows.AllowedImpersonationLevel =
                    TokenImpersonationLevel.Impersonation;
                factory.Credentials.Windows.ClientCredential =
                    System.Net.CredentialCache.DefaultNetworkCredentials;
            }
            return factory.CreateChannel();
        }
    }
}
