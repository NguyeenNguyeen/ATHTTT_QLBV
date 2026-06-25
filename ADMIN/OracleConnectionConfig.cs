using System;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public static class OracleConnectionConfig
    {
        public static string Host => Environment.GetEnvironmentVariable("ATHTTT_ORACLE_HOST") ?? "127.0.0.1";
        public static string Port => Environment.GetEnvironmentVariable("ATHTTT_ORACLE_PORT") ?? "1521";
        public static string AdminServiceName => Environment.GetEnvironmentVariable("ATHTTT_ORACLE_ADMIN_SERVICE") ?? "orclpdb1";
        public static string LoginServiceName => Environment.GetEnvironmentVariable("ATHTTT_ORACLE_LOGIN_SERVICE") ?? "orclpdb1";
        public static string AdminUser => Environment.GetEnvironmentVariable("ATHTTT_ORACLE_ADMIN_USER") ?? "ADMIN_PHANHE1";
        public static string AdminPassword => Environment.GetEnvironmentVariable("ATHTTT_ORACLE_ADMIN_PASSWORD") ?? "Admin@123456";

        public static string BuildConnectionString(string userId, string password, string serviceName, bool sysdba = false)
        {
            var builder = new OracleConnectionStringBuilder
            {
                UserID = userId,
                Password = password,
                DataSource = $"(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={Host})(PORT={Port}))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={serviceName})))"
            };

            if (sysdba)
            {
                builder.DBAPrivilege = "SYSDBA";
            }

            return builder.ConnectionString;
        }

        public static string BuildAdminConnectionString()
        {
            return BuildConnectionString(AdminUser, AdminPassword, AdminServiceName);
        }
    }
}
