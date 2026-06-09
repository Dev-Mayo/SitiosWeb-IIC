using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace AdminPersonalWebCore.Pages.SEG
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;
        private readonly MenuService _menuService;
        private readonly BitacoraService _bitacoraService;

        public LoginModel(AuthService authService, MenuService menuService,
            BitacoraService bitacoraService)
        {
            _authService = authService;
            _menuService = menuService;
            _bitacoraService = bitacoraService;
        }

        public string Mensaje { get; set; }
        public string MensajeClase { get; set; } = "alert-warning";
        public bool Bloqueado { get; set; }
        public string Username { get; set; }

        public void OnGet()
        {
            string msg = Request.Query["msg"];

            if (msg == "login")
            {
                Mensaje = "Por favor inicie sesión para utilizar el sistema.";
                MensajeClase = "alert-warning";
            }
            else if (msg == "expirado")
            {
                Mensaje = "Su sesión ha expirado por inactividad.";
                MensajeClase = "alert-warning";
            }
        }

        public IActionResult OnPost(string Username, string Password)
        {
            this.Username = Username;

            var result = _authService.Login(Username, Password);

            if (!result.success)
            {
                Mensaje = result.mensaje;
                MensajeClase = "alert-danger";
                Bloqueado = result.mensaje.Contains("bloqueado");
                return Page();
            }

            HttpContext.Session.SetString("username", result.usuario.nombreusuario);
            HttpContext.Session.SetString("nombre_completo", result.usuario.nombre_completo);
            HttpContext.Session.SetInt32("id_usuario", result.usuario.id_usuario);

            var modulos = _menuService.ObtenerModulos(result.usuario.id_usuario);
            var modulosNombres = modulos.Select(m => m.nombre_modulo).ToList();
            HttpContext.Session.SetString("modulos",
                JsonSerializer.Serialize(modulosNombres));

            _bitacoraService.Registrar(new Bitacora
            {
                Usuario = result.usuario.nombre_completo,
                Accion = AccionBitacora.READ,
                DescripcionJson = _bitacoraService.CrearJsonConsulta("Login")
            });

            return Redirect($"/SEG/Bienvenida?u={result.usuario.nombreusuario}");
        }
    }
}