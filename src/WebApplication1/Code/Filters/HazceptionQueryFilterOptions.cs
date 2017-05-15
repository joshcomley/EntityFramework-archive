using System.Linq;
using Microsoft.AspNetCore.Http;
using WebApplication2.Entities;
using WebApplication2.Hazception;

namespace EfSample9.Hazception.Filters
{
    public class HazceptionQueryFilterOptions
    {
        public ApplicationDbContext Db { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ApplicationUser _currentUser;
        public HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public ApplicationUser CurrentUser => _currentUser = _currentUser ?? Db.Users.Single(u => u.Email == "joshcomley@googlemail.com");

        public HazceptionQueryFilterOptions(
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext db
        )
        {
            Db = db;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}