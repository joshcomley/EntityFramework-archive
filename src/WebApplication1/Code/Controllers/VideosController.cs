using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2.Entities;
using WebApplication2.Hazception;

namespace WebApplication2
{
    [EnableQuery]
    [Route("odata_/Videos")]
    public class VideosController : Controller
    {
        private ApplicationDbContext _sampleContext;

        public VideosController(IServiceProvider serviceProvider)
        {
            _sampleContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(serviceProvider);
        }

        [HttpGet]
        public IQueryable<Video> Get()
        {
            return _sampleContext.Videos;
        }

        [HttpGet("{key}")]
        public SingleResult<Video> Get(int key)
        {
            return new SingleResult<Video>(_sampleContext.Videos.Where(e => e.Id == key));
        }
    }
}