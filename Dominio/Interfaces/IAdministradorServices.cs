using APIMINIMADIO.Dominio.Entidades;
using APIMINIMADIO.DTOs;

namespace APIMINIMADIO.Dominio.Interfaces;
public interface IAdministradorServices
{
    Administrador? Login(LoginDTO loginDTO);
    Administrador Incluir(Administrador administradores);
    Administrador? BuscaPorId(int Id);
    List<Administrador> Todos(int? pagina);
}