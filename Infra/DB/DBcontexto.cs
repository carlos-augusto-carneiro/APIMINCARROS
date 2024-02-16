using Microsoft.EntityFrameworkCore;
using APIMINIMADIO.Dominio.Entidades;

namespace APIMINIMADIO.Infra.DB;

public class DBcontexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;

    public DBcontexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }

    public DbSet<Administrador> Administradores { get; set; } = default!;

    public DbSet<Veiculo> Veiculos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador {
                Id = 1,
                Email = "adm@hotmail.com",
                Senha = "123456",
                Perfil = "ADM"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured){
        
            var stringConexao = _configuracaoAppSettings.GetConnectionString("mysql")?.ToString();
            if(!string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseMySql(
                stringConexao, 
                ServerVersion.AutoDetect(stringConexao)
                );
            }
        }

    }
}