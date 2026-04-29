using MiniMarket.Domain.Interfaces;

namespace MiniMarket.Domain.Strategies;

/// <summary>
/// Estrategia concreta: productos exentos de IVA (granos básicos, medicamentos, etc.).
/// </summary>
public class ImpuestoExento : IImpuestoStrategy
{
    public decimal CalcularImpuesto(decimal precioBase) => 0m;
    public string ObtenerTipoImpuesto() => "Exento";
}
