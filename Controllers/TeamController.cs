using CRM.Manager;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CRM.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TeamController : Controller
    {
        private readonly CRMContext _context;
        DBManager db = new DBManager();
        private readonly DBMethods dbmet;
        private readonly ExternalApiService _externalApiService;

        public TeamController(CRMContext context, DBMethods _dbmet, ExternalApiService externalApiService)
        {
            _context = context;
            this.dbmet = _dbmet;
            _externalApiService = externalApiService;
        }
        public class TeamParam
        {
            public int? Id { get; set; }
            public int? UserId { get; set; }
            public string? Name { get; set; }
            public int page { get; set; }

        }
        public class TeamPaginateModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<TeamModel> items { get; set; } = new List<TeamModel>(); // Prevent null reference
        }
        public class TeamModel
        {
            public int? TeamId { get; set; }
            public string? TeamName { get; set; }
            public int? SuperVisorId { get; set; }
            public string? SuperVisorName { get; set; }
            public int? ClientId { get; set; }
            public string? ClientName { get; set; }
            public List<TeamMemberModel> Members { get; set; } = new List<TeamMemberModel>(); // Prevent null reference
        }
        public class TeamMemberModel
        {
            public int? MemberId { get; set; }
            public string? MemberName { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> PostList(TeamParam data)
        {
            List<GetAllUserDetailsResult> apiUsers = await _externalApiService.FetchDataFromExternalApi();
            var dbteam = _context.TblTeam.OrderByDescending(a => a.Id).ToList();
            var dbteammember = _context.TblTeamMembers.OrderByDescending(a => a.Id).ToList();
            var dbclient = _context.TblClient.OrderByDescending(a => a.Id).ToList();
            // Step 3: Join API data with database data
            int pageSize = 25;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
            string page_size = pageSize == 0 ? "10" : pageSize.ToString();
            try
            {
                var result = from team in dbteam
                             join apiUser in apiUsers
                             on team.SuperVisorId equals apiUser.Id into apiUserGroup
                             from apiUser in apiUserGroup.DefaultIfEmpty() // Left Join with API Users

                             join member in dbteammember
                             on team.Id equals member.Team into memberGroup
                             from member in memberGroup.DefaultIfEmpty() // Left Join with Team Members

                             join memberName in apiUsers
                             on member != null ? member.MemberId : null equals memberName.Id into memberNameGroup
                             from memberName in memberNameGroup.DefaultIfEmpty() // Left Join with API Users for Member Details

                             join clientName in dbclient
                             on team != null ? team.ClientId : null equals clientName.Id into clientNameGroup
                             from clientName in clientNameGroup.DefaultIfEmpty() // Left Join with Clients

                             where team.DeleteFlag == false  // Filtering condition

                             group new
                             {
                                 MemberId = member?.MemberId, // Keep null if no member exists
                                 MemberName = memberName?.Fullname, // Keep null if no name exists
                             }
                             by new
                             {
                                 TeamId = team?.Id,
                                 TeamName = team?.TeamName,
                                 SuperVisorId = team?.SuperVisorId,
                                 SuperVisorName = apiUser?.Fullname,
                                 ClientId = clientName?.Id,
                                 ClientName = clientName?.ClientName
                             } into teamGroup
                             select new TeamModel // ✅ Explicitly map to TeamModel
                             {
                                 TeamId = teamGroup.Key.TeamId,
                                 TeamName = teamGroup.Key.TeamName,
                                 SuperVisorId = teamGroup.Key.SuperVisorId,
                                 SuperVisorName = teamGroup.Key.SuperVisorName,
                                 ClientId = teamGroup.Key.ClientId,
                                 ClientName = teamGroup.Key.ClientName,
                                 Members = teamGroup
                                            .Where(m => m.MemberId != null) // Exclude null members
                                            .Select(m => new TeamMemberModel
                                            {
                                                MemberId = m.MemberId ?? 0,
                                                MemberName = m.MemberName ?? "Unknown",
                                            }).ToList()
                             };
                // **Add condition dynamically if `data` is not null**
                if (data.Id != null && data.Id != 0)
                {
                    result = result.Where(t => t.TeamId == data.Id);
                }
                if (data.Name != null && data.Name != "")
                {
                    result = result.Where(t => t.TeamName.ToLower().Contains(data.Name.ToLower()));
                }
                // Convert to a List
                var finalResult = result.ToList();
                totalItems = finalResult.Count;
                totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));
                items = finalResult.Skip((data.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                int pages = data.page == 0 ? 1 : data.page;
                var results = new List<TeamPaginateModel>();
                var item = new TeamPaginateModel();
                item.CurrentPage = data.page == 0 ? "1" : data.page.ToString();
                int page_prev = pages - 1;
                double t_records = Math.Ceiling(double.Parse(totalItems.ToString()) / double.Parse(page_size));
                int page_next = data.page >= t_records ? 0 : pages + 1;
                item.NextPage = items.Count % int.Parse(page_size) >= 0 ? page_next.ToString() : "0";
                item.PrevPage = pages == 1 ? "0" : page_prev.ToString();
                item.TotalPage = t_records.ToString();
                item.PageSize = page_size;
                item.TotalRecord = totalItems.ToString();
                item.items = items;
                results.Add(item);
                return Ok(results);
                //return Ok(finalResult);
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR:"+ ex);
            }
            
        }
        
        public class TeamResponse
        {
            public int Id { get; set; }
            public string TeamName { get; set; }
            public int? SuperVisorId { get; set; }
            public int? ClientId { get; set; }
            public int? CreatedBy { get; set; }
            public List<TblTeamMemberModel> TeamMembers { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<TblTeamModel>> Save(TeamResponse data)
        {
            string status = "";
           
            try
            {

                if (data.Id == 0)
                {
                    bool hasDuplicateOnSave = (_context.TblTeam?.Any(a => a.TeamName == data.TeamName && a.DeleteFlag == false)).GetValueOrDefault();
                    
                    if (hasDuplicateOnSave)
                    {
                        return Conflict("Entity already exists");
                    }
                    // Insert Team
                    var team = new TblTeamModel
                    {
                        TeamName = data.TeamName,
                        SuperVisorId = data.SuperVisorId,
                        DeleteFlag = false, // Default Active
                        CreatedBy = data.CreatedBy,
                        DateCreated = DateTime.Now,
                        ClientId = data.ClientId
                    };
                    _context.TblTeam.Add(team);
                    await _context.SaveChangesAsync();
                    var members = data.TeamMembers.Select(member => new TblTeamMemberModel
                    {
                        MemberId = member.MemberId,
                        Team = team.Id, // Foreign Key (After saving team)
                    }).ToList();

                    _context.TblTeamMembers.AddRange(members);
                    await _context.SaveChangesAsync();
                    status = "Team successfully saved";
                    dbmet.InsertAuditTrail("Save Team" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Team Module", "User", "0");
                }
                else
                {
                    // Update Existing Team
                    var existingTeam = await _context.TblTeam.FindAsync(data.Id);
                    if (existingTeam != null)
                    {
                        existingTeam.TeamName = data.TeamName;
                        existingTeam.SuperVisorId = data.SuperVisorId;
                        existingTeam.UpdatedBy = data.CreatedBy; // Assuming CreatedBy is the user updating
                        existingTeam.DateUpdated = DateTime.Now;

                        _context.TblTeam.Update(existingTeam);
                        await _context.SaveChangesAsync();

                        // Remove existing team members before adding new ones (if necessary)
                        var existingMembers = _context.TblTeamMembers.Where(m => m.Team == data.Id);
                        _context.TblTeamMembers.RemoveRange(existingMembers);
                        await _context.SaveChangesAsync();
                        // Insert updated team members
                        var newMembers = data.TeamMembers.Select(member => new TblTeamMemberModel
                        {
                            MemberId = member.MemberId,
                            Team = data.Id, // Foreign Key (Already exists)
                        }).ToList();

                        _context.TblTeamMembers.AddRange(newMembers);
                        await _context.SaveChangesAsync();

                        status = "Team successfully updated";
                        dbmet.InsertAuditTrail("Update Team " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Team Module", "User", data.Id.ToString());
                    }
                }

                
                return CreatedAtAction("save", new { id = data.Id }, data);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Team" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Team Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<TblTeamModel>> Delete(TeamParam data)
        {

            var isExisting = _context.TblTeam.Where(a => a.Id == data.Id).OrderByDescending(a => a.Id).ToList();
            string status = "";
            
            try
            {

                var existingTeam = await _context.TblTeam.FindAsync(data.Id);
                if (existingTeam != null)
                {
                    
                    existingTeam.DateDeleted = DateTime.Now;
                    existingTeam.DeletedBy = data.UserId;
                    existingTeam.DeleteFlag = true;

                    _context.TblTeam.Update(existingTeam);
                    await _context.SaveChangesAsync();
                    status = "Team successfully deleted";
                    dbmet.InsertAuditTrail("Delete Team" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Team Module", "User", "0");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Delete Team" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Team Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }
    }
}
