using System.Collections.Generic;
using System.Threading.Tasks;
using ApiApplication.Services.Showtimes;
using ApiApplication.Services.Showtimes.Models;
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
        public async Task<ActionResult<IEnumerable<Showtime>>> GetShowtimes()
        {
            var showtimes = await _showtimesService.GetAllAsync();
            return  Ok(showtimes);
        }
        
        [HttpPost("/create")]
        public async Task<ActionResult<Showtime>> CreateShowtime([FromBody] ShowTimeCreationParameters creationParameters)
        {
            var showtime = await _showtimesService.CreateShowtime(creationParameters);
            return  CreatedAtAction(nameof(CreateShowtime), showtime);
        }
    }
}