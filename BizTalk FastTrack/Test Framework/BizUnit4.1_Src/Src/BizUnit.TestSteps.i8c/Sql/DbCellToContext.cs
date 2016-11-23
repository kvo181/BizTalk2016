
namespace BizUnit.TestSteps.i8c.Sql
{
    ///<summary>
    /// Database cell to be retrieve content from and put in the context
    ///</summary>
    public class DbCellToContext
    {
        ///<summary>
        /// The name of the cell to validate
        ///</summary>
        public string ColumnName { get; set; }

        ///<summary>
        /// the Key in the contet
        ///</summary>
        public string Key { get; set; }
    }
}
