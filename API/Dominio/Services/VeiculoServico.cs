using System.Numerics;
using APIMINIMADIO.Dominio.Entidades;
using APIMINIMADIO.Dominio.Interfaces;
using APIMINIMADIO.Infra.DB;
using Microsoft.EntityFrameworkCore;

namespace APIMINIMADIO.Dominio.Services;

public class VeiculoServico : IVeciulosServices
{
    private readonly DBcontexto _contexto;
    public VeiculoServico(DBcontexto contexto)
    {
        _contexto = contexto;
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public Veiculo? BuscaPorId(int Id)
    {
        return _contexto.Veiculos.Where(v => v.Id == Id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }


    public List<Veiculo> Todos(int? pagina = 1, string? Nome = null, string? Marca = null)
    {
        var query = _contexto.Veiculos.AsQueryable();
        if(!string.IsNullOrEmpty(Nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{Nome.ToLower()}%"));
        }

        int ItensPorPaginas = 10;

        if(pagina!=null)
        {
            query = query.Skip(((int)pagina - 1) * ItensPorPaginas).Take(ItensPorPaginas);
        }
        

        return query.ToList();
    }
}