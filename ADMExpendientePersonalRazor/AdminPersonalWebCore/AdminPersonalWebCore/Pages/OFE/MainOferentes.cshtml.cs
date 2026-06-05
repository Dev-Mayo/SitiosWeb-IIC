using System.Collections.Generic;
using System.Linq;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Repository.ModuloOferenteRepository;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPersonalWebCore.Pages.OFE
{
    public class MainOferentes : PageModel
    {
        private readonly IOferenteService _oferenteService;

        public MainOferentes(IOferenteService oferenteService)
        {
            _oferenteService = oferenteService;
        }

        // Propiedades para la vista
        public List<OferenteTemporal> Oferentes { get; set; } = new();
        [BindProperty] public OferenteTemporal Oferente { get; set; } = new();
        [BindProperty] public string IdOferente { get; set; }
        [BindProperty] public string Accion { get; set; }
        public string MensajeError { get; set; }
        public string MensajeModal { get; set; }
        public bool MostrarModal { get; set; }

        public string FormUsuario = "Desconocido";

        public List<SelectListItem> TiposIdentificacion => new List<SelectListItem>
        {
            new SelectListItem { Value = "DIMEX", Text = "DIMEX" },
            new SelectListItem { Value = "Pasaporte", Text = "Pasaporte" },
            new SelectListItem { Value = "Cedula", Text = "Cedula" }
        };

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }



        // Listar oferentes
        public async Task OnGetAsync(string u, int page = 1)
        {
            FormUsuario = u;

            Oferentes = (await _oferenteService.ObtenerOferentesAsync(FormUsuario)).ToList();

            CurrentPage = page;
            TotalPages = (int)Math.Ceiling(Oferentes.Count() / (double)PageSize);

            Oferentes = Oferentes
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        // Abrir modal Nuevo
        public IActionResult OnPostNuevo()
        {
            Oferente = new OferenteTemporal();
            Accion = "Nuevo";
            MostrarModal = true;
            return Page();
        }

        // Abrir modal Editar
        public async Task<IActionResult> OnPostEditar(string id)
        {
            Oferente = await _oferenteService.ObtenerOferenteAsync(FormUsuario, id);
            Accion = "Editar";
            MostrarModal = true;
            return Page();
        }

        // Guardar (Insertar o Actualizar)
        public async Task<IActionResult> OnPostGuardarOferente()
        {
            if (Accion == "Nuevo")
                await _oferenteService.InsertarOferenteAsync(Oferente, FormUsuario);
            else if (Accion == "Editar")
                await _oferenteService.ActualizarOferenteAsync(Oferente, FormUsuario);

            return RedirectToPage();
        }

        // Abrir modal Eliminar
        public IActionResult OnPostEliminar(string id)
        {
            IdOferente = id;
            MostrarModal = true;
            return Page();
        }

        // Confirmar eliminación
        public async Task<IActionResult> OnPostConfirmarEliminar()
        {
            var oferente = await _oferenteService.ObtenerOferenteAsync(FormUsuario, IdOferente);
            await _oferenteService.EliminarOferenteAsync(oferente, FormUsuario);
            return RedirectToPage();
        }

        // Handlers para correos y teléfonos
        public IActionResult OnPostAgregarCorreo()
        {
            Oferente.Email.Add(string.Empty);
            MostrarModal = true;
            return Page();
        }

        public IActionResult OnPostEliminarCorreo(int index)
        {
            if (index >= 0 && index < Oferente.Email.Count)
                Oferente.Email.RemoveAt(index);
            MostrarModal = true;
            return Page();
        }

        public IActionResult OnPostAgregarTelefono()
        {
            Oferente.Telefono.Add(string.Empty);
            MostrarModal = true;
            return Page();
        }

        public IActionResult OnPostEliminarTelefono(int index)
        {
            if (index >= 0 && index < Oferente.Telefono.Count)
                Oferente.Telefono.RemoveAt(index);
            MostrarModal = true;
            return Page();
        }
    }
}
