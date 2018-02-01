namespace Our.Umbraco.RedirectsViewer.Tests.Mapping
{
    using AutoMapper;

    using global::Umbraco.Core.Models.Membership;
    using global::Umbraco.Tests.TestHelpers;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.RedirectsViewer.Mapping;
    using Our.Umbraco.RedirectsViewer.Models;

    /// <summary>
    /// The usergroup mapping tests.
    /// </summary>
    [TestFixture]
    public class UsergroupMappingTests : BaseRoutingTest
    {
        /// <summary>
        /// Initializes the test
        /// </summary>
        [SetUp]
        public override void Initialize()
        {      
            base.Initialize();
                
            Mapper.Initialize(
                configuration =>
                    {
                        var mapping = new UsergroupMapping();
                         mapping.ConfigureMappings(configuration, this.ApplicationContext);
                    });
        }

        /// <summary>
        /// Test if mapping configuration is valid
        /// </summary>
        [Test]
        public void MappingConfigurationShouldBeValid()
        {
            Mapper.AssertConfigurationIsValid();            
        }

        /// <summary>
        /// The mapping user type object should return hydrated model.
        /// </summary>
        [Test]
        public void MappingUserTypeObjectShouldReturnHydratedModel()
        {
            // arrange
            var usertype = new Mock<IUserType>();

            var alias = "editor";
            var groupName = "Editors";

            usertype.SetupGet(x => x.Alias).Returns(alias);
            usertype.SetupGet(x => x.Name).Returns(groupName);
            
            // act
            var result = Mapper.Map<UserGroupDisplay>(usertype);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(alias, result.Alias);
            Assert.AreEqual(groupName, alias);
        }
    }
}
