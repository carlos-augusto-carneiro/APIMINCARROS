using APIMINIMADIO.Dominio.Enuns;

namespace APIMINIMADIO.DTOs;

public class AdministradoresDTO{

    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public Perfil? Perfil { get; set; } = default!;

}