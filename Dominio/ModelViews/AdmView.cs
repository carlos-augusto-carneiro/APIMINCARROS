
namespace APIMINIMADIO.Dominio.ModelViews;

public record AdmView
{
    public int Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;
}