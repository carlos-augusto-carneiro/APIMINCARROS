using APIMINIMADIO.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class VeiculosTest
{
    [TestMethod]
    public void TestarGetSetVeiculos()
    {
        var TestVeiculos = new Veiculo();

        TestVeiculos.Id = 1;
        TestVeiculos.Nome = "Onix";
        TestVeiculos.Marca = "chevrolet";
        TestVeiculos.Ano = 2017;

        Assert.AreEqual(1, TestVeiculos.Id);
        Assert.AreEqual("Onix", TestVeiculos.Nome);
        Assert.AreEqual("chevrolet", TestVeiculos.Marca);
        Assert.AreEqual(2017, TestVeiculos.Ano);
    }
}