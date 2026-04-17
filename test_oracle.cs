using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
namespace TestApp {
    class Program {
        static void Main() {
            string connStr = ""User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));"";
            using(var conn = new OracleConnection(connStr)) {
                conn.Open();
                using(var cmd = new OracleCommand(""ADMIN_PHANHE1.SP_XEM_ALL_NHANVIEN"", conn)) {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var pCursor = new OracleParameter(""p_CURSOR"", OracleDbType.RefCursor);
                    pCursor.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pCursor);
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd)) {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        foreach(DataColumn col in dt.Columns) {
                            Console.WriteLine(""Col: "" + col.ColumnName);
                        }
                        if(dt.Rows.Count > 0) {
                            var row = dt.Rows[dt.Rows.Count - 1]; // Let's see the last one
                            Console.WriteLine(""CMND: "" + row[""CMND""]);
                            Console.WriteLine(""Date: "" + row[""NgaySinh""]);
                        }
                    }
                }
            }
        }
    }
}
