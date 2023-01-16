using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Responses;
using VacationRental.Core.Models;
using Xunit;

namespace VacationRental.Api.Tests.IntegrationTests;

[Collection("Integration")]
public class UpdateRentalTests
{
    private readonly HttpClient _client;

    public UpdateRentalTests(IntegrationFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
    {
        var request = new RentalBindingModel
        {
            Units = 25,
            PreparationTimeInDays = 5
        };

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync("/api/v1/rentals", request))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
        {
            Assert.True(getResponse.IsSuccessStatusCode);

            var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
            Assert.Equal(request.Units, getResult.Units);
        }
        
        request = new RentalBindingModel
        {
            Units = 20,
            PreparationTimeInDays = 8
        };

        using (var postResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", request))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
        {
            Assert.True(getResponse.IsSuccessStatusCode);

            var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
            Assert.Equal(request.Units, getResult.Units);
        }
    }
}