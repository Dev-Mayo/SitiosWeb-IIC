using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services
{
    public class EntrevistaService
    {
        private readonly EntrevistaRepository _entrevistaRepository;
        private readonly BitacoraService _bitacoraService;

        public EntrevistaService(EntrevistaRepository entrevistaRepository, BitacoraService bitacoraService)
        {
            _entrevistaRepository = entrevistaRepository;
            _bitacoraService = bitacoraService;
        }

        public void GenericoCrearBitacora(string usuario, int accion, int resultado,
            object? objetoOriginal = null, object? objetoAnterior = null, string? detalles = null)
        {
            _bitacoraService.GenericoCrearBitacora(usuario, accion, resultado, objetoOriginal, objetoAnterior, detalles);
        }

        public async Task<IEnumerable<Empleado>> ObtenerNombreEmpleadosAsync()
        {
            return await _entrevistaRepository.ObtenerNombreEmpleadosAsync();
        }

        public async Task<IEnumerable<Entrevista>> ObtenerEntrevistasAsync(string usuario, int? entrevistaId = null)
        {
            var entrevistas = await _entrevistaRepository.ObtenerEntrevistasAsync(entrevistaId);
            GenericoCrearBitacora(usuario, 3, 1, detalles: "Entrevistas");
            return entrevistas;
        }

        public async Task<Entrevista?> ObtenerEntrevistaAsync(string usuario, int entrevistaId)
        {
            var entrevista = (await _entrevistaRepository.ObtenerEntrevistasAsync(entrevistaId)).FirstOrDefault();
            GenericoCrearBitacora(usuario, 3, 1, detalles: $"Entrevista {entrevistaId}");
            return entrevista;
        }

        public async Task<int> InsertarEntrevistaAsync(Entrevista entrevista, string usuario)
        {
            if (string.IsNullOrEmpty(entrevista.OferenteIdentificacion) || entrevista.EmpleadoId <= 0 || entrevista.FechaEntrevista == default)
                return 2; // Datos inválidos

            if (entrevista.FechaEntrevista < DateTime.Now)
                return 3; // Fecha inválida

            var resultado = await _entrevistaRepository.CrearEntrevistaAsync(entrevista);
            GenericoCrearBitacora(usuario, 0, resultado, objetoOriginal: entrevista);
            return resultado;
        }

        public async Task<int> ModificarEntrevistaAsync(Entrevista entrevista, string usuario)
        {
            if (string.IsNullOrEmpty(entrevista.OferenteIdentificacion) || entrevista.EmpleadoId <= 0 || entrevista.FechaEntrevista == default)
                return 2; // Datos inválidos

            if (entrevista.FechaEntrevista < DateTime.Now)
                return 3; // Fecha inválida

            var anterior = (await _entrevistaRepository.ObtenerEntrevistasAsync(entrevista.EntrevistaId)).FirstOrDefault();
            var resultado = await _entrevistaRepository.ModificarEntrevistaAsync(entrevista);
            GenericoCrearBitacora(usuario, 1, resultado, entrevista, anterior);
            return resultado;
        }

        public async Task<int> EliminarEntrevistaAsync(int entrevistaId, string usuario)
        {
            var anterior = (await _entrevistaRepository.ObtenerEntrevistasAsync(entrevistaId)).FirstOrDefault();
            var resultado = await _entrevistaRepository.EliminarEntrevistaAsync(entrevistaId);
            GenericoCrearBitacora(usuario, 2, resultado, objetoOriginal: anterior);
            return resultado;
        }

        public async Task<int> CambiarEstadoEntrevistaAsync(int entrevistaId, string usuario)
        {
            var anterior = (await _entrevistaRepository.ObtenerEntrevistasAsync(entrevistaId)).FirstOrDefault();
            if (anterior == null) return 0;

            var nuevoEstado = anterior;
            nuevoEstado.Estado = "Realizada";

            var resultado = await _entrevistaRepository.CambiarEstadoEntrevistaAsync(entrevistaId);
            GenericoCrearBitacora(usuario, 1, resultado, nuevoEstado, anterior);
            return resultado;
        }
    }
}
