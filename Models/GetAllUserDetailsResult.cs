﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models
{
    public partial class GetAllUserDetailsResult
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Mname { get; set; }
        public string Suffix { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string EmployeeId { get; set; }
        public string Jwtoken { get; set; }
        public string? FilePath { get; set; }
        public int? Active { get; set; }
        public string Cno { get; set; }
        public string Address { get; set; }
        public int? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool DeleteFlag { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DateDeleted { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DateRestored { get; set; }
        public string RestoredBy { get; set; }
        public int? Department { get; set; }
        public bool? AgreementStatus { get; set; }

        public string RememberToken { get; set; }

        public int? UserType { get; set; }
        public int? EmployeeType { get; set; }
        public int? SalaryType { get; set; }
        public string Rate { get; set; }
        public string DaysInMonth { get; set; }
        public int? PayrollType { get; set; }

        public DateTime? DateStarted { get; set; }

        public int? Position { get; set; }
        public int? PositionLevelId { get; set; }
        public int? ManagerId { get; set; }
        public bool isLoggedIn { get; set; }

        //public int? CompanyId { get; set; }
    }
}
