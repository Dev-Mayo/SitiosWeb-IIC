using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class MainOferentesModel : PageModel
    {
        private readonly IOferenteService _oferenteService;
        public IEnumerable<OferenteTemporal> Oferentes { get; set; } = Enumerable.Empty<OferenteTemporal>();

        public MainOferentesModel(IOferenteService oferenteService)
        {
            _oferenteService = oferenteService;
        }

        public async Task OnGetAsync(string u)
        {
            Oferentes = await _oferenteService.ObtenerOferentesAsync(u) ?? new List<OferenteTemporal>();
        }
    }
}
