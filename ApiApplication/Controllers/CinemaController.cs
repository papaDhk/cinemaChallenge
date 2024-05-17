using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Services.Movies;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CinemaController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly IHttpClientFactory _httpClientFactory;
        public CinemaController(IMoviesService moviesService, IHttpClientFactory factory)
        {
            _moviesService = moviesService;
            _httpClientFactory = factory;
        }
        
        // GET
        [HttpGet("GetMovies")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://api:7172");
            client.DefaultRequestHeaders.Add("X-Apikey","68e5fbda-9ec9-4858-97b2-4a8349764c63");
            var result = await client.GetAsync("/v1/movies");
            //var movies = await _moviesService.GetAllMoviesAsync();
            return  Ok(result.IsSuccessStatusCode);
        }
    }
}