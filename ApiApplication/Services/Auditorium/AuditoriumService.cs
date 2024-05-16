using System;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Database.Repositories.Abstractions;

namespace ApiApplication.Services.Auditorium
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumsRepository _auditoriumsRepository;

        public AuditoriumService(IAuditoriumsRepository auditoriumsRepository)
        {
            _auditoriumsRepository = auditoriumsRepository;
        }

        public async Task<bool> IsAuditoriumAvailable(int auditoriumId, DateTime dateTime, TimeSpan reservationDuration, CancellationToken cancellationToken = default)
        {
            var auditorium = await _auditoriumsRepository.GetAsync(auditoriumId, cancellationToken);
            return auditorium != null;
            
            //Ideally we should make sure the auditorium is available for the movie duration,
            //i.e there is no scheduled show during the session date and the session date + movie duration
        }

       
    }
}