namespace GestionOrdinateurs.API;

public sealed class StockService : IStockService
{
    private readonly object _verrou = new();
    private int _stockSSD = 10;

    public int StockSSD
    {
        get
        {
            lock (_verrou)
            {
                return _stockSSD;
            }
        }
    }

    public async Task ConsommerSSDAsync(
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken);

        lock (_verrou)
        {
            if (_stockSSD > 0)
            {
                _stockSSD--;
            }
        }
    }

}
