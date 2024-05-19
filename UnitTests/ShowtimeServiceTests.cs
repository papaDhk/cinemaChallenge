using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Services;
using ApiApplication.Services.Auditorium;
using ApiApplication.Services.Movies;
using ApiApplication.Services.Showtimes;
using ApiApplication.Services.Showtimes.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTests;

public class ShowtimeServiceTests
{
    private readonly Mock<IShowtimesRepository> _showtimesRepositoryMock;
    private readonly Mock<IMoviesService> _moviesServiceMock;
    private readonly Mock<IAuditoriumService> _auditoriumServiceMock;
    private readonly ShowtimeService _showtimeService;

    public ShowtimeServiceTests()
    {
        _showtimesRepositoryMock = new Mock<IShowtimesRepository>();
        _moviesServiceMock = new Mock<IMoviesService>();
        _auditoriumServiceMock = new Mock<IAuditoriumService>();
        _showtimeService = new ShowtimeService(_showtimesRepositoryMock.Object, _moviesServiceMock.Object, _auditoriumServiceMock.Object);
    }

    [Fact]
    public async Task CreateShowtime_ThrowsShowtimeCreationException_WhenSessionDateIsInThePast()
    {
        var showTimeCreationParameters = new ShowTimeCreationParameters
        {
            SessionDate = DateTime.UtcNow.AddTicks(-1),
            AuditoriumId = 1,
            MovieImDbId = "test-movie-id"
        };

        Func<Task> sut = async () => await _showtimeService.CreateShowtime(showTimeCreationParameters, CancellationToken.None);

        await sut.Should().ThrowAsync<ShowtimeCreationException>().WithMessage("Cannot create a showtime in the past");
    }

    [Fact]
    public async Task CreateShowtime_ThrowsAuditoriumNotAvailableException_WhenAuditoriumIsNotAvailable()
    {
        var showTimeCreationParameters = new ShowTimeCreationParameters
        {
            SessionDate = DateTime.UtcNow.AddHours(1),
            AuditoriumId = new Random().Next(),
            MovieImDbId = "test-movie-id"
        };

        _auditoriumServiceMock
            .Setup(service => service.IsAuditoriumAvailable(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Func<Task> sut = async () => await _showtimeService.CreateShowtime(showTimeCreationParameters);

        await sut.Should().ThrowAsync<AuditoriumNotAvailableException>().WithMessage("The auditorium requested for this showtime is not available");
    }

    [Fact]
    public async Task CreateShowtime_ThrowsNotFoundException_WhenMovieIsNotFound()
    {
        var showTimeCreationParameters = new ShowTimeCreationParameters
        {
            SessionDate = DateTime.UtcNow.AddHours(1),
            AuditoriumId = new Random().Next(),
            MovieImDbId = "test-movie-id"
        };

        _auditoriumServiceMock
            .Setup(service => service.IsAuditoriumAvailable(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _moviesServiceMock
            .Setup(service => service.GetMovieByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie)null);

        Func<Task> sut = async () => await _showtimeService.CreateShowtime(showTimeCreationParameters);

        await sut.Should().ThrowAsync<NotFoundException>().WithMessage("The movie requested for this showtime is not found");
    }

    [Fact]
    public async Task CreateShowtime_ReturnsValidShowtime_WhenAllConditionsAreMet()
    {
        var showTimeCreationParameters = new ShowTimeCreationParameters
        {
            SessionDate = DateTime.UtcNow.AddHours(1),
            AuditoriumId = new Random().Next(),
            MovieImDbId = "test-movie-id"
        };

        var movie = new Movie { Id = new Random().Next(), ImdbId = showTimeCreationParameters.MovieImDbId, Title = "Test Movie" };
        var showtimeEntity = new ShowtimeEntity { Id = 1, AuditoriumId = showTimeCreationParameters.AuditoriumId, MovieId = movie.Id, SessionDate = showTimeCreationParameters.SessionDate };

        _auditoriumServiceMock
            .Setup(service => service.IsAuditoriumAvailable(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _moviesServiceMock
            .Setup(service => service.GetMovieByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        _showtimesRepositoryMock
            .Setup(repo => repo.CreateShowtime(It.IsAny<ShowtimeEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(showtimeEntity);

        var result = await _showtimeService.CreateShowtime(showTimeCreationParameters);

        result.Should().NotBeNull();
        result.SessionDate.Should().Be(showTimeCreationParameters.SessionDate);
        result.Movie.Id.Should().Be(movie.Id);
        result.AuditoriumId.Should().Be(showTimeCreationParameters.AuditoriumId);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllShowtimes()
    {
        var showtimeEntities = new List<ShowtimeEntity>
        {
            new ShowtimeEntity { Id = 1, AuditoriumId = 1, MovieId = 1, SessionDate = DateTime.UtcNow },
            new ShowtimeEntity { Id = 2, AuditoriumId = 2, MovieId = 2, SessionDate = DateTime.UtcNow.AddHours(1) }
        };

        _showtimesRepositoryMock
            .Setup(repo => repo.GetAllAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(showtimeEntities);

        var result = await _showtimeService.GetAllAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(showtimeEntities.Count);
    }

    [Fact]
    public async Task GetShowtimeByIdAsync_ReturnsShowtime()
    {
        var showtimeEntity = new ShowtimeEntity { Id = new Random().Next(), AuditoriumId = new Random().Next(), MovieId = new Random().Next(), SessionDate = DateTime.UtcNow };

        _showtimesRepositoryMock
            .Setup(repo => repo.GetWithMoviesByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(showtimeEntity);

        var result = await _showtimeService.GetShowtimeByIdAsync(1, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(showtimeEntity.Id);
        result.AuditoriumId.Should().Be(showtimeEntity.AuditoriumId);
        result.SessionDate.Should().Be(showtimeEntity.SessionDate);
    }
}