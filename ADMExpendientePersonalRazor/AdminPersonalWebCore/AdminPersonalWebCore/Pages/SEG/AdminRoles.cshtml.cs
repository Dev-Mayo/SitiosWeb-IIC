using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using AdminPersonalWebCore.Services.ModuloOferenteServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPersonalWebCore.Pages.SEG
{
    public class AdminRolesModel : PageModel
    {

        private readonly IAdminRolService _rolService;
        private readonly BitacoraService _bitacoraService;

        public AdminRolesModel(IAdminRolService rolService, BitacoraService bitacoraService)
        {
            _rolService = rolService;
            _bitacoraService = bitacoraService;
        }

        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }

        public IEnumerable<Rol> Roles { get; set; } = Enumerable.Empty<Rol>();
        [BindProperty]
        public string Mensaje { get; set; }

        public async Task OnGetAsync(string u, int pageIndex = 1)
        {
            try
            {
                PageIndex = pageIndex;

                var roles = (await _rolService.ObtenerRolesAsync(u)).ToList();

                int pageSize = 10;
                TotalPages = (int)Math.Ceiling(roles.Count / (double)pageSize);

                Roles = roles.Skip((PageIndex - 1) * pageSize).Take(pageSize);
                //var usuario = await _authService.ObtenerUsuarioPorNombreAsync(User.Identity?.Name ?? "Sistema");
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(ex, u);
                TempData["ModalMensaje"] = "Error inesperado: " + ex.Message;
            }
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id, string u)
        {
            try
            {
                //var usuario = await _authService.ObtenerUsuarioPorNombreAsync(User.Identity?.Name ?? "Sistema");
                int resultado = await _rolService.EliminarRolAsync(id, u);

                TempData["ModalMensaje"] = resultado switch
                {
                    2 => "Rol eliminado correctamente.",
                    0 => "No se puede eliminar un registro con datos relacionados.",
                    _ => "No se ha eliminado."
                };

                Roles = await _rolService.ObtenerRolesAsync(u);
                return RedirectToPage("AdminRoles", new { u });
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(ex, u);
                TempData["ModalMensaje"] = "Error inesperado: " + ex.Message;
                return Page();
            }
        }

        private async Task RegistrarErrorAsync(Exception ex, string u)
        {
            //var usuario = await _authService.ObtenerUsuarioPorNombreAsync(User.Identity?.Name ?? "Sistema");
            _bitacoraService.Registrar(new Bitacora
            {
                Usuario = u,
                Accion = AccionBitacora.ERROR,
                DescripcionJson = _bitacoraService.CrearJsonError("Error inesperado: " + ex)
            });
        }
    }
}
