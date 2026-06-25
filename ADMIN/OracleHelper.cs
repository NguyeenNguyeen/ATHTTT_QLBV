using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ADMIN
{
    public static class OracleHelper
    {
        public static readonly string AdminConnectionString = BuildAdminConnectionString();

        private static string BuildAdminConnectionString()
        {
            string explicitConn = Environment.GetEnvironmentVariable("ATBM_ADMIN_CONN") ?? "";
            if (!string.IsNullOrWhiteSpace(explicitConn))
                return explicitConn;

            string user = Environment.GetEnvironmentVariable("ATBM_DB_USER") ?? "ADMIN_PHANHE1";
            string password = Environment.GetEnvironmentVariable("ATBM_DB_PASSWORD") ?? "Admin@123456";
            string host = Environment.GetEnvironmentVariable("ATBM_DB_HOST") ?? "localhost";
            string port = Environment.GetEnvironmentVariable("ATBM_DB_PORT") ?? "1521";
            string service = Environment.GetEnvironmentVariable("ATBM_DB_SERVICE") ?? "orcl21";
            string sid = Environment.GetEnvironmentVariable("ATBM_DB_SID") ?? "";
            string connectData = string.IsNullOrWhiteSpace(sid)
                ? $"SERVICE_NAME={service}"
                : $"SID={sid}";

            return $"User Id={user};Password={password};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=({connectData})));";
        }

        public static void TestAdminConnection()
        {
            using (OracleConnection conn = new OracleConnection(AdminConnectionString))
            {
                conn.Open();
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

        public static void ExecuteNonQuery(string query, Dictionary<string, object> parameters = null, CommandType commandType = CommandType.Text)
        {
            using (OracleConnection conn = new OracleConnection(AdminConnectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    cmd.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.Add(param.Key, OracleDbType.Varchar2).Value = param.Value ?? DBNull.Value;
                    }
                    cmd.ExecuteNonQuery();
                }
            }
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
