namespace MiniMarket.Domain.Entities;

/// <summary>
/// Representa una línea dentro de una venta.
/// SRP: calcula subtotal e impuesto de esa línea delegando al Strategy del producto.
/// </summary>
public class LineaVenta
{
    public Guid Id { get; private set; }
    public Producto Producto { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }

    public LineaVenta(Producto producto, int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0.");
        if (producto.Estado == EstadoProducto.Vencido)
            throw new InvalidOperationException(
                $"'{producto.Nombre}' está vencido y no puede venderse.");
        if (producto.Estado == EstadoProducto.Retirado)
            throw new InvalidOperationException(
                $"'{producto.Nombre}' ha sido retirado del inventario.");

        Id = Guid.NewGuid();
        Producto = producto;
        Cantidad = cantidad;
        PrecioUnitario = producto.Precio;
    }

    /// <summary>Subtotal antes de impuesto: precio × cantidad.</summary>
    public decimal CalcularSubtotal() =>
        Math.Round(PrecioUnitario * Cantidad, 2);

    /// <summary>Impuesto según la estrategia del producto (IVA o 0.00 si exento).</summary>
    public decimal CalcularImpuesto() =>
        Math.Round(Producto.EstrategiaImpuesto.CalcularImpuesto(PrecioUnitario) * Cantidad, 2);

    /// <summary>Total de la línea: subtotal + impuesto.</summary>
    public decimal CalcularTotal() => CalcularSubtotal() + CalcularImpuesto();
}
