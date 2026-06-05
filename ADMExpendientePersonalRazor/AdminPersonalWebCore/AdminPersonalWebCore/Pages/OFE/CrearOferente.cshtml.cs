using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class CrearOferenteModel : PageModel
    {
        readonly IOferenteService _oferenteService;

        public CrearOferenteModel(IOferenteService oferenteService)
        {
            _oferenteService = oferenteService ?? throw new ArgumentNullException(nameof(oferenteService));
        }

        [BindProperty] public OferenteTemporal Oferente { get; set; } = new();
        public List<SelectListItem> TiposIdentificacion { get; set; }
        public string MensajeError { get; set; }

        private string FormUsuario;

        public IEnumerable<ConcursoTemporal> Concursos { get; set; } = Enumerable.Empty<ConcursoTemporal>();
        public List<SelectListItem> ListaConcursos { get; set; }

        public async Task OnGetAsync(string u)
        {
            FormUsuario = u;
            TiposIdentificacion = new List<SelectListItem>
            {
                new("Cédula","Cedula"),
                new("DIMEX","Dimex"),
                new("Pasaporte","Pasaporte")
            };

            // Inicializar listas
            Oferente.Email = new List<string>();
            Oferente.Telefono = new List<string>();
            Oferente.CodigoConcurso = new List<string>();


            Concursos = await _oferenteService.ObtenerConcursosAsync(FormUsuario);
            ListaConcursos = Concursos.Select(c => new SelectListItem {Value = c.CodigoConcurso, Text = c.Nombre}).ToList();
        }

        public IActionResult OnPostAgregarCorreo()
        {
            Oferente.Email.Add(string.Empty);
            return Page();
        }

        public IActionResult OnPostEliminarCorreo(int index)
        {
            if (index >= 0 && index < Oferente.Email.Count)
                Oferente.Email.RemoveAt(index);
            return Page();
        }

        public IActionResult OnPostAgregarTelefono()
        {
            Oferente.Telefono.Add(string.Empty);
            return Page();
        }

        public IActionResult OnPostEliminarTelefono(int index)
        {
            if (index >= 0 && index < Oferente.Telefono.Count)
                Oferente.Telefono.RemoveAt(index);
            return Page();
        }

        public async Task<IActionResult> OnPostGuardarOferente(List<string> ConcursosSeleccionados)
        {
            if (!ModelState.IsValid)
            {
                MensajeError = "Datos inválidos";
                return Page();
            }

            Oferente.CodigoConcurso = ConcursosSeleccionados ?? new List<string>();

            // Aquí llamas tu servicio/repositorio para insertar
            int resultado = await _oferenteService.InsertarOferenteAsync(Oferente, FormUsuario);

            if (resultado <= 0)
            {
                MensajeError = "Error al guardar oferente";
                return Page();
            }

            return RedirectToPage("MainOferentes");
        }
    }
}
