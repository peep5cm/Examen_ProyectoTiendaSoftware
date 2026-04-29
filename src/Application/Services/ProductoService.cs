using MiniMarket.Application.DTOs;
using MiniMarket.Application.Interfaces;
using MiniMarket.Domain.Interfaces;

namespace MiniMarket.Application.Services;

/// <summary>
/// Caso de uso: consultas de inventario.
/// SRP: mapea entidades de dominio a DTOs para la capa de presentación.
/// </summary>
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repo;

    public ProductoService(IProductoRepository repo) =>
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));

    public async Task<IEnumerable<ProductoDTO>> ListarProductosAsync()
    {
        var productos = await _repo.ObtenerTodosAsync();
        return productos.Select(ToDto);
    }

    public async Task<IEnumerable<ProductoDTO>> ListarStockCriticoAsync()
    {
        var productos = await _repo.ObtenerStockCriticoAsync();
        return productos.Select(ToDto);
    }

    public async Task<ProductoDTO?> BuscarPorIdAsync(Guid id)
    {
        var p = await _repo.ObtenerPorIdAsync(id);
        return p is null ? null : ToDto(p);
    }

    private static ProductoDTO ToDto(MiniMarket.Domain.Entities.Producto p) => new(
        p.Id, p.Nombre, p.Codigo, p.Categoria,
        p.Precio, p.Stock,
        p.EstrategiaImpuesto.ObtenerTipoImpuesto(),
        p.Estado.ToString()
    );
}
