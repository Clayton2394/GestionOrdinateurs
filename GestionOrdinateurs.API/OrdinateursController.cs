using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GestionOrdinateurs.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdinateursController : ControllerBase
{
    private readonly IOrdinateurService _service;

    private readonly ILogger<OrdinateursController> _logger;
    private readonly IConfiguration _configuration;

    public OrdinateursController(IOrdinateurService service,
                            ILogger<OrdinateursController> logger,
                            IConfiguration configuration)
    {
        _service = service;
        _logger = logger;
        _configuration = configuration;

        if (service is OrdinateurService ordinateurService)
        {
            ordinateurService.OrdinateurAjoute
                += OrdinateurEventHandlers.AfficherMessageAjout;
            ordinateurService.OrdinateurSupprime
                += OrdinateurEventHandlers.AfficherMessageSuppression;
            ordinateurService.OrdinateurMisAJour
                += OrdinateurEventHandlers.AfficherMessageMiseAJour;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Récupération de la liste des ordinateurs.");
        return Ok(await _service.ListerOrdinateursAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComputerById(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recherche de l'ordinateur ID={Id}", id);

        var ordinateur = await _service.RenvoyerOrdinateurParIDAsync(
            id,
            cancellationToken);
        if (ordinateur == null)
        {
            _logger.LogWarning("Ordinateur ID={Id} introuvable", id);
            return NotFound();
        }

        return Ok(ordinateur);
    }

    [HttpGet("{id}/diagnostic")]
    public async Task<IActionResult> Diagnostic(
        int id,
        CancellationToken cancellationToken)
    {
        var ordinateur = await _service.RenvoyerOrdinateurParIDAsync(
            id,
            cancellationToken);

        if (ordinateur == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            OrdinateurId = ordinateur.Id,
            Ordinateur = ordinateur.Nom,
            Diagnostic = _service.Diagnostiquer(ordinateur)
        });
    }

    [HttpGet("statistiques")]
    public IActionResult Statistiques()
    {
        var (
            total,
            enStock,
            valeurStock,
            nbReparations) = _service.CalculerStatistiques();

        return Ok(new
        {
            TotalOrdinateur = total,
            OrdiDisponibles = enStock,
            ValeurStock = valeurStock,
            NbReparations = nbReparations
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] Ordinateur ordinateur,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ajout d'un ordinateur : {Nom}", ordinateur.Nom);
        await _service.AjouterOrdinateurAsync(ordinateur, cancellationToken);
        return CreatedAtAction(nameof(GetComputerById), new { id = ordinateur.Id }, ordinateur);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] Ordinateur ordinateur,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Mise à jour de l'ordinateur ID={Id}", id);
        var updated = await _service.MettreAJourAsync(
            id,
            ordinateur,
            cancellationToken);
        if (updated == null)
        {
            _logger.LogWarning("Mise à jour impossible : ordinateur ID={Id} introuvable", id);
            return NotFound();
        }
        return Ok(ordinateur);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Suppression de l'ordinateur ID={Id}", id);
        var deleted = await _service.SupprimerOrdinateurAsync(
            id,
            cancellationToken);
        if (!deleted)
        {
            _logger.LogWarning("Suppression impossible : ordinateur ID={Id} introuvable", id);
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("disponibles")]
    public async Task<IActionResult> GetAvailable(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recherche des ordinateurs disponibles.");
        return Ok(await _service.ListerOrdinateursDisponiblesAsync(
            cancellationToken));
    }

    [HttpGet("marque/{marque}")]
    public async Task<IActionResult> GetByBrand(
        string marque,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Recherche des ordinateurs de la marque {Marque}.",
            marque);
        return Ok(await _service.RechercherParMarqueAsync(
            marque,
            cancellationToken));
    }

    [HttpGet("prix-decrescent")]
    public async Task<IActionResult> GetByDescendingPrice(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Classement des ordinateurs par prix décroissant.");
        return Ok(await _service.ListerParPrixDecroissantAsync(
            cancellationToken));
    }

    [HttpGet("inventaire/{nom}")]
    public async Task<IActionResult> ChercherDansInventaire(
        string nom,
        CancellationToken cancellationToken)
    {
        var inventaire = new InventaireOrdinateurs
        {
            Ordinateurs = await _service.ListerOrdinateursAsync(
                cancellationToken)
        };
        var ordinateur = inventaire[nom];

        if (ordinateur == null)
        {
            return NotFound();
        }

        return Ok(ordinateur);
    }

    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        var appName = _configuration["Application:Name"];
        var appVersion = _configuration["Application:Version"];

        return Ok(new
        {
            Name = appName,
            Version = appVersion
        });
    }
}