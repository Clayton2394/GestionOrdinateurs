using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

public sealed class ReparationService : IReparationService
{
    private readonly IDbContextFactory<ParcInformatiqueContext> _contextFactory;
    private readonly JournalService _journalService;

    public ReparationService(
        IDbContextFactory<ParcInformatiqueContext> contextFactory,
        JournalService journalService)
    {
        _contextFactory = contextFactory;
        _journalService = journalService;
    }

    public async Task<Reparation> SimulerReparationAsync(
        Ordinateur ordinateur,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(
            cancellationToken);

        var reparation = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            TypeReparation = "Réparation générale",
            Technicien = "Technicien principal",
            DateDebut = DateTimeOffset.UtcNow,
            Statut = "En cours"
        };

        context.Reparations.Add(reparation);
        await context.SaveChangesAsync(cancellationToken);

        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        reparation.Statut = "Terminée";
        await context.SaveChangesAsync(cancellationToken);

        _journalService.Ecrire($"{ordinateur.Nom} réparé");
        return reparation;
    }

    public async Task<List<Reparation>> ListerReparationsAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(
            cancellationToken);

        return await context.Reparations
            .AsNoTracking()
            .Include(reparation => reparation.UnOrdinateur)
            .OrderByDescending(reparation => reparation.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<HistoriqueReparationRecord>> ListerHistoriqueAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(
            cancellationToken);

        return await context.Reparations
            .AsNoTracking()
            .OrderByDescending(reparation => reparation.Id)
            .Select(reparation => new HistoriqueReparationRecord(
                reparation.OrdinateurId,
                reparation.UnOrdinateur != null
                    ? reparation.UnOrdinateur.Nom
                    : string.Empty,
                reparation.Technicien,
                reparation.TypeReparation,
                reparation.DateDebut))
            .ToListAsync(cancellationToken);
    }
}
