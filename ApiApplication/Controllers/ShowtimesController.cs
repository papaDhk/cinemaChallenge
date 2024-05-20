using System.Collections.Generic;
using System.Threading.Tasks;
using ApiApplication.Services;
using ApiApplication.Services.Showtimes;
using ApiApplication.Services.Showtimes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimesController : ControllerBase
    {
        private readonly IShowtimesService _showtimesService;
        public ShowtimesController(IShowtimesService showtimesService)
        {
            _showtimesService = showtimesService;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Showtime>>> GetShowtimes()
        {
            var showtimes = await _showtimesService.GetAllAsync();
            return  Ok(showtimes);
        }
        
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error),StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Error),StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Showtime>> CreateShowtime([FromBody] ShowTimeCreationParameters creationParameters)
        {
            var showtime = await _showtimesService.CreateShowtime(creationParameters);
            return  CreatedAtAction(nameof(CreateShowtime), showtime);
        }
    }
}