namespace GestionOrdinateurs.Shared;

public class InventaireOrdinateurs
{
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    public Ordinateur? this[string nom]
    {
        get => Ordinateurs.FirstOrDefault(
            ordinateur => string.Equals(
                ordinateur.Nom,
                nom,
                StringComparison.OrdinalIgnoreCase));
    }

    public Ordinateur? this[int index]
    {
        get => index >= 0 && index < Ordinateurs.Count
            ? Ordinateurs[index]
            : null;
    }
}
