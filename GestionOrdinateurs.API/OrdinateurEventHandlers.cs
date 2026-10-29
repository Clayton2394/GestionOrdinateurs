namespace GestionOrdinateurs.API;

public static class OrdinateurEventHandlers
{
    public static void AfficherMessageAjout(
        object? sender,
        OrdinateurAjouteEventArgs e)
    {
        Console.WriteLine(
            $"[EVENT] Ordinateur ajouté : {e.Ordinateur?.Nom}");
    }

    public static void AfficherMessageSuppression(
        object? sender,
        OrdinateurSupprimeEventArgs e)
    {
        Console.WriteLine(
            $"[EVENT] Ordinateur supprimé : ID {e.Id}");
    }

    public static void AfficherMessageMiseAJour(
        object? sender,
        OrdinateurMisAJourEventArgs e)
    {
        Console.WriteLine(
            $"[EVENT] Ordinateur mis à jour : {e.Ordinateur?.Nom}");
    }
}
