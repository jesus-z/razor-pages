using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razorweb.Models;
using System.Text.Json;

namespace razorweb.Pages
{
    public class IndexModel : PageModel
    {
        public List<Tarea> Tareas { get; set; } = new();
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 8;

        private readonly ILogger<IndexModel> _logger;
        private readonly string _jsonFilePath;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");
        }

        public void OnGet(string? busqueda, string? filtroEstado, int pagina = 1)
        {
            var todasLasTareas = CargarTareas();

            var tareasFiltradas = todasLasTareas
                .Where(t => (t.estado == "Pendiente" || t.estado == "En curso") &&
                           FiltrarPorEstado(t, filtroEstado) &&
                           FiltrarPorBusqueda(t, busqueda))
                .ToList();

            // Configurar paginación
            PaginaActual = pagina < 1 ? 1 : pagina;
            TotalPaginas = (int)Math.Ceiling(tareasFiltradas.Count / (double)TamanoPagina);

            Tareas = tareasFiltradas
                .Skip((PaginaActual - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();

            ViewData["filtroEstado"] = filtroEstado;
            ViewData["busqueda"] = busqueda;
        }

        public IActionResult OnPostCancelar(string nombreTarea)
        {
            return CambiarEstadoTarea(nombreTarea, "Cancelado");
        }

        public IActionResult OnPostConcluir(string nombreTarea)
        {
            return CambiarEstadoTarea(nombreTarea, "Finalizado");
        }
        private List<Tarea> CargarTareas()
        {
            if (!System.IO.File.Exists(_jsonFilePath))
            {
                return new List<Tarea>();
            }

            try
            {
                var jsonContent = System.IO.File.ReadAllText(_jsonFilePath);
                return JsonSerializer.Deserialize<List<Tarea>>(jsonContent) ?? new List<Tarea>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar tareas desde JSON");
                return new List<Tarea>();
            }
        }

        private void GuardarTareas(List<Tarea> tareas)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(tareas, options);
                System.IO.File.WriteAllText(_jsonFilePath, jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar tareas en JSON");
            }
        }

        private bool FiltrarPorEstado(Tarea tarea, string? filtroEstado)
        {
            return string.IsNullOrEmpty(filtroEstado) || filtroEstado == "todos" ||
                   (filtroEstado == "pendiente" && tarea.estado == "Pendiente") ||
                   (filtroEstado == "en-curso" && tarea.estado == "En curso");
        }

        private bool FiltrarPorBusqueda(Tarea tarea, string? busqueda)
        {
            return string.IsNullOrWhiteSpace(busqueda) ||
                   tarea.nombreTarea.Contains(busqueda, StringComparison.OrdinalIgnoreCase);
        }

        private IActionResult CambiarEstadoTarea(string nombreTarea, string nuevoEstado)
        {
            var todasLasTareas = CargarTareas();
            var tarea = todasLasTareas.FirstOrDefault(t => t.nombreTarea == nombreTarea);

            if (tarea != null)
            {
                tarea.estado = nuevoEstado;
                GuardarTareas(todasLasTareas);
                _logger.LogInformation($"Tarea '{nombreTarea}' cambió a estado: {nuevoEstado}");
            }
            else
            {
                _logger.LogWarning($"No se encontró la tarea: {nombreTarea}");
            }

            return RedirectToPage();
        }
    }
}
