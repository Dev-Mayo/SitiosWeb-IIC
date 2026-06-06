using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class ExpLaboralModel : SecurePageModel
    {
        private readonly IExpLaboralService _expLaboralService;
        private readonly AuthService _authService;

        private const int PageSize = 10;

        // ── Datos para la vista ──────────────────────────────────────────────
        public IEnumerable<ExpLaboral> ExpLaboralesPaginadas { get; private set; } = [];
        public string ExpLaboralesJson { get; private set; } = "[]";
        public IEnumerable<Compania> Companias { get; private set; } = [];

        public int PaginaActual { get; private set; } = 1;
        public int TotalPaginas { get; private set; } = 1;

        // ── Mensajes y estado del modal ──────────────────────────────────────
        public string? MensajeSistema { get; private set; }
        public string? MensajeError { get; private set; }
        public bool MostrarModalExpLaboral { get; private set; }
        public string AccionActual { get; private set; } = "0";
        public string IdActual { get; private set; } = "0";
        public string EmpresaActual { get; private set; } = "";
        public string PuestoActual { get; private set; } = "";
        public string FechaInicioActual { get; private set; } = "";
        public string FechaFinActual { get; private set; } = "";

        public ExpLaboralModel(IExpLaboralService expLaboralService, AuthService authService)
        {
            _expLaboralService = expLaboralService;
            _authService = authService;
        }

        private IActionResult? ValidarSession(string? id = null) //La primera vez en el get de la pagina se debe pasar id para guardar en la session y no redirecionar
        {
            var check = CheckSession();
            if (check != null) return check;

            if (UsuarioActual == null)
                return RedirectToPage("/SEG/Login"); //redireccion si noy hay login

            if (!string.IsNullOrEmpty(id))
            {
                IdGeneral = id;
                return RedirectToPage(new {u = UsuarioActual});
            }

            if (!TieneIdGeneral)
                return RedirectToPage("/OFE/MainOferentes"); //redireccion si no hay id

            var usuario = _authService.ObtenerPorNombre(UsuarioActual);
            if (usuario == null)
                return Redirect("/SEG/Login?msg=login");

            return null;
        }
        public async Task<IActionResult> OnGetAsync(string? id = null, int pagina = 1)
        {
            try
            {
                var check = ValidarSession(id);
                if (check != null) return check;

                await CargarExpLaboralesAsync(pagina);
                Companias = _expLaboralService.ObtenerCompanias(UsuarioActual);

                if (TempData.ContainsKey("MensajeSistema"))
                    MensajeSistema = TempData["MensajeSistema"]?.ToString();

                if (TempData.ContainsKey("MensajeError"))
                {
                    MensajeError = TempData["MensajeError"]?.ToString();
                    MostrarModalExpLaboral = true;
                    AccionActual = TempData["Accion"]?.ToString() ?? "0";
                    IdActual = TempData["Id"]?.ToString() ?? "0";
                    EmpresaActual = TempData["Empresa"]?.ToString() ?? "";
                    PuestoActual = TempData["Puesto"]?.ToString() ?? "";
                    FechaInicioActual = TempData["FechaInicio"]?.ToString() ?? "";
                    FechaFinActual = TempData["FechaFin"]?.ToString() ?? "";
                }

                return Page();
            }
            catch (Exception ex)
            {
                _expLaboralService.GenericoCrearBitacora(UsuarioActual, 4, 1, detalles: ex.Message);
                MensajeSistema = $"Error inesperado: {ex.Message}";
                return Page();
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  POST: Guardar (insertar o modificar)
        // ────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnPostGuardarAsync(
            int Accion,
            int Id,
            string Empresa,
            string Puesto,
            string FechaInicio,
            string FechaFin)
        {
            try
            {
                var check = ValidarSession();
                if (check != null) return check;

                // Validación
                if (string.IsNullOrWhiteSpace(Empresa) ||
                    string.IsNullOrWhiteSpace(Puesto) ||
                    string.IsNullOrWhiteSpace(FechaInicio) ||
                    string.IsNullOrWhiteSpace(FechaFin))
                {
                    GuardarTempDataError("Todos los campos son requeridos.",
                        Accion, Id, Empresa, Puesto, FechaInicio, FechaFin);
                    Companias = _expLaboralService.ObtenerCompanias(UsuarioActual);
                    return RedirectToPage(new { u = UsuarioActual });
                }

                if (!DateTime.TryParse(FechaInicio, out DateTime fechaInicio) ||
                    !DateTime.TryParse(FechaFin, out DateTime fechaFin))
                {
                    GuardarTempDataError("Las fechas ingresadas no son válidas.",
                        Accion, Id, Empresa, Puesto, FechaInicio, FechaFin);
                    Companias = _expLaboralService.ObtenerCompanias(UsuarioActual);
                    return RedirectToPage(new { u = UsuarioActual });
                }

                if (fechaFin < fechaInicio)
                {
                    GuardarTempDataError("La fecha de fin no puede ser anterior a la fecha de inicio.",
                        Accion, Id, Empresa, Puesto, FechaInicio, FechaFin);
                    Companias = _expLaboralService.ObtenerCompanias(UsuarioActual);
                    return RedirectToPage(new { u = UsuarioActual });
                }

                var exp = new ExpLaboral
                {
                    Id = Accion == 0 ? 0 : Id,
                    OferenteId = IdGeneral,
                    Empresa = Empresa,
                    Puesto = Puesto,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                int resultado = Accion == 0
                    ? await _expLaboralService.CrearExpLaboralAsync(exp, NombreCompleto)
                    : await _expLaboralService.ModificarExpLaboralAsync(exp, NombreCompleto);

                string? errorMsg = resultado switch
                {
                    1 => null,
                    2 => "Verifica todos los datos ingresados.",
                    _ => Accion == 0
                            ? "Error desconocido al registrar la experiencia laboral."
                            : "Error desconocido al modificar la experiencia laboral."
                };

                if (errorMsg is not null)
                    GuardarTempDataError(errorMsg, Accion, Id, Empresa, Puesto, FechaInicio, FechaFin);
                else
                    TempData["MensajeSistema"] = Accion == 0
                        ? "Experiencia laboral registrada correctamente."
                        : "Experiencia laboral modificada correctamente.";

                return RedirectToPage(new { u = UsuarioActual });
            }
            catch (Exception ex)
            {
                _expLaboralService.GenericoCrearBitacora(UsuarioActual, 4, 1, detalles: ex.Message);
                TempData["MensajeSistema"] = $"Error inesperado: {ex.Message}";
                return RedirectToPage(new { u = UsuarioActual });
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  POST: Eliminar
        // ────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            try
            {
                var check = ValidarSession();
                if (check != null) return check;

                if (id == 0)
                {
                    TempData["MensajeSistema"] = "No se reconoció el ID de la experiencia laboral.";
                    return RedirectToPage(new { u = UsuarioActual });
                }

                int resultado = await _expLaboralService.EliminarExpLaboralAsync(id, NombreCompleto);

                TempData["MensajeSistema"] = resultado == 1
                    ? "Experiencia laboral eliminada correctamente."
                    : "No se pudo eliminar la experiencia laboral.";

                return RedirectToPage(new { u = UsuarioActual });
            }
            catch (Exception ex)
            {
                _expLaboralService.GenericoCrearBitacora(UsuarioActual, 4, 1, detalles: ex.Message);
                TempData["MensajeSistema"] = $"Error inesperado: {ex.Message}";
                return RedirectToPage(new { u = UsuarioActual });
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Helpers privados
        // ────────────────────────────────────────────────────────────────────
        private async Task CargarExpLaboralesAsync(int pagina)
        {
            var todas = (await _expLaboralService.ObtenerExpLaboralAsync(IdGeneral!, NombreCompleto)).ToList();

            TotalPaginas = (int)Math.Ceiling(todas.Count / (double)PageSize);
            PaginaActual = Math.Max(1, Math.Min(pagina, Math.Max(TotalPaginas, 1)));

            ExpLaboralesPaginadas = todas
                .Skip((PaginaActual - 1) * PageSize)
                .Take(PageSize);

            ExpLaboralesJson = JsonSerializer.Serialize(todas.Select(e => new
            {
                id = e.Id,
                empresa = e.Empresa,
                puesto = e.Puesto,
                fechaInicio = e.FechaInicio.ToString("yyyy-MM-dd"),
                fechaFin = e.FechaFin.ToString("yyyy-MM-dd")
            }), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        private void GuardarTempDataError(string error, int accion, int id,
            string empresa, string puesto, string fechaInicio, string fechaFin)
        {
            TempData["MensajeError"] = error;
            TempData["Accion"] = accion.ToString();
            TempData["Id"] = id.ToString();
            TempData["Empresa"] = empresa;
            TempData["Puesto"] = puesto;
            TempData["FechaInicio"] = fechaInicio;
            TempData["FechaFin"] = fechaFin;
        }
    }
}