using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Services.Auditorium
{
    public interface IAuditoriumService
    {
        public Task<bool> IsAuditoriumAvailable(int auditoriumId, DateTime sessionDate, TimeSpan reservationDuration, CancellationToken cancellationToken = default);
        public Task<Auditorium> GetAuditorium(int auditoriumId, CancellationToken cancellationToken = default);
        public Task<IEnumerable<Auditorium>> GetAllAuditoriums( CancellationToken cancellationToken = default);


    }
}