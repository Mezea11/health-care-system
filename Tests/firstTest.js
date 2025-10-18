using Xunit;

namespace HealthCareSystem.Tests
{
    public class JournalServiceTests
    {
        [Fact]
        public void AddEntry_ShouldIncreaseCount()
        {
            var service = new JournalService();
            service.AddEntry("Doctor", "Patient is fine");

            Assert.Single(service.Entries);
        }
    }
}
