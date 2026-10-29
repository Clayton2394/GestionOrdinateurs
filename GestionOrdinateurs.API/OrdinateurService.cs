using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

public class OrdinateurService : IOrdinateurService
{
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService? _journalService;

    public OrdinateurService(
        ParcInformatiqueContext context,
        JournalService? journalService = null)
    {
        _context = context;
        _journalService = journalService;
    }

    public async Task<List<Ordinateur>> ListerOrdinateursAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Ordinateurs
            .Include(ordinateur => ordinateur.Caracteristiques)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Ordinateur?> RenvoyerOrdinateurParIDAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Ordinateurs
            .Include(ordinateur => ordinateur.Caracteristiques)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;

    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;

    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    public async Task AjouterOrdinateurAsync(
        Ordinateur? ordi,
        CancellationToken cancellationToken = default)
    {
        if (ordi == null) return;

        _context.Ordinateurs.Add(ordi);
        await _context.SaveChangesAsync(cancellationToken);
        _journalService?.EcrireActionOrdinateur(
            $"Ordinateur {ordi.Nom} ajouté");

        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs
        {
            Ordinateur = ordi
        });
    }

    public async Task<bool> SupprimerOrdinateurAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var ordi = await _context.Ordinateurs
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (ordi == null) return false;

        _context.Ordinateurs.Remove(ordi);
        await _context.SaveChangesAsync(cancellationToken);
        _journalService?.EcrireActionOrdinateur(
            $"Ordinateur {ordi.Nom} supprimé");

        OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs
        {
            Id = id
        });

        return true;
    }

    public async Task<Ordinateur?> MettreAJourAsync(
        int id,
        Ordinateur ordinateurMisAJour,
        CancellationToken cancellationToken = default)
    {
        var ordi = await _context.Ordinateurs
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (ordi == null) return null;

        ordi.Nom = ordinateurMisAJour.Nom;
        ordi.Caracteristiques = ordinateurMisAJour.Caracteristiques;
        ordi.Stock = ordinateurMisAJour.Stock;
        ordi.Prix = ordinateurMisAJour.Prix;
        ordi.Vendu = ordinateurMisAJour.Vendu;
        ordi.Etat = ordinateurMisAJour.Etat;

        await _context.SaveChangesAsync(cancellationToken);
        _journalService?.EcrireActionOrdinateur(
            $"Ordinateur {ordi.Nom} modifié");

        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs
        {
            Ordinateur = ordi
        });

        return ordi;
    }

    public async Task<List<Ordinateur>> ListerOrdinateursDisponiblesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Ordinateurs
            .Include(ordinateur => ordinateur.Caracteristiques)
            .AsNoTracking()
            .Where(o => o.Stock > 0)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Ordinateur>> RechercherParMarqueAsync(
        string marque,
        CancellationToken cancellationToken = default)
    {
        return await _context.Ordinateurs
            .AsNoTracking()
            .Where(o => o.Caracteristiques.Marque.ToLower() == marque.ToLower())
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Ordinateur>> ListerParPrixDecroissantAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Ordinateurs
            .AsNoTracking()
            .OrderByDescending(o => o.Prix)
            .ToListAsync(cancellationToken);
    }

    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            { Etat: EtatOrdinateur.HorsService }
                => "Remplacement",

            {
                Etat: EtatOrdinateur.Lent,
                Caracteristiques: { RamGo: < 8 }
            }
                => "Ajouter RAM",

            { Etat: EtatOrdinateur.EnPanne }
                => "Lancer une réparation",

            { Etat: EtatOrdinateur.Reparable }
                => "Réparer",

            _ => "Aucune action"
        };
    }

    public (
        int TotalOrdinateur,
        int OrdiDisponibles,
        decimal ValeurStock,
        int NbReparations)
        CalculerStatistiques()
    {
        var ordinateurs = _context.Ordinateurs
            .AsNoTracking()
            .ToList();

        return (
            TotalOrdinateur: ordinateurs.Count,
            OrdiDisponibles: ordinateurs.Count(ordinateur => ordinateur.Stock > 0),
            ValeurStock: ordinateurs.Sum(
                ordinateur => ordinateur.Stock * ordinateur.Prix),
            NbReparations: _context.Reparations.Count());
    }
}