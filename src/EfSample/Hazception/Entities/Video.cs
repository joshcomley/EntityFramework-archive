using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Entities
{
    public class Video : DbObject, IRevisionable, IClientObject
	{
        public Client Client { get; set; }
        public int ClientId { get; set; }
        //public Guid ClientGuid { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        //public string ScreenshotUri { get; set; }
		public double Duration { get; set; }
        public List<Hazard> Hazards { get; set; }
        public List<Exam> Exams { get; set; }
        public List<ExamResult> Results { get; set; }
        public List<ExamCandidateResult> CandidateResults { get; set; }
        public List<ExamCandidate> Candidates { get; set; }
	    public int ResultsCount { get; set; } 
	    public int CandidateResultsCount { get; set; } 
	    public int CandidatesCount { get; set; } 
	    public int ExamCount { get; set; } 
	    public int HazardCount { get; set; } 
		public string RevisionKey { get; set; }

		public string ScreenshotUrl { get; set; }
		public string ScreenshotMiniUrl { get; set; }
	}
}