using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Services;
using ApiApplication.Services.Movies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        public MoviesController(IMoviesService moviesService)
        {
            _moviesService = moviesService;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error),StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Error),StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _moviesService.GetAllMoviesAsync(CancellationToken.None);
            return  Ok(movies);
            
        }
    }
}