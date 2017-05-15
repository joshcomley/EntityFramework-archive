using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Entities
{
	public class ExamCandidateResult : IClientObject, IDbObject<int>
	{
		public int Id { get; set; }
		public Exam Exam { get; set; }
		[Required]
		public int ExamId { get; set; }
		public Video Video { get; set; }
		[Required]
		public int VideoId { get; set; }
		public Client Client { get; set; }
		[Required]
		public int ClientId { get; set; }
		[Required]
		public int ExamCandidateId { get; set; }
		public ExamCandidate ExamCandidate { get; set; }
		public ApplicationUser Candidate { get; set; }
		[Required]
		public string CandidateId { get; set; }
		public double Score { get; set; }
		public bool Pass { get; set; }
		public string ClickData { get; set; }
		public int ClickCount { get; set; }
		public List<ExamResult> Results { get; set; }
		public DateTimeOffset Date { get; set; }
	}
}