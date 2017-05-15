using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2.Entities;
using WebApplication2.Hazception;

namespace WebApplication2
{
    [EnableQuery]
    [Route("odata_/Clients")]
    public class ClientsController : Controller
    {
        private ApplicationDbContext _sampleContext;

        public ClientsController(IServiceProvider serviceProvider)
        {
            _sampleContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(serviceProvider);
        }

        [HttpGet]
        public IQueryable<Client> Get()
        {
            return _sampleContext.Clients;
        }

        private void Check()
        {
            /*
              * {value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[WebApplication2.Client])
              * .Select(Param_0 => new SelectAllAndExpand`1() {
              *  ModelID = "f1e4044c-ea37-41a1-bb58-ab94fc7997e9", 
              *  Instance = Param_0, 
              *  Container = new NamedProperty`1() {
              *      Name = "Users", 
              *      Value = Param_0.Users.Skip(value(Microsoft.AspNetCore.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.Int32]).TypedProperty)
              *          .Select(Param_1 => new SelectAll`1() {
              *              ModelID = "f1e4044c-ea37-41a1-bb58-ab94fc7997e9", 
              *              Instance = Param_1})}})}
              */
            var y = _sampleContext.Clients.Select(c => new SelectExpandBinder.SelectAllAndExpand<Client>
            {
                ModelID = "f1e4044c-ea37-41a1-bb58-ab94fc7997e9",
                Instance = c,
                Container = new PropertyContainer.NamedProperty<IEnumerable<SelectExpandBinder.SelectAll<ApplicationUser>>>
                {
                    Name = "Users",
                    Value = c.Users.Select(u =>
                    new SelectExpandBinder.SelectAll<ApplicationUser>
                    {
                        ModelID = "f1e4044c-ea37-41a1-bb58-ab94fc7997e9",
                        Instance = u
                    })
                }
            });
            var xyz = y.ToList();
            var x = _sampleContext.Clients.Select(c => new SelectExpandBinder.SelectAllAndExpand<Client>
            {
                ModelID = "f1e4044c-ea37-41a1-bb58-ab94fc7997e9",
                Instance = c,
                Container = new PropertyContainer.NamedProperty<IEnumerable<SelectExpandBinder.SelectAll<ApplicationUser>>>
                {
                    Name = "Users",
                    Value = c.Users.Select(u =>
                    new SelectExpandBinder.SelectAll<ApplicationUser>
                    {
                        ModelID = "f1e4044c-ea37-41a1-bb58-ab94fc7997e9",
                        Instance = u
                    })
                }
            });
            var xr = x.ToList();
            var container =
                xr.First().Container as
                    PropertyContainer.NamedProperty<IEnumerable<SelectExpandBinder.SelectAll<ApplicationUser>>>;
            var users = container.Value.ToList();
        }
    }
}