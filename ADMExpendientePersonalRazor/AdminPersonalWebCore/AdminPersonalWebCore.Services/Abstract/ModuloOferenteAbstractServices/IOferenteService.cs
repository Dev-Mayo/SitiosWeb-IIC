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
        Task<IEnumerable<Oferente>> ObtenerOferentesAsync(string usuario);
        Task<Oferente> ObtenerOferenteAsync(string usuario, string identificacion);
        Task<int> InsertarOferenteAsync(Oferente oferente, string usuario);
        Task<int> ActualizarOferenteAsync(Oferente oferente, string usuario);
        Task<int> EliminarOferenteAsync(Oferente oferente, string usuario);
        Task<IEnumerable<Oferente>> ObtenerNombreOferentesAsync();
    }
}
