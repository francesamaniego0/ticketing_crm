using CRM.Models;

namespace CRM.Manager
{
    public class DBMethods
    {
        private readonly CRMContext _context;
        string sql = "";
        string Stats = "";
        string Mess = "";
        string JWT = "";
        DBManager db = new DBManager();
        // Inject DbContext through constructor
        public DBMethods(CRMContext context)
        {
            _context = context;
        }
        public string InsertAuditTrail(string actions, string datecreated, string module, string userid, string read)
        {
            string Insert = $@"INSERT INTO [dbo].[tbl_audittrail]
                           ([Actions]
                           ,[Module]
                           ,[DateCreated]
                           ,[UserId]
                           ,[status])
                         VALUES
                               ('" + actions + "'," +
                             "'" + module + "'," +
                             "'" + datecreated + "'," +
                             "'" + userid + "'," +
                              "'" + read + "') ";

            return db.DB_WithParam(Insert);
        }
    }
}
