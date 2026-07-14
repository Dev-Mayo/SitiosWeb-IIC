using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services
{
    public class OferenteService
    {
        private readonly OferenteRepository _oferenteRepository;
        private readonly BitacoraService _bitacoraService;

        public OferenteService(OferenteRepository oferenteRepository, BitacoraService bitacoraService)
        {
            _oferenteRepository = oferenteRepository;
            _bitacoraService = bitacoraService;
        }

        private void GenericoCrearBitacora(string usuario, int accion, int resultado, object objetoOriginal = null, object objetoAnterior = null, string detalles = null)
        {
            _bitacoraService.GenericoCrearBitacora(usuario, accion, resultado, objetoOriginal, objetoAnterior, detalles);
        }

        public async Task<IEnumerable<ConcursoTemporal>> ObtenerConcursosAsync(string usuario, string identificacion = null)
        {
            var concursos = await _oferenteRepository.ObtenerConcursosAsync(identificacion);
            //GenericoCrearBitacora(usuario, 3, 1, detalles: "Concursos Temporales");
            return concursos;
        }

        public async Task<IEnumerable<Oferente>> ObtenerOferentesAsync(string usuario)
        {
            var oferentes = await _oferenteRepository.ObtenerOferentesAsync();
            GenericoCrearBitacora(usuario, 3, 1, detalles: "Oferentes");
            return oferentes;
        }

        public async Task<IEnumerable<Oferente>> ObtenerNombreOferentesAsync()
        {
            return await _oferenteRepository.ObtenerNombreOferentesAsync();
        }

        public async Task<Oferente> ObtenerOferenteAsync(string usuario, string identificacion)
        {
            var oferente = (await _oferenteRepository.ObtenerOferentesAsync(identificacion)).FirstOrDefault();
            GenericoCrearBitacora(usuario, 3, 1, detalles: $"Oferente {identificacion}");
            return oferente;
        }

        /// <returns>0 = fallo, 1 = éxito, 2 = ya asignado, 3 = ya existe, 4 = datos inválidos</returns>
        public async Task<int> InsertarOferenteAsync(Oferente oferente, string usuario)
        {
            if (!oferente.ValidarDatos())
                return 4; // Datos inválidos

            var resultado = await _oferenteRepository.GestionarOferenteAsync(0, oferente);
            GenericoCrearBitacora(usuario, 0, resultado, objetoOriginal: oferente);
            return resultado;
        }

        public async Task<int> ActualizarOferenteAsync(Oferente oferente, string usuario)
        {
            if (!oferente.ValidarDatos())
                return 4; // Datos inválidos

            var oferenteAnterior = (await _oferenteRepository.ObtenerOferentesAsync(oferente.Identificacion)).FirstOrDefault();
            var resultado = await _oferenteRepository.GestionarOferenteAsync(1, oferente);
            GenericoCrearBitacora(usuario, 1, resultado, objetoOriginal: oferente, objetoAnterior: oferenteAnterior);
            return resultado;
        }

        public async Task<int> EliminarOferenteAsync(Oferente oferente, string usuario)
        {
            var resultado = await _oferenteRepository.GestionarOferenteAsync(2, oferente);
            GenericoCrearBitacora(usuario, 2, resultado, objetoOriginal: oferente);
            return resultado;
        }
    }
}
