namespace GestionOrdinateurs.Shared;

public static class ParcInitializer
{
    public static List<Ordinateur> Ordinateurs { get; set; } = new List<Ordinateur>
    {
        new Ordinateur
        {
            Id = 1,
            Nom = "ThinkPad T14",
            Caracteristiques = new(16, TypeOrdinateur.Portable, 512, "Intel Core i5", "Lenovo")
        },
        new Ordinateur
        {
            Id = 2,
            Nom = "MacBook Pro",
            Caracteristiques = new(16, TypeOrdinateur.Portable, 512, "Apple M2", "Apple")
        },
        new Ordinateur
        {
            Id = 3,
            Nom = "OptiPlex 7090",
            Caracteristiques = new(32, TypeOrdinateur.Fixe, 1000, "Intel Core i7", "Dell")
        },
        new Ordinateur
        {
            Id = 4,
            Nom = "Omen 45L",
            Caracteristiques = new(32, TypeOrdinateur.Fixe, 2000, "AMD Ryzen 9", "HP")
        },
        new Ordinateur
        {
            Id = 5,
            Nom = "ZenBook 14",
            Caracteristiques = new(8, TypeOrdinateur.Portable, 512, "Intel Core i5", "Asus")
        }
    };
}