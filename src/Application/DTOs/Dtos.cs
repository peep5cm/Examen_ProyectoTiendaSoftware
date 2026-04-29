namespace MiniMarket.Application.DTOs;

public record ProductoDTO(
    Guid Id,
    string Nombre,
    string Codigo,
    string Categoria,
    decimal Precio,
    int Stock,
    string TipoImpuesto,
    string Estado
);

public record LineaVentaDTO(
    string Producto,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal,
    decimal Impuesto,
    decimal Total,
    string TipoImpuesto
);

public record VentaDTO(
    Guid Id,
    DateTime Fecha,
    string CajeroId,
    IReadOnlyList<LineaVentaDTO> Lineas,
    decimal Subtotal,
    decimal Impuesto,
    decimal Total
);

public record AgregarLineaRequest(Guid ProductoId, int Cantidad);
