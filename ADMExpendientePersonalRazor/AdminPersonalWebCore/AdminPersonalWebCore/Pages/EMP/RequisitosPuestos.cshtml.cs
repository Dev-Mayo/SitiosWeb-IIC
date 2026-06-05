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

        // P�gina actualmente seleccionada
        public int PaginaActual { get; set; } = 1;

        // Cantidad total de p�ginas calculadas
        public int TotalPaginas { get; set; }

        // M�ximo de registros por p�gina seg�n la HU
        public int TamanoPagina { get; set; } = 10;

        public void OnGet(int pagina = 1)
        {
            // Recupera mensajes guardados en TempData despu�s de un RedirectToPage
            if (TempData["MensajeExito"] != null)
            {
                MensajeExito = TempData["MensajeExito"]!.ToString();
            }

            // Recupera mensajes de error guardados en TempData despu�s de un RedirectToPage
            if (TempData["MensajeError"] != null)
            {
                MensajeError = TempData["MensajeError"]!.ToString();
            }

            // Carga �nicamente los registros de la p�gina solicitada
            CargarDatos(pagina);
        }

        public IActionResult OnPostCrear(string nombre)
        {
            // Obtiene el usuario actual para enviarlo al Service y registrar bit�cora
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                var requisito = new RequisitoPuesto
                {
                    nombre = nombre
                };

                _service.Insertar(requisito, usuarioActual);

                // Guarda el mensaje para mostrarlo despu�s del RedirectToPage
                TempData["MensajeExito"] = "El requisito de puesto ha sido registrado correctamente.";

                // Redirecciona para evitar repostear el formulario y aplicar la paginaci�n
                return RedirectToPage(new
                {
                    u = usuarioActual,
                    pagina = 1
                });
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se muestra el mensaje y se recarga la p�gina
                // manteniendo la l�gica de paginaci�n
                MensajeError = ex.Message;
                CargarDatos(1);

                return Page();
            }
        }

        public IActionResult OnPostEditar(int requisito_id, string nombre)
        {
            // Obtiene el usuario actual para enviarlo al Service y registrar bit�cora
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                var requisito = new RequisitoPuesto
                {
                    requisito_id = requisito_id,
                    nombre = nombre
                };

                _service.Actualizar(requisito, usuarioActual);

                // Guarda mensaje de �xito para mostrarlo despu�s de recargar la p�gina
                TempData["MensajeExito"] = "Requisito actualizado correctamente.";

                // Redirecciona para limpiar el handler de la URL y aplicar la paginaci�n
                return RedirectToPage(new
                {
                    u = usuarioActual,
                    pagina = 1
                });
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se muestra el mensaje y se recarga la p�gina
                // manteniendo la l�gica de paginaci�n
                MensajeError = ex.Message;
                CargarDatos(1);

                return Page();
            }
        }

        public IActionResult OnPostEliminar(int id)
        {
            // Obtiene el usuario actual para enviarlo al Service y registrar bit�cora
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                _service.Eliminar(id, usuarioActual);

                // Guarda mensaje de �xito para mostrarlo despu�s de recargar la p�gina
                TempData["MensajeExito"] = "Requisito eliminado correctamente.";

                // Redirecciona para limpiar el handler de la URL y aplicar la paginaci�n
                return RedirectToPage(new
                {
                    u = usuarioActual,
                    pagina = 1
                });
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se muestra el mensaje y se recarga la p�gina
                // manteniendo la l�gica de paginaci�n
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

            // Guarda la p�gina actual seleccionada
            PaginaActual = pagina;

            // Calcula cu�ntas p�ginas existen seg�n la cantidad total de registros
            TotalPaginas = (int)Math.Ceiling(listaCompleta.Count / (double)TamanoPagina);

            // Aplica la paginaci�n:
            // Skip omite los registros de p�ginas anteriores
            // Take toma �nicamente los registros de la p�gina actual
            Requisitos = listaCompleta
                .Skip((PaginaActual - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();
        }
        private string ObtenerUsuarioActual()
        {
            var usuario = Request.Query["u"].ToString();

            if (string.IsNullOrWhiteSpace(usuario))
            {
                usuario = HttpContext.Session.GetString("usuario")
                       ?? HttpContext.Session.GetString("nombreusuario")
                       ?? HttpContext.Session.GetString("Usuario")
                       ?? User.Identity?.Name
                       ?? "UsuarioDesconocido";
            }

            return usuario;
        }
    }
}