namespace CiCd.With.Jenkins.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        List<Item> result = Enumerable.
            Range(1, Random.Shared.Next(0, 10)).
            Select(x => Generate(id: x)).
            ToList();

        _logger.LogDebug("Returning {Count} items", result.Count);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (Random.Shared.Next(0, 10) % 2 == 0)
        {
            _logger.LogWarning("Item with id={Id} was not found", id);
            return NotFound();
        }

        Item result = Generate(id: id);

        _logger.LogInformation("Item with id={Id} was found", id);
        _logger.LogDebug("Item found: {Item}", result);

        return Ok(result);
    }

    [HttpPost]
    public IActionResult Create(string? title)
    {
        Item result = Generate(title: title);

        _logger.LogInformation("Item with id={Id} was created", result.Id);
        _logger.LogDebug("Item was created: {Item}", result);

        return Created(
            Url.ActionLink(nameof(GetById), values: new { id = result.Id })!,
            result);
    }

    private static Item Generate(int? id = null, string? title = null)
    {
        var result = new Item
        (
            Id: id ?? Random.Shared.Next(1, 1000),
            Title: title ?? $"Title {Guid.NewGuid()}"
        );

        return result;
    }
}
