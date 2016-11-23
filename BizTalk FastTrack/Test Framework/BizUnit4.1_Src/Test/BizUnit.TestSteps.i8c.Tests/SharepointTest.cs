using System.Collections.Generic;
using BizUnit.TestSteps.i8c.Sharepoint;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.TestSteps.i8c.Tests
{
    [TestClass]
    public class SharepointTest
    {
        [TestMethod]
        public void PostToSharepoint()
        {
            TestCase btc = new TestCase()
            {
                Name = "Post a document to sharepoint",
                Description = "Check/Validate the Sharepoint steps",
                BizUnitVersion = "4.0.0.1"
            };

            var docToSharepoint = new PostDocumentToSharepointStep()
            {
                DocumentToUploadPath = "../../../../BizUnit4.0_Src/Test/BizUnit.TestSteps.i8c.Tests/TestData/Personeel_small.xlsx",
                DocumentName = "Personeel small",
                SharePointSite = @"http://SHP-2010001:99/sites/BizTalk",
                DocumentLibraryName = "BizTalk Document Library",
                ColumnParameters = new Dictionary<string, string> {{"Processing_x0020_State", "1"}, {"Validation_x0020_State", "1"}}
            };

            btc.ExecutionSteps.Add(docToSharepoint);

            BizUnit bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "DocumentToSharepoint.xaml");
            bu.RunTest();
        }
    }
}