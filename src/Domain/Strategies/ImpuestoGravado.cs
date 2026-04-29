using MiniMarket.Domain.Interfaces;

namespace MiniMarket.Domain.Strategies;

/// <summary>
/// Estrategia concreta: aplica IVA al precio del producto.
/// SRP: única responsabilidad = calcular el impuesto para productos gravados.
/// </summary>
public class ImpuestoGravado : IImpuestoStrategy
{
    private readonly decimal _tasaIva;

    public ImpuestoGravado(decimal tasaIva = 0.15m)
    {
        if (tasaIva < 0 || tasaIva > 1)
            throw new ArgumentException("La tasa IVA debe estar entre 0 y 1.", nameof(tasaIva));
        _tasaIva = tasaIva;
    }

    public decimal CalcularImpuesto(decimal precioBase) =>
        Math.Round(precioBase * _tasaIva, 2);

    public string ObtenerTipoImpuesto() => $"IVA {_tasaIva:P0}";
}
