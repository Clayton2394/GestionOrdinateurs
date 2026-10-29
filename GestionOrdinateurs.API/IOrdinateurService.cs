using GestionOrdinateurs.Shared;
namespace GestionOrdinateurs.API;

public interface IOrdinateurService
{
    Task<List<Ordinateur>> ListerOrdinateursAsync(
        CancellationToken cancellationToken = default);
    Task AjouterOrdinateurAsync(
        Ordinateur? ordi,
        CancellationToken cancellationToken = default);
    Task<Ordinateur?> RenvoyerOrdinateurParIDAsync(
        int id,
        CancellationToken cancellationToken = default);
    Task<bool> SupprimerOrdinateurAsync(
        int id,
        CancellationToken cancellationToken = default);
    Task<Ordinateur?> MettreAJourAsync(
        int id,
        Ordinateur ordinateurMisAJour,
        CancellationToken cancellationToken = default);
    Task<List<Ordinateur>> ListerOrdinateursDisponiblesAsync(
        CancellationToken cancellationToken = default);
    Task<List<Ordinateur>> RechercherParMarqueAsync(
        string marque,
        CancellationToken cancellationToken = default);
    Task<List<Ordinateur>> ListerParPrixDecroissantAsync(
        CancellationToken cancellationToken = default);
    string Diagnostiquer(Ordinateur ordinateur);
    (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)
        CalculerStatistiques();
}