using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System.Collections.Generic;

namespace AdminPersonalWebCore.Services
{
    public class MenuService
    {
        private readonly MenuRepository _repo;
        public MenuService(MenuRepository repo) => _repo = repo;

        public List<Modulo> ObtenerModulos(int idUsuario) =>
            _repo.ObtenerModulosPorUsuario(idUsuario);
    }
}