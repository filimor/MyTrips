using Newtonsoft.Json;

namespace MyTrips.IntegrationTests.Extensions;

public static class HttpExtensions
{
    public static async Task<T?> DeserializedContentAsync<T>(this HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseString);
    }
}