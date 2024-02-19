using APIMINIMADIO.Dominio.DTO;
using APIMINIMADIO.Dominio.Entidades;

namespace APIMINIMADIO.Dominio.Interfaces;
public interface IAdministradorServices
{
    Administrador? Login(LoginDTO loginDTO);
    Administrador Incluir(Administrador administradores);
    Administrador? BuscaPorId(int Id);
    List<Administrador> Todos(int? pagina);
}