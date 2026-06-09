using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.BLL
{
    public class MenuBLL
    {
        private readonly MenuDAL _dal = new MenuDAL();

        public List<Modulo> ObtenerModulosPorUsuario(int idUsuario)
        {
            return _dal.ObtenerModulosPorUsuario(idUsuario);
        }
    }
}
