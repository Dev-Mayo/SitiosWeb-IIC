using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using System.Linq;

namespace AdminPersonalWebCore.Pages.EMP
{
    public class RequisitosPuestosModel : PageModel
    {
        private readonly RequisitoPuestoService _service;

        public RequisitosPuestosModel(RequisitoPuestoService service)
        {
            _service = service;
        }

        public List<RequisitoPuesto> Requisitos { get; set; } = new();

        [BindProperty]
        public RequisitoPuesto Requisito { get; set; } = new();

        public string? MensajeExito { get; set; }
        public string? MensajeError { get; set; }

        // Página actualmente seleccionada
        public int PaginaActual { get; set; } = 1;

        // Cantidad total de páginas calculadas
        public int TotalPaginas { get; set; }

        // Máximo de registros por página según la HU
        public int TamanoPagina { get; set; } = 10;

        public void OnGet(int pagina = 1)
        {
            // Recupera mensajes guardados en TempData después de un RedirectToPage
            if (TempData["MensajeExito"] != null)
            {
                MensajeExito = TempData["MensajeExito"]!.ToString();
            }

            // Recupera mensajes de error guardados en TempData después de un RedirectToPage
            if (TempData["MensajeError"] != null)
            {
                MensajeError = TempData["MensajeError"]!.ToString();
            }

            // Carga únicamente los registros de la página solicitada
            CargarDatos(pagina);
        }

        public IActionResult OnPostCrear(string nombre)
        {
            // Obtiene el usuario actual para enviarlo al Service y registrar bitácora
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                var requisito = new RequisitoPuesto
                {
                    nombre = nombre
                };

                _service.Insertar(requisito, usuarioActual);

                // Guarda el mensaje para mostrarlo después del RedirectToPage
                TempData["MensajeExito"] = "El requisito de puesto ha sido registrado correctamente.";

                // Redirecciona para evitar repostear el formulario y aplicar la paginación
                return RedirectToPage(new
                {
                    u = usuarioActual,
                    pagina = 1
                });
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se muestra el mensaje y se recarga la página
                // manteniendo la lógica de paginación
                MensajeError = ex.Message;
                CargarDatos(1);

                return Page();
            }
        }

        public IActionResult OnPostEditar(int requisito_id, string nombre)
        {
            // Obtiene el usuario actual para enviarlo al Service y registrar bitácora
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                var requisito = new RequisitoPuesto
                {
                    requisito_id = requisito_id,
                    nombre = nombre
                };

                _service.Actualizar(requisito, usuarioActual);

                // Guarda mensaje de éxito para mostrarlo después de recargar la página
                TempData["MensajeExito"] = "Requisito actualizado correctamente.";

                // Redirecciona para limpiar el handler de la URL y aplicar la paginación
                return RedirectToPage(new
                {
                    u = usuarioActual,
                    pagina = 1
                });
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se muestra el mensaje y se recarga la página
                // manteniendo la lógica de paginación
                MensajeError = ex.Message;
                CargarDatos(1);

                return Page();
            }
        }

        public IActionResult OnPostEliminar(int id)
        {
            // Obtiene el usuario actual para enviarlo al Service y registrar bitácora
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                _service.Eliminar(id, usuarioActual);

                // Guarda mensaje de éxito para mostrarlo después de recargar la página
                TempData["MensajeExito"] = "Requisito eliminado correctamente.";

                // Redirecciona para limpiar el handler de la URL y aplicar la paginación
                return RedirectToPage(new
                {
                    u = usuarioActual,
                    pagina = 1
                });
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se muestra el mensaje y se recarga la página
                // manteniendo la lógica de paginación
                MensajeError = ex.Message;
                CargarDatos(1);

                return Page();
            }
        }

        private void CargarDatos(int pagina = 1)
        {
            // Obtiene el usuario actual para consultar desde el Service
            var usuarioActual = ObtenerUsuarioActual();

            // Obtiene todos los registros desde la capa de servicios
            var listaCompleta = _service.ObtenerTodos(usuarioActual);

            // Guarda la página actual seleccionada
            PaginaActual = pagina;

            // Calcula cuántas páginas existen según la cantidad total de registros
            TotalPaginas = (int)Math.Ceiling(listaCompleta.Count / (double)TamanoPagina);

            // Aplica la paginación:
            // Skip omite los registros de páginas anteriores
            // Take toma únicamente los registros de la página actual
            Requisitos = listaCompleta
                .Skip((PaginaActual - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();
        }

        private string ObtenerUsuarioActual()
        {
            // Primero intenta obtener el usuario enviado por QueryString.
            // Ejemplo: ?u=JuPerez
            var usuarioQuery = Request.Query["u"].ToString();

            if (!string.IsNullOrWhiteSpace(usuarioQuery))
            {
                return usuarioQuery;
            }

            // Si no existe en QueryString, intenta obtenerlo desde la sesión.
            // Si tampoco existe en sesión, usa "Sistema" como valor por defecto.
            return HttpContext.Session.GetString("Usuario") ?? "Sistema";
        }
    }
}