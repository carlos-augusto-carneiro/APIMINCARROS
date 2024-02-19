using APIMINIMADIO.Dominio.Enuns;

namespace APIMINIMADIO.Dominio.DTO;
public class AdministradoresDTO{

    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public Perfil? Perfil { get; set; } = default!;

}