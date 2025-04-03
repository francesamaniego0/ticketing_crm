namespace CRM.Models
{
    public class TblTeamModel
    {
        public int Id { get; set; }
        public string? TeamName { get; set; }
        public int? ClientId { get; set; }
        public int? SuperVisorId { get; set; }
        public bool DeleteFlag { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
