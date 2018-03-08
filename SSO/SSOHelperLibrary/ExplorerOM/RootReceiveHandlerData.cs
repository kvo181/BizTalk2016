using System.Data;
using System.Data.SqlClient;

namespace bizilante.SSO.Helper.ExplorerOM
{
    internal class RootReceiveHandlerData
    {
        internal DataTable table;

        internal SqlDataAdapter dataAdapter;

        private static RootReceiveHandlerData _instance = null;
        internal static RootReceiveHandlerData Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new RootReceiveHandlerData();
                return _instance;
            }
        }

        internal void SelectSchema(DataSet dataSet, SqlConnection connection)
        {
            if (dataSet.Tables.Contains("ReceiveHandler")) return;
            table = dataSet.Tables.Add("ReceiveHandler");
            string selectCommandText = "SELECT a.[Name] 'Adapter', h.[Name] 'Host',[uidCustomCfgID],[uidReceiveLocationSSOAppID] FROM[dbo].[adm_ReceiveHandler] r INNER JOIN BizTalkMgmtDb.dbo.adm_Adapter a on a.Id = r.AdapterId INNER JOIN BizTalkMgmtDb.dbo.adm_Host h on h.Id = r.HostId";
            dataAdapter = new SqlDataAdapter(selectCommandText, connection);
            dataAdapter.FillSchema(table, System.Data.SchemaType.Source);
            dataAdapter.Fill(table);
        }
    }
}
