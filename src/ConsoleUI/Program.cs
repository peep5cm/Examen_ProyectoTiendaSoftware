using MiniMarket.Application.DTOs;
using MiniMarket.Application.Interfaces;
using MiniMarket.Application.Services;
using MiniMarket.Domain.Interfaces;
using MiniMarket.Infrastructure.Persistence;
using MiniMarket.Infrastructure.Repositories;
System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-HN");

IProductoRepository productoRepo = new InMemoryProductoRepository();
IVentaRepository    ventaRepo    = new InMemoryVentaRepository();
IVentaService       ventaService = new VentaService(productoRepo, ventaRepo);
IProductoService    productoSvc  = new ProductoService(productoRepo);

await DataSeeder.SembrarAsync(productoRepo);

bool salir = false;
while (!salir)
{
    Console.Clear();
    Console.WriteLine("╔══════════════════════════════════════════╗");
    Console.WriteLine("║   MINI-MARKET — Sistema de Ventas v1.0   ║");
    Console.WriteLine("╠══════════════════════════════════════════╣");
    Console.WriteLine("║  1. Ver inventario completo              ║");
    Console.WriteLine("║  2. Ver alertas de stock crítico         ║");
    Console.WriteLine("║  3. Procesar nueva venta                 ║");
    Console.WriteLine("║  4. Reabastecer Stock (Aumentar)         ║"); 
    Console.WriteLine("║  0. Salir                                ║");
    Console.WriteLine("╚══════════════════════════════════════════╝");
    Console.Write("\nOpción: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": await MostrarInventarioAsync(productoSvc);            break;
        case "2": await MostrarStockCriticoAsync(productoSvc);          break;
        case "3": await ProcesarVentaAsync(ventaService, productoSvc);  break;
        case "4": await ReabastecerStockAsync(productoSvc); break; // <-- Nueva línea
        case "0": salir = true;                                         break;
        default:  Console.WriteLine("Opción inválida."); Pausar();      break;
    }
}

Console.WriteLine("\n¡Hasta pronto!");

async Task MostrarInventarioAsync(IProductoService svc)
{
    Console.Clear();
    Console.WriteLine("── INVENTARIO COMPLETO ─────────────────────────────────────────────────────");
    
    Console.WriteLine($"{"Código",-8} {"Nombre",-24} {"Precio",10} {"Stock",6} {"Impuesto",12} {"Estado",-14}");
    Console.WriteLine(new string('─', 85));

    foreach (var p in await svc.ListarProductosAsync())
    {

        Console.ForegroundColor = p.Estado switch
        {
            "StockCritico" => ConsoleColor.Yellow,
            "Agotado"      => ConsoleColor.Red,
            _              => ConsoleColor.White
        };

        Console.WriteLine($"{p.Codigo,-8} {p.Nombre,-24} {p.Precio,10:C2} {p.Stock,6} {p.TipoImpuesto,12} {p.Estado,-14}");
        
        Console.ResetColor();
    }
    
    Console.WriteLine(new string('─', 85));
    Pausar();
}

