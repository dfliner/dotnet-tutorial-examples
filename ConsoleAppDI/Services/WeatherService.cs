using Microsoft.Extensions.Logging;

namespace ConsoleAppDI.Services
{
    public interface IWeatherService
    {
        Task<string> GetWeatherServiceMetadataAsync();
    }

    // https://www.weather.gov/documentation/services-web-api

    internal class WeatherService : IWeatherService
    {
        private const string baseAddress = @"https://api.weather.gov";
        private const double SpaceNeedleLatitude = 47.6204;
        private const double SpaceNeedleLongitude = -122.3494;

        // metadata service:
        // https://api.weather.gov/points/{lat},{lon}
        // forcast service:
        // https://api.weather.gov/gridpoints/SEW/124,69/forecast
        // hourly forcast service:
        // https://api.weather.gov/gridpoints/SEW/124,69/forecast/hourly

        private readonly ILogger<IWeatherService> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public WeatherService(IHttpClientFactory httpClientFactory, ILogger<IWeatherService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<string> GetWeatherServiceMetadataAsync()
        {
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                $"{baseAddress}/points/{SpaceNeedleLatitude},{SpaceNeedleLongitude}")
            {
                Headers =
                {
                    {"User-Agent", "ConsoleApp.WeatherService" }
                }
            };

            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(httpRequestMessage);

            // throws if request returns error
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
