using System.Linq;
using NUnit.Framework;
using Themis.Cfg;

namespace Themis.Tests.Examples
{
    [TestFixture]
    public class AllowedValuesTests
    {
        [Test]
        public void GetAllowedValues_WhenUsedWithBelowDefinitions_ThenReturnsTwoValuesForAdminAndOneForUser( )
        {
            var service = Fluently.Configure()
                .AddRoleDefinition(new AdminRoleDefinition())
                .AddRoleDefinition(new UserRoleDefinition())
                .BuildDemandService();

            var rights = service.GetAllowedValues<OfferType>(new Admin());

            Assert.AreEqual(2, rights.Length);
            Assert.True(rights.Contains(OfferType.Internal));
            Assert.True(rights.Contains(OfferType.External));

            rights = service.GetAllowedValues<OfferType>(new User());

            Assert.AreEqual(1, rights.Length);
            Assert.True(rights.Contains(OfferType.Internal));
            Assert.False(rights.Contains(OfferType.External));
        }

        private class AdminRoleDefinition : RoleDefinitionBase<Admin>
        {
            public AdminRoleDefinition( )
            {
                ValueIsAllowed(OfferType.Internal);
                ValueIsAllowed(OfferType.External);
            }
        }

        private class UserRoleDefinition : RoleDefinitionBase<User>
        {
            public UserRoleDefinition( )
            {
                ValueIsAllowed(OfferType.Internal);
            }
        }

        private class Admin
        {
        }

        private class User
        {

        }

        private enum OfferType
        {
            Internal = 1,
            External = 2,
        }
    }
}