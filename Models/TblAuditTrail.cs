namespace CRM.Models
{
    public class TblAudittrail
    {
        public int Id { get; set; }

        public string? Actions { get; set; }

        public string? Module { get; set; }

        public DateTime? DateCreated { get; set; }

        public string? UserId { get; set; }

        public int? Status { get; set; }
    }
}
