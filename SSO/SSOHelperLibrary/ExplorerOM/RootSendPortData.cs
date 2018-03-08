using System.Data;
using System.Data.SqlClient;

namespace bizilante.SSO.Helper.ExplorerOM
{
    internal class RootSendPortData
    {
        internal DataTable table;

        internal SqlDataAdapter dataAdapter;

        private static RootSendPortData _instance = null;
        internal static RootSendPortData Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new RootSendPortData();
                return _instance;
            }
        }

        internal void SelectSchema(DataSet dataSet, SqlConnection connection)
        {
            if (dataSet.Tables.Contains("SendPort")) return;
            table = dataSet.Tables.Add("SendPort");
            string selectCommandText = "select transportinfo.uidGUID, handler.uidTransmitLocationSSOAppId, sendport.nvcName as 'SendPort', app.nvcName as 'Application' from bts_sendport_transport transportinfo inner join adm_SendHandler handler on (transportinfo.nSendHandlerID = handler.Id) inner join bts_sendport sendport on transportinfo.nSendPortID = sendport.nID inner join bts_application app on sendport.nApplicationID = app.nID where transportinfo.bSSOMappingExists <> 0";
            dataAdapter = new SqlDataAdapter(selectCommandText, connection);
            dataAdapter.FillSchema(this.table, System.Data.SchemaType.Source);
            dataAdapter.Fill(table);
        }
    }
}
