using Gig.Framework.EventBus.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace WebTest.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IPublishEndpoint _endPoint;
    private readonly IGigEventBus _eventBus;

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IGigEventBus eventBus,
        IPublishEndpoint endPoint)
    {
        _logger = logger;
        _eventBus = eventBus;
        _endPoint = endPoint;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        await _endPoint.Publish(
            new Startup.TestEvent
            {
                Id = 1
            });
        await _eventBus.PublishAsync(
            new Startup.MyEvent
            {
                Name = "mohammad",
                CorrelationEventId = Guid.Parse("15d18496-9e0c-47e0-8f14-5efdea659cf7"),
                CompanyId = 1,
                BranchId = 1,
                UserId = 1,
                LangTypeCode = 1,
                SubSystemId = 1
            });
        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
    }
}