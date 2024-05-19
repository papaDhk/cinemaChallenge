using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Services;
using ApiApplication.Services.Movies;
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
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _moviesService.GetAllMoviesAsync(CancellationToken.None);
            return  Ok(movies);
            
        }
    }
}