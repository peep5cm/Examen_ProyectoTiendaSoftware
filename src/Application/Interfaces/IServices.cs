using MiniMarket.Application.DTOs;

namespace MiniMarket.Application.Interfaces;

public interface IVentaService
{
    Task<VentaDTO> CrearVentaAsync(string cajeroId, IEnumerable<AgregarLineaRequest> lineas);
    Task<VentaDTO?> ObtenerVentaAsync(Guid ventaId);
}

public interface IProductoService
{
    Task<IEnumerable<ProductoDTO>> ListarProductosAsync();
    Task<IEnumerable<ProductoDTO>> ListarStockCriticoAsync();
    Task<ProductoDTO?> BuscarPorIdAsync(Guid id);
}
