using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ExampleWCFServiceApplication
{
    public class Service1 : IService1
    {
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            composite.Result = composite.FirstValue + composite.SecondValue;
            return composite;
        }
    }
}
