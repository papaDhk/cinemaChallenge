using System.Net;
using System.Text.Json;
using ApiApplication.Services.Showtimes.Models;
using FluentAssertions;
using Tests.Endpoints;
using Xunit;

namespace Testss.Endpoints;

public class ShowtimesEndpointsTests : IClassFixture<EndpointTestWebAppFactory>
{
    private readonly HttpClient _httpClient;
    public ShowtimesEndpointsTests(EndpointTestWebAppFactory fixture)
    {
        _httpClient = fixture.CreateClient();
    }

    //This is an example of endpoint testing
    [Fact]
    public async Task ShowtimesGetEndpoint_ShouldReturnOK_WhenEverythingIsOK()
    {
        var response = await _httpClient.GetAsync("/api/showtimes");
        var contentJson = await response.Content.ReadAsStringAsync();
        var showtimes = JsonSerializer.Deserialize<IEnumerable<Showtime>>(contentJson);


        response.StatusCode.Should().Be(HttpStatusCode.OK);
        showtimes.Should().HaveCount(1);
    }
}