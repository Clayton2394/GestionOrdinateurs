namespace GestionOrdinateurs.Shared;

public class Caracteristiques
{
    public int RamGo { get; set; }
    public TypeOrdinateur TypeOrdi { get; set; }
    public int StockageGo { get; set; }
    public string Processeur { get; set; } = string.Empty;
    public string Marque { get; set; } = string.Empty;

    public Caracteristiques(
        int ramGo,
        TypeOrdinateur typeOrdi,
        int stockageGo,
        string processeur,
        string marque = "")
    {
        RamGo = ramGo;
        TypeOrdi = typeOrdi;
        StockageGo = stockageGo;
        Processeur = processeur;
        Marque = marque;
    }

    public Caracteristiques()
    {
    }
}
