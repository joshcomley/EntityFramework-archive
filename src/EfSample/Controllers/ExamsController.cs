using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2.Entities;
using WebApplication2.Hazception;

namespace WebApplication2
{
    [EnableQuery]
    [Route("odata_/Exams")]
    public class ExamsController : Controller
    {
        private ApplicationDbContext _sampleContext;

        public ExamsController(IServiceProvider serviceProvider)
        {
            _sampleContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(serviceProvider);
        }

        [HttpGet]
        public async Task<List<Exam>> Get()
        {
            return await _sampleContext.Exams.ToListAsync();
        }

        [HttpGet("{key}")]
        public SingleResult<Exam> Get(int key)
        {
            return new SingleResult<Exam>(_sampleContext.Exams.Where(e => e.Id == key));
        }
    }
}