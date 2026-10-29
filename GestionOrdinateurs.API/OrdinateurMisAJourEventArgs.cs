using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

public class OrdinateurMisAJourEventArgs : EventArgs
{
    public Ordinateur? Ordinateur { get; set; }
}