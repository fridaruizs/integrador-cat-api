using Datadog.Trace;
using integrador_cat_api.Models;
using integrador_cat_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace integrador_cat_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatController : ControllerBase
    {
        private readonly ICatService _catService;

        public CatController(ICatService catService)
        {
            _catService = catService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Cat>> GetAll()
        {
            using (var scope = Tracer.Instance.StartActive("getallcats.endpoint"))
            {
                var cats = _catService.GetAll();
                return Ok(cats);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Cat> GetById(int id)
        {
            var cat = _catService.GetById(id);
            if (cat == null)
            {
                return NotFound($"Cat with id {id} not found.");
            }
            return Ok(cat);
        }
        
        [HttpGet("add/{name}")]
        public ActionResult<Cat> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Cat name cannot be empty.");
            }

            var newCat = _catService.Add(name);
            return CreatedAtAction(newCat.Name, new { id = newCat.Id }, newCat);
        }

        [HttpGet("getimage/{id}")]
        public async Task<ActionResult<string>> AssignImage(int id)
        {
            try
            {
                var imageUrl = await _catService.AssignImageToCatAsync(id);
                var cat = _catService.GetById(id);
                // return Ok(imageUrl);
                var htmlResponse = $@"
            <html>
                <body>
                    <h1>Cat:  {cat.Name}</h1>
                    <img src='{imageUrl}' alt='Cat Image' style='max-width: 500px;'/>
                </body>
            </html>";

                return Content(htmlResponse, "text/html");
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while assigning an image: {e.Message}");
            }
        }
    }
}
