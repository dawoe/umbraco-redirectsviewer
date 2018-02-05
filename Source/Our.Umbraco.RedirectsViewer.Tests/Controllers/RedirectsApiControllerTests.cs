namespace Our.Umbraco.RedirectsViewer.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using System.Web.Routing;

    using AutoMapper;

    using global::Umbraco.Core.Configuration;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Tests.TestHelpers;
    using global::Umbraco.Web.Models.ContentEditing;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.RedirectsViewer.Controllers;

    using umbraco;

    /// <summary>
    /// The redirects api controller tests.
    /// </summary>
    [TestFixture]
    public class RedirectsApiControllerTests : BaseRoutingTest
    {
        /// <summary>
        /// The redirect url service mock.
        /// </summary>
        private Mock<IRedirectUrlService> redirectUrlServiceMock;

        /// <summary>
        /// The mapping engine.
        /// </summary>
        private Mock<IMappingEngine> mappingEngineMock;

        /// <summary>
        /// The controller.
        /// </summary>
        private RedirectsApiController controller;

        /// <summary>
        /// Initialize test
        /// </summary>
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            this.redirectUrlServiceMock = new Mock<IRedirectUrlService>();
            this.mappingEngineMock = new Mock<IMappingEngine>();

          // Mocked settings are now necessary
            SettingsForTests.ConfigureSettings(SettingsForTests.GenerateMockSettings());

            //UmbracoConfig.For.UmbracoSettings().WebRouting.DisableRedirectUrlTracking


            var umbracoContext = this.GetUmbracoContext("http://localhost", -1, new RouteData(), false);

            this.controller = new RedirectsApiController(umbracoContext, this.redirectUrlServiceMock.Object, this.mappingEngineMock.Object)
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
            this.redirectUrlServiceMock = null;
            this.mappingEngineMock = null;

            base.TearDown();
        }

        /// <summary>
        /// Get redirects for content should return conflictt when url tracking is disabled.
        /// </summary>
        [Test]
        public void GetRedirectsForContentShouldReturnConflicttWhenUrlTrackingIsDisabled()
        {
            // arrange
            var guid = Guid.NewGuid();

            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(true);

            this.redirectUrlServiceMock.Setup(x => x.GetContentRedirectUrls(guid));

            this.mappingEngineMock.Setup(
                x => x.Map<IEnumerable<ContentRedirectUrl>>(It.IsAny<IEnumerable<IRedirectUrl>>()));

            // act
            var result = this.controller.GetRedirectsForContent(guid);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.GetContentRedirectUrls(guid), Times.Never);
            this.mappingEngineMock.Verify(
                x => x.Map<IEnumerable<ContentRedirectUrl>>(It.IsAny<IEnumerable<IRedirectUrl>>()), Times.Never);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.Conflict, result.StatusCode);
        }

        /// <summary>
        /// The get redirects for content should return result when url tracking is enabled.
        /// </summary>
        [Test]
        public void GetRedirectsForContentShouldReturnResultWhenUrlTrackingIsEnabled()
        {
            // arrange
            var guid = Guid.NewGuid();

            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(false);

            var redirects  = new List<IRedirectUrl>();

            var redirectMock = new Mock<IRedirectUrl>();
            redirectMock.SetupGet(x => x.Url).Returns("http://foo");

            redirects.Add(redirectMock.Object);

            this.redirectUrlServiceMock.Setup(x => x.GetContentRedirectUrls(guid)).Returns(redirects);

            List<IRedirectUrl> actualMappedRedirects = null;

            this.mappingEngineMock.Setup(
                x => x.Map<IEnumerable<ContentRedirectUrl>>(It.IsAny<IEnumerable<IRedirectUrl>>()))
                .Callback(
                    (object src) => { actualMappedRedirects = ((IEnumerable<IRedirectUrl>)src).ToList(); })
                .Returns(new List<ContentRedirectUrl>());

            // act
            var result = this.controller.GetRedirectsForContent(guid);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.GetContentRedirectUrls(guid), Times.Once);
            this.mappingEngineMock.Verify(
                x => x.Map<IEnumerable<ContentRedirectUrl>>(It.IsAny<IEnumerable<IRedirectUrl>>()), Times.Once);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<IEnumerable<ContentRedirectUrl>>(content.Value);

            Assert.AreEqual(redirects.Count, actualMappedRedirects.Count);
        }
    }
}
