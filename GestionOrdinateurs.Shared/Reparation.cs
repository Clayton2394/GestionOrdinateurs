namespace GestionOrdinateurs.Shared;

public class Reparation
{
    public int Id { get; set; }
    public int OrdinateurId { get; set; }
    public Ordinateur? UnOrdinateur { get; set; }
    public string TypeReparation { get; set; } = string.Empty;
    public string Technicien { get; set; } = string.Empty;
    public DateTimeOffset DateDebut { get; set; }
    public string Statut { get; set; } = string.Empty;
}
