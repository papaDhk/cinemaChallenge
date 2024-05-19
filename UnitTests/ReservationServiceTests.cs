using FluentAssertions;
using Moq;
using Xunit;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Services;
using ApiApplication.Services.Auditorium;
using ApiApplication.Services.ReservationService;
using ApiApplication.Services.Showtimes;
using ApiApplication.Services.Showtimes.Models;
using ApiApplication.Database;
using AutoFixture;

namespace UnitTests;

public class ReservationServiceTests
{
    private readonly Mock<ITicketsRepository> _ticketsRepositoryMock;
    private readonly Mock<IAuditoriumService> _auditoriumServiceMock;
    private readonly Mock<IShowtimesService> _showtimesServiceMock;
    private readonly ReservationService _reservationService;
    private readonly IFixture _fixture;

    public ReservationServiceTests()
    {
        _ticketsRepositoryMock = new Mock<ITicketsRepository>();
        _auditoriumServiceMock = new Mock<IAuditoriumService>();
        _showtimesServiceMock = new Mock<IShowtimesService>();
        _reservationService = new ReservationService(_ticketsRepositoryMock.Object, _auditoriumServiceMock.Object, _showtimesServiceMock.Object);
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
    }
    
    //Todo: Test cases when we have paid tickets, reserved tickets and expired reserved tickets


