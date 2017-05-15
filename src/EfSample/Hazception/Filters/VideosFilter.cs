using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using WebApplication2.Entities;

namespace EfSample9.Hazception.Filters
{
    public class VideosFilter : IQueryFilter<Video>
    {
        public VideosFilter()
        {
            ClientId = 2;
        }

        public int ClientId { get; set; }

        public Expression<Func<Video, bool>> Filter(EntityFilterContext context)
        {
            return video => video.ClientId == this.ClientId;
        }
    }
    //public class VideosFilter : HazceptionQueryFilter<Video>
    //{
    //    public VideosFilter(HazceptionQueryFilterOptions options) : base(options)
    //    {
    //    }

    //    protected override Expression<Func<Video, bool>> Client(EntityFilterContext context)
    //    {
    //        return entity => entity.ClientId == Options.CurrentUser.ClientId.Value;
    //    }

    //    protected override Expression<Func<Video, bool>> Candidate(EntityFilterContext context)
    //    {
    //        //var exams = Options.Db.ExamCandidates
    //        //    .Where(e => e.CandidateId == Options.CurrentUser.Id)
    //        //    .Select(c => c.Exam.VideoId)
    //        //    .ToArray();
    //        //if (exams.Any())
    //        //{
    //        //    return entity => exams.Contains(entity.Id);
    //        //}
    //        return entity => entity.Id == -1;
    //    }
    //}
}