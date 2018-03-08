using System.Data;
using System.Data.SqlClient;

namespace bizilante.SSO.Helper.ExplorerOM
{
    internal class RootSendHandlerData
    {
        internal DataTable table;

        internal SqlDataAdapter dataAdapter;

        private static RootSendHandlerData _instance = null;
        internal static RootSendHandlerData Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new RootSendHandlerData();
                return _instance;
            }
        }

        internal void SelectSchema(DataSet dataSet, SqlConnection connection)
        {
            if (dataSet.Tables.Contains("SendHandler")) return;
            table = dataSet.Tables.Add("SendHandler");
            string selectCommandText = "select a.[Name] 'Adapter', h.[Name] 'Host', IsDefault, uidCustomCfgID, uidTransmitLocationSSOAppId from adm_SendHandler sh inner join adm_Adapter a on sh.AdapterId = a.Id inner join adm_Host h on sh.HostId = h.Id";
            dataAdapter = new SqlDataAdapter(selectCommandText, connection);
            dataAdapter.FillSchema(this.table, System.Data.SchemaType.Source);
            dataAdapter.Fill(table);
        }
    }
}
