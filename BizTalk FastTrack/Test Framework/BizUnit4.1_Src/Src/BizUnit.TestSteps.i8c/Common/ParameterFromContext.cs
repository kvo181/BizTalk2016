
namespace BizUnit.TestSteps.i8c.Common
{
    ///<summary>
    /// Some SQL query parameters can be found in the context
    ///</summary>
    public class ParameterFromContext
    {
        ///<summary>
        /// The index in the parameter collection
        ///</summary>
        public int Index { get; set; }

        ///<summary>
        /// the Key in the contet
        ///</summary>
        public string Key { get; set; }

        /// <summary>
        /// The optional datatype of the Key.
        /// (we use it to convert Guid to its default string format "D", without brackets)
        /// </summary>
        public string DataType { get; set; }
    }
}
