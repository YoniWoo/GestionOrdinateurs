namespace GestionOrdinateurs.API.Services;

public class StockService
{
    private readonly object _verrou = new object();
    public int StockSSD = 10;

    public async void ConsommerSSD()
    {
        Task.Delay(10).Wait();
        lock (_verrou)
        {
            StockSSD--;        
        }
    }
}