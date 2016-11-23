using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ExampleWCFServiceApplication
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations
    [DataContract]
    public class CompositeType
    {
        int firstValue;
        int secondValue;
        int result;

        [DataMember]
        public int FirstValue
        {
            get { return firstValue; }
            set { firstValue = value; }
        }

        [DataMember]
        public int SecondValue
        {
            get { return secondValue; }
            set { secondValue = value; }
        }

        [DataMember]
        public int Result
        {
            get { return result; }
            set { result = value; }
        }
    }
}
