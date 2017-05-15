using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Entities
{
    public class DbObject : DbObjectLite
    {
        public Guid Guid { get; set; }
    }
    public class DbObjectLite : DbObjectBase<int>
    {

    }

    public class DbObjectBase<TKey> : IDbObject<TKey>, ICreatedDate
    {
        public TKey Id { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }

    public interface ICreatedDate
    {
        DateTimeOffset CreatedDate { get; set; }
    }
    public interface IClientObject
    {
        Client Client { get; set; }
        int ClientId { get; set; }
    }
    public interface IRevisionable
    {
        string RevisionKey { get; set; }
    }
    public enum UserType
    {
        Super = 1,
        Client = 2,
        Candidate = 3
    }
    public interface IDbObject<TKey>
    {
        TKey Id { get; set; }
    }
    public class ApplicationUser : IDbObject<string>
	{
		//[Required(ErrorMessage = "Please enter an email address")]
		[EmailAddress(ErrorMessage = "Please provide a valid email address")]
		public string Email { get; set; }
        //public override string NormalizedEmail { get; set; }
		[Required(ErrorMessage = "Please select a user type")]
		public UserType UserType { get; set; }
		public Client Client { get; set; }
		public int? ClientId { get; set; }
		[Required(ErrorMessage = "Please provide a name")]
		public string FullName { get; set; }
		public ApplicationUser CreatedByUser { get; set; }
		public List<ExamCandidate> Exams { get; set; } 
		public List<ExamResult> Results { get; set; } 
		public List<ExamCandidateResult> ExamResults { get; set; } 
		public string CreatedByUserId { get; set; }
		//public Guid Guid { get; }

	    //[NotMapped]
	    //public bool IsLockedOut
	    //{
	    //    get { return LockoutEnd > DateTime.Now; }
	    //    set { }
	    //}

	    public string Id { get; set; }
	}
}
