using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Repository.ModuloOferenteRepository;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services.ModuloOferenteServices
{
    public class PrepAcademicaService : IPrepAcademicaService
    {
        private readonly PrepAcademicaRepository _prepAcademicaRepository;
        private readonly BitacoraService _bitacoraService;
        private int resultado;

        public PrepAcademicaService(
            PrepAcademicaRepository prepAcademicaRepository,
            BitacoraService bitacoraService)
        {
            _prepAcademicaRepository = prepAcademicaRepository;
            _bitacoraService = bitacoraService;
        }

        public void GenericoCrearBitacora(string usuario, int accion, int resultado,
            object objetoOriginal = null, object objetoAnterior = null, string detalles = null)
        {
            if (resultado == 1) // solo registrar si fue exitoso
                _bitacoraService.GenericoCrearBitacora(usuario, accion, resultado, objetoOriginal, objetoAnterior, detalles);
        }

        public async Task<IEnumerable<PreparacionAcad>> ObtenerPreparacionAcadAsync(string identificacion, string usuario)
        {
            var lista = await _prepAcademicaRepository.ObtenerPreparacionAcadAsync(identificacion);
            GenericoCrearBitacora(usuario, 3, 1, detalles: "PrepAcademica");
            return lista;
        }

        public async Task<PreparacionAcad?> ObtenerPreparacionAcadPorIdAsync(int id, string usuario)
        {
            var result = await _prepAcademicaRepository.ObtenerPreparacionAcadPorIdAsync(id);
            GenericoCrearBitacora(usuario, 3, 1, detalles: "PrepAcademica");
            return result;
        }

        public async Task<int> CrearPreparacionAcadAsync(PreparacionAcad prep, string usuario)
        {
            if (string.IsNullOrEmpty(prep.CodigoInstitucion) || string.IsNullOrEmpty(prep.OferenteId)
                || string.IsNullOrEmpty(prep.Titulo) || prep.FechaInicio == default || prep.FechaFin == default)
                return 2; // Error: campos obligatorios

            if (prep.FechaFin < prep.FechaInicio)
                return 3; // Error: fechas

            if (prep.Titulo.Length > 100 || !Regex.IsMatch(prep.Titulo, @"^[a-zA-Z\s]+$"))
                return 4; // Error: título inválido

            resultado = await _prepAcademicaRepository.CrearPreparacionAcadAsync(prep);
            GenericoCrearBitacora(usuario, 0, resultado, objetoOriginal: prep);
            return resultado;
        }

        public async Task<int> ModificarPreparacionAcadAsync(PreparacionAcad prep, string usuario)
        {
            var anterior = await ObtenerPreparacionAcadPorIdAsync(prep.Id, usuario);

            if (string.IsNullOrEmpty(prep.CodigoInstitucion) || prep.Id <= 0
                || string.IsNullOrEmpty(prep.Titulo) || prep.FechaInicio == default || prep.FechaFin == default)
                return 2;

            if (prep.FechaFin < prep.FechaInicio)
                return 3;

            if (prep.Titulo.Length > 100)
                return 4;

            resultado = await _prepAcademicaRepository.ModificarPreparacionAcadAsync(prep);
            GenericoCrearBitacora(usuario, 1, resultado, prep, anterior);
            return resultado;
        }

        public async Task<int> EliminarPreparacionAcadAsync(int id, string usuario)
        {
            var anterior = await ObtenerPreparacionAcadPorIdAsync(id, usuario);
            resultado = await _prepAcademicaRepository.EliminarPreparacionAcadAsync(id);
            GenericoCrearBitacora(usuario, 2, resultado, anterior);
            return resultado;
        }
    }
}
