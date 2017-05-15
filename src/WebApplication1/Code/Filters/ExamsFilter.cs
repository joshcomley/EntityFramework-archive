using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using WebApplication2.Entities;

namespace EfSample9.Hazception.Filters
{
    public class ExamsQueryFilter2 : IQueryFilter<Exam>
    {
        public Expression<Func<Exam, bool>> Filter(EntityFilterContext context)
        {
            return e => e.Title.Contains("test");
        }
    }
    public class ExamsQueryFilter : HazceptionQueryFilter<Exam>
    {
        public ExamsQueryFilter(HazceptionQueryFilterOptions options) : base(options)
        {
        }

        protected override Expression<Func<Exam, bool>> Client(EntityFilterContext context)
        {
            return entity => entity.ClientId == Options.CurrentUser.ClientId;
        }

        protected override Expression<Func<Exam, bool>> Candidate(EntityFilterContext context)
        {
            var exams = Options.Db.ExamCandidates.Where(e => e.CandidateId == Options.CurrentUser.Id)
                .Select(c => c.ExamId)
                //.Cast<int>()
                .ToArray();
            return entity => exams.Contains(entity.Id);
        }
    }
}