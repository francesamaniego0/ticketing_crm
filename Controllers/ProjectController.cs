using CRM.Manager;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using static CRM.Controllers.ClientController;
using static CRM.Controllers.TeamController;

namespace CRM.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProjectController : Controller
    {
        private readonly CRMContext _context;
        DBManager db = new DBManager();
        private readonly DBMethods dbmet;
        private readonly ExternalApiService _externalApiService;

        public ProjectController(CRMContext context, DBMethods _dbmet, ExternalApiService externalApiService)
        {
            _context = context;
            this.dbmet = _dbmet;
            _externalApiService = externalApiService;
        }
        public class ProjectParam
        {
            public int? Id { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? clientId { get; set; }
            public int page { get; set; }
        }

        public class ProjectPaginateModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<ProjectModel> items { get; set; } = new List<ProjectModel>(); // Prevent null reference
        }
        public class ProjectModel
        {
            public int? ProjectId { get; set; }
            public string? ProjectTitle { get; set; }
            public string? ProjectDescription { get; set; }
            public int? ClientId { get; set; }
            public string? ClientName { get; set; }
            public string? Location { get; set; }
            public int? TeamId { get; set; }
            public string? TeamName { get; set; }
            public string? TeamSuperVisorId { get; set; }
            public string? TeamSuperVisorName { get; set; }
            public string? TeamSuperVisorEmail { get; set; }
            public List<ContactPersonsModel> ContactPersons { get; set; } = new List<ContactPersonsModel>(); // Prevent null reference
            public List<TeamMemberModel> Members { get; set; } = new List<TeamMemberModel>(); // Prevent null reference
        }
        public class TeamMemberModel
        {
            public int? MemberId { get; set; }
            public string? MemberName { get; set; }
            public string? MemberEmail { get; set; }
        }
        public class ContactPersonsModel
        {
            public string? ContactPersonId { get; set; }
            public string? ContactPersonName { get; set; }
            public string? ContactPersonEmail { get; set; }
            public string? ContactPersonCNumber { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> PostList(ProjectParam data)
        {
            List<GetAllUserDetailsResult> apiUsers = await _externalApiService.FetchDataFromExternalApi();
            var dbProject = _context.TblProjects.OrderByDescending(a => a.Id).ToList();
            var dbpcperson = _context.TblProjectContactPersons.OrderByDescending(a => a.Id).ToList();
            var dbclient = _context.TblClient.OrderByDescending(a => a.Id).ToList();
            var dbcperson = _context.TblClientContactPersons.OrderByDescending(a => a.Id).ToList();
            var dbteam = _context.TblTeam.OrderByDescending(a => a.Id).ToList();
            var dbteammember = _context.TblTeamMembers.OrderByDescending(a => a.Id).ToList();
            //Step 3: Join API data with database data
            int pageSize = 25;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
            string page_size = pageSize == 0 ? "10" : pageSize.ToString();
            try {
                //var result = from project in dbProject

                //             join dbpcpersons in dbpcperson
                //             on project.Id equals dbpcpersons.ProjectId into pcpersonsGroup
                //             from dbpcpersons in pcpersonsGroup.DefaultIfEmpty() // Left Join with Team Members

                //             join dbcpersons in dbcperson
                //             on dbpcpersons.ContactPersonId equals dbcpersons.Id into dbcpersonsGroup
                //             from dbcpersons in dbcpersonsGroup.DefaultIfEmpty() // Left Join with Team Members

                //             join dbteams in dbteam
                //             on project.Team equals dbteams.Id into teamsGroup
                //             from dbteams in teamsGroup.DefaultIfEmpty() // Left Join with Team Members

                //             join superVisor in apiUsers
                //             on dbteams.SuperVisorId equals superVisor.Id into superVisorGroup
                //             from superVisor in superVisorGroup.DefaultIfEmpty() // Left Join with API Users

                //             join dbteammembers in dbteammember
                //             on dbteams.Id equals dbteammembers.Team into teammembersGroup
                //             from dbteammembers in teammembersGroup.DefaultIfEmpty() // Left Join with Team Members

                //             join memberName in apiUsers
                //             on dbteammembers != null ? dbteammembers.MemberId : null equals memberName.Id into memberNameGroup
                //             from memberName in memberNameGroup.DefaultIfEmpty() // Left Join with API Users for Member Details

                //             join dbclients in dbclient
                //             on project.Client equals dbclients.Id into clientGroup
                //             from dbclients in clientGroup.DefaultIfEmpty() // Left Join with Team Members

                //             where project.DeleteFlag == false // Filtering condition
                //             group new
                //             {
                //                 ContactPersonId = dbcpersons?.Id, // Keep null if no member exists
                //                 ContactPersonName = dbcpersons?.Fullname, // Keep null if no name exists
                //                 ContactPersonEmail = dbcpersons?.Email,
                //                 ContactPersonCNumber = dbcpersons?.ContactNumber,

                //                 MemberId = memberName?.Id, // Keep null if no member exists
                //                 MemberName = memberName?.Fullname, // Keep null if no name exists
                //                 MemberEmail = memberName?.Email, // Keep null if no name exists
                //             }
                //             by new
                //             {
                //                 ProjectId = project?.Id,
                //                 ProjectTitle = project?.Title,
                //                 ProjectDescription = project?.Description,
                //                 TeamId = dbteams?.Id,
                //                 TeamName = dbteams?.TeamName,
                //                 TeamSuperVisorId = superVisor?.Id,
                //                 TeamSuperVisorName = superVisor?.Fullname,
                //                 TeamSuperVisorEmail = superVisor?.Email,
                //                 ClientId = dbclients?.Id,
                //                 ClientName = dbclients?.ClientName,
                //                 Location = dbclients?.Location,
                //                 ContactPersonId = dbpcpersons?.ContactPersonId
                //             }
                //             into projectGroup

                //             select new ProjectModel
                //             {
                //                 ProjectId = projectGroup.Key.ProjectId,
                //                 ProjectTitle = projectGroup.Key.ProjectTitle,
                //                 ProjectDescription = projectGroup.Key.ProjectDescription,
                //                 ClientId = projectGroup.Key.ClientId,
                //                 ClientName = projectGroup.Key.ClientName,
                //                 Location = projectGroup.Key.Location,
                //                 ContactPersons = projectGroup
                //                                   .Where(m => m.ContactPersonId != null) // Ensure we exclude null members
                //                                   .DistinctBy(m => m.ContactPersonId)
                //                                   .Select(m => new ContactPersonsModel
                //                                   {
                //                                       ContactPersonId = m.ContactPersonId.ToString() ?? "No Member",
                //                                       ContactPersonName = m.ContactPersonName ?? "Unknown",
                //                                       ContactPersonEmail = m.ContactPersonEmail ?? "Unknown",
                //                                       ContactPersonCNumber = m.ContactPersonCNumber ?? "Unknown",

                //                                   }).ToList(),
                //                 TeamId = projectGroup.Key.TeamId,
                //                 TeamName = projectGroup.Key.TeamName,
                //                 TeamSuperVisorId = projectGroup.Key.TeamSuperVisorId.ToString(),
                //                 TeamSuperVisorName = projectGroup.Key.TeamSuperVisorName,
                //                 TeamSuperVisorEmail = projectGroup.Key.TeamSuperVisorEmail,
                //                 Members = projectGroup
                //                            .Where(m => m.MemberId != null) // Exclude null members
                //                            .DistinctBy(m => m.MemberId)
                //                            .Select(m => new TeamMemberModel
                //                            {
                //                                MemberId = m.MemberId ?? 0,
                //                                MemberName = m.MemberName ?? "Unknown",
                //                                MemberEmail = m.MemberEmail ?? "Unknown",
                //                            }).ToList()

                //             };
                var result = from project in dbProject
                             join dbteams in dbteam on project.Team equals dbteams.Id into teamsGroup
                             from dbteams in teamsGroup.DefaultIfEmpty()

                             join superVisor in apiUsers on dbteams.SuperVisorId equals superVisor.Id into superVisorGroup
                             from superVisor in superVisorGroup.DefaultIfEmpty()

                             join dbclients in dbclient on project.Client equals dbclients.Id into clientGroup
                             from dbclients in clientGroup.DefaultIfEmpty()

                             where project.DeleteFlag == false

                             select new ProjectModel
                             {
                                 ProjectId = project.Id,
                                 ProjectTitle = project.Title,
                                 ProjectDescription = project.Description,
                                 ClientId = dbclients?.Id,
                                 ClientName = dbclients?.ClientName,
                                 Location = dbclients?.Location,

                                 TeamId = dbteams?.Id,
                                 TeamName = dbteams?.TeamName,
                                 TeamSuperVisorId = superVisor?.Id.ToString(),
                                 TeamSuperVisorName = superVisor?.Fullname,
                                 TeamSuperVisorEmail = superVisor?.Email,

                                 ContactPersons = (from pcp in dbpcperson
                                                   join cp in dbcperson on pcp.ContactPersonId equals cp.Id
                                                   where pcp.ProjectId == project.Id
                                                   select new ContactPersonsModel
                                                   {
                                                       ContactPersonId = cp.Id.ToString(),
                                                       ContactPersonName = cp.Fullname ?? "Unknown",
                                                       ContactPersonEmail = cp.Email ?? "Unknown",
                                                       ContactPersonCNumber = cp.ContactNumber ?? "Unknown"
                                                   })
                                                   .DistinctBy(cp => cp.ContactPersonId)
                                                   .ToList(),

                                 Members = (from tm in dbteammember
                                            join user in apiUsers on tm.MemberId equals user.Id
                                            where tm.Team == dbteams.Id
                                            select new TeamMemberModel
                                            {
                                                MemberId = user.Id,
                                                MemberName = user.Fullname ?? "Unknown",
                                                MemberEmail = user.Email ?? "Unknown"
                                            })
                                            .DistinctBy(m => m.MemberId)
                                            .ToList()
                             };

                // **Add condition dynamically if `data` is not null**
                if (data.Id != null && data.Id != 0)
                {
                    result = result.Where(t => t.ProjectId == data.Id);
                }
                if (data.Title != null && data.Title != "")
                {
                    result = result.Where(t => t.ProjectTitle.ToLower().Contains(data.Title.ToLower()));
                }
                if (data.clientId != null && data.clientId != 0)
                {
                    result = result.Where(t => t.ClientId == data.clientId);
                }
                // Convert to a List
                var finalResult = result.ToList();
                totalItems = finalResult.Count;
                totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));
                items = finalResult.Skip((data.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                int pages = data.page == 0 ? 1 : data.page;
                var results = new List<ProjectPaginateModel>();
                var item = new ProjectPaginateModel();
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
        public class ProjectResponse
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string? Description { get; set; }
            public int? Client { get; set; }
            public int? Team { get; set; }
            public int? CreatedBy { get; set; }
            public List<TblProjectContactPersonsModel> ProjectContactPersons { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<TblProjectModel>> Save(ProjectResponse data)
        {

            string status = "";
            try
            {
                if (data.Id == 0)
                { // Insert Team
                    var Project = new TblProjectModel
                    {
                        Title = data.Title,
                        Description = data.Description,
                        DeleteFlag = false, // Default Active
                        CreatedBy = data.CreatedBy,
                        DateCreated = DateTime.Now,
                        Team = data.Team,
                        Client = data.Client
                    };
                    _context.TblProjects.Add(Project);
                    await _context.SaveChangesAsync();
                    var projectContactPersons = data.ProjectContactPersons.Select(cperson => new TblProjectContactPersonsModel
                    {
                        ProjectId = Project.Id,// Foreign Key (After saving team)
                        ContactPersonId = cperson.ContactPersonId,
                    }).ToList();

                    _context.TblProjectContactPersons.AddRange(projectContactPersons);
                    await _context.SaveChangesAsync();
                    status = "Project successfully saved";
                    dbmet.InsertAuditTrail("Save Project" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Project Module", "User", "0");
                }
                else
                {
                    // Update Existing Team
                    var existingProject = await _context.TblProjects.FindAsync(data.Id);
                    if (existingProject != null)
                    {
                        existingProject.Title = data.Title;
                        existingProject.Description = data.Description;
                        existingProject.Client = data.Client;
                        existingProject.Team = data.Team;
                        existingProject.UpdatedBy = data.CreatedBy; // Assuming CreatedBy is the user updating
                        existingProject.DateUpdated = DateTime.Now;

                        _context.TblProjects.Update(existingProject);
                        await _context.SaveChangesAsync();

                        // Remove existing team members before adding new ones (if necessary)
                        var existingContactPerson = _context.TblProjectContactPersons.Where(m => m.ProjectId == data.Id);
                        _context.TblProjectContactPersons.RemoveRange(existingContactPerson);
                        await _context.SaveChangesAsync();
                        // Insert updated team members
                        var newCperson = data.ProjectContactPersons.Select(cperson => new TblProjectContactPersonsModel
                        {
                            ProjectId = data.Id,
                            ContactPersonId = cperson.ContactPersonId, // Foreign Key (Already exists)
                        }).ToList();

                        _context.TblProjectContactPersons.AddRange(newCperson);
                        await _context.SaveChangesAsync();

                        status = "Project successfully updated";
                        dbmet.InsertAuditTrail("Update Project " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Project Module", "User", data.Id.ToString());
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
        public async Task<ActionResult<TblProjectModel>> Delete(ProjectParam data)
        {

            var isExisting = _context.TblProjects.Where(a => a.Id == data.Id).OrderByDescending(a => a.Id).ToList();
            string status = "";

            try
            {

                var existingTeam = await _context.TblProjects.FindAsync(data.Id);
                if (existingTeam != null)
                {

                    existingTeam.DateDeleted = DateTime.Now;
                    existingTeam.DeletedBy = data.UserId;
                    existingTeam.DeleteFlag = true;

                    _context.TblProjects.Update(existingTeam);
                    await _context.SaveChangesAsync();
                    status = "Project successfully deleted";
                    dbmet.InsertAuditTrail("Delete Project" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Project Module", "User", "0");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Delete Project" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Project Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveProjectDocuments([FromBody] List<TblProjectDocument> data)
        {
            try
            {
                foreach (var item in data)
                {
                    _context.TblProjectDocument.Add(item);
                }

                await _context.SaveChangesAsync();
                return Ok("Files saved.");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Project File " + ex.Message,
                    DateTime.Now.ToString("yyyy-MM-dd"),
                    "Project File Module",
                    "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }
        public class ProjectDocumentsDeleteParam{
            public int Id { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> DeleteProjectDocuments( List<ProjectDocumentsDeleteParam> data)
        {
            try
            {
                foreach (var item in data)
                {
                    var items = _context.TblProjectDocument.Where(a => a.Id == item.Id).FirstOrDefault();
                    
                    _context.TblProjectDocument.Remove(items);
                    
                }
                //_context.SaveChanges();
                await _context.SaveChangesAsync();
                return Ok("Files Deleted.");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Delete File " + ex.Message,
                    DateTime.Now.ToString("yyyy-MM-dd"),
                    "Project File Module",
                    "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }
        public class ProjectDocumentsParam
        {
            public int? ProjectId { get; set; }
            public string? FileName { get; set; }
        }
        public class ProjectDocumentsModel
        {
            public int Id { get; set; }
            public int? ProjectId { get; set; }
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public string? FileExtension { get; set; }
            public bool? IsDeleted { get; set; }
            public int? CreatedBy { get; set; }
            public string? CreatedByName { get; set; }
            public DateTime? DateCreated { get; set; }
            public int? DeletedBy { get; set; }
            public DateTime? DateDeleted { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> PostProjectDocuments(ProjectDocumentsParam data)
        {
            try
            {
                List<GetAllUserDetailsResult> apiUsers = await _externalApiService.FetchDataFromExternalApi();
                var ProjectDocumentsDB = _context.TblProjectDocument.Where(a => a.IsDeleted == false).ToList();
                var result = from projectDocumentDB in ProjectDocumentsDB
                             join user in apiUsers on projectDocumentDB.CreatedBy equals user.Id into userGroup
                             from user in userGroup.DefaultIfEmpty()
                             select new ProjectDocumentsModel
                             {
                                 Id = projectDocumentDB.Id,
                                 ProjectId = projectDocumentDB.ProjectId,
                                 FileName = projectDocumentDB.FileName,
                                 FilePath = projectDocumentDB.FilePath,
                                 FileExtension = projectDocumentDB.FileExtension,
                                 IsDeleted = projectDocumentDB.IsDeleted,
                                 CreatedBy = projectDocumentDB.CreatedBy,
                                 CreatedByName = user != null ? user.Fullname ?? "Unknown" : "Unknown",
                                 DateCreated = projectDocumentDB.DateCreated,
                                 DeletedBy = projectDocumentDB.DeletedBy,
                                 DateDeleted = projectDocumentDB.DateDeleted,


                             };
                if (data.ProjectId != 0)
                {
                    result = result.Where(a => a.ProjectId == data.ProjectId).ToList();
                }
                if(data.FileName != "" && data.FileName != null)
                {
                    result = result.Where(a => a.FileName.ToLower().Contains(data.FileName.ToLower())).ToList();
                }
                return Ok(result.ToList());
            }
            catch (Exception ex)
            {
               
                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostProjectDocumentsFilteredById(List<ProjectDocumentsDeleteParam> data)
        {
            try
            {   // Get list of IDs from the input
                var idsToFilter = data.Select(d => d.Id).ToList();
                List<GetAllUserDetailsResult> apiUsers = await _externalApiService.FetchDataFromExternalApi();
                //var ProjectDocumentsDB = _context.TblProjectDocument.Where(a => a.IsDeleted == false).ToList();
                // Get project documents that are not deleted AND match the filtered IDs
                var ProjectDocumentsDB = _context.TblProjectDocument
                    .Where(a => a.IsDeleted == false && idsToFilter.Contains(a.Id))
                    .ToList();


                var result = from projectDocumentDB in ProjectDocumentsDB
                             join user in apiUsers on projectDocumentDB.CreatedBy equals user.Id into userGroup
                             from user in userGroup.DefaultIfEmpty()
                             select new ProjectDocumentsModel
                             {
                                 Id = projectDocumentDB.Id,
                                 ProjectId = projectDocumentDB.ProjectId,
                                 FileName = projectDocumentDB.FileName,
                                 FilePath = projectDocumentDB.FilePath,
                                 FileExtension = projectDocumentDB.FileExtension,
                                 IsDeleted = projectDocumentDB.IsDeleted,
                                 CreatedBy = projectDocumentDB.CreatedBy,
                                 CreatedByName = user != null ? user.Fullname ?? "Unknown" : "Unknown",
                                 DateCreated = projectDocumentDB.DateCreated,
                                 DeletedBy = projectDocumentDB.DeletedBy,
                                 DateDeleted = projectDocumentDB.DateDeleted,


                             };
                return Ok(result.ToList());
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> ProjectContactPerson(ProjectParam data)
        //{
        //    var dbpcperson = _context.TblProjectContactPersons.OrderByDescending(a => a.Id).ToList();

        //    try
        //    {
        //        var results = dbpcperson.Where()
        //        return Ok(results);
        //        //return Ok(finalResult);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("ERROR:" + ex);
        //    }
        //}
    }
}
