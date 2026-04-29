using MiniMarket.Domain.Entities;

namespace MiniMarket.Domain.Interfaces;

/// <summary>
/// Patrón Repository — abstrae la fuente de datos para los productos.
/// DIP: Application depende de esta interfaz, no de la implementación concreta.
/// </summary>
public interface IProductoRepository
{
    Task<Producto?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Producto>> ObtenerTodosAsync();
    Task<IEnumerable<Producto>> ObtenerStockCriticoAsync();
    Task GuardarAsync(Producto producto);
    Task ActualizarAsync(Producto producto);
}