async Task MostrarStockCriticoAsync(IProductoService svc)
{
    Console.Clear();
    Console.WriteLine("── ALERTAS DE INVENTARIO (CRÍTICO / AGOTADO) ────────────────────");
    
    // Obtenemos solo los productos que necesitan atención
    var criticos = (await svc.ListarStockCriticoAsync()).ToList();

    if (!criticos.Any()) 
    { 
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n ✅ ¡Todo bien! No hay productos con stock bajo."); 
        Console.ResetColor();
    }
    else 
    {
        Console.WriteLine($"\n{"Estado",-14} {"Código",-8} {"Nombre",-24} {"Stock",6}");
        Console.WriteLine(new string('─', 60));

        foreach (var p in criticos)
        {
            // Aplicamos el color según el estado
            Console.ForegroundColor = p.Estado switch
            {
                "Agotado"      => ConsoleColor.Red,    // Rojo si es 0
                "StockCritico" => ConsoleColor.Yellow, // Amarillo si es < 5
                _              => ConsoleColor.White
            };

            Console.WriteLine($"{p.Estado,-14} {p.Codigo,-8} {p.Nombre,-24} {p.Stock,6}");
        }
        Console.ResetColor();
        
        Console.WriteLine(new string('─', 60));
        Console.WriteLine($"Total de alertas: {criticos.Count}");
    }
    
    Pausar();
}
async Task ProcesarVentaAsync(IVentaService vSvc, IProductoService pSvc)
{
    Console.Clear();
    Console.WriteLine("── NUEVA VENTA ──────────────────────────────────────────────────────");
    
    var productos = (await pSvc.ListarProductosAsync()).ToList();
    
    Console.WriteLine($"\n{"#",-4} {"Código",-8} {"Nombre",-24} {"Precio",9} {"Stock",7}");
    Console.WriteLine(new string('─', 65));
    
    // Aquí corregimos el error de la 'p' asegurándonos de que el nombre coincida
    foreach (var p in productos)
    {
        if (p.Stock <= 0) Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{productos.IndexOf(p) + 1,-4} {p.Codigo,-8} {p.Nombre,-24} {p.Precio,9:C2} {p.Stock,7}");
        Console.ResetColor();
    }

    var lineas = new List<AgregarLineaRequest>();
    while (true)
    {
        Console.Write("\nNúmero producto (0 = confirmar): ");
        if (!int.TryParse(Console.ReadLine(), out int idx) || idx == 0) break;
        if (idx < 1 || idx > productos.Count) { Console.WriteLine("  ❌ Inválido."); continue; }

        var seleccionado = productos[idx - 1];

        if (seleccionado.Stock <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Error: '{seleccionado.Nombre}' está agotado.");
            Console.ResetColor();
            continue;
        }

        Console.Write($"Cantidad (Disponible {seleccionado.Stock}): ");
        if (!int.TryParse(Console.ReadLine(), out int cant) || cant <= 0) { Console.WriteLine("  ❌ Cantidad inválida."); continue; }

        if (cant > seleccionado.Stock)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠️ No hay suficiente stock.");
            Console.ResetColor();
            continue;
        }

        lineas.Add(new AgregarLineaRequest(seleccionado.Id, cant));
        
        // --- SE ELIMINÓ LA LÍNEA QUE DABA ERROR CS8852 ---
        // El stock se descuenta automáticamente en el servicio al crear la venta.

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  + {seleccionado.Nombre} × {cant} agregado.");
        Console.ResetColor();
    }

    if (!lineas.Any()) { Console.WriteLine("Venta cancelada."); Pausar(); return; }

try
    {
        var venta = await vSvc.CrearVentaAsync("CAJERO-01", lineas);
        
        Console.Clear();
        Console.WriteLine("\n       ╔═══════════════════════════════════════════╗");
        Console.WriteLine("       ║         MINI-MARKET — COMPROBANTE         ║");
        Console.WriteLine($"       ║  Fecha: {venta.Fecha:dd/MM/yyyy HH:mm}                  ║");
        Console.WriteLine("       ╠═══════════════════════════════════════════╣");
        Console.WriteLine($"       ║ {"Producto",-20} {"Cant",4} {"Subt",10}      ║");
        Console.WriteLine("       ║ ----------------------------------------- ║");

        foreach (var l in venta.Lineas)
        {
            // Cortamos el nombre si es muy largo para que no rompa el cuadro
            string nombreCorto = l.Producto.Length > 20 ? l.Producto.Substring(0, 17) + "..." : l.Producto;
            Console.WriteLine($"       ║ {nombreCorto,-20} {l.Cantidad,4} {l.Subtotal,10:C2}      ║");
        }

        Console.WriteLine("       ║ ----------------------------------------- ║");
        
        // --- AQUÍ ESTÁN LOS DATOS QUE FALTABAN ---
        Console.WriteLine($"       ║ {"SUBTOTAL:",-30} {venta.Subtotal,10:C2} ║");
        Console.WriteLine($"       ║ {"IMPUESTOS (15%):",-30} {venta.Impuesto,10:C2} ║");
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"       ║ {"TOTAL A PAGAR:",-30} {venta.Total,10:C2} ║");
        Console.ResetColor();
        
        Console.WriteLine("       ╚═══════════════════════════════════════════╝");
        Console.WriteLine("\n            ¡GRACIAS POR SU PREFERENCIA!");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  ❌ Error: {ex.Message}");
        Console.ResetColor();
    }
    Pausar();
}

async Task ReabastecerStockAsync(IProductoService svc)
{
    Console.Clear();
    Console.WriteLine("── REABASTECER STOCK (INGRESO DE MERCADERÍA) ───────────────────────");
    
    // 1. Listado mejorado con colores para ver qué urge reabastecer
    var productosDtos = (await svc.ListarProductosAsync()).ToList();
    foreach (var p in productosDtos)
    {
        Console.ForegroundColor = p.Estado switch {
            "Agotado" => ConsoleColor.Red,
            "StockCritico" => ConsoleColor.Yellow,
            _ => ConsoleColor.Gray
        };
        Console.WriteLine($"  [{p.Codigo,-8}] {p.Nombre,-24} | Stock Actual: {p.Stock,4}");
    }
    Console.ResetColor();

    Console.Write("\n➤ Ingrese el CÓDIGO del producto: ");
    string? codigoEntrada = Console.ReadLine()?.Trim().ToUpper();

    if (string.IsNullOrEmpty(codigoEntrada)) return;

    // 2. Búsqueda en el repositorio
    var todosLosProductos = await productoRepo.ObtenerTodosAsync();
    var prodReal = todosLosProductos.FirstOrDefault(p => p.Codigo == codigoEntrada);

    if (prodReal != null)
    {
        Console.Write($"¿Cuántas unidades ingresan de '{prodReal.Nombre}'?: ");
        string? inputCant = Console.ReadLine();

        if (int.TryParse(inputCant, out int cant))
        {
            try 
            {
                // Intentamos actualizar el objeto de dominio
                prodReal.Reabastecer(cant); 
                
                // Persistimos el cambio
                await productoRepo.ActualizarAsync(prodReal);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ OPERACIÓN EXITOSA");
                Console.WriteLine($"   Producto: {prodReal.Nombre}");
                Console.WriteLine($"   Nuevo Stock: {prodReal.Stock} ({prodReal.Estado})");
                Console.ResetColor();
            }
            catch (ArgumentException ex) // Captura los errores de validación de tu clase Producto
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ ERROR DE VALIDACIÓN: {ex.Message}");
                Console.ResetColor();
            }
        }
        else 
        {
            Console.WriteLine("\n❌ Error: La cantidad debe ser un número entero válido.");
        }
    }
    else 
    { 
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("\n❌ ERROR: El código de producto no existe en el sistema.");
        Console.ResetColor();
    }
    
    Pausar();
}
void Pausar() { Console.WriteLine("\n  [Enter para continuar]"); Console.ReadKey(true); }
