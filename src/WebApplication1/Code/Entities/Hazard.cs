using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Entities
{
    /*    
        _hazards.push({
            name: "Third hazard",
            timeFrom: 6,
            timeTo: 16,
            xFrom: 0,
            yFrom: 50,
            xTo: 50,
            yTo: 100
        });
    */

    public class Hazard : DbObject, IRevisionable, IClientObject
    {
        private const double MinNonZeroDouble = 0.000000001;
        private const string InvalidNumber = "Number";

        public Video Video { get; set; }

        [Required]
        public int VideoId { get; set; }
        public Client Client { get; set; }
        public int ClientId { get; set; }

        public Guid VideoGuid { get; set; }

        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; }

        public string Description { get; set; }

        //[NotEmptyAndWithinRange(0d, int.MaxValue, ErrorMessage = "Please enter a valid start time")]
        public double TimeFrom { get; set; }

        //[NotEmptyAndWithinRange(MinNonZeroDouble, int.MaxValue, ErrorMessage = "Please specify a valid duration")]
        public double Duration { get; set; }

        //[NotEmptyAndWithinRange(MinNonZeroDouble, int.MaxValue, ErrorMessage = InvalidNumber)]
        public double Left { get; set; }

        //[NotEmptyAndWithinRange(MinNonZeroDouble, int.MaxValue, ErrorMessage = InvalidNumber)]
        public double Top { get; set; }

        //[NotEmptyAndWithinRange(MinNonZeroDouble, int.MaxValue, ErrorMessage = InvalidNumber)]
        public double Width { get; set; }

        //[NotEmptyAndWithinRange(MinNonZeroDouble, int.MaxValue, ErrorMessage = InvalidNumber)]
        public double Height { get; set; }

        public string RevisionKey { get; set; }
		[NotMapped]
		public string ImageUrl { get; set; }
		public List<ExamResult> Results { get; set; }
	}
}