using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices
{
    public interface IOferenteService
    {
        Task<IEnumerable<ConcursoTemporal>> ObtenerConcursosAsync(string usuario, string identificacion = null);
        Task<IEnumerable<OferenteTemporal>> ObtenerOferentesAsync(string usuario);
        Task<IEnumerable<OferenteTemporal>> ObtenerNombreOferentesAsync();
        Task<OferenteTemporal> ObtenerOferenteAsync(string usuario, string identificacion);
        Task<int> InsertarOferenteAsync(OferenteTemporal oferente, string usuario);
        Task<int> ActualizarOferenteAsync(OferenteTemporal oferente, string usuario);
        Task<int> EliminarOferenteAsync(OferenteTemporal oferente, string usuario);
    }
}
