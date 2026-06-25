using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ADMIN
{
    public static class OracleHelper
    {
        public static readonly string AdminConnectionString = OracleConnectionConfig.BuildAdminConnectionString();

        public static void TestAdminConnection()
        {
            using (OracleConnection conn = new OracleConnection(AdminConnectionString))
            {
                conn.Open();
            }
        }

        public static void ExecuteNonQuery(string commandText, Dictionary<string, object> parameters = null, CommandType commandType = CommandType.Text)
        {
            using (OracleConnection conn = new OracleConnection(AdminConnectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(commandText, conn))
                {
                    cmd.BindByName = true;
                    cmd.CommandType = commandType;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(param.Key, OracleDbType.Varchar2).Value = param.Value ?? DBNull.Value;
                        }
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

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
