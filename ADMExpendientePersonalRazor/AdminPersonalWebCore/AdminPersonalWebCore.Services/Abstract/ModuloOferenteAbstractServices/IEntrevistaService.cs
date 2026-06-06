using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices
{
    public interface IEntrevistaService
    {
        void GenericoCrearBitacora(string usuario, int accion, int resultado, object? objetoOriginal = null, object? objetoAnterior = null, string? detalles = null);
        Task<IEnumerable<Empleado>> ObtenerNombreEmpleadosAsync();
        Task<IEnumerable<Entrevista>> ObtenerEntrevistasAsync(string usuario, int? entrevistaId = null);
        Task<Entrevista?> ObtenerEntrevistaAsync(string usuario, int entrevistaId);
        Task<int> InsertarEntrevistaAsync(Entrevista entrevista, string usuario);
        Task<int> ModificarEntrevistaAsync(Entrevista entrevista, string usuario);
        Task<int> EliminarEntrevistaAsync(int entrevistaId, string usuario);
        Task<int> CambiarEstadoEntrevistaAsync(int entrevistaId, string usuario);
    }
}

