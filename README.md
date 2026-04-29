# 🛒 Mini-Market — Sistema de Ventas en C# (.NET 10
)

## Estructura del Proyecto (Clean Architecture)

```
MiniMarket/
├── MiniMarket.csproj
└── src/
    ├── Domain/                          ← Capa de Dominio (núcleo)
    │   ├── Entities/
    │   │   ├── EstadoProducto.cs        ← Enum: ciclo de vida del producto
    │   │   ├── Producto.cs              ← Entidad principal (usa IImpuestoStrategy)
    │   │   ├── LineaVenta.cs            ← Línea de venta con cálculo delegado
    │   │   └── Venta.cs                 ← Agregado raíz (Subtotal / IVA / Total)
    │   ├── Interfaces/
    │   │   ├── IImpuestoStrategy.cs     ← Contrato del patrón Strategy
    │   │   ├── IProductoRepository.cs   ← Contrato del patrón Repository
    │   │   └── IVentaRepository.cs
    │   └── Strategies/
    │       ├── ImpuestoGravado.cs       ← IVA 15% sobre productos gravados
    │       └── ImpuestoExento.cs        ← 0% para granos, medicamentos, etc.
    │
    ├── Application/                     ← Capa de Aplicación (casos de uso)
    │   ├── DTOs/
    │   │   └── Dtos.cs                  ← ProductoDTO, VentaDTO, LineaVentaDTO, etc.
    │   ├── Interfaces/
    │   │   └── IServices.cs             ← IVentaService, IProductoService
    │   └── Services/
    │       ├── VentaService.cs          ← Caso de uso: procesar venta
    │       └── ProductoService.cs       ← Caso de uso: consultar inventario
    │
    ├── Infrastructure/                  ← Capa de Infraestructura (persistencia)
    │   ├── Repositories/
    │   │   ├── InMemoryProductoRepository.cs
    │   │   └── InMemoryVentaRepository.cs
    │   └── Persistence/
    │       └── DataSeeder.cs            ← Datos de prueba del mini-market
    │
    └── ConsoleUI/                       ← Capa de Presentación
        └── Program.cs                   ← Menú interactivo + impresión de ticket
```

## Cómo ejecutar

```bash
# Requiere .NET 8 SDK (https://dotnet.microsoft.com/download)
cd MiniMarket
dotnet run
```

## Patrones y principios implementados

| Elemento                  | Patrón / Principio       | Dónde                                  |
|---------------------------|--------------------------|----------------------------------------|
| `IImpuestoStrategy`       | **Strategy** (GoF)       | Domain/Interfaces + Domain/Strategies  |
| `IProductoRepository`     | **Repository** (GoF)     | Domain/Interfaces + Infrastructure     |
| Clases con una sola razón | **SRP** (SOLID)          | Todas las entidades y servicios        |
| Depender de abstracciones | **DIP** (SOLID)          | Application → Domain interfaces        |
| `VentaService`            | **Use Case** (Clean Arch)| Application/Services                   |

## Ejemplo de ticket generado

```
╔═══════════════════════════════════════════╗
║         MINI-MARKET — COMPROBANTE         ║
║  Fecha: 28/04/2026 10:35                  ║
╠═══════════════════════════════════════════╣
  Producto               Cant   Subtotal      IVA
  ───────────────────────────────────────────────
  Arroz Diana 1kg           2     50.00      0.00 (Exento)
  Refresco Coca 500ml       1     18.00      2.70 (IVA 15%)
  Jabón Palmolive           1     35.00      5.25 (IVA 15%)
  ───────────────────────────────────────────────
  Subtotal:                              103.00
  IVA 15%:                                 7.95
  TOTAL:                                 110.95
╚═══════════════════════════════════════════╝
```
