using MiniMarket.Domain.Interfaces;

namespace MiniMarket.Domain.Entities;

/// <summary>
/// Entidad raíz del dominio — Mini-Market.
/// SRP: gestiona sus propios datos y transiciones de estado.
/// DIP: usa IImpuestoStrategy, no una implementación concreta.
/// </summary>
public class Producto
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; }
    public string Codigo { get; private set; }
    public string Categoria { get; private set; }
    public decimal Precio { get; private set; }
    public int Stock { get; private set; }
    public int StockMinimo { get; private set; }
    public DateTime? FechaVencimiento { get; private set; }
    public EstadoProducto Estado { get; private set; }
    public IImpuestoStrategy EstrategiaImpuesto { get; private set; }

    public Producto(
        string nombre,
        string codigo,
        string categoria,
        decimal precio,
        int stock,
        IImpuestoStrategy estrategiaImpuesto,
        int stockMinimo = 5,
        DateTime? fechaVencimiento = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
        if (precio < 0)
            throw new ArgumentException("El precio no puede ser negativo.", nameof(precio));
        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo.", nameof(stock));

        Id = Guid.NewGuid();
        Nombre = nombre;
        Codigo = codigo;
        Categoria = categoria;
        Precio = precio;
        Stock = stock;
        StockMinimo = stockMinimo;
        FechaVencimiento = fechaVencimiento;
        EstrategiaImpuesto = estrategiaImpuesto
            ?? throw new ArgumentNullException(nameof(estrategiaImpuesto));
        Estado = DeterminarEstado();
    }

    /// <summary>Precio final incluyendo el impuesto según la estrategia asignada.</summary>
    public decimal CalcularPrecioConImpuesto() =>
        Precio + EstrategiaImpuesto.CalcularImpuesto(Precio);

    /// <summary>Descuenta stock y recalcula el estado. Lanza si no hay suficiente stock.</summary>
    public void ReducirStock(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0.", nameof(cantidad));
        if (cantidad > Stock)
            throw new InvalidOperationException(
                $"Stock insuficiente: solo hay {Stock} unidades de '{Nombre}'.");

        Stock -= cantidad;
        Estado = DeterminarEstado();
    }

    public void Reabastecer(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad de reabastecimiento debe ser mayor a 0.");
        Stock += cantidad;
        Estado = DeterminarEstado();
    }

    public void Retirar() => Estado = EstadoProducto.Retirado;

    /// <summary>Permite cambiar la estrategia de impuesto en tiempo de ejecución (patrón Strategy).</summary>
    public void ActualizarEstrategia(IImpuestoStrategy nuevaEstrategia)
    {
        EstrategiaImpuesto = nuevaEstrategia
            ?? throw new ArgumentNullException(nameof(nuevaEstrategia));
    }

    private EstadoProducto DeterminarEstado()
    {
        if (FechaVencimiento.HasValue && FechaVencimiento.Value.Date < DateTime.Today)
            return EstadoProducto.Vencido;
        if (Stock == 0)
            return EstadoProducto.Agotado;
        if (Stock <= StockMinimo)
            return EstadoProducto.StockCritico;
        return EstadoProducto.Disponible;
    }
}
