using Microsoft.AspNetCore.Mvc.RazorPages;
using razorweb.Models;
using System.Text.Json;

namespace razorweb.Pages
{
    public class CanceladaModel : PageModel
    {
        public List<Tarea> TareasCanceladas { get; set; } = new();

        public void OnGet(string? busqueda)
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

            if (!System.IO.File.Exists(jsonFilePath))
            {
                TareasCanceladas = new List<Tarea>();
                return;
            }

            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent) ?? new List<Tarea>();

            TareasCanceladas = todasLasTareas
                .Where(t => t.estado == "Cancelado" &&
                           (string.IsNullOrWhiteSpace(busqueda) ||
                            t.nombreTarea.Contains(busqueda, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            ViewData["busqueda"] = busqueda;
        }
    }
}