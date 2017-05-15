using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using WebApplication2.Entities;

namespace EfSample9.Hazception.Filters
{
    public abstract class HazceptionQueryFilter<T> : IQueryFilter<T>
    {
        public HazceptionQueryFilterOptions Options { get; }
        public HttpContext HttpContext => Options.HttpContext;

        protected HazceptionQueryFilter(HazceptionQueryFilterOptions options)
        {
            Options = options;
        }

        protected virtual Expression<Func<T, bool>> Super(EntityFilterContext context)
        {
            return null;
        }

        protected abstract Expression<Func<T, bool>> Client(EntityFilterContext context);
        protected abstract Expression<Func<T, bool>> Candidate(EntityFilterContext context);

        public Expression<Func<T, bool>> Filter(EntityFilterContext context)
        {
            //if (!HttpContext.User.Identity.IsAuthenticated)
            //{
            //    return entity => false;
            //}
            switch (Options.CurrentUser.UserType)
            {
                case UserType.Candidate:
                    return Candidate(context);
                case UserType.Client:
                    return Client(context);
                case UserType.Super:
                    return Super(context);
            }
            return entity => false;
        }
    }
}