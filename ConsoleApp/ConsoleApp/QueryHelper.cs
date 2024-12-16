using ConsoleApp.Model;
using ConsoleApp.Model.Enum;
using ConsoleApp.OutputTypes;

namespace ConsoleApp;

public class QueryHelper : IQueryHelper
{
    // 1. Знайти всі оплачені доставки
    public IEnumerable<Delivery> GetPaidDeliveries(IEnumerable<Delivery> deliveries)
    {
        return deliveries.Where(d => d.PaymentId != null);
    }

    // 2. Знайти всі доставки, що зараз опрацьовуються системою
    public IEnumerable<Delivery> GetDeliveriesInProgress(IEnumerable<Delivery> deliveries)
    {
        return deliveries.Where(d => d.Status != DeliveryStatus.Canceled && d.Status != DeliveryStatus.Completed);
    }

    // 3. Сформувати DeliveriesShortInfo з усіх доставок певного клієнта
    public IEnumerable<DeliveriesShortInfo> GetShortInfoByClient(IEnumerable<Delivery> deliveries, Guid clientId)
    {
        return deliveries
            .Where(d => d.ClientId == clientId)
            .Select(d => new DeliveriesShortInfo
            {
                DeliveryId = d.Id,
                DeliveryType = d.Type,
                StartCity = d.Direction.Origin.City,
                EndCity = d.Direction.Destination.City
            });
    }

    // 4. Повернути перших 10 доставок певного типу, що починаються з певного міста
    public IEnumerable<Delivery> GetFirst10DeliveriesOfTypeFromCity(IEnumerable<Delivery> deliveries, DeliveryType type, string startCity)
    {
        return deliveries
            .Where(d => d.Type == type && d.Direction.Origin.City == startCity)
            .Take(10);
    }

    // 5. Відсортувати записи по їх статусу, якщо статуси однакові, відсортувати за часом початку завантаження (за зростанням)
    public IEnumerable<Delivery> SortDeliveriesByStatusAndStartTime(IEnumerable<Delivery> deliveries)
    {
        return deliveries
            .OrderBy(d => d.Status)
            .ThenBy(d => d.TimePeriod.Start);
    }

    // 6. Підрахувати кількість унікальних типів вантажів
    public int GetUniqueCargoTypeCount(IEnumerable<Delivery> deliveries)
    {
        return deliveries
            .Select(d => d.Cargo.Type)
            .Distinct()
            .Count();
    }

    // 7. Згрупувати доставки за їх статусом та підрахувати кількість доставок в кожній групі
    public Dictionary<DeliveryStatus, int> GroupDeliveriesByStatus(IEnumerable<Delivery> deliveries)
    {
        return deliveries
            .GroupBy(d => d.Status)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    // 8. Згрупувати доставки за парами "місто старту-місто фінішу" та порахувати середній проміжок між кінцем часу завантаження та початком часу прибуття
    public Dictionary<(string StartCity, string EndCity), double> GetAverageDurationByCityPairs(IEnumerable<Delivery> deliveries)
    {
        return deliveries
            .GroupBy(d => (d.Direction.Origin.City, d.Direction.Destination.City))
            .ToDictionary(
                g => g.Key,
                g => g.Average(d => (d.TimePeriod.End - d.TimePeriod.Start)?.TotalHours ?? 0)
            );
    }

    // 9. Метод, що віддає певну сторінку даних
    public IEnumerable<Delivery> GetPagedDeliveries(
        IEnumerable<Delivery> deliveries,
        Func<Delivery, bool>? filter,
        Func<Delivery, object>? sorter,
        int pageSize,
        int pageNumber)
    {
        var filteredDeliveries = filter != null ? deliveries.Where(filter) : deliveries;
        return filteredDeliveries
            .OrderBy(sorter ?? (d => d.Id))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
}
