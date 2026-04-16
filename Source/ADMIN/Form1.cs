using System.Data;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = "User Id=SYSTEM;Password=oracle;Data Source=localhost:1521/XEPDB1";
        private static readonly Regex OracleIdentifierRegex = new("^[A-Za-z][A-Za-z0-9_$#]*$", RegexOptions.Compiled);

        public Form1()
        {
            InitializeComponent();

            
        }

        
    }
}
