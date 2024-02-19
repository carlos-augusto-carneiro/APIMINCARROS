using System.Numerics;
using APIMINIMADIO.Dominio.Entidades;


namespace APIMINIMADIO.Dominio.Interfaces;
public interface IVeciulosServices
{
    List<Veiculo> Todos(int? pagina = 1, string? Nome = null, string? Marca = null);
    Veiculo? BuscaPorId(int Id);
    void Incluir(Veiculo veiculo);
    void Atualizar(Veiculo veiculo);
    void Apagar(Veiculo veiculo);

}