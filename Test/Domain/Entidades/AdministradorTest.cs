using APIMINIMADIO.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetAdministrador()
    {  
        var Adm = new Administrador();

        Adm.Id = 1;
        Adm.Email = "teste@teste.com";
        Adm.Senha = "teste";
        Adm.Perfil = "adm";

        Assert.AreEqual(1,Adm.Id);
        Assert.AreEqual("teste@teste.com",Adm.Email);
        Assert.AreEqual("teste",Adm.Senha);
        Assert.AreEqual("adm",Adm.Perfil);

    }
}