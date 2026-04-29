using MiniMarket.Domain.Entities;
using MiniMarket.Domain.Interfaces;

namespace MiniMarket.Infrastructure.Repositories;

public class InMemoryVentaRepository : IVentaRepository
{
    private readonly Dictionary<Guid, Venta> _store = new();

    public Task<Venta?> ObtenerPorIdAsync(Guid id) =>
        Task.FromResult(_store.TryGetValue(id, out var v) ? v : null);

    public Task<IEnumerable<Venta>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta) =>
        Task.FromResult<IEnumerable<Venta>>(
            _store.Values
                  .Where(v => v.Fecha >= desde && v.Fecha <= hasta)
                  .ToList());

    public Task GuardarAsync(Venta venta)
    {
        _store[venta.Id] = venta;
        return Task.CompletedTask;
    }
}
