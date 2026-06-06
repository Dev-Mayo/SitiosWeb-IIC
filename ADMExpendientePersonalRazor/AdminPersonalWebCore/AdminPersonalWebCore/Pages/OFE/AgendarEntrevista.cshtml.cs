using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class AgendarEntrevistaModel : PageModel
    {
        private readonly IEntrevistaService _entrevistaService;
        private readonly IOferenteService _oferenteService;
        //private readonly IAuthService _authService;

        private const int PageSize = 10;

        // ── Datos para la vista ──────────────────────────────────────────────
        public IEnumerable<Entrevista> EntrevistasPaginadas { get; private set; } = [];
        public IEnumerable<Empleado> Empleados { get; private set; } = [];
        public IEnumerable<Oferente> Oferentes { get; private set; } = [];
        public string EntrevistasJson { get; private set; } = "[]";

        public int PaginaActual { get; private set; } = 1;
        public int TotalPaginas { get; private set; } = 1;

        // ── Mensajes y estado del modal ──────────────────────────────────────
        public string? MensajeSistema { get; private set; }
        public string? MensajeError { get; private set; }
        public bool MostrarModalEntrevista { get; private set; }
        public string AccionActual { get; private set; } = "0";
        public string EntrevistaIdActual { get; private set; } = "0";
        public string OferenteActual { get; private set; } = "";
        public string EmpleadoIdActual { get; private set; } = "";
        public string FechaActual { get; private set; } = "";

        public AgendarEntrevistaModel( IEntrevistaService entrevistaService, IOferenteService oferenteService) //IAuthService authService)
        {
            _entrevistaService = entrevistaService;
            _oferenteService = oferenteService;
           // _authService = authService;
        }

        // ────────────────────────────────────────────────────────────────────
        //  GET
        // ────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnGetAsync(int pagina = 1)
        {
            try
            {
                await CargarDatosBaseAsync();
                await CargarEntrevistasAsync(pagina);

                // Recuperar mensajes de TempData (tras redirect)
                if (TempData.ContainsKey("MensajeSistema"))
                    MensajeSistema = TempData["MensajeSistema"]?.ToString();

                if (TempData.ContainsKey("MensajeError"))
                {
                    MensajeError = TempData["MensajeError"]?.ToString();
                    MostrarModalEntrevista = true;
                    AccionActual = TempData["Accion"]?.ToString() ?? "0";
                    EntrevistaIdActual = TempData["EntrevistaId"]?.ToString() ?? "0";
                    OferenteActual = TempData["Oferente"]?.ToString() ?? "";
                    EmpleadoIdActual = TempData["EmpleadoId"]?.ToString() ?? "";
                    FechaActual = TempData["Fecha"]?.ToString() ?? "";
                }

                return Page();
            }
            catch (Exception ex)
            {
                await ReportarFallosAsync(ex);
                return Page();
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  POST: Guardar (insertar o modificar)
        // ────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnPostGuardarAsync(
            int Accion,
            int EntrevistaId,
            string OferenteIdentificacion,
            int EmpleadoId,
            string FechaEntrevista)
        {
            try
            {
                var usuario = await ObtenerUsuarioAsync();

                // Validación básica
                if (string.IsNullOrEmpty(OferenteIdentificacion) ||
                    EmpleadoId == 0 ||
                    string.IsNullOrEmpty(FechaEntrevista) ||
                    !DateTime.TryParse(FechaEntrevista, out DateTime fecha))
                {
                    GuardarTempDataError(
                        Accion == 0 ? "Todos los campos son requeridos." : "Todos los campos son requeridos.",
                        Accion, EntrevistaId, OferenteIdentificacion, EmpleadoId, FechaEntrevista);
                    return RedirectToPage(new { u = Request.Query["u"] });
                }

                var entrevista = new Entrevista
                {
                    EntrevistaId = Accion == 0 ? 0 : EntrevistaId,
                    EmpleadoId = EmpleadoId,
                    OferenteIdentificacion = OferenteIdentificacion,
                    FechaEntrevista = fecha
                };

                int resultado;
                if (Accion == 0)
                    resultado = await _entrevistaService.InsertarEntrevistaAsync(entrevista, usuario);
                else
                    resultado = await _entrevistaService.ModificarEntrevistaAsync(entrevista, usuario);

                string? errorMsg = resultado switch
                {
                    2 => "Verifica todos los espacios y datos ingresados.",
                    3 => "La fecha de la entrevista no puede ser anterior a la fecha actual.",
                    1 => null,
                    _ => Accion == 0
                            ? "Error desconocido al registrar la entrevista."
                            : "Error desconocido al modificar la entrevista."
                };

                if (errorMsg is not null)
                {
                    GuardarTempDataError(errorMsg, Accion, EntrevistaId,
                        OferenteIdentificacion, EmpleadoId, FechaEntrevista);
                }
                else
                {
                    TempData["MensajeSistema"] = Accion == 0
                        ? "La entrevista ha sido registrada correctamente."
                        : "La entrevista ha sido modificada correctamente.";
                }

                return RedirectToPage(new { u = Request.Query["u"] });
            }
            catch (Exception ex)
            {
                await ReportarFallosAsync(ex);
                return RedirectToPage(new { u = Request.Query["u"] });
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  POST: Eliminar
        // ────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnPostEliminarAsync(int entrevistaId)
        {
            try
            {
                var usuario = await ObtenerUsuarioAsync();

                if (entrevistaId == 0)
                {
                    TempData["MensajeSistema"] = "No se reconoció el id de la entrevista.";
                    return RedirectToPage(new { u = Request.Query["u"] });
                }

                int resultado = await _entrevistaService.EliminarEntrevistaAsync(entrevistaId, usuario);
                TempData["MensajeSistema"] = resultado == 1
                    ? "Eliminado correctamente."
                    : "No se ha podido eliminar la entrevista.";

                return RedirectToPage(new { u = Request.Query["u"] });
            }
            catch (Exception ex)
            {
                await ReportarFallosAsync(ex);
                return RedirectToPage(new { u = Request.Query["u"] });
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  POST: Cambiar Estado
        // ────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnPostCambiarEstadoAsync(int entrevistaId)
        {
            try
            {
                var usuario = await ObtenerUsuarioAsync();
                int resultado = await _entrevistaService.CambiarEstadoEntrevistaAsync(entrevistaId, usuario);

                TempData["MensajeSistema"] = resultado == 1
                    ? "Estado actualizado correctamente."
                    : "Estado ya cambiado o falló al cambiarlo.";

                return RedirectToPage(new { u = Request.Query["u"] });
            }
            catch (Exception ex)
            {
                await ReportarFallosAsync(ex);
                return RedirectToPage(new { u = Request.Query["u"] });
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Helpers
        // ────────────────────────────────────────────────────────────────────
        private async Task CargarDatosBaseAsync()
        {
            Empleados = await _entrevistaService.ObtenerNombreEmpleadosAsync();
            Oferentes = await _oferenteService.ObtenerNombreOferentesAsync();
        }

        private async Task CargarEntrevistasAsync(int pagina)
        {
            var usuario = await ObtenerUsuarioAsync();
            var todas = (await _entrevistaService.ObtenerEntrevistasAsync(usuario)).ToList();

            TotalPaginas = (int)Math.Ceiling(todas.Count / (double)PageSize);
            PaginaActual = Math.Max(1, Math.Min(pagina, TotalPaginas));

            EntrevistasPaginadas = todas
                .Skip((PaginaActual - 1) * PageSize)
                .Take(PageSize);

            // Serializar para uso en JS (edición)
            EntrevistasJson = JsonSerializer.Serialize(todas.Select(e => new
            {
                entrevistaId = e.EntrevistaId,
                oferenteIdentificacion = e.OferenteIdentificacion,
                empleadoId = e.EmpleadoId,
                fechaEntrevista = e.FechaEntrevista.ToString("yyyy-MM-dd"),
                estado = e.Estado
            }), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        private async Task<string> ObtenerUsuarioAsync()
        {
            var u = Request.Query["u"].ToString();
            //var usuario = await _authService.ObtenerUsuarioPorNombreAsync(u);
            return u ?? "Desconocido";
        }

        private async Task ReportarFallosAsync(Exception ex)
        {
            try
            {
                var usuario = await ObtenerUsuarioAsync();
                _entrevistaService.GenericoCrearBitacora(usuario, 4, 1, $"Error: {ex}");
                MensajeSistema = $"Error inesperado: {ex.Message}";
            }
            catch
            {
                MensajeSistema = $"Error no controlado: {ex.Message}";
            }
        }

        private void GuardarTempDataError(string error, int accion, int entrevistaId,
            string oferente, int empleadoId, string fecha)
        {
            TempData["MensajeError"] = error;
            TempData["Accion"] = accion.ToString();
            TempData["EntrevistaId"] = entrevistaId.ToString();
            TempData["Oferente"] = oferente;
            TempData["EmpleadoId"] = empleadoId.ToString();
            TempData["Fecha"] = fecha;
        }
    }
}