using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

public interface IReparationService
{
    Task<Reparation> SimulerReparationAsync(
        Ordinateur ordinateur,
        CancellationToken cancellationToken = default);

    Task<List<Reparation>> ListerReparationsAsync(
        CancellationToken cancellationToken = default);

    Task<List<HistoriqueReparationRecord>> ListerHistoriqueAsync(
        CancellationToken cancellationToken = default);
}
