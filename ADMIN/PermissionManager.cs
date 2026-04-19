using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ADMIN
{
    public class PermissionManager
    {
        private readonly string _connStr = OracleHelper.AdminConnectionString;

        /// <summary>
        /// Cấp quyền với support column-level, WITH GRANT OPTION
        /// Procedure SP_GRANT_ANY_OBJECT xử lý mọi loại object (TABLE, VIEW, PROCEDURE, FUNCTION)
        /// </summary>
        public void GrantPrivilege(string grantee, string privilege, string objectName, 
            string columns = "", bool withGrantOption = false)
        {
            using (OracleConnection conn = new OracleConnection(_connStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_GRANT_ANY_OBJECT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_GRANTEE", OracleDbType.Varchar2).Value = grantee;
                    cmd.Parameters.Add("p_PRIVILEGE", OracleDbType.Varchar2).Value = privilege;
                    cmd.Parameters.Add("p_OBJECT_NAME", OracleDbType.Varchar2).Value = objectName;
                    // Truyền NULL nếu columns rỗng (table-level), hoặc giá trị cột nếu cần column-level
                    cmd.Parameters.Add("p_COLUMNS", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(columns) ? null : (object)columns;
                    cmd.Parameters.Add("p_GRANT_OPTION", OracleDbType.Int32).Value = withGrantOption ? 1 : 0;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Cấp quyền cấp cột (Column-level privilege) - DEPRECATED
        /// Sử dụng GrantPrivilege() với tham số columns thay thế
        /// </summary>

        /// <summary>
        /// Thu hồi quyền từ user/role
        /// </summary>
        public void RevokePrivilege(string grantee, string privilege, string objectName)
        {
            using (OracleConnection conn = new OracleConnection(_connStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_REVOKE_PRIVILEGE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_GRANTEE", OracleDbType.Varchar2).Value = grantee;
                    cmd.Parameters.Add("p_PRIVILEGE", OracleDbType.Varchar2).Value = privilege;
                    cmd.Parameters.Add("p_OBJECT_NAME", OracleDbType.Varchar2).Value = objectName;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Cấp ROLE cho USER
        /// </summary>
        public void GrantRoleToUser(string roleName, string userName, bool withAdminOption = false)
        {
            using (OracleConnection conn = new OracleConnection(_connStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_GRANT_ROLE_TO_USER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_ROLE_NAME", OracleDbType.Varchar2).Value = roleName;
                    cmd.Parameters.Add("p_USER_NAME", OracleDbType.Varchar2).Value = userName;
                    cmd.Parameters.Add("p_ADMIN_OPTION", OracleDbType.Int32).Value = withAdminOption ? 1 : 0;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả User/Role (C## prefix)
        /// </summary>
        public List<string> GetAllGrantees()
        {
            List<string> grantees = new();
            using (OracleConnection conn = new OracleConnection(_connStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(
                    "SELECT USERNAME FROM DBA_USERS WHERE USERNAME LIKE 'C##%' ORDER BY USERNAME", conn))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            grantees.Add(reader[0].ToString());
                    }
                }
            }
            return grantees;
        }

        /// <summary>
        /// Lấy danh sách đối tượng theo loại (TABLE, VIEW, PROCEDURE, FUNCTION)
        /// </summary>
        public List<string> GetObjectNamesByType(string objectType)
        {
            List<string> objects = new();
            string query = objectType switch
            {
                "TABLE" => "SELECT TABLE_NAME FROM ALL_TABLES WHERE OWNER='ADMIN_PHANHE1' ORDER BY TABLE_NAME",
                "VIEW" => "SELECT VIEW_NAME FROM ALL_VIEWS WHERE OWNER='ADMIN_PHANHE1' ORDER BY VIEW_NAME",
                "PROCEDURE" or "FUNCTION" => $"SELECT OBJECT_NAME FROM ALL_OBJECTS WHERE OWNER='ADMIN_PHANHE1' AND OBJECT_TYPE='{objectType}' ORDER BY OBJECT_NAME",
                _ => ""
            };

            if (string.IsNullOrEmpty(query)) return objects;

            using (OracleConnection conn = new OracleConnection(_connStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            objects.Add(reader[0].ToString());
                    }
                }
            }
            return objects;
        }

        /// <summary>
        /// Lấy danh sách cột của bảng/view
        /// </summary>
        public List<string> GetColumnsOfTable(string tableName)
        {
            List<string> columns = new();
            using (OracleConnection conn = new OracleConnection(_connStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(
                    "SELECT COLUMN_NAME FROM ALL_TAB_COLUMNS WHERE OWNER='ADMIN_PHANHE1' AND TABLE_NAME=UPPER(:table_name) ORDER BY COLUMN_ID", conn))
                {
                    cmd.Parameters.Add("table_name", OracleDbType.Varchar2).Value = tableName;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            columns.Add(reader[0].ToString());
                    }
                }
            }
            return columns;
        }

        /// <summary>
        /// Xem quyền trên bảng
        /// </summary>
        public DataTable GetTablePrivileges()
        {
            return OracleHelper.ExecuteStoredProcedureWithCursor("ADMIN_PHANHE1.SP_XEM_QUYEN_ALL_USER");
        }

        /// <summary>
        /// Xem quyền trên cột
        /// </summary>
        public DataTable GetColumnPrivileges()
        {
            return OracleHelper.ExecuteStoredProcedureWithCursor("ADMIN_PHANHE1.SP_XEM_QUYEN_COT_ALL_USER");
        }

        /// <summary>
        /// Xem quyền trên view
        /// </summary>
        public DataTable GetViewPrivileges()
        {
            return OracleHelper.ExecuteStoredProcedureWithCursor("ADMIN_PHANHE1.SP_XEM_QUYEN_VIEW_ALL_USER");
        }

        /// <summary>
        /// Xem quyền trên procedure/function
        /// </summary>
        public DataTable GetProcedurePrivileges()
        {
            return OracleHelper.ExecuteStoredProcedureWithCursor("ADMIN_PHANHE1.SP_XEM_QUYEN_PROC_ALL_USER");
        }
    }
}
