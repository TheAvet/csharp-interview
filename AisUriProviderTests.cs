using NUnit.Framework;
using System.Linq;

[TestFixture]
public class AisUriProviderTests
{
    [Test]
    public void Get_Returns10Uris()
    {
        var provider = new AisUriProvider();
        var uris = provider.Get();
        Assert.AreEqual(10, uris.Count());
    }
}
