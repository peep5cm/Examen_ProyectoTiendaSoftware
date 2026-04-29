using MiniMarket.Application.DTOs;
using MiniMarket.Application.Interfaces;
using MiniMarket.Domain.Entities;
using MiniMarket.Domain.Interfaces;

namespace MiniMarket.Application.Services;

/// <summary>
/// Caso de uso: crear y consultar ventas.
/// SRP: orquesta el flujo sin contener lógica de dominio ni acceso directo a datos.
/// DIP: depende de IProductoRepository e IVentaRepository — nunca de implementaciones concretas.
/// </summary>
public class VentaService : IVentaService
{
    private readonly IProductoRepository _productoRepo;
    private readonly IVentaRepository    _ventaRepo;

    public VentaService(IProductoRepository productoRepo, IVentaRepository ventaRepo)
    {
        _productoRepo = productoRepo ?? throw new ArgumentNullException(nameof(productoRepo));
        _ventaRepo    = ventaRepo    ?? throw new ArgumentNullException(nameof(ventaRepo));
    }

    public async Task<VentaDTO> CrearVentaAsync(
        string cajeroId,
        IEnumerable<AgregarLineaRequest> lineas)
    {
        var venta = new Venta(cajeroId);

        foreach (var req in lineas)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(req.ProductoId)
                ?? throw new InvalidOperationException(
                    $"Producto con ID {req.ProductoId} no encontrado.");

            venta.AgregarLinea(producto, req.Cantidad);
            await _productoRepo.ActualizarAsync(producto); // persiste el stock reducido
        }

        if (!venta.TieneLineas())
            throw new InvalidOperationException("La venta debe tener al menos una línea.");

        await _ventaRepo.GuardarAsync(venta);
        return MapearVenta(venta);
    }

    public async Task<VentaDTO?> ObtenerVentaAsync(Guid ventaId)
    {
        var venta = await _ventaRepo.ObtenerPorIdAsync(ventaId);
        return venta is null ? null : MapearVenta(venta);
    }

    // ── Mapper privado: Venta → VentaDTO ─────────────────────────────────────
    private static VentaDTO MapearVenta(Venta v) => new(
        v.Id,
        v.Fecha,
        v.CajeroId,
        v.Lineas.Select(l => new LineaVentaDTO(
            l.Producto.Nombre,
            l.Cantidad,
            l.PrecioUnitario,
            l.CalcularSubtotal(),
            l.CalcularImpuesto(),
            l.CalcularTotal(),
            l.Producto.EstrategiaImpuesto.ObtenerTipoImpuesto()
        )).ToList().AsReadOnly(),
        v.CalcularSubtotal(),
        v.CalcularImpuesto(),
        v.CalcularTotal()
    );
}
