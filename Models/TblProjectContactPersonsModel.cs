namespace CRM.Models
{
    public class TblProjectContactPersonsModel
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? ContactPersonId { get; set; }
    }
}
