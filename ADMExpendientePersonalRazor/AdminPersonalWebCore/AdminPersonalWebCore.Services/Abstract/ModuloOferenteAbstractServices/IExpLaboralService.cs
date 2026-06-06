using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices
{
    public interface IExpLaboralService
    {
        public List<Compania> ObtenerCompanias(string usuarioActual);
        void GenericoCrearBitacora(string usuario, int accion, int resultado, object objetoOriginal = null, object objetoAnterior = null, string detalles = null);
        Task<IEnumerable<ExpLaboral>> ObtenerExpLaboralAsync(string oferenteIdentificacion, string usuarioActual);
        Task<int> CrearExpLaboralAsync(ExpLaboral exp, string usuarioActual);
        Task<int> ModificarExpLaboralAsync(ExpLaboral exp, string usuarioActual);
        Task<int> EliminarExpLaboralAsync(int id, string usuarioActual);
    }
}
