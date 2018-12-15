using ApiCollection.Client;
using Microsoft.AspNetCore.Mvc;

namespace ApiCollection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchOmdbController : ControllerBase
    {
        private readonly OmdbClient _omdbClient;
        public SearchOmdbController(OmdbClient omdbClient)
        {
            _omdbClient = omdbClient;
        }

        // GET api/SearchOmdb/Lion/1
        [HttpGet("{movieName}/{page}")]
        public IActionResult Get(string movieName, string page)
        {
            return new JsonResult(_omdbClient.GetBySearchAsync(new RequestBySearch
            {
                Search = movieName,
                Page = page
            }).Result);
        }
    }
}