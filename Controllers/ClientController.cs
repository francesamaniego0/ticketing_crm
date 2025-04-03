using CRM.Manager;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using static CRM.Controllers.TeamController;
using Microsoft.EntityFrameworkCore;
//using CRM.Services;


namespace CRM.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : Controller
    {
        private readonly CRMContext _context;
        DBManager db = new DBManager();
        private readonly DBMethods dbmet;
        private readonly ExternalApiService _externalApiService;

        public ClientController(CRMContext context, DBMethods _dbmet, ExternalApiService externalApiService)
        {
            _context = context;
            this.dbmet = _dbmet;
            _externalApiService = externalApiService;
        }
        public class ClientParam
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Name { get; set; }
            public int page { get; set; }
        }
        public class ClientPaginateModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<ClientModel> items { get; set; } = new List<ClientModel>(); // Prevent null reference
        }
        public class ClientModel
        {
            public int? ClientId { get; set; }
            public string? ClientName { get; set; }
            public string? Location { get; set; }
            public List<ContactPersonsModel> ContactPersons { get; set; } = new List<ContactPersonsModel>(); // Prevent null reference
        }
        public class ContactPersonsModel
        {
            public string? contactPersonId { get; set; }
            public string? contactPersonName { get; set; }
            public string? contactPersonEmail { get; set; }
            public string? contactPersonCNumber { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> PostList(ClientParam data)
        {
            List<GetAllUserDetailsResult> apiUsers = await _externalApiService.FetchDataFromExternalApi();
            var dbclient = _context.TblClient.OrderByDescending(a => a.Id).ToList();
            var dbcperson = _context.TblClientContactPersons.OrderByDescending(a => a.Id).ToList();
            // Step 3: Join API data with database data
            int pageSize = 25;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
            string page_size = pageSize == 0 ? "10" : pageSize.ToString();
            try
            {
                var result = from client in dbclient
                             join cpersons in dbcperson
                             on client.Id equals cpersons.ClientId into cpersonsGroup
                             from cpersons in cpersonsGroup.DefaultIfEmpty() // Left Join with Contact Persons
                             where client.DeleteFlag == false  // Filtering condition
                             group new
                             {
                                 ContactPersonId = cpersons?.Id,
                                 ContactPersonName = cpersons?.Fullname,
                                 ContactPersonEmail = cpersons?.Email,
                                 ContactPersonCNumber = cpersons?.ContactNumber,
                             }
                             by new
                             {
                                 ClientId = client?.Id,
                                 ClientName = client?.ClientName,
                                 Location = client?.Location
                             }
                            into contactPersonGroup
                             select new ClientModel // ✅ Explicitly map to ClientModel
                             {
                                 ClientId = contactPersonGroup.Key.ClientId,
                                 ClientName = contactPersonGroup.Key.ClientName,
                                 Location = contactPersonGroup.Key.Location,
                                 ContactPersons = contactPersonGroup
                                                    .Where(m => m.ContactPersonId != null) // Exclude null contacts
                                                    .Select(m => new ContactPersonsModel
                                                    {
                                                        contactPersonId = m.ContactPersonId.ToString() ?? "No ID",
                                                        contactPersonName = m.ContactPersonName ?? "Unknown",
                                                        contactPersonEmail = m.ContactPersonEmail ?? "Unknown",
                                                        contactPersonCNumber = m.ContactPersonCNumber ?? "Unknown",
                                                    }).ToList()
                             };
                // **Add condition dynamically if `data` is not null**
                if (data.Id != null && data.Id != 0)
                {
                    result = result.Where(t => t.ClientId == data.Id);
                }
                if (data.Name != null && data.Name != "")
                {
                    result = result.Where(t => t.ClientName.ToLower().Contains(data.Name.ToLower()));
                }
                // Convert to a List
                var finalResult = result.ToList();
                totalItems = finalResult.Count;
                totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));
                items = finalResult.Skip((data.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                int pages = data.page == 0 ? 1 : data.page;
                var results = new List<ClientPaginateModel>();
                var item = new ClientPaginateModel();
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
            
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR:"+ ex);
            }
        }

        public class ClientResponse
        {
            public int Id { get; set; }
            public string ClientName { get; set; }
            public string? Location { get; set; }
            public int? CreatedBy { get; set; }
            public List<TblClientContactPersonsModel> ClientContactPersons { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<TblClientModel>> Save(ClientResponse data)
        {
            string status = "";

            try
            {

                if (data.Id == 0)
                { // Insert Team
                    bool hasDuplicateOnSave = (_context.TblClient?.Any(a => a.ClientName == data.ClientName && a.DeleteFlag == false)).GetValueOrDefault();
                    
                    if (hasDuplicateOnSave)
                    {
                        return Conflict("Entity already exists");
                    }
                    var client = new TblClientModel
                    {
                        ClientName = data.ClientName,
                        Location = data.Location,
                        DeleteFlag = false, // Default Active
                        CreatedBy = data.CreatedBy,
                        DateCreated = DateTime.Now
                    };
                    _context.TblClient.Add(client);
                    await _context.SaveChangesAsync();
                    var contactPerson = data.ClientContactPersons.Select(cperson => new TblClientContactPersonsModel
                    {
                        
                        ClientId = client.Id, // Foreign Key (After saving team)
                        Fullname = cperson.Fullname,
                        Email = cperson.Email,
                        ContactNumber = cperson.ContactNumber,
                    }).ToList();

                    _context.TblClientContactPersons.AddRange(contactPerson);
                    await _context.SaveChangesAsync();
                    status = "Client successfully saved";
                    dbmet.InsertAuditTrail("Save Client" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Client Module", "User", "0");
                }
                else
                {
                    // Update Existing Team
                    var existingClient = await _context.TblClient.FindAsync(data.Id);
                    if (existingClient != null)
                    {
                        existingClient.ClientName = data.ClientName;
                        existingClient.Location = data.Location;
                        existingClient.UpdatedBy = data.CreatedBy; // Assuming CreatedBy is the user updating
                        existingClient.DateUpdated = DateTime.Now;

                        _context.TblClient.Update(existingClient);
                        await _context.SaveChangesAsync();

                        //// Remove existing team members before adding new ones (if necessary)
                        //var existingClientCPerson = _context.TblClientContactPersons.Where(m => m.ClientId == data.Id);
                        //_context.TblClientContactPersons.RemoveRange(existingClientCPerson);
                        //await _context.SaveChangesAsync();
                        //// Insert updated team members
                        //var newcontactPerson = data.ClientConcactPersons.Select(cperson => new TblClientContactPersonsModel
                        //{

                        //    ClientId = data.Id, // Foreign Key (Already exists)
                        //    Fullname = cperson.Fullname,
                        //    Email = cperson.Email,
                        //    ContactNumber = cperson.ContactNumber,
                        //}).ToList();

                        //_context.TblClientContactPersons.AddRange(newcontactPerson);
                        //await _context.SaveChangesAsync();

                        // Fetch all existing contact persons for this client at once
                        var existingContacts = await _context.TblClientContactPersons
                        .Where(m => m.ClientId == data.Id)
                        .ToListAsync();
                        // Get IDs of new contact persons from request
                        var newContactIds = data.ClientContactPersons.Select(c => c.Id).ToList();

                        // Find contacts that should be deleted (exist in DB but not in new request)
                        var toDeleteContacts = existingContacts.Where(c => !newContactIds.Contains(c.Id)).ToList();

                        // Remove unnecessary contacts in one operation
                        if (toDeleteContacts.Any())
                        {
                            _context.TblClientContactPersons.RemoveRange(toDeleteContacts);
                        }
                        // Process each contact from request
                        foreach (var contact in data.ClientContactPersons)
                        {
                            var existingContact = existingContacts.FirstOrDefault(c => c.Id == contact.Id);

                            if (existingContact != null) {
                                existingContact.Fullname = contact.Fullname;
                                existingContact.Email = contact.Email;
                                existingContact.ContactNumber = contact.ContactNumber;
                            }
                            else
                            {
                                var ContactPerson = new TblClientContactPersonsModel
                                {
                                    ClientId = data.Id,
                                    Fullname = contact.Fullname,
                                    Email = contact.Email,
                                    ContactNumber = contact.ContactNumber
                                };
                                _context.TblClientContactPersons.Add(ContactPerson);
                            }
                        }

                        // Save all changes once
                        await _context.SaveChangesAsync();


                        status = "Client successfully updated";
                        dbmet.InsertAuditTrail("Update Client " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Client Module", "User", data.Id.ToString());
                    }
                }


                return CreatedAtAction("save", new { id = data.Id }, data);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Client" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Team Client", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<TblClientModel>> Delete(ClientParam data)
        {

            var isExisting = _context.TblClient.Where(a => a.Id == data.Id).OrderByDescending(a => a.Id).ToList();
            string status = "";

            try
            {
                var existingTeam = await _context.TblClient.FindAsync(data.Id);
                if (existingTeam != null)
                {

                    existingTeam.DateDeleted = DateTime.Now;
                    existingTeam.DeletedBy = data.UserId;
                    existingTeam.DeleteFlag = true;

                    _context.TblClient.Update(existingTeam);
                    await _context.SaveChangesAsync();
                    status = "Client successfully deleted";
                    dbmet.InsertAuditTrail("Delete Client" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Client Module", "User", "0");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Delete Client" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Client Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }
    }
}
