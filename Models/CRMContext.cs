
using Microsoft.EntityFrameworkCore;

namespace CRM.Models
{
    public class CRMContext : DbContext
    {
        public CRMContext(DbContextOptions<CRMContext> options)
        : base(options)
        {
        }

        public virtual DbSet<TblClientModel> TblClient { get; set; }
        public virtual DbSet<TblClientContactPersonsModel> TblClientContactPersons { get; set; }
        public virtual DbSet<TblTeamModel> TblTeam { get; set; }
        public virtual DbSet<TblTeamMemberModel> TblTeamMembers { get; set; }
        public virtual DbSet<TblProjectModel> TblProjects { get; set; }
        public virtual DbSet<TblProjectContactPersonsModel> TblProjectContactPersons { get; set; }
        public virtual DbSet<TblAudittrail> TblAudittrails { get; set; }
    }
}
