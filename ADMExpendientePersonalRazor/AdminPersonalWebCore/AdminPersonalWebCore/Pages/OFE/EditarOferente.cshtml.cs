using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class EditarOferenteModel : PageModel
    {
        readonly IOferenteService _oferenteService;

        [BindProperty] public OferenteTemporal Oferente { get; set; }
        public List<SelectListItem> TiposIdentificacion { get; set; }
        public string MensajeError { get; set; }
        public List<string> ConcursosSeleccionados { get; set; } = new();
        public List<SelectListItem> Concursos { get; set; }
        private string FormUsuario;

        public void OnGet(string id, string u)
        {
            FormUsuario = u;
            TiposIdentificacion = new List<SelectListItem>
            {
                new("Cédula","Cedula"),
                new("DIMEX","Dimex"),
                new("Pasaporte","Pasaporte")
            };

            // Cargar oferente desde servicio/repositorio
            Oferente = _oferenteService.ObtenerOferenteAsync(FormUsuario, id).Result;

            // Precargar concursos seleccionados
            Concursos = _oferenteService.ObtenerConcursosAsync(FormUsuario).Result
                .Select(c => new SelectListItem { Value = c.CodigoConcurso, Text = c.Nombre })
                .ToList();
            ConcursosSeleccionados = Oferente.CodigoConcurso;
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

        public IActionResult OnPostGuardarOferente(List<string> ConcursosSeleccionados)
        {
            if (!ModelState.IsValid)
            {
                MensajeError = "Datos inválidos";
                return Page();
            }

            Oferente.CodigoConcurso = ConcursosSeleccionados ?? new List<string>();

            // Actualizar en BD
            int resultado = _oferenteService.ActualizarOferenteAsync(Oferente, FormUsuario).Result;

            if (resultado <= 0)
            {
                MensajeError = "Error al actualizar oferente";
                return Page();
            }

            return RedirectToPage("MainOferentes");
        }
    }
}
