using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Entities
{
	public class ExamResult : IClientObject, IDbObject<int>
	{
		public int Id { get; set; }
		public bool Success { get; set; }
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
		public int CandidateResultId { get; set; }
		public ExamCandidateResult CandidateResult { get; set; }
		public ApplicationUser Candidate { get; set; }
		[Required]
		public string CandidateId { get; set; }
		public Hazard Hazard { get; set; }
		[Required]
		public int HazardId { get; set; }
		[Required]
		public double X { get; set; }
		[Required]
		public double Y { get; set; }
		[Required]
		public double Time { get; set; }
	}
}