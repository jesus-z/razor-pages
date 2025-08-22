using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WARazor.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace WARazor.Pages;

public class IndexModel : PageModel
{
    public List<Tarea> Tareas { get; set; }
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
    public int TamanoPagina { get; set; } = 8;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(string busqueda, int pagina = 1)
    {
        string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

        var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
        var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent);

        var tareasFiltradas = todasLasTareas
            .Where(t => (t.estado == "Pendiente" || t.estado == "En curso") &&
                        (string.IsNullOrWhiteSpace(busqueda) || t.nombreTarea.Contains(busqueda, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        PaginaActual = pagina < 1 ? 1 : pagina;
        TotalPaginas = (int)Math.Ceiling(tareasFiltradas.Count / (double)TamanoPagina);
        Tareas = tareasFiltradas.Skip((PaginaActual - 1) * TamanoPagina).Take(TamanoPagina).ToList();
    }

}
