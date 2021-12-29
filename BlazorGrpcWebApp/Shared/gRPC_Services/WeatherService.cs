using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using BlazorGrpcWebApp.Shared;

public class WeatherService : WeatherForecasts.WeatherForecastsBase
{
	private static readonly string[] Summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	public override Task<WeatherResponse> GetWeather(WeatherForecastRequest request, ServerCallContext context)
	{
		var reply = new WeatherResponse();
		var rng = new Random();

		reply.ForecastsProto.Add(Enumerable.Range(1, 10).Select(index => new WeatherForecastRequest
		{
			DateTimeStampProto = Timestamp.FromDateTime(DateTime.UtcNow),
			TemperatureProto = rng.Next(20, 55),
			SummaryProto = Summaries[rng.Next(Summaries.Length)]
		})); ;

		return Task.FromResult(reply);
	}
}