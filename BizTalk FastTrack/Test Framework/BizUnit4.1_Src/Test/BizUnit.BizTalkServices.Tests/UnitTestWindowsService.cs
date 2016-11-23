using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BizUnitWcfServiceLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.BizTalkServices.Tests
{
    /// <summary>
    /// Summary description for UnitTestWindowsService
    /// </summary>
    [TestClass]
    public class UnitTestWindowsService
    {
        public UnitTestWindowsService()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestStopSendPort()
        {
            const string machineName = "localhost";
            var proxy = Helper.BizUnitService(machineName);
            using (proxy as IDisposable)
            {
                try
                {
                    var step = new SendPortConductorStep
                                                     {
                                                         SendPortName = "HS_FileVerif_FILE",
                                                         Action = SendPortAction.Stop,
                                                         DelayForCompletion = 5
                                                     };
                    proxy.SendPortConductorStep(step);
                }
                catch (System.ServiceModel.FaultException fex)
                {
                    throw new Exception(fex.Message, fex);
                }
                catch (System.ServiceModel.CommunicationException cex)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        [TestMethod]
        public void TestStartSendPort()
        {
            const string machineName = "localhost";
            var proxy = Helper.BizUnitService(machineName);
            using (proxy as IDisposable)
            {
                try
                {
                    var step = new SendPortConductorStep
                    {
                        SendPortName = "HS_FileVerif_FILE",
                        Action = SendPortAction.Start,
                        DelayForCompletion = 5
                    };
                    proxy.SendPortConductorStep(step);
                }
                catch (System.ServiceModel.FaultException fex)
                {
                    throw new Exception(fex.Message, fex);
                }
                catch (System.ServiceModel.CommunicationException cex)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        [TestMethod]
        public void TestStartRemoteSendPort()
        {
            const string machineName = "99-001-097-V058";
            var proxy = Helper.BizUnitService(machineName);
            using (proxy as IDisposable)
            {
                try
                {
                    var step = new SendPortConductorStep
                    {
                        SendPortName = "HS_FileVerif_FILE",
                        Action = SendPortAction.Start,
                        DelayForCompletion = 5
                    };
                    proxy.SendPortConductorStep(step);
                }
                catch (System.ServiceModel.FaultException fex)
                {
                    throw new Exception(fex.Message, fex);
                }
                catch (System.ServiceModel.CommunicationException cex)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
