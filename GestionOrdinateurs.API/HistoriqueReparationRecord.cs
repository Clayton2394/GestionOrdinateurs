namespace GestionOrdinateurs.API;

public record HistoriqueReparationRecord(
    int OrdinateurId,
    string NomOrdinateur,
    string Technicien,
    string TypeReparation,
    DateTimeOffset DateIntervention);
