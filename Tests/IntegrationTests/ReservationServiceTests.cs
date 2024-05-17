using ApiApplication.Services.ReservationService;
using ApiApplication.Services.Showtimes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.IntegrationTests;
using Xunit;

namespace Tests;

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
        
        var ticket = await reservationService.ReserveSeats(22, 617);
        
        //var ticket2 = await reservationService.ReserveSeats(22, 4);
        
        ticket.Should().NotBeNull();
    }
}