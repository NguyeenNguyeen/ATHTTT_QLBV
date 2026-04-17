using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

class Program {
    static void Main() {
        string connStr = ""User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));"";
        using(var conn = new OracleConnection(connStr)) {
            conn.Open();
            using(var cmd = new OracleCommand(""SELECT * FROM ADMIN_PHANHE1.NHANVIEN WHERE HOTEN = 'Trần Thị B'"", conn)) {
                using(var reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        for(int i=0; i<reader.FieldCount; i++) {
                            Console.WriteLine(reader.GetName(i) + "": "" + reader[i]);
                        }
                    }
                }
            }
        }
    }
}
