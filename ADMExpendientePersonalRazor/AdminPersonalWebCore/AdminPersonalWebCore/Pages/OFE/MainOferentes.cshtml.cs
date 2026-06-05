using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class MainOferentesModel : PageModel
    {
        private readonly IOferenteService _oferenteService;
       // private readonly IAuthService _authService;
        private const int TamPagina = 10;

        public MainOferentesModel(IOferenteService oferenteService)//, IAuthService authService)
        {
            _oferenteService = oferenteService;
            //_authService = authService;
        }

        public IEnumerable<Oferente> OferentesPaginados { get; set; } = Enumerable.Empty<Oferente>();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync(string u, int pagina = 1)
        {
            try
            {
                var usuario = await ObtenerUsuarioAsync(u);
                var todos = (await _oferenteService.ObtenerOferentesAsync(usuario)).ToList();

                TotalPaginas = (int)Math.Ceiling(todos.Count / (double)TamPagina);
                PaginaActual = Math.Clamp(pagina, 1, Math.Max(1, TotalPaginas));
                OferentesPaginados = todos.Skip((PaginaActual - 1) * TamPagina).Take(TamPagina);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error inesperado: " + ex.Message;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostEliminarAsync(string u, string identificacion)
        {
            try
            {
                var usuario = await ObtenerUsuarioAsync(u);
                var oferente = await _oferenteService.ObtenerOferenteAsync(usuario, identificacion);

                if (oferente == null)
                {
                    TempData["Mensaje"] = "No se encontró el oferente.";
                    return RedirectToPage(new { u });
                }

                int resultado = await _oferenteService.EliminarOferenteAsync(oferente, usuario);
                TempData["Mensaje"] = resultado switch
                {
                    1 => "Oferente eliminado correctamente.",
                    2 => "No se puede eliminar un registro con datos relacionados.",
                    _ => "No se ha podido eliminar." + resultado
                };
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error inesperado: " + ex.Message;
            }

            return RedirectToPage(new { u });
        }

        private async Task<string> ObtenerUsuarioAsync(string u)
        {
            /*var user = await _authService.ObtenerUsuarioPorNombreAsync(u);
            return user?.NombreCompleto ?? "Desconocido";*/
            return u;
        }
    }
}