    [Fact]
    public async Task ReserveSeats_ThrowsNotEnoughSeatsAvailableException_WhenSeatsAreNotEnough()
    {
        var showtime = new Showtime { Id = 1, AuditoriumId = 1 };
        _showtimesServiceMock
            .Setup(service => service.GetShowtimeByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(showtime);

        var auditorium = new Auditorium
        {
            Id = 1, RowsCount = 10, NumberOfSeatsPerRow = 10,
            Seats = SampleData.GenerateSeats(1, 10, 10 ).Select(s => s.ToSeat())
        };
        _auditoriumServiceMock
            .Setup(service => service.GetAuditorium(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auditorium);

        const int wantedAvailableSeatsNumber = 2;
        var ticket = new TicketEntity
        {
            Id = Guid.NewGuid(),
            ShowtimeId = showtime.Id,
            TicketSeats = _fixture.CreateMany<TicketSeatEntity>(auditorium.Seats.Count() - wantedAvailableSeatsNumber).ToList(),
            CreatedTime = DateTime.UtcNow,
            Paid = false,
            Showtime = showtime.ToShowtimeEntity()
        };
        
        _ticketsRepositoryMock
            .Setup(repo => repo.GetEnrichedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TicketEntity>{ ticket });

        Func<Task> action = async () => await _reservationService.ReserveSeats(showtime.Id, wantedAvailableSeatsNumber+1, CancellationToken.None);

        await action.Should().ThrowAsync<NotEnoughSeatsAvailableException>();
    }

    [Fact]
    public async Task ReserveSeats_ReturnsTicket_WhenSeatsAreAvailable()
    {
        var auditorium = new Auditorium {
            Id = 1, RowsCount = 10, NumberOfSeatsPerRow = 10,
            Seats = SampleData.GenerateSeats(1, 10, 10 ).Select(s => s.ToSeat(10))
        };
        _auditoriumServiceMock
            .Setup(service => service.GetAuditorium(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auditorium);
        
        var showtime = new Showtime { Id = new Random().Next(), AuditoriumId = auditorium.Id };
        _showtimesServiceMock
            .Setup(service => service.GetShowtimeByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(showtime);

        const int wantedNumberOfAvailableSeats = 2;
        var ticket = new TicketEntity
        {
            Id = Guid.NewGuid(),
            ShowtimeId = showtime.Id,
            TicketSeats = auditorium.Seats.Take(auditorium.Size - wantedNumberOfAvailableSeats).Select(s => new TicketSeatEntity
            {
                SeatEntity = s.ToSeatEntity()
            }).ToList(),
            CreatedTime = DateTime.UtcNow,
            Paid = false,
            Showtime = showtime.ToShowtimeEntity()
        };
        
        _ticketsRepositoryMock
            .Setup(repo => repo.GetEnrichedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TicketEntity>{ ticket });
        var expectedTicketEntity = new TicketEntity
        {
            Id = Guid.NewGuid(), Paid = false, CreatedTime = DateTime.UtcNow, ShowtimeId = showtime.Id,
            TicketSeats = _fixture.CreateMany<TicketSeatEntity>(wantedNumberOfAvailableSeats).ToList()
        };
        
        _ticketsRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<ShowtimeEntity>(), It.IsAny<IEnumerable<SeatEntity>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTicketEntity);

        var result = await _reservationService.ReserveSeats(showtime.Id, wantedNumberOfAvailableSeats, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(expectedTicketEntity.Id);
        result.CreatedTime.Should().Be(expectedTicketEntity.CreatedTime);
        result.ShowtimeId.Should().Be(showtime.Id);
        result.Paid.Should().Be(false);
    }

    [Fact]
    public async Task GetAvailableSeats_ReturnsAvailableSeats_WhenShowtimeAndAuditoriumExist()
    {
        var showtime = new Showtime { Id = 1, AuditoriumId = 1 };
        var auditorium = new Auditorium
        {
            Id = 1, RowsCount = 10, NumberOfSeatsPerRow = 10,
            Seats = SampleData.GenerateSeats(1, 10, 10 ).Select(s => s.ToSeat(10))
        };

        _showtimesServiceMock
            .Setup(service => service.GetShowtimeByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(showtime);

        _auditoriumServiceMock
            .Setup(service => service.GetAuditorium(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auditorium);

        const int wantedNumberOfReserveSeats = 30;
        var expectedNumberOfAvailableSeats = auditorium.Size - wantedNumberOfReserveSeats;
        var ticket = new TicketEntity
        {
            Id = Guid.NewGuid(),
            ShowtimeId = showtime.Id,
            TicketSeats = auditorium.Seats.Take(wantedNumberOfReserveSeats).Select(s => new TicketSeatEntity
            {
                SeatEntity = s.ToSeatEntity()
            }).ToList(),
            CreatedTime = DateTime.UtcNow,
            Paid = false,
            Showtime = showtime.ToShowtimeEntity()
        };
        
        _ticketsRepositoryMock
            .Setup(repo => repo.GetEnrichedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TicketEntity>{ ticket });

        var result = await _reservationService.GetAvailableSeats(showtime.Id, CancellationToken.None);
        
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedNumberOfAvailableSeats);
        result.First().VirtualSeatNumber.Should().Be(wantedNumberOfReserveSeats + 1);
        result.Last().VirtualSeatNumber.Should().Be(auditorium.Size);
    }
    
    //Todo: Test cases when we have paid tickets, reserved tickets and expired reserved tickets
    
    [Fact]
    public async Task ConfirmSeatReservation_ThrowsNotFoundException_WhenTicketDoesNotExist()
    {
        _ticketsRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketEntity)null);

        Func<Task> sut = async () => await _reservationService.ConfirmSeatReservation(Guid.NewGuid(), CancellationToken.None);

        await sut.Should().ThrowAsync<NotFoundException>().WithMessage("There is not ticket corresponding to the id *");
    }

    [Fact]
    public async Task ConfirmSeatReservation_ThrowsTicketAlreadyPaidException_WhenTicketIsAlreadyPaid()
    {
        _ticketsRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TicketEntity { Paid = true });

        Func<Task> sut = async () => await _reservationService.ConfirmSeatReservation(Guid.NewGuid(), CancellationToken.None);

        await sut.Should().ThrowAsync<TicketAlreadyPaidException>();
    }

    [Fact]
    public async Task ConfirmSeatReservation_ThrowsSeatsReservationExpiredException_WhenReservationIsExpired()
    {
        _ticketsRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TicketEntity { Paid = false, CreatedTime = DateTime.UtcNow.AddMinutes(-11) });

        Func<Task> sut = async () => await _reservationService.ConfirmSeatReservation(Guid.NewGuid(), CancellationToken.None);

        await sut.Should().ThrowAsync<SeatsReservationExpiredException>();
    }

    [Fact]
    public async Task ConfirmSeatReservation_ReturnsTicket_WhenReservationIsValid()
    {
        var ticketId = Guid.NewGuid();
        var ticketEntity = new TicketEntity
        {
            Id = ticketId, Paid = false, CreatedTime = DateTime.UtcNow,
            TicketSeats =_fixture.CreateMany<TicketSeatEntity>(3).ToList()
        };

        _ticketsRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketEntity);

        _ticketsRepositoryMock
            .Setup(repo => repo.ConfirmPaymentAsync(It.IsAny<TicketEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketEntity);

        var result = await _reservationService.ConfirmSeatReservation(ticketId);

        result.Should().NotBeNull();
        result.Id.Should().Be(ticketId);
    }
    
    
    [Fact]
    public void SelectContiguousSeatToReserve_ReturnsSingleSeat_WhenNumberOfSeatsToReserveIsOne()
    {
        var availableSeats = new List<Seat>
        {
            new(0, 0, 0, 1 ),
            new(0, 0, 0, 2 ),
        };

        var result = ReservationService.SelectContiguousSeatToReserve(availableSeats, 1);

        result.Should().ContainSingle().Which.VirtualSeatNumber.Should().Be(1);
    }

    [Fact]
    public void SelectContiguousSeatToReserve_ReturnsAllSeats_WhenNumberOfSeatsToReserveEqualsAvailableSeatsCountAndSeatsAreContiguous()
    {
        var availableSeats = new List<Seat>
        {
            new(0, 0, 0, 1 ),
            new(0, 0, 0, 2 ),
            new(0, 0, 0, 3 ),
        };

        var result = ReservationService.SelectContiguousSeatToReserve(availableSeats, 3);

        result.Should().HaveCount(3);
        result.Select(s => s.VirtualSeatNumber).Should().Equal(1, 2, 3);
    }

    [Fact]
    public void SelectContiguousSeatToReserve_ThrowsException_WhenNumberOfSeatsToReserveEqualsAvailableSeatsCountAndSeatsAreNotContiguous()
    {
        var availableSeats = new List<Seat>
        {
            new(0, 0, 0, 1 ),
            new(0, 0, 0, 3 ),
            new(0, 0, 0, 4 ),
        };

        Action sut = () => ReservationService.SelectContiguousSeatToReserve(availableSeats, 3);

        sut.Should().Throw<NotEnoughSeatsAvailableException>().WithMessage("Unable to find 3 contiguous free seats");
    }

    [Fact]
    public void SelectContiguousSeatToReserve_ReturnsContiguousSeats_WhenTheyAreFound()
    {
        var availableSeats = new List<Seat>
        {
            new(0, 0, 0, 1 ),
            new(0, 0, 0, 2 ),
            new(0, 0, 0, 4 ),
            new(0, 0, 0, 6 ),
            new(0, 0, 0, 8 ),
            new(0, 0, 0, 9 ),
            new(0, 0, 0, 10 ),
            new(0, 0, 0, 11 ),
            new(0, 0, 0, 12 ),
            new(0, 0, 0, 16 ),
            new(0, 0, 0, 17 ),
            new(0, 0, 0, 18 ),
        };

        var result = ReservationService.SelectContiguousSeatToReserve(availableSeats, 3);

        result.Should().HaveCount(3);
        result.Select(s => s.VirtualSeatNumber).Should().Equal(8,9, 10);
    }

    [Fact]
    public void SelectContiguousSeatToReserve_ThrowsException_WhenContiguousSeatsAreNotFound()
    {
        var availableSeats = new List<Seat>
        {
            new(0, 0, 0, 2 ),
            new(0, 0, 0, 4 ),
            new(0, 0, 0, 6 ),
            new(0, 0, 0, 8 ),
        };

        Action sut = () => ReservationService.SelectContiguousSeatToReserve(availableSeats, 2);

        sut.Should().Throw<NotEnoughSeatsAvailableException>().WithMessage("Unable to find 2 contiguous free seats");
    }
}
