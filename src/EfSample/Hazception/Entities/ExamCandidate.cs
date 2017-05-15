using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Entities
{
	public class ExamCandidate : IDbObject<int>, IClientObject
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public Exam Exam { get; set; }
        public int ExamId { get; set; }
		public Video Video { get; set; }
        public int VideoId { get; set; }
		public double LastTime { get; set; }
        public ApplicationUser Candidate { get; set; }
        public string CandidateId { get; set; }
        public Client Client { get; set; }
        public int ClientId { get; set; }
        public ExamCandidateStatus Status { get; set; }
		public List<ExamCandidateResult> CandidateResults { get; set; }
		public List<ExamResult> Results { get; set; }
	}
}