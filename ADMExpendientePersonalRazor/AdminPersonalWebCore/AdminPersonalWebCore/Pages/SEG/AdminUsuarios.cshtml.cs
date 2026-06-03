using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdminPersonalWebCore.Pages.SEG
{
    public class AdminUsuariosModel : SecurePageModel
    {
        private readonly AdminUsuarioService _svc;
        private readonly BitacoraService _bitacora;

        public AdminUsuariosModel(AdminUsuarioService svc, BitacoraService bitacora)
        {
            _svc = svc;
            _bitacora = bitacora;
        }

     
        public List<Usuario> Usuarios { get; set; } = new();
        public List<Rol> Roles { get; set; } = new();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public const int PageSize = 10;

      
        public string MensajeExito { get; set; }
        public string MensajeError { get; set; }
        public string ErrorNuevo { get; set; }
        public string ErrorEditar { get; set; }

        
        public string FormUsername { get; set; }
        public string FormNombreCompleto { get; set; }
        public string FormCorreo { get; set; }

        public IActionResult OnGet(int page = 1)
        {
            var check = CheckSession();
            if (check != null) return check;

            PaginaActual = page;
            CargarDatos();
            return Page();
        }

        public IActionResult OnPostCambiarEstado(int idUsuario, string nuevoEstado)
        {
            var check = CheckSession();
            if (check != null) return check;

            try
            {
                _svc.CambiarEstado(idUsuario, nuevoEstado, NombreCompleto);
                MensajeExito = $"Usuario {(nuevoEstado == "Activo" ? "activado" : "inactivado")} correctamente.";
            }
            catch
            {
                MensajeError = "Error al cambiar estado.";
            }

            CargarDatos();
            return Page();
        }

        public IActionResult OnPostCrear(string username, string nombreCompleto,
            string correo, string password, string confirmarPassword,
            List<string> roles)
        {
            var check = CheckSession();
            if (check != null) return check;

            FormUsername = username;
            FormNombreCompleto = nombreCompleto;
            FormCorreo = correo;

            string error = Validar(username, nombreCompleto, correo,
                password, confirmarPassword, roles, true);

            if (error != null)
            {
                ErrorNuevo = error;
                CargarDatos();
                return Page();
            }

            if (_svc.ValidarDuplicados(username, correo))
            {
                ErrorNuevo = "El nombre de usuario o correo ya existe.";
                CargarDatos();
                return Page();
            }

            try
            {
                string rolesStr = string.Join(",", roles);
                _svc.Insertar(username, nombreCompleto, correo,
                    password, rolesStr, NombreCompleto);
                MensajeExito = "Usuario creado correctamente.";
            }
            catch (Exception ex)
            {
                MensajeError = "Error al crear usuario: " + ex.Message;
            }

            CargarDatos();
            return Page();
        }

        public IActionResult OnPostEditar(int idUsuario, string username,
    string nombreCompleto, string correo, string password,
    string confirmarPassword, string estado, List<string> roles)
        {
            var check = CheckSession();
            if (check != null) return check;

            
            bool validarPwd = !string.IsNullOrEmpty(password);

            
            if (string.IsNullOrEmpty(username))
            { ErrorEditar = "El nombre de usuario es requerido."; CargarDatos(); return Page(); }

            if (string.IsNullOrEmpty(nombreCompleto))
            { ErrorEditar = "El nombre completo es requerido."; CargarDatos(); return Page(); }

            var regexCorreo = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (string.IsNullOrEmpty(correo) || !regexCorreo.IsMatch(correo))
            { ErrorEditar = "Debe ingresar un correo válido."; CargarDatos(); return Page(); }

            if (roles == null || roles.Count == 0)
            { ErrorEditar = "Debe seleccionar al menos un rol."; CargarDatos(); return Page(); }

           
            if (validarPwd)
            {
                var regexPwd = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
                if (!regexPwd.IsMatch(password))
                { ErrorEditar = "La contraseña debe tener mínimo 8 caracteres, mayúsculas, minúsculas, números y caracteres especiales."; CargarDatos(); return Page(); }
                if (password != confirmarPassword)
                { ErrorEditar = "Las contraseñas no coinciden."; CargarDatos(); return Page(); }
            }

            if (_svc.ValidarDuplicados(username, correo, idUsuario))
            { ErrorEditar = "El nombre de usuario o correo ya existe en otro usuario."; CargarDatos(); return Page(); }

            try
            {
                string rolesStr = string.Join(",", roles);
                _svc.Actualizar(idUsuario, username, nombreCompleto,
                    correo, estado, rolesStr, password, NombreCompleto);
                MensajeExito = "Usuario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message.Contains("No se puede")
                    ? ex.Message
                    : "Error al actualizar usuario: " + ex.Message;
            }

            CargarDatos();
            return Page();
        }

        public IActionResult OnPostEliminar(int idUsuario)
        {
            var check = CheckSession();
            if (check != null) return check;

            try
            {
                _svc.Eliminar(idUsuario, NombreCompleto);
                MensajeExito = "Usuario eliminado correctamente.";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message.Contains("No se puede eliminar")
                    ? ex.Message
                    : "Error al eliminar usuario: " + ex.Message;
            }

            CargarDatos();
            return Page();
        }

        // ─── Helpers ────────────────────────────────────────────────

        private void CargarDatos()
        {
            Roles = _svc.ObtenerRoles();
            var todos = _svc.ObtenerUsuarios(NombreCompleto);
            TotalPaginas = (int)Math.Ceiling(todos.Count / (double)PageSize);
            Usuarios = todos
                .Skip((PaginaActual - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        private string Validar(string username, string nombreCompleto, string correo,
            string password, string confirmar, List<string> roles, bool validarPwd)
        {
            var regexCorreo = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var regexPwd = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");

            if (string.IsNullOrEmpty(username)) return "El nombre de usuario es requerido.";
            if (string.IsNullOrEmpty(nombreCompleto)) return "El nombre completo es requerido.";
            if (string.IsNullOrEmpty(correo) || !regexCorreo.IsMatch(correo))
                return "Debe ingresar un correo válido.";
            if (roles == null || roles.Count == 0) return "Debe seleccionar al menos un rol.";

            if (validarPwd)
            {
                if (string.IsNullOrEmpty(password)) return "La contraseña es requerida.";
                if (!regexPwd.IsMatch(password))
                    return "La contraseña debe tener mínimo 8 caracteres, mayúsculas, minúsculas, números y caracteres especiales.";
                if (password != confirmar) return "Las contraseñas no coinciden.";
            }

            return null;
        }
    }
}