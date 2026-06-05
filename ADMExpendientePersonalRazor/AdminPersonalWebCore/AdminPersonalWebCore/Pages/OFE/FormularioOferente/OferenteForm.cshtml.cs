using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AdminPersonalWebCore.Pages.OFE.FormularioOferente
{
    public class OferenteFormModel : PageModel
    {
        private readonly IOferenteService _oferenteService;
        //private readonly IAuthService _authService;

        public OferenteFormModel(IOferenteService oferenteService)//, IAuthService authService)
        {
            _oferenteService = oferenteService;
            //_authService = authService;
        }

        [BindProperty]
        public OferenteInputModel Oferente { get; set; } = new();

        public IEnumerable<ConcursoTemporal> Concursos { get; set; } = Enumerable.Empty<ConcursoTemporal>();

        public SelectList TiposIdentificacion { get; set; }

        public string Accion { get; set; } = "nuevo"; // "nuevo" o "editar", se asigna desde la URL



        // ── GET: cargar formulario nuevo o con datos para editar ───────────
        public async Task<IActionResult> OnGetAsync(string u, string accion, string? id = null)
        {
            Accion = accion; // Para usar en la vista y saber si es nuevo o editar
            var usuario = await ObtenerUsuarioAsync(u);
            Concursos = await _oferenteService.ObtenerConcursosAsync(usuario);

            TiposIdentificacion = new SelectList(new[]{
                new { Value = "Cedula",    Text = "Cédula de identidad" },
                new { Value = "Dimex",     Text = "DIMEX" },
                new { Value = "Pasaporte", Text = "Pasaporte" }
            }, "Value", "Text", Oferente.TipoIdentificacion);

            if (accion != "nuevo" && !string.IsNullOrEmpty(id))
            {
                var oferente = await _oferenteService.ObtenerOferenteAsync(usuario, id);
                if (oferente == null)
                {
                    TempData["Mensaje"] = "No se encontró el oferente.";
                    return RedirectToPage("/OFE/MainOferentes", new { u });
                }

                Oferente = new OferenteInputModel
                {
                    Identificacion = oferente.Identificacion,
                    TipoIdentificacion = oferente.TipoIdentificacion,
                    NombreCompleto = oferente.NombreCompleto,
                    FechaNacimiento = oferente.FechaNacimiento,
                    Email = oferente.Email,
                    Telefono = oferente.Telefono,
                    CodigoConcurso = oferente.CodigoConcurso
                };
            }
            else
            {
                // Valores por defecto para el formulario vacío
                Oferente = new OferenteInputModel
                {
                    FechaNacimiento = DateTime.Today,
                    Email = new List<string> { "" },
                    Telefono = new List<string> { "" }
                };
            }

            return Page();
        }

        // ── POST: guardar (nuevo o editar según Oferente.Accion) ───────────
        public async Task<IActionResult> OnPostAsync(string u, string accion)
        {
            Accion = accion;
            // Siempre recargar concursos primero, los necesitamos si hay cualquier error
            var usuario = await ObtenerUsuarioAsync(u);
            Concursos = await _oferenteService.ObtenerConcursosAsync(usuario);

            // Validaciones de listas (ModelState no las cubre)
            if (!Oferente.Email.Any(e => !string.IsNullOrWhiteSpace(e)))
                ModelState.AddModelError("", "Debe ingresar al menos un correo.");
            if (!Oferente.Telefono.Any(t => !string.IsNullOrWhiteSpace(t)))
                ModelState.AddModelError("", "Debe ingresar al menos un teléfono.");
            if (!Oferente.CodigoConcurso.Any())
                ModelState.AddModelError("", "Debe seleccionar al menos un concurso.");

            // Si ModelState tiene errores, devolver la misma página con los datos que el usuario ingresó
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var entidad = Oferente.ToEntity();
                int resultado;

                if (accion == "nuevo")
                {
                    resultado = await _oferenteService.InsertarOferenteAsync(entidad, usuario); /// <returns>0 = fallo, 1 = éxito, 2 = ya asignado, 3 = ya existe, 4 = datos inválidos</returns>
                    if (resultado != 1)
                    {
                        // Error del servicio y quedarse en la página con el mensaje
                        ModelState.AddModelError("", resultado switch
                        {
                            2 => "El oferente ya está asignado.",
                            4 => "Datos inválidos.",
                            3 => "El oferente ya existe.",
                            _ => "Error desconocido al registrar." + resultado
                        });
                        return Page(); // <-- se queda, NO redirige
                    }
                }
                else if (accion == "editar")
                {
                    resultado = await _oferenteService.ActualizarOferenteAsync(entidad, usuario);
                    if (resultado != 1)
                    {
                        ModelState.AddModelError("", resultado switch
                        {
                            4 => "Datos inválidos.",
                            _ => "Error desconocido al actualizar." + resultado
                        });
                        return Page(); // <-- se queda, NO redirige
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error desconocido");
                    return Page();
                }


                // Solo llega aquí si fue exitoso
                TempData["Mensaje"] = accion == "nuevo"
                    ? "Oferente registrado correctamente."
                    : "Oferente actualizado correctamente.";
                return RedirectToPage("/OFE/MainOferentes", new { u }); // <-- solo si éxito
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error inesperado: " + ex);
                return Page(); // <-- también se queda en excepción
            }
        }

        private async Task<string> ObtenerUsuarioAsync(string u)
        {
            /*
            var user = await _authService.ObtenerUsuarioPorNombreAsync(u);
            return user?.NombreCompleto ?? "Desconocido";*/
            return u;
        }
    }

    // ── ViewModel compartido entre ambas opciones ─────────────────────────
    public class OferenteInputModel
    {

        [Required(ErrorMessage = "La identificación es requerida.")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de identificación es requerido.")]
        public string TipoIdentificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es requerido.")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida.")]
        public DateTime FechaNacimiento { get; set; }

        public List<string> Email { get; set; } = new();
        public List<string> Telefono { get; set; } = new();
        public List<string> CodigoConcurso { get; set; } = new();

        public Oferente ToEntity() => new Oferente
        {
            Identificacion = Identificacion.Trim(),
            TipoIdentificacion = TipoIdentificacion,
            NombreCompleto = NombreCompleto.Trim(),
            FechaNacimiento = FechaNacimiento,
            Email = Email.Where(e => !string.IsNullOrWhiteSpace(e)).ToList(),
            Telefono = Telefono.Where(t => !string.IsNullOrWhiteSpace(t)).ToList(),
            CodigoConcurso = CodigoConcurso
        };
    }
}