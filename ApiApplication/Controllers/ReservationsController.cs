using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Database.Entities;
using ApiApplication.Services.Auditorium;
using ApiApplication.Services.ReservationService;
using ApiApplication.Services.ReservationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }
        
        [HttpPost("confirm/{ticketId:guid}")]
        public async Task<ActionResult<Ticket>> ConfirmSeatReservation(Guid ticketId)
        {
            var ticket = await _reservationService.ConfirmSeatReservation(ticketId);
            return  Ok(ticket);
        }
        
        [HttpPost]
        public async Task<ActionResult<Ticket>> ReserveSeat([FromQuery(Name = "showtimeId")]int showtimeId, [FromQuery(Name = "numberOfSeats")] int numberOfSeats)
        {
            var ticket = await _reservationService.ReserveSeats(showtimeId, numberOfSeats);
            return  Ok(ticket);
        }
        
        [HttpGet("availableSeats/{showtimeId:int}")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetAvailableSeats(int showtimeId)
        {
            var availableSeats = await _reservationService.GetAvailableSeats(showtimeId);
            return  Ok(availableSeats);
        }
    }
}