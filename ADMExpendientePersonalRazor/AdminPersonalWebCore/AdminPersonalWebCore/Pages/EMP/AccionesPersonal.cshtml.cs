using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using AdminPersonalWebCore.Services;
using System.Text.Json;

namespace AdminPersonalWebCore.Pages.EMP
{
    public class AccionesPersonalModel : PageModel
    {
        private readonly AccionPersonalRepository _accionRepository;
        private readonly BitacoraRepository _bitacoraRepository;
        private readonly ParametroService _parametroService;

        public AccionesPersonalModel(
            AccionPersonalRepository accionRepository,
            BitacoraRepository bitacoraRepository,
            ParametroService parametroService)
        {
            _accionRepository = accionRepository;
            _bitacoraRepository = bitacoraRepository;
            _parametroService = parametroService;
        }

        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 10;

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public List<AccionPersonal> Acciones { get; set; } = new();
        public List<Empleado> Empleados { get; set; } = new();

        [BindProperty]
        public AccionPersonal NuevaAccion { get; set; } = new();

        [BindProperty]
        public AccionPersonal AccionEditar { get; set; } = new();

        [BindProperty]
        public int AccionEliminarId { get; set; }

        public void OnGet(int pagina = 1)
        {
            try
            {
                CargarDatos(pagina);

                RegistrarBitacora(AccionBitacora.READ, new
                {
                    Mensaje = "El usuario consulta acciones de personal"
                });
            }
            catch
            {
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "Error al consultar acciones de personal"
                });

                MensajeError = "Ocurrió un error al consultar las acciones de personal.";
            }
        }

        public IActionResult OnPostCrear()
        {
            var usuarioActual = ObtenerUsuarioActual();

            if (!ValidarAccion(NuevaAccion))
            {
                return RedirectToPage(new { u = usuarioActual, pagina = 1 });
            }

            try
            {
                _accionRepository.Insertar(NuevaAccion);

                RegistrarBitacora(AccionBitacora.CREATE, new
                {
                    Nuevo = NuevaAccion
                });

                MensajeExito = "La acción de personal se registró correctamente.";
            }
            catch
            {
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "Error al registrar acción de personal",
                    Datos = NuevaAccion
                });

                MensajeError = "Ocurrió un error al registrar la acción de personal.";
            }

            return RedirectToPage(new { u = usuarioActual, pagina = 1 });
        }

        public IActionResult OnPostEditar()
        {
            var usuarioActual = ObtenerUsuarioActual();

            if (!ValidarAccion(AccionEditar))
            {
                return RedirectToPage(new { u = usuarioActual, pagina = 1 });
            }

            try
            {
                var accionAnterior = _accionRepository.ObtenerPorId(AccionEditar.AccionId);

                _accionRepository.Actualizar(AccionEditar);

                RegistrarBitacora(AccionBitacora.UPDATE, new
                {
                    Antes = accionAnterior,
                    Despues = AccionEditar
                });

                MensajeExito = "La acción de personal se actualizó correctamente.";
            }
            catch
            {
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "Error al actualizar acción de personal",
                    Datos = AccionEditar
                });

                MensajeError = "Ocurrió un error al actualizar la acción de personal.";
            }

            return RedirectToPage(new { u = usuarioActual, pagina = 1 });
        }

        public IActionResult OnPostEliminar()
        {
            var usuarioActual = ObtenerUsuarioActual();

            try
            {
                var accionEliminada = _accionRepository.ObtenerPorId(AccionEliminarId);

                _accionRepository.Eliminar(AccionEliminarId);

                RegistrarBitacora(AccionBitacora.DELETE, new
                {
                    Eliminado = accionEliminada
                });

                MensajeExito = "La acción de personal se eliminó correctamente.";
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "No se puede eliminar un registro con datos relacionados",
                    Id = AccionEliminarId
                });

                MensajeError = "No se puede eliminar un registro con datos relacionados.";
            }
            catch
            {
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "Error al eliminar acción de personal",
                    Id = AccionEliminarId
                });

                MensajeError = "Ocurrió un error al eliminar la acción de personal.";
            }

            return RedirectToPage(new { u = usuarioActual, pagina = 1 });
        }

        private void CargarDatos(int pagina = 1)
        {
            TamanoPagina = _parametroService.ObtenerValorEnteroODefecto(
                "CANTIDAD_REGISTROS_PAGINA",
                10
            );

            var todas = _accionRepository.ObtenerTodos();

            TotalPaginas = (int)Math.Ceiling(todas.Count / (double)TamanoPagina);

            if (TotalPaginas == 0)
                TotalPaginas = 1;

            if (pagina < 1)
                pagina = 1;

            if (pagina > TotalPaginas)
                pagina = TotalPaginas;

            PaginaActual = pagina;

            Acciones = todas
                .Skip((PaginaActual - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();

            Empleados = _accionRepository.ObtenerEmpleados();
        }

        private bool ValidarAccion(AccionPersonal accion)
        {
            if (accion.CodigoAccion <= 0)
            {
                MensajeError = "El código de acción es requerido.";
                return false;
            }

            if (accion.Fecha == default)
            {
                MensajeError = "La fecha es requerida.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(accion.Descripcion))
            {
                MensajeError = "La descripción es requerida.";
                return false;
            }

            if (accion.Descripcion.Length > 500)
            {
                MensajeError = "La descripción no puede superar los 500 caracteres.";
                return false;
            }

            if (accion.EmpleadoId <= 0)
            {
                MensajeError = "Debe seleccionar un empleado.";
                return false;
            }

            if (accion.JefaturaId <= 0)
            {
                MensajeError = "Debe seleccionar una jefatura.";
                return false;
            }

            return true;
        }

        private void RegistrarBitacora(AccionBitacora accion, object datos)
        {
            var usuario = ObtenerUsuarioActual();

            var bitacora = new Bitacora
            {
                Usuario = usuario,
                Accion = accion,
                DescripcionJson = JsonSerializer.Serialize(datos)
            };

            _bitacoraRepository.Registrar(bitacora);
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