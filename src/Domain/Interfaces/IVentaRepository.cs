using MiniMarket.Domain.Entities;

namespace MiniMarket.Domain.Interfaces;

public interface IVentaRepository
{
    Task<Venta?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Venta>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta);
    Task GuardarAsync(Venta venta);
}
