using System.Reflection;
using APIMINIMADIO.Dominio.Entidades;
using APIMINIMADIO.Dominio.Services;
using APIMINIMADIO.Infra.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Test.Domain.Services;

[TestClass]
public class AdministradorTestServico
{

    private DBcontexto CriarConetextoDeTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "","..","..",".."));

        var builder = new ConfigurationBuilder()
        .SetBasePath(path ?? Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

        var config = builder.Build();

        return new DBcontexto(config);
    }
    [TestMethod]
    public void SalvarAdm()
    {
        var context = CriarConetextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLES administradores");
        var Adm = new Administrador();

        Adm.Id = 1;
        Adm.Email = "Teste@teste.com";
        Adm.Senha = "teste";
        Adm.Perfil = "adm";

        var AdministradorServices = new AdministradorServicos(context);

        AdministradorServices.Incluir(Adm);

        Assert.AreEqual(1, AdministradorServices.Todos(1).Count());
    }

    [TestMethod]
    public void BuscarPORID()
    {
        var context = CriarConetextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");
        var Adm = new Administrador();

        Adm.Id = 1;
        Adm.Email = "Teste@teste.com";
        Adm.Senha = "teste";
        Adm.Perfil = "adm";

        
        var AdministradorServices = new AdministradorServicos(context);

        AdministradorServices.Incluir(Adm);
        var buscar = AdministradorServices.BuscaPorId(Adm.Id);

        if(buscar != null) Assert.AreEqual(1,buscar.Id);

        else Assert.Fail("adm não encotrado");

    }
}