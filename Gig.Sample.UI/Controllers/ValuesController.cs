using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DataProviders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gig.Sample.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMemoryCacheManager _memoryCacheManager;
        private readonly IDistributeCacheManager _distributeCacheManager;
        private readonly IUnitOfWork _testDbContext;

        public ValuesController(IMemoryCacheManager memoryCacheManager, IDistributeCacheManager distributeCacheManager, IUnitOfWork testDbContext)
        {
            _memoryCacheManager = memoryCacheManager;
            _distributeCacheManager = distributeCacheManager;
            _testDbContext = testDbContext;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        //[Authorize(1,2)]
        public IEnumerable<string> Get()
        {

            //TestKey objKey = new TestKey();
            //var sm = new { id = 10, name = "testvalue" };
            //_memoryCacheManager.AddAsync(objKey, sm).GetAwaiter().GetResult();
            //var testMemoryCacheManager = _memoryCacheManager.GetAsync<object>(objKey).GetAwaiter().GetResult();
            //_distributeCacheManager.AddByExpireTimeAsync(objKey, sm, ExpirationMode.Sliding, TimeSpan.FromSeconds(20));
            //var testdistributeCacheManager = _distributeCacheManager.GetAsync<object>(objKey).GetAwaiter().GetResult();
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        //[Authorize(4, 5)]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
