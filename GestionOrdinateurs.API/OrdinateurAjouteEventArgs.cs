using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

public class OrdinateurAjouteEventArgs : EventArgs
{
    public Ordinateur? Ordinateur { get; set; }
}
