namespace CRM.Models
{
    public class TblProjectDocument
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? FileExtension { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
