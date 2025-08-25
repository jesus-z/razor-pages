using Microsoft.AspNetCore.Mvc.RazorPages;
using razorweb.Models;
using System.Text.Json;

namespace razorweb.Pages
{
    public class ConcluidaModel : PageModel
    {
        public List<Tarea> TareasConcluidas { get; set; } = new();

        public void OnGet(string? busqueda)
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

            if (!System.IO.File.Exists(jsonFilePath))
            {
                TareasConcluidas = new List<Tarea>();
                return;
            }

            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent) ?? new List<Tarea>();

            TareasConcluidas = todasLasTareas
                .Where(t => t.estado == "Finalizado" &&
                           (string.IsNullOrWhiteSpace(busqueda) ||
                            t.nombreTarea.Contains(busqueda, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            ViewData["busqueda"] = busqueda;
        }
    }
}