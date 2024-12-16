using ConsoleApp.Model;
using ConsoleApp.Model.Enum;
using ConsoleApp.OutputTypes;

namespace ConsoleApp;

public class QueryHelper : IQueryHelper
{
    /// <summary>
    /// Get Deliveries that has payed
    /// </summary>
    public IEnumerable<Delivery> Paid(IEnumerable<Delivery> deliveries)
    => deliveries.Where(d => d.IsPaid); //TODO: Завдання 1
    
    /// <summary>
    /// Get Deliveries that now processing by system (not Canceled or Done)
    /// </summary>
    public IEnumerable<Delivery> NotFinished(IEnumerable<Delivery> deliveries)
    => deliveries.Where(d => d.Status != DeliveryStatus.Canceled && d.Status != DeliveryStatus.Done); //TODO: Завдання 2
    
    /// <summary>
    /// Get DeliveriesShortInfo from deliveries of specified client
    /// </summary>
    public IEnumerable<DeliveryShortInfo> DeliveryInfosByClient(IEnumerable<Delivery> deliveries, string clientId)
    => deliveries
        .Where(d => d.ClientId == clientId)
        .Select(d => new DeliveryShortInfo
        {
            DeliveryId = d.Id,
            CargoType = d.CargoType,
            StartCity = d.StartCity,
            EndCity = d.EndCity,
            Status = d.Status
        }); //TODO: Завдання 3
    
    /// <summary>
    /// Get first ten Deliveries that starts at specified city and have specified type
    /// </summary>
    public IEnumerable<Delivery> DeliveriesByCityAndType(IEnumerable<Delivery> deliveries, string cityName, DeliveryType type)
    => deliveries
        .Where(d => d.StartCity == cityName && d.Type == type)
        .Take(10); //TODO: Завдання 4
    
    /// <summary>
    /// Order deliveries by status, then by start of loading period
    /// </summary>
    public IEnumerable<Delivery> OrderByStatusThenByStartLoading(IEnumerable<Delivery> deliveries)
    => deliveries
        .OrderBy(d => d.Status)
        .ThenBy(d => d.LoadingStartTime); //TODO: Завдання 5

    /// <summary>
    /// Count unique cargo types
    /// </summary>
    public int CountUniqCargoTypes(IEnumerable<Delivery> deliveries)
    => deliveries
        .Select(d => d.CargoType)
        .Distinct()
        .Count(); //TODO: Завдання 6
    
    /// <summary>
    /// Group deliveries by status and count deliveries in each group
    /// </summary>
    public Dictionary<DeliveryStatus, int> CountsByDeliveryStatus(IEnumerable<Delivery> deliveries)
    => deliveries
        .GroupBy(d => d.Status)
        .ToDictionary(g => g.Key, g => g.Count()); //TODO: Завдання 7
    
    /// <summary>
    /// Group deliveries by start-end city pairs and calculate average gap between end of loading period and start of arrival period (calculate in minutes)
    /// </summary>
    public IEnumerable<AverageGapsInfo> AverageTravelTimePerDirection(IEnumerable<Delivery> deliveries)
    => deliveries
        .GroupBy(d => new { d.StartCity, d.EndCity })
        .Select(g => new AverageGapsInfo
        {
            StartCity = g.Key.StartCity,
            EndCity = g.Key.EndCity,
            AverageGapInMinutes = g
                .Average(d => (d.ArrivalStartTime - d.LoadingEndTime).TotalMinutes)
        }); //TODO: Завдання 8

    /// <summary>
    /// Paging helper
    /// </summary>
    public IEnumerable<TElement> Paging<TElement, TOrderingKey>(
    IEnumerable<TElement> elements,
    Func<TElement, TOrderingKey> ordering,
    Func<TElement, bool>? filter = null,
    int countOnPage = 100,
    int pageNumber = 1)
{
    return elements
        .Where(filter ?? (_ => true)) // Якщо фільтрація не вказана, беремо всі елементи
        .OrderBy(ordering)
        .Skip((pageNumber - 1) * countOnPage)
        .Take(countOnPage);
} //TODO: Завдання 9 
}
