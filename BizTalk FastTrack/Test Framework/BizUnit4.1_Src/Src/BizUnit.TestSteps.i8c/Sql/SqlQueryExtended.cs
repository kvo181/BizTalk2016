using System;
using System.Collections.ObjectModel;
using BizUnit.BizUnitOM;
using BizUnit.TestSteps.i8c.Common;
using BizUnit.TestSteps.Sql;

namespace BizUnit.TestSteps.i8c.Sql
{
    ///<summary>
    /// Database query definition
    ///</summary>
    public class SqlQueryExtended : SqlQuery
    {
        /// <summary>
        /// The parameters to substitute into the the cref="RawSqlQuery", 
        /// can also come from the context
        /// (these will override the value given via QueryParameters)
        /// </summary>
        public Collection<ParameterFromContext> QueryContextParameters { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SqlQueryExtended()
        {
            QueryContextParameters = new Collection<ParameterFromContext>();
        }

        ///<summary>
        /// Formats the query string, replacing the formatting instructions in cref="RawSqlQuery" with the parameters in cref="QueryParameters"
        ///</summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        ///<returns></returns>
        public override string GetFormattedSqlQuery(Context context)
        {
            if (QueryParameters.Count > 0)
            {
                var objParams = new object[QueryParameters.Count];
                int c = 0;

                foreach (var obj in QueryParameters)
                {
                    object objValue = obj.GetType() == typeof(ContextProperty) ? ((ContextProperty)obj).GetPropertyValue(context) : obj;

                    if (objValue.GetType() == typeof(DateTime))
                    {
                        // Convert to SQL Datetime
                        objParams[c++] = ((DateTime)objValue).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else
                    {
                        objParams[c++] = objValue;
                    }
                }

                foreach (var obj in QueryContextParameters)
                {
                    if (!context.ContainsKey(obj.Key)) continue;
                    var objValue = context.GetValue(obj.Key);
                    if (objParams.Length > obj.Index)
                        objParams[obj.Index] = objValue;
                }

                return string.Format(RawSqlQuery, objParams);
            }
            
            return RawSqlQuery;
        }
    }
}
