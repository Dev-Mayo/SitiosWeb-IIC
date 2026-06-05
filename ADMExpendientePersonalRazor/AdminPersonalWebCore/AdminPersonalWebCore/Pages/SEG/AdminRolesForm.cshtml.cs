using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using AdminPersonalWebCore.Services.ModuloOferenteServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace AdminPersonalWebCore.Pages.SEG
{
    public class AdminRolesFormModel : PageModel
    {
        private readonly IAdminRolService _rolService;
        private readonly BitacoraService _bitacoraService;

        public AdminRolesFormModel(IAdminRolService rolService, BitacoraService bitacoraService)
        {
            _rolService = rolService;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public Rol Rol { get; set; }
        public bool EsEdicion => Rol?.id_rol > 0;


        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
                Rol = await _rolService.ObtenerRolAsync(id);
            else
                Rol = new Rol();
        }

        public async Task<IActionResult> OnPostAsync(string u)
        {
            try
            {
                string nombreRol = Rol.nombre_rol;

                // Validaciones
                Regex regex = new Regex("^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$");
                if (string.IsNullOrEmpty(nombreRol))
                {
                    ModelState.AddModelError("Rol.nombre_rol", "El nombre del rol es requerido.");
                    return Page();
                }
                if (!regex.IsMatch(nombreRol))
                {
                    ModelState.AddModelError("Rol.nombre_rol", "Solo se permiten letras y espacios.");
                    return Page();
                }
                if (nombreRol.Length > 40)
                {
                    ModelState.AddModelError("Rol.nombre_rol", "El nombre del rol no puede superar los 40 caracteres.");
                    return Page();
                }
                if (await _rolService.ValidarDuplicadosAsync(nombreRol) && !EsEdicion)
                {
                    ModelState.AddModelError("Rol.nombre_rol", "El nombre del rol ya existe.");
                    return Page();
                }

                //var usuario = await _authService.ObtenerUsuarioPorNombreAsync(User.Identity?.Name ?? "Sistema");

                if (!EsEdicion) // Crear
                {
                    await _rolService.InsertarRolAsync(nombreRol, u);
                }
                else // Editar
                {
                    await _rolService.ActualizarRolAsync(Rol.id_rol, nombreRol, u);
                }

                return RedirectToPage("AdminRoles", new { u });
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(ex, u);
                ModelState.AddModelError("", "Error inesperado.");
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