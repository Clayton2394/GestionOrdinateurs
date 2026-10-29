namespace GestionOrdinateurs.API;

public interface IStockService
{
    int StockSSD { get; }

    Task ConsommerSSDAsync(CancellationToken cancellationToken = default);
}
