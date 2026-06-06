using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class PreparacionAcademicaModel : PageModel
    {
        private readonly IPrepAcademicaService _prepAcademicaService;
       // private readonly AuthBLL _authBLL;

        public IEnumerable<PreparacionAcad> Preparaciones { get; set; } = [];
        public string Mensaje { get; set; } = string.Empty;
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        private const int PageSize = 10;

        public PreparacionAcademicaModel(IPrepAcademicaService prepAcademicaService)
        {
            _prepAcademicaService = prepAcademicaService;
        }

        public async Task<IActionResult> OnGetAsync(string id, string u, int pagina = 1)
        {
            try
            {
                string usuario = await ObtenerUsuarioAsync(u);
                var todas = await _prepAcademicaService.ObtenerPreparacionAcadAsync(id, usuario);
                var lista = todas.ToList();

                TotalPaginas = (int)Math.Ceiling(lista.Count / (double)PageSize);
                PaginaActual = pagina;
                Preparaciones = lista
                    .Skip((pagina - 1) * PageSize)
                    .Take(PageSize);

                // Leer mensaje de TempData si viene de un redirect
                if (TempData["Mensaje"] is string msg)
                    Mensaje = msg;

                return Page();
            }
            catch (Exception ex)
            {
                await ReportarFallosAsync(u, ex);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEliminarAsync(int preparacionAcadId, string u, string oferenteId)
        {
            try
            {
                string usuario = await ObtenerUsuarioAsync(u);
                int resultado = await _prepAcademicaService.EliminarPreparacionAcadAsync(preparacionAcadId, usuario);

                TempData["Mensaje"] = resultado switch
                {
                    1 => "Eliminado correctamente.",
                    _ => "No se ha podido eliminar."
                };
            }
            catch (Exception ex)
            {
                await ReportarFallosAsync(u, ex);
                TempData["Mensaje"] = "Error inesperado al eliminar.";
            }

            return RedirectToPage(new { id = oferenteId, u });
        }

        private async Task<string> ObtenerUsuarioAsync(string u)
        {
           // var usuario = await Task.Run(() => _authBLL.ObtenerUsuarioPorNombre(u));
            return u ?? "Desconocido";
        }

        private async Task ReportarFallosAsync(string u, Exception ex)
        {
            try
            {
                string usuario = await ObtenerUsuarioAsync(u);
                await Task.Run(() => _prepAcademicaService.GenericoCrearBitacora(usuario, 4, 1, detalles: "Error: " + ex));
                Mensaje = "Error inesperado: " + ex.Message;
            }
            catch
            {
                Mensaje = "Error no controlado.";
            }
        }
    }
}