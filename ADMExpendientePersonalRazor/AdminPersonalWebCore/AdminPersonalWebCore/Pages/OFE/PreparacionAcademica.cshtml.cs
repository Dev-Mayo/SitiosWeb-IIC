using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class PreparacionAcademicaModel : SecurePageModel
    {
        private readonly PrepAcademicaService _prepAcademicaService;
        private readonly InstEducativaService _instEducativa;
        private readonly AuthService _authService;
        private readonly ParametroService _parametroService;

        // ── Datos para la vista ──────────────────────────────────────────────
        public IEnumerable<PreparacionAcad> Preparaciones { get; private set; } = [];
        public List<SelectListItem> Instituciones { get; private set; } = [];
        public int PaginaActual { get; private set; } = 1;
        public int TotalPaginas { get; private set; } = 1;

        // ── Estado modales ───────────────────────────────────────────────────
        public string MensajeSistema { get; private set; } = "";
        public bool MostrarModalForm { get; private set; }
        public string MensajeError { get; private set; } = "";

        // Valores para repintar el modal tras error
        public int FormId { get; private set; }
        public string FormCodigoInstitucion { get; private set; } = "";
        public string FormTitulo { get; private set; } = "";
        public string FormFechaInicio { get; private set; } = "";
        public string FormFechaFin { get; private set; } = "";

        public PreparacionAcademicaModel(
            PrepAcademicaService prepAcademicaService,
            InstEducativaService instEducativa,
            AuthService authService, ParametroService parametroService)
        {
            _prepAcademicaService = prepAcademicaService;
            _instEducativa = instEducativa;
            _authService = authService;
            _parametroService = parametroService;
        }

        // ── Helper: validar sesión y contexto ────────────────────────────────
        private IActionResult? ValidarSession(string? id = null)
        {
            var check = CheckSession();
            if (check != null) return check;

            if (UsuarioActual == null)
                return RedirectToPage("/SEG/Login");

            if (!string.IsNullOrEmpty(id))
            {
                IdGeneral = id;
                return RedirectToPage(new { u = UsuarioActual });
            }

            if (!TieneIdGeneral)
                return RedirectToPage("/OFE/MainOferentes");

            var usuario = _authService.ObtenerPorNombre(UsuarioActual);
            if (usuario == null)
                return Redirect("/SEG/Login?msg=login");

            return null;
        }

        // ── GET ──────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnGetAsync(string? id = null, int pagina = 1)
        {
            var check = ValidarSession(id);
            if (check != null) return check;

            // Guardar id del oferente en session si viene en la URL
            if (!string.IsNullOrEmpty(id))
            {
                IdGeneral = id;
                return RedirectToPage(new { u = UsuarioActual });
            }

            if (!TieneIdGeneral)
                return RedirectToPage("/OFE/MainOferentes");

            try
            {
                await CargarDatosAsync(pagina);

                if (TempData.ContainsKey("MensajeSistema"))
                    MensajeSistema = TempData["MensajeSistema"]?.ToString() ?? "";

                return Page();
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(ex);
                MensajeSistema = "Error inesperado: " + ex.Message;
                return Page();
            }
        }

        // ── POST: Guardar (insertar o modificar) ─────────────────────────────
        public async Task<IActionResult> OnPostGuardarAsync(
            int formId,
            string? formCodigoInstitucion,
            string? formTitulo,
            string formFechaInicio,
            string formFechaFin)
        {
            var ctx = ValidarSession();
            if (ctx != null) return ctx;

            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(formCodigoInstitucion))
                    return await PaginaConError(formId, formCodigoInstitucion ?? "", formTitulo ?? "",
                        formFechaInicio, formFechaFin, "La institución es requerida.");

                if (string.IsNullOrWhiteSpace(formTitulo))
                    return await PaginaConError(formId, formCodigoInstitucion, formTitulo ?? "",
                        formFechaInicio, formFechaFin, "El título es requerido.");

                if (formTitulo.Length > 100)
                    return await PaginaConError(formId, formCodigoInstitucion, formTitulo,
                        formFechaInicio, formFechaFin, "El título no puede superar los 100 caracteres.");

                if (!System.Text.RegularExpressions.Regex.IsMatch(formTitulo, @"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$"))
                    return await PaginaConError(formId, formCodigoInstitucion, formTitulo,
                        formFechaInicio, formFechaFin, "El título solo permite letras y espacios.");

                if (!DateTime.TryParse(formFechaInicio, out DateTime fechaInicio) ||
                    !DateTime.TryParse(formFechaFin, out DateTime fechaFin))
                    return await PaginaConError(formId, formCodigoInstitucion, formTitulo,
                        formFechaInicio, formFechaFin, "Las fechas ingresadas no son válidas.");

                if (fechaInicio >= fechaFin)
                    return await PaginaConError(formId, formCodigoInstitucion, formTitulo,
                        formFechaInicio, formFechaFin, "La fecha de inicio debe ser menor a la fecha de fin.");

                bool esNuevo = formId == 0;
                var prep = new PreparacionAcad
                {
                    Id = formId,
                    CodigoInstitucion = formCodigoInstitucion,
                    OferenteId = IdGeneral!,
                    Titulo = formTitulo,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                int resultado = esNuevo
                    ? await _prepAcademicaService.CrearPreparacionAcadAsync(prep, NombreCompleto)
                    : await _prepAcademicaService.ModificarPreparacionAcadAsync(prep, NombreCompleto);

                if (resultado == 1)
                {
                    TempData["MensajeSistema"] = esNuevo
                        ? "Preparación académica registrada correctamente."
                        : "Preparación académica modificada correctamente.";
                    return RedirectToPage(new { u = UsuarioActual });
                }

                string errorMsg = resultado switch
                {
                    2 => "Verifica todos los espacios y datos ingresados.",
                    3 => "Error en las fechas: la fecha inicial debe ser menor a la final.",
                    4 => "El título no puede superar los 100 caracteres y solo permite letras y espacios.",
                    _ => "Error desconocido al guardar la preparación académica."
                };

                return await PaginaConError(formId, formCodigoInstitucion, formTitulo,
                    formFechaInicio, formFechaFin, errorMsg);
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(ex);
                TempData["MensajeSistema"] = "Error inesperado: " + ex.Message;
                return RedirectToPage(new { u = UsuarioActual });
            }
        }

        // ── POST: Eliminar ───────────────────────────────────────────────────
        public async Task<IActionResult> OnPostEliminarAsync(int preparacionAcadId)
        {
            var check = ValidarSession();
            if (check != null) return check;

            try
            {
                int resultado = await _prepAcademicaService.EliminarPreparacionAcadAsync(
                    preparacionAcadId, NombreCompleto);

                TempData["MensajeSistema"] = resultado == 1
                    ? "Eliminado correctamente."
                    : "No se ha podido eliminar.";

                return RedirectToPage(new { u = UsuarioActual });
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(ex);
                TempData["MensajeSistema"] = "Error inesperado al eliminar.";
                return RedirectToPage(new { u = UsuarioActual });
            }
        }

        // ── Helpers privados ─────────────────────────────────────────────────
        private async Task CargarDatosAsync(int pagina)
        {
            int cantidad = _parametroService.ObtenerValorEnteroODefecto("CANTIDAD_REGISTROS_PAGINA", 10);
            // Tabla
            var todas = (await _prepAcademicaService.ObtenerPreparacionAcadAsync(
                IdGeneral!, NombreCompleto)).ToList();
            TotalPaginas = (int)Math.Ceiling(todas.Count / (double)cantidad);
            PaginaActual = Math.Max(1, Math.Min(pagina, Math.Max(TotalPaginas, 1)));
            Preparaciones = todas.Skip((PaginaActual - 1) * cantidad).Take(cantidad);

            // Instituciones para el select del modal
            var insts = await Task.Run(() => _instEducativa.ObtenerTodas(NombreCompleto));
            Instituciones = insts.Select(i => new SelectListItem
            {
                Value = i.codigo_institucion,
                Text = i.nombre
            }).ToList();
        }

        private async Task<IActionResult> PaginaConError(
            int id, string institucion, string titulo,
            string fechaInicio, string fechaFin, string error)
        {
            await CargarDatosAsync(PaginaActual);
            MostrarModalForm = true;
            FormId = id;
            FormCodigoInstitucion = institucion;
            FormTitulo = titulo;
            FormFechaInicio = fechaInicio;
            FormFechaFin = fechaFin;
            MensajeError = error;
            return Page();
        }

        private async Task RegistrarErrorAsync(Exception ex)
        {
            try
            {
                await Task.Run(() => _prepAcademicaService.GenericoCrearBitacora(
                    NombreCompleto, 4, 1, detalles: "Error: " + ex));
            }
            catch { /* no propagar errores de bitácora */ }
        }
    }
}