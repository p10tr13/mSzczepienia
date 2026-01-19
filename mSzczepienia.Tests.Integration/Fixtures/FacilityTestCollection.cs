
namespace mSzczepienia.Tests.Integration.Fixtures;

[CollectionDefinition("Facility Controller Collection")]
public class FacilityTestCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and never creates an instance of itself.
    // It's just used to define and name the test collection.
}
