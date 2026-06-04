using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System.Text.Json;

namespace AdminPersonalWebCore.Pages.EMP
{
    public class AccionesPersonalModel : PageModel
    {
        private readonly AccionPersonalRepository _accionRepository;
        private readonly BitacoraRepository _bitacoraRepository;

        public AccionesPersonalModel(
            AccionPersonalRepository accionRepository,
            BitacoraRepository bitacoraRepository)
        {
            _accionRepository = accionRepository;
            _bitacoraRepository = bitacoraRepository;
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
                PaginaActual = pagina;

                var todas = _accionRepository.ObtenerTodos();

                TotalPaginas = (int)Math.Ceiling(todas.Count / (double)TamanoPagina);

                Acciones = todas
                    .Skip((PaginaActual - 1) * TamanoPagina)
                    .Take(TamanoPagina)
                    .ToList();

                Empleados = _accionRepository.ObtenerEmpleados();

                // Aquí guardamos los READ en bitácora
                RegistrarBitacora(AccionBitacora.READ, new
                {
                    Mensaje = "El usuario consulta acciones de personal"
                });
            }
            catch
            {
                // Aquí guardamos los ERROR en bitácora si falla la consulta
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "Error al consultar acciones de personal"
                });

                MensajeError = "Ocurrió un error al consultar las acciones de personal.";
            }
        }

        public IActionResult OnPostCrear()
        {
            if (!ValidarAccion(NuevaAccion))
            {
                return RedirectToPage(new { u = Request.Query["u"].ToString() });
            }

            try
            {
                _accionRepository.Insertar(NuevaAccion);

                // Aquí guardamos los CREATE en bitácora
                RegistrarBitacora(AccionBitacora.CREATE, new
                {
                    Nuevo = NuevaAccion
                });

                MensajeExito = "La acción de personal se registró correctamente.";
            }
            catch
            {
                // Aquí guardamos los ERROR en bitácora si falla el registro
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "Error al registrar acción de personal",
                    Datos = NuevaAccion
                });

                MensajeError = "Ocurrió un error al registrar la acción de personal.";
            }

            return RedirectToPage(new { u = Request.Query["u"].ToString() });
        }

        public IActionResult OnPostEditar()
        {
            if (!ValidarAccion(AccionEditar))
            {
                return RedirectToPage(new { u = Request.Query["u"].ToString() });
            }

            try
            {
                // Aquí obtenemos el registro anterior para guardar el antes y después
                var accionAnterior = _accionRepository.ObtenerPorId(AccionEditar.AccionId);

                _accionRepository.Actualizar(AccionEditar);

                // Aquí guardamos los UPDATE en bitácora
                RegistrarBitacora(AccionBitacora.UPDATE, new
                {
                    Antes = accionAnterior,
                    Despues = AccionEditar
                });

                MensajeExito = "La acción de personal se actualizó correctamente.";
            }
            catch
            {
                // Aquí guardamos los ERROR en bitácora si falla la actualización
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "Error al actualizar acción de personal",
                    Datos = AccionEditar
                });

                MensajeError = "Ocurrió un error al actualizar la acción de personal.";
            }

            return RedirectToPage(new { u = Request.Query["u"].ToString() });
        }

        public IActionResult OnPostEliminar()
        {
            try
            {
                // Aquí obtenemos el registro antes de eliminarlo
                var accionEliminada = _accionRepository.ObtenerPorId(AccionEliminarId);

                _accionRepository.Eliminar(AccionEliminarId);

                // Aquí guardamos los DELETE en bitácora
                RegistrarBitacora(AccionBitacora.DELETE, new
                {
                    Eliminado = accionEliminada
                });

                MensajeExito = "La acción de personal se eliminó correctamente.";
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                // Aquí guardamos los ERROR en bitácora cuando MySQL no permite eliminar por datos relacionados
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "No se puede eliminar un registro con datos relacionados",
                    Id = AccionEliminarId
                });

                MensajeError = "No se puede eliminar un registro con datos relacionados.";
            }
            catch
            {
                // Aquí guardamos los ERROR en bitácora si falla la eliminación
                RegistrarBitacora(AccionBitacora.ERROR, new
                {
                    Mensaje = "Error al eliminar acción de personal",
                    Id = AccionEliminarId
                });

                MensajeError = "Ocurrió un error al eliminar la acción de personal.";
            }

            return RedirectToPage(new { u = Request.Query["u"].ToString() });
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
            string usuario = ObtenerUsuarioActual();

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
                       ?? User.Identity?.Name
                       ?? "UsuarioDesconocido";
            }

            return usuario;
        }
    }
}