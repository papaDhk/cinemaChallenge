using ApiApplication.Services.ReservationService;
using ApiApplication.Services.Showtimes;
using ApiApplication.Services.Showtimes.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.IntegrationTests;
using Xunit;

namespace IntegrationTests;

public class ReservationServiceTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _testFixture;
    public ReservationServiceTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
    }
    
    [Fact]
    public async Task ReserveSeatsTest()
    {
        await Task.Delay(10000);

        var showtimesService = _testFixture.ServiceProvider.GetRequiredService<IShowtimesService>();
        var reservationService = _testFixture.ServiceProvider.GetRequiredService<IReservationService>();

        // var movieImDbId = "tt0111161";
        // var showtime = await showtimesService.CreateShowtime(new ShowTimeCreationParameters
        // {
        //     AuditoriumId = 1,
        //     MovieImDbId = movieImDbId,
        //     SessionDate = DateTime.UtcNow.AddDays(1),
        // }, CancellationToken.None);
        
        var ticket = await reservationService.ReserveSeats(22, 7);
        
        ticket = await reservationService.ConfirmSeatReservation(ticket.Id);
        
        ticket.Should().NotBeNull();
        ticket.Paid.Should().BeTrue();
    }
}