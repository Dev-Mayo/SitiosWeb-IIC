using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class EliminarOferenteModel : PageModel
    {
        private readonly IOferenteService _oferenteService;
        [BindProperty] public string IdOferente { get; set; }

        public EliminarOferenteModel(IOferenteService oferenteService)
        {
            _oferenteService = oferenteService;
        }

        public void OnGet(string id)
        {
            IdOferente = id;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var oferente = await _oferenteService.ObtenerOferenteAsync("usuarioDemo", IdOferente);
            await _oferenteService.EliminarOferenteAsync(oferente, "usuarioDemo");
            return RedirectToPage("MainOferentes");
        }
    }
}
