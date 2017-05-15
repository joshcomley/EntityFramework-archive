using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Entities
{
    public class Exam : DbObject, IClientObject
    {
        [Required(ErrorMessage = "Please select a video for this exam")]
        [Range(1, Int32.MaxValue)]
        public int VideoId { get; set; }
        public Video Video { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public DateTimeOffset? ScheduledDate { get; set; }
		[Range(0, 100)]
		public int PassMark { get; set; }
        [Required(ErrorMessage = "Please enter a title for this exam")]
        public string Title { get; set; }
		public bool AllowAnyArea { get; set; }
        public string Description { get; set; }
        //[ReadOnly(true)]
        public ExamStatus Status { get; set; }
        [NotMapped]
        public bool NotStarted
        {
            get { return Status == ExamStatus.NotStarted; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }
        [NotMapped]
        public bool Complete
        {
            get { return Status == ExamStatus.Completed; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }
        [NotMapped]
        public bool InProgress
        {
            get { return Status == ExamStatus.InProgress; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }
        /// <summary>
        ///  This is temporary until the OData left-join fix is implemented
        /// </summary>
        public int CandidateCount { get; set; }
        public List<ExamCandidate> Candidates { get; set; }
		public List<ExamCandidateResult> CandidateResults { get; set; }
		public List<ExamResult> Results { get; set; }
    }
}