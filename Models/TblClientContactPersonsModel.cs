namespace CRM.Models
{
    public class TblClientContactPersonsModel
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
    }
}
