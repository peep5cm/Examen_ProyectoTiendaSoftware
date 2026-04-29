namespace MiniMarket.Domain.Interfaces;

/// <summary>
/// Patrón Strategy — contrato para el cálculo de impuestos.
/// SRP: única responsabilidad = definir cómo se calcula el impuesto.
/// DIP: capas superiores dependen de esta abstracción, nunca de implementaciones concretas.
/// </summary>
public interface IImpuestoStrategy
{
    decimal CalcularImpuesto(decimal precioBase);
    string ObtenerTipoImpuesto();
}
