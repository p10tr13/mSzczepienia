
namespace mSzczepienia.Tests.Integration.Fixtures;

[CollectionDefinition("Vaccines Controller Collection")]
public class VaccinesTestCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and never creates an instance of itself.
    // It's just used to define and name the test collection.
}
