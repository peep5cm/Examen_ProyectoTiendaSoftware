using MiniMarket.Domain.Entities;
using MiniMarket.Domain.Interfaces;
using MiniMarket.Domain.Strategies;

namespace MiniMarket.Infrastructure.Persistence;

/// <summary>
/// Carga datos de prueba realistas para el mini-market.
/// En producción, se reemplaza por migraciones de base de datos.
/// </summary>
public static class DataSeeder
{
    public static async Task SembrarAsync(IProductoRepository repo)
    {
        var gravado = new ImpuestoGravado(0.15m);
        var exento  = new ImpuestoExento();

        var productos = new[]
        {
            // Productos exentos de IVA (granos básicos y lácteos)
            new Producto("Arroz Diana 1kg",     "ARR001", "Granos",    25.00m, 50,  exento,  8),
            new Producto("Frijoles Rojos 1kg",  "FRJ001", "Granos",    22.00m, 40,  exento,  8),
            new Producto("Maíz Molido 1kg",     "MAI001", "Granos",    18.00m, 35,  exento,  8),

            // Productos gravados con IVA 15%
            new Producto("Aceite Corona 1L",    "ACE001", "Aceites",   65.00m, 30,  gravado, 5),
            new Producto("Refresco Coca 500ml", "RFR001", "Bebidas",   18.00m, 100, gravado, 10),
            new Producto("Jabón Palmolive",     "JAB001", "Aseo",      35.00m, 25,  gravado, 5),
            new Producto("Shampoo Sedal 200ml", "SHA001", "Aseo",      55.00m, 20,  gravado, 5),

            // Stock crítico y agotado para probar alertas
            new Producto("Leche Sula 1L",       "LEC001", "Lácteos",   42.00m, 4,   exento,  8), // stock crítico
            new Producto("Pan Bimbo Blanco",    "PAN001", "Panadería", 28.00m, 0,   gravado, 5), // agotado
        };

        foreach (var p in productos)
            await repo.GuardarAsync(p);
    }
}
