using Xunit;
using App;

namespace HealthCareSystem.Tests
{
    public class AdminTests
    {
        [Fact]
        public void Admin_ShouldStartWithPendingRegistration()
        {
            var admin = new User(100, "admin1", "securepass", Role.Admin);

            Assert.Equal(Role.Admin, admin.GetRole());
            Assert.Equal(Registration.Pending, admin.GetRegistration());
        }

        [Fact]
        public void Admin_CanGrantPermission()
        {
            var admin = new User(101, "admin2", "securepass", Role.Admin);

            admin.GrantPermission(Permissions.AddLocation);

            Assert.True(admin.HasPermission(Permissions.AddLocation));
        }

        [Fact]
        public void Admin_CanRevokePermission()
        {
            var admin = new User(102, "admin3", "securepass", Role.Admin);
            admin.GrantPermission(Permissions.AddAdmin);

            admin.RevokePermission(Permissions.AddAdmin);

            Assert.False(admin.HasPermission(Permissions.AddAdmin));
            Assert.Contains(Permissions.None, admin.PermissionList);
        }

        [Fact]
        public void SuperAdmin_ShouldBeRecognizedAsRole()
        {
            var superAdmin = new User(103, "super", "superpass", Role.SuperAdmin);

            Assert.Equal(Role.SuperAdmin, superAdmin.GetRole());
        }

        [Fact]
        public void Personnel_ShouldHaveAcceptedRegistrationByDefault()
        {
            var personnel = new User(104, "nurse", "nursepass", Role.Personnel);

            Assert.Equal(Role.Personnel, personnel.GetRole());
            Assert.Equal(Registration.Accepted, personnel.GetRegistration());
        }

        [Fact]
        public void Patient_ShouldStartWithPendingRegistration()
        {
            var patient = new User(105, "patient", "patientpass", Role.Patient);

            Assert.Equal(Role.Patient, patient.GetRole());
            Assert.Equal(Registration.Pending, patient.GetRegistration());
        }
    }
}
