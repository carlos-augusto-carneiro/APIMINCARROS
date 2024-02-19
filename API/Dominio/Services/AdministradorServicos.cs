using APIMINIMADIO.Dominio.DTO;
using APIMINIMADIO.Dominio.Entidades;
using APIMINIMADIO.Dominio.Interfaces;
using APIMINIMADIO.Infra.DB;
using Microsoft.EntityFrameworkCore;

namespace APIMINIMADIO.Dominio.Services;

public class AdministradorServicos : IAdministradorServices
{
    private readonly DBcontexto _contexto;
    public AdministradorServicos(DBcontexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? BuscaPorId(int Id)
    {
        return _contexto.Administradores.Where(a => a.Id == Id).FirstOrDefault();
    }

    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
        
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();

        int ItensPorPaginas = 10;

        if(pagina!=null)
        {
            query = query.Skip(((int)pagina - 1) * ItensPorPaginas).Take(ItensPorPaginas);
        }
        

        return query.ToList();
    }
}