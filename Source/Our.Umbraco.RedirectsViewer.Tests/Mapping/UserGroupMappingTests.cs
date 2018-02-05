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
    /// The user group mapping tests.
    /// </summary>
    [TestFixture]
    public class UserGroupMappingTests : BaseUmbracoApplicationTest
    {
        /// <summary>
        /// Initialize test
        /// </summary>
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            Mapper.Initialize(
                cfg =>
                    {
                        var mapping = new UsergroupMapping();
                        mapping.ConfigureMappings(cfg, this.ApplicationContext);
                    });
        }

        /// <summary>
        /// Teardown test
        /// </summary>
        [TearDown]
        public override void TearDown()
        {
            Mapper.Reset();
            base.TearDown();
        }

        /// <summary>
        /// Mapping configuration should be valid.
        /// </summary>
        [Test]
        public void MappingConfigurationShouldBeValid()
        {
            Mapper.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Mapping should return correct filled properties.
        /// </summary>
        [Test]
        public void MappingShouldReturnCorrectFilledProperties()
        {
            // arrange
            var userGroupMock = new Mock<IUserGroup>();

            var groupAlias = "alias";
            var groupName = "name";

            userGroupMock.SetupGet(x => x.Alias).Returns(groupAlias);
            userGroupMock.SetupGet(x => x.Name).Returns(groupName);
            
            // act
            var result = Mapper.Map<UserGroupDisplay>(userGroupMock.Object);

            // assert
            Assert.IsNotNull(result);

            Assert.AreEqual(groupAlias, result.Alias);
            Assert.AreEqual(groupName, result.Name);
        }
    }
}
