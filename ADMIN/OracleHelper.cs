using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ADMIN
{
    public static class OracleHelper
    {
        public static readonly string AdminConnectionString = 
            @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl21)));";

        public static void ExecuteStoredProcedure(string procedureName, Dictionary<string, object> parameters)
        {
            using (OracleConnection conn = new OracleConnection(AdminConnectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var param in parameters)
                    {
                        if (param.Value is OracleParameter oraParam)
                            cmd.Parameters.Add(oraParam);
                        else
                            cmd.Parameters.Add(param.Key, OracleDbType.Varchar2).Value = param.Value ?? DBNull.Value;
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(AdminConnectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.Add(param.Key, OracleDbType.Varchar2).Value = param.Value ?? DBNull.Value;
                    }
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static DataTable ExecuteStoredProcedureWithCursor(string procedureName)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(AdminConnectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    OracleParameter pCursor = new OracleParameter("p_CURSOR", OracleDbType.RefCursor);
                    pCursor.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pCursor);
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}
