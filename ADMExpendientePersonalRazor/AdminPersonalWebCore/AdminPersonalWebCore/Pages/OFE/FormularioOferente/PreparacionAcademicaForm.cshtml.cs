using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace AdminPersonalWebCore.Pages.OFE.FormularioOferente
{
    public class PreparacionAcademicaFormModel : PageModel
    {
        private readonly IPrepAcademicaService _iprepAcademicaService;
        //private readonly AuthBLL _authBLL;
        private readonly InstEducativaService _instEducativa;

        public bool EsNuevo => Input.Id == 0;
        public string ErrorMessage { get; set; } = string.Empty;
        public List<SelectListItem> Instituciones { get; set; } = [];

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "La institución es requerida.")]
            public string CodigoInstitucion { get; set; } = string.Empty;

            public string OferenteId { get; set; } = string.Empty;

            [Required(ErrorMessage = "El título es requerido.")]
            [MaxLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El título solo permite letras y espacios.")]
            public string Titulo { get; set; } = string.Empty;

            [Required(ErrorMessage = "La fecha de inicio es requerida.")]
            public DateTime? FechaInicio { get; set; }

            [Required(ErrorMessage = "La fecha de fin es requerida.")]
            public DateTime? FechaFin { get; set; }
        }

        public PreparacionAcademicaFormModel(IPrepAcademicaService iprepAcademicaService, InstEducativaService instEducativa)//, AuthBLL authBLL)
        {
            _iprepAcademicaService = iprepAcademicaService;
            _instEducativa = instEducativa;
            //_authBLL = authBLL;
        }

        // GET: cargar instituciones y, si es edición, los datos del registro
        public async Task<IActionResult> OnGetAsync(int? id, string oferenteId, string u)
        {
            try
            {
                string usuario = await ObtenerUsuarioAsync(u);
                await CargarInstitucionesAsync(usuario);

                if (id.HasValue && id.Value > 0)
                {
                    var prep = await _iprepAcademicaService.ObtenerPreparacionAcadPorIdAsync(id.Value, usuario);
                    if (prep == null)
                    {
                        TempData["Mensaje"] = "No se encontró la preparación académica seleccionada.";
                        return RedirectToPage("/OFE/PreparacionAcademica", new { id = oferenteId, u });
                    }

                    Input = new InputModel
                    {
                        Id = prep.Id,
                        CodigoInstitucion = prep.CodigoInstitucion,
                        OferenteId = oferenteId,
                        Titulo = prep.Titulo,
                        FechaInicio = prep.FechaInicio,
                        FechaFin = prep.FechaFin
                    };
                }
                else
                {
                    Input.OferenteId = oferenteId;
                }

                return Page();
            }
            catch (Exception ex)
            {
                await ReportarFallosAsync(u, ex);
                return Page();
            }
        }

        // POST: guardar (crear o editar)
        public async Task<IActionResult> OnPostAsync(string u)
        {
            try
            {
                string usuario = await ObtenerUsuarioAsync(u);
                await CargarInstitucionesAsync(usuario);

                if (!ModelState.IsValid)
                {
                    ErrorMessage = "Verifica todos los campos requeridos.";
                    return Page();
                }

                if (Input.FechaInicio >= Input.FechaFin)
                {
                    ErrorMessage = "La fecha de inicio debe ser menor a la fecha de fin.";
                    return Page();
                }

                var prep = new PreparacionAcad
                {
                    Id = Input.Id,
                    CodigoInstitucion = Input.CodigoInstitucion,
                    OferenteId = Input.OferenteId,
                    Titulo = Input.Titulo,
                    FechaInicio = Input.FechaInicio!.Value,
                    FechaFin = Input.FechaFin!.Value
                };

                int resultado = EsNuevo
                    ? await _iprepAcademicaService.CrearPreparacionAcadAsync(prep, usuario)
                    : await _iprepAcademicaService.ModificarPreparacionAcadAsync(prep, usuario);

                if (resultado == 1)
                {
                    TempData["Mensaje"] = EsNuevo ? "Registrado correctamente." : "Modificado correctamente.";
                    return RedirectToPage("/OFE/PreparacionAcademica", new { id = Input.OferenteId, u });
                }

                ErrorMessage = resultado switch
                {
                    2 => "Verifica todos los espacios y datos ingresados.",
                    3 => "Error en las fechas: la fecha inicial debe ser menor a la final.",
                    4 => "El título no puede superar los 100 caracteres y solo permite letras y espacios.",
                    _ => "Error desconocido al registrar la preparación académica."
                };

                return Page();
            }
            catch (Exception ex)
            {
                await ReportarFallosAsync(u, ex);
                return Page();
            }
        }

        private async Task CargarInstitucionesAsync(string usuario)
        {
            var instituciones = await Task.Run(() => _instEducativa.ObtenerTodas(usuario));
            Instituciones = instituciones
                .Select(i => new SelectListItem
                {
                    Value = i.codigo_institucion,
                    Text = i.nombre,
                    Selected = i.codigo_institucion == Input.CodigoInstitucion
                })
                .ToList();
        }

        private async Task<string> ObtenerUsuarioAsync(string u)
        {
            //var usuario = await Task.Run(() => _authBLL.ObtenerUsuarioPorNombre(u));
            return u ?? "Desconocido";
        }

        private async Task ReportarFallosAsync(string u, Exception ex)
        {
            try
            {
                string usuario = await ObtenerUsuarioAsync(u);
                await Task.Run(() => _iprepAcademicaService.GenericoCrearBitacora(usuario, 4, 1, detalles: "Error: " + ex));
                ErrorMessage = "Error inesperado: " + ex.Message;
            }
            catch
            {
                ErrorMessage = "Error no controlado.";
            }
        }
    }
}