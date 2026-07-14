using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace AdminPersonalWebCore.Pages.SEG
{
    public class AdminRolesModel : SecurePageModel
    {
        private readonly AdminRolService _rolService;
        private readonly BitacoraService _bitacoraService;
        private readonly AuthService _authService;
        private readonly ParametroService _parametroService;

        

        public AdminRolesModel(AdminRolService rolService, BitacoraService bitacoraService, AuthService authService, ParametroService parametroService)
        {
            _rolService = rolService;
            _bitacoraService = bitacoraService;
            _authService = authService;
            _parametroService = parametroService;
        }

        // ── Datos para la vista ──────────────────────────────────────────────
        public IEnumerable<Rol> Roles { get; private set; } = [];
        public int PageIndex { get; private set; } = 1;
        public int TotalPages { get; private set; }

        // ── Estado del modal de form ─────────────────────────────────────────
        public bool MostrarModalForm { get; private set; }
        public string MensajeError { get; private set; } = "";
        public int RolIdActual { get; private set; }
        public string NombreRolActual { get; private set; } = "";

        private IActionResult? ValidarSession()
        {
            var check = CheckSession();
            if (check != null) return check;

            var usuario = _authService.ObtenerPorNombre(UsuarioActual);
            if (usuario == null)
                return Redirect("/SEG/Login?msg=login");
            return null;
        }

        // ── GET ──────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            var check = ValidarSession();
            if (check != null) return check;

            try
            {
                await CargarRolesAsync(pageIndex);
                return Page();
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(ex);
                TempData["ModalMensaje"] = "Error inesperado: " + ex.Message;
                return Page();
            }
        }

        // ── POST: Guardar (insertar o modificar) ─────────────────────────────
        public async Task<IActionResult> OnPostGuardarAsync(int rolId, string? nombreRol)
        {
            var check = ValidarSession();
            if (check != null) return check;

            try
            {
                // Validaciones — todas antes de tocar servicios
                if (string.IsNullOrWhiteSpace(nombreRol))
                    return await PaginaConError(rolId, "", "El nombre del rol es requerido.");

                nombreRol = nombreRol.Trim();

                var regex = new Regex("^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$");
                if (!regex.IsMatch(nombreRol))
                    return await PaginaConError(rolId, nombreRol, "Solo se permiten letras y espacios.");

                if (nombreRol.Length > 40)
                    return await PaginaConError(rolId, nombreRol, "El nombre del rol no puede superar los 40 caracteres.");

                bool esEdicion = rolId > 0;

                if (!esEdicion && await _rolService.ValidarDuplicadosAsync(nombreRol))
                    return await PaginaConError(rolId, nombreRol, "El nombre del rol ya existe.");

                if (!esEdicion)
                    await _rolService.InsertarRolAsync(nombreRol, NombreCompleto);
                else
                    await _rolService.ActualizarRolAsync(rolId, nombreRol, NombreCompleto);

                TempData["ModalMensaje"] = esEdicion
                    ? "Rol actualizado correctamente."
                    : "Rol creado correctamente.";

                return RedirectToPage(new { u = UsuarioActual });
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(ex);
                TempData["ModalMensaje"] = "Error inesperado: " + ex.Message;
                return RedirectToPage(new { u = UsuarioActual });
            }
        }

        // ── POST: Eliminar ───────────────────────────────────────────────────
        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var check = ValidarSession();
            if (check != null) return check;

            try
            {
                int resultado = await _rolService.EliminarRolAsync(id, NombreCompleto);

                TempData["ModalMensaje"] = resultado switch
                {
                    2 => "Rol eliminado correctamente.",
                    0 => "No se puede eliminar un registro con datos relacionados.",
                    _ => "No se ha podido eliminar el rol."
                };

                return RedirectToPage(new { u = UsuarioActual });
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(ex);
                TempData["ModalMensaje"] = "Error inesperado: " + ex.Message;
                return RedirectToPage(new { u = UsuarioActual });
            }
        }

        // ── Helpers privados ─────────────────────────────────────────────────
        private async Task CargarRolesAsync(int pageIndex)
        {
            int cantidad = _parametroService.ObtenerValorEnteroODefecto("CANTIDAD_REGISTROS_PAGINA", 10);
            var todos = (await _rolService.ObtenerRolesAsync(NombreCompleto)).ToList();
            TotalPages = (int)Math.Ceiling(todos.Count / (double)cantidad);
            PageIndex = Math.Max(1, Math.Min(pageIndex, Math.Max(TotalPages, 1)));
            Roles = todos.Skip((PageIndex - 1) * cantidad).Take(cantidad);
        }

        // Carga la tabla y devuelve Page() con el modal de error abierto
        private async Task<IActionResult> PaginaConError(int rolId, string nombreRol, string error)
        {
            await CargarRolesAsync(PageIndex);
            MostrarModalForm = true;
            RolIdActual = rolId;
            NombreRolActual = nombreRol;
            MensajeError = error;
            return Page();
        }

        private async Task RegistrarErrorAsync(Exception ex)
        {
            _bitacoraService.Registrar(new Bitacora
            {
                Usuario = NombreCompleto,
                Accion = AccionBitacora.ERROR,
                DescripcionJson = _bitacoraService.CrearJsonError("Error inesperado: " + ex)
            });
            await Task.CompletedTask;
        }
    }
}
