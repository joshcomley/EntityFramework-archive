using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2.Entities;
using WebApplication2.Hazception;

namespace WebApplication2.Controllers
{
    [EnableQuery]
    [Route("odata_/Users")]
    public class UsersController : Controller
    {
        private ApplicationDbContext _sampleContext;

        public UsersController(IServiceProvider serviceProvider)
        {
            _sampleContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(serviceProvider);
        }

        [HttpGet]
        public IQueryable<ApplicationUser> Get()
        {
            return _sampleContext.Users;
        }

        void TestSinglExpand()
        {
            /* 
             * {value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[WebApplication2.ApplicationUser])
             *  .Select(Param_0 => new SelectAllAndExpand`1() {
             *      ModelID = "17861011-9cd6-40fb-a0d4-086f5d0e9712", 
             *      Instance = Param_0, 
             *      Container = new SingleExpandedProperty`1() {
             *          Name = "Client", 
             *          Value = new SelectAll`1() {
             *              ModelID = "17861011-9cd6-40fb-a0d4-086f5d0e9712", 
             *              Instance = Param_0.Client}, IsNull = (Param_0.Client == null)}})}
             */
            var y = _sampleContext.Users.Select(u => new// SelectExpandBinder.SelectAllAndExpand<ApplicationUser>
            {
                //ModelID = "17861011-9cd6-40fb-a0d4-086f5d0e9712",
                //Instance = u,
                u.Client,
                IsNull = Equals(u.Client, null)
            });
            var xyz = y.ToList();
            var y2 = _sampleContext.Users.Select(u => new SelectExpandBinder.SelectAllAndExpand<ApplicationUser>
            {
                ModelID = "17861011-9cd6-40fb-a0d4-086f5d0e9712",
                Instance = u,
                Container = new PropertyContainer.SingleExpandedProperty<SelectExpandBinder.SelectAll<Client>>
                {
                    Name = "Client",
                    Value = new SelectExpandBinder.SelectAll<Client>
                    {
                        ModelID = "17861011-9cd6-40fb-a0d4-086f5d0e9712",
                        Instance = u.Client,
                    },
                    IsNull = u.Client == null
                }
            });
        }
    }
}