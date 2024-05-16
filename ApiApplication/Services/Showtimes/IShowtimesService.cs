using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Database.Entities;
using ApiApplication.Services.Showtimes.Models;

namespace ApiApplication.Services.Showtimes
{
    public interface IShowtimesService
    {
        Task<Showtime> CreateShowtime(ShowTimeCreationParameters showTimeCreationParameters, CancellationToken cancel = default);
        Task<IEnumerable<Showtime>> GetAllAsync(CancellationToken cancel = default);
        Task<Showtime> GetShowtimeByIdAsync(int id, CancellationToken cancel = default);
    }
}