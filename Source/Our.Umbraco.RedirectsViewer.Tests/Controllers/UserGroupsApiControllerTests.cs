namespace Our.Umbraco.RedirectsViewer.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;

    using AutoMapper;

    using global::Umbraco.Core.Models.Membership;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Tests.TestHelpers;


    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.RedirectsViewer.Controllers;
    using Our.Umbraco.RedirectsViewer.Models;
    using System.Web.Routing;

    /// <summary>
    /// The user groups api controller tests.
    /// </summary>
    [TestFixture]
    public class UserGroupsApiControllerTests : BaseRoutingTest
    {
        /// <summary>
        /// The user service mock.
        /// </summary>
        private Mock<IUserService> userServiceMock;

        /// <summary>
        /// The mapping engine.
        /// </summary>
        private Mock<IMappingEngine> mappingEngineMock;

        /// <summary>
        /// The controller.
        /// </summary>
        private UserGroupsApiController controller;

        /// <summary>
        /// Initialize test
        /// </summary>
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            this.userServiceMock = new Mock<IUserService>();
            this.mappingEngineMock = new Mock<IMappingEngine>();


            var umbracoContext = this.GetUmbracoContext("http://localhost", -1, new RouteData(), false);

            this.controller = new UserGroupsApiController(umbracoContext, this.userServiceMock.Object, this.mappingEngineMock.Object)
                                  {
                                      Request = new HttpRequestMessage
                                                    {
                                                        Properties =
                                                            {
                                                                {
                                                                    HttpPropertyKeys.HttpConfigurationKey,
                                                                    new HttpConfiguration()
                                                                }
                                                            }
                                                    }
                                  };      
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        [TearDown]
        public override void TearDown()
        {
            this.controller = null;
            this.userServiceMock = null;
            this.mappingEngineMock = null;

            base.TearDown();
        }

        /// <summary>
        /// When no user groups api should return empty collection.
        /// </summary>
        [Test]
        public void WhenNoUserGroupsApiShouldReturnEmptyCollection()
        {
            // arrange
            this.userServiceMock.Setup(x => x.GetAllUserGroups()).Returns(new List<IUserGroup>());

            this.mappingEngineMock.Setup(x => x.Map<IEnumerable<UserGroupDisplay>>(It.IsAny<IEnumerable<IUserGroup>>()));

            // act
            var result = this.controller.GetUserGroups();

            // assert
            this.userServiceMock.Verify(x => x.GetAllUserGroups(), Times.Once);
            this.mappingEngineMock.Verify(x => x.Map<IEnumerable<UserGroupDisplay>>(It.IsAny<IEnumerable<IUserGroup>>()), Times.Never);

            Assert.IsNotNull(result);        

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<IEnumerable<UserGroupDisplay>>(content.Value);

            var model = (IEnumerable<UserGroupDisplay>)content.Value;

            Assert.IsFalse(model.Any());
        }

        /// <summary>
        /// The when user groups api should return filled collection.
        /// </summary>
        [Test]
        public void WhenUserGroupsApiShouldReturnFilledCollection()
        {
            // arrange
            var editorGroup = new Mock<IUserGroup>();
            editorGroup.SetupGet(x => x.Alias).Returns("editor");

            var writerGroup = new Mock<IUserGroup>();
            writerGroup.SetupGet(x => x.Alias).Returns("writer");

            var userGroups = new List<IUserGroup> { editorGroup.Object, writerGroup.Object };
            
            this.userServiceMock.Setup(x => x.GetAllUserGroups()).Returns(userGroups);

            IEnumerable<IUserGroup> actualMappedItems = null;
            
            this.mappingEngineMock.Setup(x => x.Map<IEnumerable<UserGroupDisplay>>(It.IsAny<IEnumerable<IUserGroup>>()))
                .Callback(
                    (object src) => { actualMappedItems = (IEnumerable<IUserGroup>)src; })
                .Returns(new List<UserGroupDisplay>());

            // act
            var result = this.controller.GetUserGroups();

            // assert
            this.userServiceMock.Verify(x => x.GetAllUserGroups(), Times.Once);
            this.mappingEngineMock.Verify(x => x.Map<IEnumerable<UserGroupDisplay>>(userGroups), Times.Once);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<IEnumerable<UserGroupDisplay>>(content.Value);

           Assert.AreEqual(userGroups.Count, actualMappedItems.Count());
        }

        [Test]
        public void ApiShouldNotReturnAdminGroupInFilledCollection()
        {
            // arrange
            var editorGroup = new Mock<IUserGroup>();
            editorGroup.SetupGet(x => x.Alias).Returns("editor");

            var writerGroup = new Mock<IUserGroup>();
            writerGroup.SetupGet(x => x.Alias).Returns("writer");

            var adminGroup = new Mock<IUserGroup>();
            adminGroup.SetupGet(x => x.Alias).Returns("admin");

            var userGroups = new List<IUserGroup> { editorGroup.Object, writerGroup.Object, adminGroup.Object };

            this.userServiceMock.Setup(x => x.GetAllUserGroups()).Returns(userGroups);

            List<IUserGroup> actualMappedItems = null;

            this.mappingEngineMock.Setup(x => x.Map<IEnumerable<UserGroupDisplay>>(It.IsAny<IEnumerable<IUserGroup>>()))
                .Callback(
                    (object src) => { actualMappedItems = ((IEnumerable<IUserGroup>)src).ToList(); })
                .Returns(new List<UserGroupDisplay>());

            // act
            var result = this.controller.GetUserGroups();

            // assert
            this.userServiceMock.Verify(x => x.GetAllUserGroups(), Times.Once);
            this.mappingEngineMock.Verify(x => x.Map<IEnumerable<UserGroupDisplay>>(It.IsAny<IEnumerable<IUserGroup>>()), Times.Once);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<IEnumerable<UserGroupDisplay>>(content.Value);

            Assert.AreEqual(userGroups.Count - 1, actualMappedItems.Count);

            Assert.IsFalse(actualMappedItems.Any(x => x.Alias == "admin"));
        }
    }
}
