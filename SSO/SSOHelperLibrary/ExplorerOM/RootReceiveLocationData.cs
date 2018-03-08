using System.Data;
using System.Data.SqlClient;

namespace bizilante.SSO.Helper.ExplorerOM
{
    internal class RootReceiveLocationData
    {
        internal DataTable table;

        internal SqlDataAdapter dataAdapter;

        private static RootReceiveLocationData _instance = null;
        internal static RootReceiveLocationData Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new RootReceiveLocationData();
                return _instance;
            }
        }

        internal void SelectSchema(DataSet dataSet, SqlConnection connection)
        {
            if (dataSet.Tables.Contains("ReceiveLocation")) return;
            table = dataSet.Tables.Add("ReceiveLocation");
            string selectCommandText = "select rxlocation.uidCustomCfgID, rxhandler.uidReceiveLocationSSOAppID, rxlocation.[Name] 'ReceiveLocation', rport.nvcName 'ReceivePort', app.nvcName 'Application' from adm_ReceiveLocation rxlocation inner join adm_ReceiveHandler rxhandler on (rxlocation.ReceiveHandlerId = rxhandler.Id) inner join bts_receiveport rport on rxlocation.ReceivePortId = rport.nID inner join bts_application app on rport.nApplicationID = app.nID where rxlocation.bSSOMappingExists <> 0";
            dataAdapter = new SqlDataAdapter(selectCommandText, connection);
            dataAdapter.FillSchema(this.table, System.Data.SchemaType.Source);
            dataAdapter.Fill(table);
        }
    }
}
