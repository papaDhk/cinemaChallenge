using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Services;
using ApiApplication.Services.Auditorium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriumsController : ControllerBase
    {
        private readonly IAuditoriumService _auditoriumService;
        public AuditoriumsController(IAuditoriumService auditoriumService)
        {
            _auditoriumService = auditoriumService ?? throw new ArgumentNullException(nameof(auditoriumService));
        }
        
        // GET
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error),StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Auditorium>>> GetAll()
        {
            var auditoriums = await _auditoriumService.GetAllAuditoriums(CancellationToken.None);
            return  Ok(auditoriums);
            
        }
    }
}