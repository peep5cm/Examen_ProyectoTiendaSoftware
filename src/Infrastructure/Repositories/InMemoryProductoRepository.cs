using MiniMarket.Domain.Entities;
using MiniMarket.Domain.Interfaces;

namespace MiniMarket.Infrastructure.Repositories;

/// <summary>
/// Implementación en memoria del repositorio de productos.
/// En producción se reemplaza por EF Core sin modificar la capa Application (DIP cumplido).
/// </summary>
public class InMemoryProductoRepository : IProductoRepository
{
    private readonly Dictionary<Guid, Producto> _store = new();

    public Task<Producto?> ObtenerPorIdAsync(Guid id) =>
        Task.FromResult(_store.TryGetValue(id, out var p) ? p : null);

    public Task<IEnumerable<Producto>> ObtenerTodosAsync() =>
        Task.FromResult<IEnumerable<Producto>>(_store.Values.ToList());

    public Task<IEnumerable<Producto>> ObtenerStockCriticoAsync() =>
        Task.FromResult<IEnumerable<Producto>>(
            _store.Values
                  .Where(p => p.Estado == EstadoProducto.StockCritico
                           || p.Estado == EstadoProducto.Agotado)
                  .ToList());

    public Task GuardarAsync(Producto producto)
    {
        _store[producto.Id] = producto;
        return Task.CompletedTask;
    }

    public Task ActualizarAsync(Producto producto)
    {
        _store[producto.Id] = producto;
        return Task.CompletedTask;
    }
}
