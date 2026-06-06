using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class MainOferentesModel : SecurePageModel
    {
        private readonly IOferenteService _oferenteService;
        private readonly AuthService _authService;
        private const int TamPagina = 10;

        public MainOferentesModel(IOferenteService oferenteService, AuthService authService)
        {
            _oferenteService = oferenteService;
            _authService = authService;
        }

        // ── Datos para la vista ──────────────────────────────────────────────
        public IEnumerable<Oferente> OferentesPaginados { get; private set; } = [];
        public IEnumerable<ConcursoTemporal> Concursos { get; private set; } = [];
        public string OferentesJson { get; private set; } = "[]";
        public int PaginaActual { get; private set; } = 1;
        public int TotalPaginas { get; private set; } = 1;

        // ── Estado modales ───────────────────────────────────────────────────
        public string MensajeSistema { get; private set; } = "";
        public bool MostrarModalForm { get; private set; }
        public string MensajeError { get; private set; } = "";

        // Valores para repintar modal tras error
        public string FormIdentificacion { get; private set; } = "";
        public string FormTipoIdentificacion { get; private set; } = "Cedula";
        public string FormNombreCompleto { get; private set; } = "";
        public string FormFechaNacimiento { get; private set; } = "";
        public List<string> FormEmails { get; private set; } = [""];
        public List<string> FormTelefonos { get; private set; } = [""];
        public List<string> FormConcursos { get; private set; } = [];
        public string FormAccion { get; private set; } = "nuevo";

        private IActionResult? ValidarSession()
        {
            var check = CheckSession();
            if (check != null) return check;

            var usuario = _authService.ObtenerPorNombre(UsuarioActual);
            if (usuario == null)
                return Redirect("/SEG/Login?msg=login");
            return null;
        }

        // ── GET ──────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnGetAsync(int pagina = 1)
        {
            var check = ValidarSession();
            if (check != null) return check;

            try
            {
                await CargarDatosAsync(pagina);

                if (TempData.ContainsKey("MensajeSistema"))
                    MensajeSistema = TempData["MensajeSistema"]?.ToString() ?? "";

                return Page();
            }
            catch (Exception ex)
            {
                MensajeSistema = "Error inesperado: " + ex.Message;
                return Page();
            }
        }

        // ── POST: Guardar (insertar o modificar) ─────────────────────────────
        public async Task<IActionResult> OnPostGuardarAsync(
            string accion,
            string? identificacion,
            string? tipoIdentificacion,
            string? nombreCompleto,
            string? fechaNacimiento,
            List<string>? emails,
            List<string>? telefonos,
            List<string>? concursos)
        {
            var check = ValidarSession();
            if (check != null) return check;

            // Normalizar nulls
            emails ??= [];
            telefonos ??= [];
            concursos ??= [];
            emails = emails.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
            telefonos = telefonos.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(identificacion))
                    return await PaginaConError(accion, identificacion ?? "", tipoIdentificacion ?? "Cedula",
                        nombreCompleto ?? "", fechaNacimiento ?? "", emails, telefonos, concursos,
                        "La identificación es requerida.");

                if (string.IsNullOrWhiteSpace(tipoIdentificacion))
                    return await PaginaConError(accion, identificacion, tipoIdentificacion ?? "Cedula",
                        nombreCompleto ?? "", fechaNacimiento ?? "", emails, telefonos, concursos,
                        "El tipo de identificación es requerido.");

                if (string.IsNullOrWhiteSpace(nombreCompleto))
                    return await PaginaConError(accion, identificacion, tipoIdentificacion,
                        nombreCompleto ?? "", fechaNacimiento ?? "", emails, telefonos, concursos,
                        "El nombre completo es requerido.");

                if (!DateTime.TryParse(fechaNacimiento, out DateTime fechaNac))
                    return await PaginaConError(accion, identificacion, tipoIdentificacion,
                        nombreCompleto, fechaNacimiento ?? "", emails, telefonos, concursos,
                        "La fecha de nacimiento no es válida.");

                if (!emails.Any())
                    return await PaginaConError(accion, identificacion, tipoIdentificacion,
                        nombreCompleto, fechaNacimiento ?? "", emails, telefonos, concursos,
                        "Debe ingresar al menos un correo electrónico.");

                if (!telefonos.Any())
                    return await PaginaConError(accion, identificacion, tipoIdentificacion,
                        nombreCompleto, fechaNacimiento ?? "", emails, telefonos, concursos,
                        "Debe ingresar al menos un teléfono.");

                var entidad = new Oferente
                {
                    Identificacion = identificacion.Trim(),
                    TipoIdentificacion = tipoIdentificacion,
                    NombreCompleto = nombreCompleto.Trim(),
                    FechaNacimiento = fechaNac,
                    Email = emails,
                    Telefono = telefonos,
                    CodigoConcurso = concursos
                };

                int resultado;
                if (accion == "nuevo")
                {
                    resultado = await _oferenteService.InsertarOferenteAsync(entidad, NombreCompleto);
                    if (resultado != 1)
                    {
                        string msg = resultado switch
                        {
                            2 => "El oferente ya está asignado.",
                            3 => "El oferente ya existe.",
                            4 => "Datos inválidos.",
                            _ => "Error desconocido al registrar."
                        };
                        return await PaginaConError(accion, identificacion, tipoIdentificacion,
                            nombreCompleto, fechaNacimiento!, emails, telefonos, concursos, msg);
                    }
                }
                else
                {
                    resultado = await _oferenteService.ActualizarOferenteAsync(entidad, NombreCompleto);
                    if (resultado != 1)
                    {
                        string msg = resultado switch
                        {
                            4 => "Datos inválidos.",
                            _ => "Error desconocido al actualizar."
                        };
                        return await PaginaConError(accion, identificacion, tipoIdentificacion,
                            nombreCompleto, fechaNacimiento!, emails, telefonos, concursos, msg);
                    }
                }

                TempData["MensajeSistema"] = accion == "nuevo"
                    ? "Oferente registrado correctamente."
                    : "Oferente actualizado correctamente.";

                return RedirectToPage(new { u = UsuarioActual });
            }
            catch (Exception ex)
            {
                TempData["MensajeSistema"] = "Error inesperado: " + ex.Message;
                return RedirectToPage(new { u = UsuarioActual });
            }
        }

        // ── POST: Eliminar ───────────────────────────────────────────────────
        public async Task<IActionResult> OnPostEliminarAsync(string identificacion)
        {
            var check = ValidarSession();
            if (check != null) return check;

            try
            {
                var oferente = await _oferenteService.ObtenerOferenteAsync(NombreCompleto, identificacion);
                if (oferente == null)
                {
                    TempData["MensajeSistema"] = "No se encontró el oferente.";
                    return RedirectToPage(new { u = UsuarioActual });
                }

                int resultado = await _oferenteService.EliminarOferenteAsync(oferente, NombreCompleto);
                TempData["MensajeSistema"] = resultado switch
                {
                    1 => "Oferente eliminado correctamente.",
                    2 => "No se puede eliminar un registro con datos relacionados.",
                    _ => "No se ha podido eliminar el oferente."
                };
            }
            catch (Exception ex)
            {
                TempData["MensajeSistema"] = "Error inesperado: " + ex.Message;
            }

            return RedirectToPage(new { u = UsuarioActual });
        }

        // ── Helpers privados ─────────────────────────────────────────────────
        private async Task CargarDatosAsync(int pagina)
        {
            var todos = (await _oferenteService.ObtenerOferentesAsync(NombreCompleto)).ToList();
            TotalPaginas = (int)Math.Ceiling(todos.Count / (double)TamPagina);
            PaginaActual = Math.Clamp(pagina, 1, Math.Max(1, TotalPaginas));
            OferentesPaginados = todos.Skip((PaginaActual - 1) * TamPagina).Take(TamPagina);

            Concursos = await _oferenteService.ObtenerConcursosAsync(NombreCompleto);

            // Serializar para JS (edición inline sin fetch)
            OferentesJson = JsonSerializer.Serialize(todos.Select(o => new
            {
                identificacion = o.Identificacion,
                tipoIdentificacion = o.TipoIdentificacion,
                nombreCompleto = o.NombreCompleto,
                fechaNacimiento = o.FechaNacimiento.ToString("yyyy-MM-dd"),
                emails = o.Email,
                telefonos = o.Telefono,
                concursos = o.CodigoConcurso
            }), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        private async Task<IActionResult> PaginaConError(
            string accion, string identificacion, string tipoIdentificacion,
            string nombreCompleto, string fechaNacimiento,
            List<string> emails, List<string> telefonos, List<string> concursos,
            string error)
        {
            await CargarDatosAsync(PaginaActual);
            MostrarModalForm = true;
            MensajeError = error;
            FormAccion = accion;
            FormIdentificacion = identificacion;
            FormTipoIdentificacion = tipoIdentificacion;
            FormNombreCompleto = nombreCompleto;
            FormFechaNacimiento = fechaNacimiento;
            FormEmails = emails.Any() ? emails : [""];
            FormTelefonos = telefonos.Any() ? telefonos : [""];
            FormConcursos = concursos;
            return Page();
        }
    }
}