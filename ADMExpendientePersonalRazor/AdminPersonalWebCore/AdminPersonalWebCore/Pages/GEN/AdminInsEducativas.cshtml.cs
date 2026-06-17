using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Pages;
using AdminPersonalWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdminPersonalWebCore.Pages.GEN
{
    public class AdminInsEducativasModel : SecurePageModel
    {
        private readonly InstEducativaService _svc;
        private readonly BitacoraService _bitacora;

        public AdminInsEducativasModel(InstEducativaService svc, BitacoraService bitacora)
        {
            _svc = svc;
            _bitacora = bitacora;
        }

        public List<InstEducativa> Instituciones { get; set; } = new();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public const int PageSize = 10;


        public string MensajeSistema { get; set; }
        public string ErrorForm { get; set; }


        public string FormCodigo { get; set; }
        public string FormNombre { get; set; }
        public string FormAccion { get; set; } = "crear";

        public IActionResult OnGet(int page = 1)
        {
            var check = CheckSession();
            if (check != null) return check;

            PaginaActual = page;
            CargarDatos();
            return Page();
        }

        public IActionResult OnPostGuardar(string accion, string codigo, string nombre)
        {
            var check = CheckSession();
            if (check != null) return check;

            FormCodigo = codigo;
            FormNombre = nombre;
            FormAccion = accion;


            string error = Validar(codigo, nombre, accion == "editar");
            if (error != null)
            {
                ErrorForm = error;
                CargarDatos();
                return Page();
            }

            try
            {
                if (accion == "crear")
                {
                    if (_svc.ValidarDuplicado(codigo, nombre))
                    {
                        ErrorForm = "El código o nombre ya existe.";
                        CargarDatos();
                        return Page();
                    }
                    _svc.Insertar(codigo, nombre, NombreCompleto);
                    MensajeSistema = "Institución creada correctamente.";
                }
                else
                {
                    if (_svc.ValidarDuplicado(codigo, nombre, codigo))
                    {
                        ErrorForm = "El nombre ya existe en otra institución.";
                        CargarDatos();
                        return Page();
                    }
                    _svc.Actualizar(codigo, nombre, NombreCompleto);
                    MensajeSistema = "Institución actualizada correctamente.";
                }
            }
            catch (Exception ex)
            {
                MensajeSistema = "Error al guardar: " + ex.Message;
            }

            CargarDatos();
            return Page();
        }

        public IActionResult OnPostEliminar(string codigo)
        {
            var check = CheckSession();
            if (check != null) return check;

            try
            {
                _svc.Eliminar(codigo, NombreCompleto);
                MensajeSistema = "Institución eliminada correctamente.";
            }
            catch (Exception ex)
            {
                MensajeSistema = ex.Message.Contains("No se puede eliminar")
                    ? ex.Message
                    : "Error al eliminar: " + ex.Message;
            }

            CargarDatos();
            return Page();
        }

        // ─── Helpers ─────────────────────────────────────────────

        private void CargarDatos()
        {
            var todas = _svc.ObtenerTodas(NombreCompleto);
            TotalPaginas = (int)Math.Ceiling(todas.Count / (double)PageSize);
            Instituciones = todas
                .Skip((PaginaActual - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        private string Validar(string codigo, string nombre, bool esEditar)
        {
            var regexSoloLetras = new Regex(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$");

            if (!esEditar && string.IsNullOrEmpty(codigo))
                return "El código es requerido.";
            if (string.IsNullOrEmpty(nombre))
                return "El nombre es requerido.";
            if (nombre.Length > 150)
                return "El nombre no puede superar los 150 caracteres.";
            if (!regexSoloLetras.IsMatch(nombre))
                return "El nombre solo puede contener letras.";

            return null;
        }
    }
}