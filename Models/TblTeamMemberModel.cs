namespace CRM.Models
{
    public class TblTeamMemberModel
    {
        public int Id { get; set; }
        public int? MemberId { get; set; }
        public int? Team { get; set; }
    }
}
