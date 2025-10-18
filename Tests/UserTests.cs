using Xunit;
using App;

namespace HealthCareSystem.Tests
{
    public class UserTests
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesAndHashPassword()
        {
            // Arrange
            var user = new User(1, "alice", "secret123", Role.Patient);

            // Act & Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("alice", user.Username);
            Assert.NotEmpty(user.PasswordHash);
            Assert.NotEmpty(user.PasswordSalt);
            Assert.Equal(Role.Patient, user.GetRole());
            Assert.Equal(Registration.Pending, user.GetRegistration()); // Patient ska börja som Pending
        }

        [Fact]
        public void TryLogin_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            var user = new User(2, "bob", "mypassword", Role.Admin);

            var result = user.TryLogin("bob", "mypassword");

            Assert.True(result);
        }

        [Fact]
        public void TryLogin_ShouldReturnFalse_WhenPasswordIsIncorrect()
        {
            var user = new User(3, "charlie", "topsecret", Role.Personnel);

            var result = user.TryLogin("charlie", "wrongpassword");

            Assert.False(result);
        }

        [Fact]
        public void AcceptPending_ShouldSetRegistrationToAccepted()
        {
            var user = new User(4, "diana", "pass", Role.Patient);

            user.AcceptPending();

            Assert.Equal(Registration.Accepted, user.GetRegistration());
        }

        [Fact]
        public void DenyPending_ShouldSetRegistrationToDenied()
        {
            var user = new User(5, "erik", "pass", Role.Patient);

            user.DenyPending();

            Assert.Equal(Registration.Denied, user.GetRegistration());
        }

        [Fact]
        public void GrantPermission_ShouldAddPermission()
        {
            var user = new User(6, "frida", "pass", Role.Admin);

            user.GrantPermission(Permissions.AddLocation);

            Assert.Contains(Permissions.AddLocation, user.PermissionList);
        }

        [Fact]
        public void RevokePermission_ShouldRemovePermission_AndFallbackToNone()
        {
            var user = new User(7, "gustav", "pass", Role.Admin);
            user.GrantPermission(Permissions.AddAdmin);

            user.RevokePermission(Permissions.AddAdmin);

            Assert.DoesNotContain(Permissions.AddAdmin, user.PermissionList);
            Assert.Contains(Permissions.None, user.PermissionList);
        }

        [Fact]
        public void HasPermission_ByName_ShouldReturnTrue_WhenPermissionExists()
        {
            var user = new User(8, "hanna", "pass", Role.Admin);
            user.GrantPermission(Permissions.ViewPermissions);

            Assert.True(user.HasPermission("ViewPermissions"));
        }

        [Fact]
        public void HasPermission_ByEnum_ShouldReturnFalse_WhenPermissionNotGranted()
        {
            var user = new User(9, "ivan", "pass", Role.Admin);

            Assert.False(user.HasPermission(Permissions.AddPersonell));
        }
    }
}
