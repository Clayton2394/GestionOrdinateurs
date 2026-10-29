using System.Globalization;

namespace GestionOrdinateurs.API;

public sealed class JournalService
{
    private readonly object _verrou = new();
    private readonly string _reparationsLogPath;
    private readonly string _ordinateursLogPath;

    public JournalService(IHostEnvironment environment)
    {
        var logsDirectory = Path.Combine(environment.ContentRootPath, "Logs");
        var reparationsDirectory = Path.Combine(logsDirectory, "Reparations_Logs");
        var ordinateursDirectory = Path.Combine(logsDirectory, "Ordinateurs");

        Directory.CreateDirectory(logsDirectory);
        Directory.CreateDirectory(reparationsDirectory);
        Directory.CreateDirectory(ordinateursDirectory);

        _reparationsLogPath = Path.Combine(
            reparationsDirectory,
            "reparations.log");
        _ordinateursLogPath = Path.Combine(
            ordinateursDirectory,
            "ordinateurs.log");
    }

    public void Ecrire(string message)
    {
        EcrireDansFichier(_reparationsLogPath, message);
    }

    public void EcrireActionOrdinateur(string message)
    {
        EcrireDansFichier(_ordinateursLogPath, message);
    }

    private void EcrireDansFichier(string chemin, string message)
    {
        var ligne = string.Format(
            CultureInfo.InvariantCulture,
            "{0:dd/MM/yyyy} - {1}",
            DateTime.Now,
            message);

        lock (_verrou)
        {
            using var writer = new StreamWriter(chemin, append: true);
            writer.WriteLine(ligne);
        }
    }
}
