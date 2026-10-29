using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace GestionOrdinateurs.Tests;

public class OrdinateurServiceTest
{
    private static ParcInformatiqueContext CreerContexte()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ParcInformatiqueContext(options);
    }

    // Q21 : vérifier l'ajout d'un ordinateur
    [Fact]
    public async Task AjouterOrdinateur_DoitAjouterUnOrdinateurALaListe()
    {
        // Arrange
        using var contexte = CreerContexte();
        var service = new OrdinateurService(contexte);
        int nombreAvant = (await service.ListerOrdinateursAsync()).Count;
        var nouveau = new Ordinateur
        {
            Id = 1001,
            Nom = "PC Test",
            Caracteristiques = new(
                16,
                TypeOrdinateur.Portable,
                512,
                "Intel i7")
        };

        // Act
        await service.AjouterOrdinateurAsync(nouveau);

        // Assert : un élément de plus, retrouvable par son ID
        Assert.Equal(
            nombreAvant + 1,
            (await service.ListerOrdinateursAsync()).Count);
        Assert.NotNull(await service.RenvoyerOrdinateurParIDAsync(1001));
    }

    // Q22a : suppression d'un ordinateur existant
    [Fact]
    public async Task SupprimerOrdinateur_Existant_RetourneTrueEtLeSupprime()
    {
        using var contexte = CreerContexte();
        var service = new OrdinateurService(contexte);
        await service.AjouterOrdinateurAsync(
            new Ordinateur
            {
                Id = 1002,
                Nom = "A supprimer",
                Caracteristiques = new(
                    8,
                    TypeOrdinateur.Fixe,
                    512,
                    "Intel i5")
            });

        bool resultat = await service.SupprimerOrdinateurAsync(1002);

        Assert.True(resultat);
        Assert.Null(await service.RenvoyerOrdinateurParIDAsync(1002));
    }

    // Q22b : suppression d'un ordinateur inexistant
    [Fact]
    public async Task SupprimerOrdinateur_Inexistant_RetourneFalse()
    {
        using var contexte = CreerContexte();
        var service = new OrdinateurService(contexte);

        bool resultat = await service.SupprimerOrdinateurAsync(99999);

        Assert.False(resultat);
    }

    [Fact]
    public async Task RequetesLinq_RetournentLesOrdinateursAttendus()
    {
        using var contexte = CreerContexte();
        var service = new OrdinateurService(contexte);

        contexte.Ordinateurs.AddRange(
            new Ordinateur
            {
                Id = 2001,
                Nom = "Portable disponible",
                Caracteristiques = new(
                    16,
                    TypeOrdinateur.Portable,
                    512,
                    "Intel i7",
                    "Dell"),
                Stock = 3,
                Prix = 900
            },
            new Ordinateur
            {
                Id = 2002,
                Nom = "Portable indisponible",
                Caracteristiques = new(
                    16,
                    TypeOrdinateur.Portable,
                    512,
                    "Intel i5",
                    "Dell"),
                Stock = 0,
                Prix = 1200
            },
            new Ordinateur
            {
                Id = 2003,
                Nom = "Mac disponible",
                Caracteristiques = new(
                    16,
                    TypeOrdinateur.Portable,
                    512,
                    "Apple M2",
                    "Apple"),
                Stock = 2,
                Prix = 1500
            });
        await contexte.SaveChangesAsync();

        var disponibles = await service.ListerOrdinateursDisponiblesAsync();
        var dell = await service.RechercherParMarqueAsync("dell");
        var parPrix = await service.ListerParPrixDecroissantAsync();

        Assert.Equal(2, disponibles.Count);
        Assert.Equal(2, dell.Count);
        Assert.Equal(2003, parPrix[0].Id);
        Assert.Equal(2002, parPrix[1].Id);
        Assert.Equal(2001, parPrix[2].Id);
    }

    [Fact]
    public void Diagnostiquer_UtiliseLePatternMatching()
    {
        using var contexte = CreerContexte();
        var service = new OrdinateurService(contexte);

        var horsService = new Ordinateur
        {
            Etat = EtatOrdinateur.HorsService
        };
        var lent = new Ordinateur
        {
            Etat = EtatOrdinateur.Lent,
            Caracteristiques = new(
                4,
                TypeOrdinateur.Portable,
                256,
                "Intel i3")
        };
        var enService = new Ordinateur
        {
            Etat = EtatOrdinateur.EnService
        };

        Assert.Equal("Remplacement", service.Diagnostiquer(horsService));
        Assert.Equal("Ajouter RAM", service.Diagnostiquer(lent));
        Assert.Equal("Aucune action", service.Diagnostiquer(enService));
    }

    [Fact]
    public async Task CalculerStatistiques_RetourneLesValeursDuParc()
    {
        using var contexte = CreerContexte();
        var service = new OrdinateurService(contexte);

        contexte.Ordinateurs.AddRange(
            new Ordinateur { Id = 3001, Stock = 2, Prix = 100 },
            new Ordinateur { Id = 3002, Stock = 0, Prix = 500 });
        contexte.Reparations.Add(
            new Reparation { OrdinateurId = 3001 });
        await contexte.SaveChangesAsync();

        var statistiques = service.CalculerStatistiques();

        Assert.Equal(2, statistiques.TotalOrdinateur);
        Assert.Equal(1, statistiques.OrdiDisponibles);
        Assert.Equal(200, statistiques.ValeurStock);
        Assert.Equal(1, statistiques.NbReparations);
    }

    [Fact]
    public void InventaireOrdinateurs_ExposeDesIndexeursParNomEtParIndex()
    {
        var premier = new Ordinateur { Id = 4001, Nom = "ThinkPad T14" };
        var second = new Ordinateur { Id = 4002, Nom = "MacBook Pro" };
        var inventaire = new InventaireOrdinateurs
        {
            Ordinateurs = new() { premier, second }
        };

        Assert.Same(premier, inventaire["thinkpad t14"]);
        Assert.Same(second, inventaire[1]);
        Assert.Null(inventaire["Ordinateur inconnu"]);
        Assert.Null(inventaire[-1]);
        Assert.Null(inventaire[2]);
    }

    [Fact]
    public async Task CreerReparation_DoitLaPersister()
    {
        using var contexte = CreerContexte();
        var ordinateur = new Ordinateur
        {
            Id = 5001,
            Nom = "PC réparation"
        };
        contexte.Ordinateurs.Add(ordinateur);

        var reparation = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            TypeReparation = "Remplacement SSD",
            Technicien = "Technicien test",
            DateDebut = DateTimeOffset.UtcNow,
            Statut = "En cours"
        };
        contexte.Reparations.Add(reparation);

        await contexte.SaveChangesAsync();

        var reparationEnregistree = await contexte.Reparations
            .SingleAsync();
        Assert.Equal(ordinateur.Id, reparationEnregistree.OrdinateurId);
        Assert.Equal("Remplacement SSD", reparationEnregistree.TypeReparation);
        Assert.Equal("Technicien test", reparationEnregistree.Technicien);
    }

    [Fact]
    public async Task SimulerReparation_DoitCreerUneReparationTerminee()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var factory = new TestDbContextFactory(options);
        var journal = new JournalService(new TestHostEnvironment());
        var service = new ReparationService(factory, journal);
        var ordinateur = new Ordinateur { Id = 5002, Nom = "PC réparé" };

        var reparation = await service.SimulerReparationAsync(ordinateur);

        using var contexte = new ParcInformatiqueContext(options);
        var reparationEnregistree = await contexte.Reparations
            .SingleAsync();
        Assert.Equal(reparationEnregistree.Id, reparation.Id);
        Assert.Equal(ordinateur.Id, reparation.OrdinateurId);
        Assert.Equal("Terminée", reparation.Statut);
        Assert.Equal("Terminée", reparationEnregistree.Statut);
    }

    private sealed class TestDbContextFactory(
        DbContextOptions<ParcInformatiqueContext> options)
        : IDbContextFactory<ParcInformatiqueContext>
    {
        public ParcInformatiqueContext CreateDbContext()
        {
            return new ParcInformatiqueContext(options);
        }
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Testing";
        public string ApplicationName { get; set; } = "GestionOrdinateurs.Tests";
        public string ContentRootPath { get; set; } =
            Path.Combine(Path.GetTempPath(), "GestionOrdinateursTests");
        public IFileProvider ContentRootFileProvider { get; set; } =
            new NullFileProvider();
    }
}