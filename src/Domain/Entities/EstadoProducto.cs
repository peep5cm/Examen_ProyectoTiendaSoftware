namespace MiniMarket.Domain.Entities;

/// <summary>
/// Diagrama de estados: refleja el ciclo de vida completo de un producto en el inventario.
/// </summary>
public enum EstadoProducto
{
    Disponible,
    StockCritico,
    Agotado,
    Vencido,
    Retirado
}
