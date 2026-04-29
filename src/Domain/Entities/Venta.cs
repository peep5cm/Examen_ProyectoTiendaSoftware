namespace MiniMarket.Domain.Entities;

/// <summary>
/// Agregado raíz que orquesta las líneas de venta.
/// SRP: acumula líneas y expone los cálculos financieros (Subtotal, Impuesto, Total).
/// </summary>
public class Venta
{
    private readonly List<LineaVenta> _lineas = new();

    public Guid Id { get; private set; }
    public DateTime Fecha { get; private set; }
    public string CajeroId { get; private set; }
    public IReadOnlyList<LineaVenta> Lineas => _lineas.AsReadOnly();

    public Venta(string cajeroId)
    {
        if (string.IsNullOrWhiteSpace(cajeroId))
            throw new ArgumentException("El ID del cajero es requerido.");
        Id = Guid.NewGuid();
        Fecha = DateTime.Now;
        CajeroId = cajeroId;
    }

    public void AgregarLinea(Producto producto, int cantidad)
    {
        producto.ReducirStock(cantidad); // lanza InvalidOperationException si stock insuficiente
        _lineas.Add(new LineaVenta(producto, cantidad));
    }

    /// <summary>Suma de (precio × cantidad) de todas las líneas, sin impuesto.</summary>
    public decimal CalcularSubtotal() =>
        Math.Round(_lineas.Sum(l => l.CalcularSubtotal()), 2);

    /// <summary>Suma de impuestos de todas las líneas (cero para exentos, IVA para gravados).</summary>
    public decimal CalcularImpuesto() =>
        Math.Round(_lineas.Sum(l => l.CalcularImpuesto()), 2);

    /// <summary>Total final: Subtotal + Impuesto.</summary>
    public decimal CalcularTotal() =>
        Math.Round(CalcularSubtotal() + CalcularImpuesto(), 2);

    public bool TieneLineas() => _lineas.Any();
}
