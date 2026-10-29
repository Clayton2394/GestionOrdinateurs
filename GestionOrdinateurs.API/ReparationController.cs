using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionOrdinateurs.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReparationController : ControllerBase
{
    private readonly IOrdinateurService _ordinateurService;
    private readonly IReparationService _reparationService;
    private readonly IStockService _stockService;
    private readonly ILogger<ReparationController> _logger;

    public ReparationController(
        IOrdinateurService ordinateurService,
        IReparationService reparationService,
        IStockService stockService,
        ILogger<ReparationController> logger)
    {
        _ordinateurService = ordinateurService;
        _reparationService = reparationService;
        _stockService = stockService;
        _logger = logger;
    }

    [HttpPost("SimulerReparation")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SimulerReparation(
        CancellationToken cancellationToken)
    {
        var ordinateurs = await _ordinateurService
            .ListerOrdinateursDisponiblesAsync(cancellationToken);

        if (ordinateurs.Count < 3)
        {
            return BadRequest(
                "Au moins trois ordinateurs disponibles sont nécessaires.");
        }

        _logger.LogInformation(
            "Démarrage de trois réparations en parallèle.");

        var reparations = await Task.WhenAll(
            ordinateurs
                .Take(3)
                .Select(ordinateur =>
                    _reparationService.SimulerReparationAsync(
                        ordinateur,
                        cancellationToken)));

        return Ok(reparations);
    }

    [HttpGet]
    public async Task<IActionResult> ListerReparations(
        CancellationToken cancellationToken)
    {
        var reparations = await _reparationService
            .ListerReparationsAsync(cancellationToken);

        return Ok(reparations);
    }

    [HttpGet("historique")]
    public async Task<IActionResult> ListerHistorique(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Récupération de l'historique des réparations.");

        var historique = await _reparationService
            .ListerHistoriqueAsync(cancellationToken);

        return Ok(historique);
    }

    [HttpGet("GetStockSSD")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStockSSD(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Démarrage de dix consommations concurrentes du stock SSD.");

        var consommations = Enumerable
            .Range(0, 10)
            .Select(_ => _stockService.ConsommerSSDAsync(
                cancellationToken));

        await Task.WhenAll(consommations);

        var stockRestant = _stockService.StockSSD;
        _logger.LogInformation(
            "Stock SSD restant après les consommations : {StockSSD}.",
            stockRestant);

        return Ok(stockRestant);
    }
}
