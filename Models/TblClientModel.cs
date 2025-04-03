namespace CRM.Models
{
    public class TblClientModel
    {
        public int Id { get; set; }
        public string? ClientName { get; set; }
        public string? Location { get; set; }
        public int? StatusId { get; set; }
        public bool? DeleteFlag { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
