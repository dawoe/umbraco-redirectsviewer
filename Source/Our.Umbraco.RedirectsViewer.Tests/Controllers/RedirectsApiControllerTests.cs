namespace Our.Umbraco.RedirectsViewer.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
    using Our.Umbraco.RedirectsViewer.Models;

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
        /// The localize text service mock.
        /// </summary>
        private Mock<ILocalizedTextService> localizeTextServiceMock;

        /// <summary>
        /// The controller.
        /// </summary>
        private RedirectsApiController controller;

        /// <summary>
        /// Gets the create redirect invalid input test data.
        /// </summary>
        public IEnumerable<TestCaseData> CreateRedirectInvalidInputTestData
        {
            get
            {
                yield return new TestCaseData(new RedirectSave { Url = "http://foo"}).SetName("Create redirect with content key not set");
                yield return new TestCaseData(new RedirectSave {ContentKey = Guid.NewGuid()}).SetName("Create redirect with url not set");
            }
        }

        /// <summary>
        /// Gets the create redirect invalid url test data.
        /// </summary>
        public IEnumerable<TestCaseData> CreateRedirectInvalidUrlTestData
        {
            get
            {                
                yield return new TestCaseData(new RedirectSave { ContentKey = Guid.NewGuid(), Url = "http://foo"}, "redirectsviewer/urlRelativeError").SetName("Create redirect with http:// in url");
                yield return new TestCaseData(new RedirectSave { ContentKey = Guid.NewGuid(), Url = "image.jpg" }, "redirectsviewer/urlNoDotsError").SetName("Create redirect with . in url");
                yield return new TestCaseData(new RedirectSave { ContentKey = Guid.NewGuid(), Url = "image jpg" }, "redirectsviewer/urlNoSpacesError").SetName("Create redirect with spaces in url");               
            }
        }

        /// <summary>
        /// Initialize test
        /// </summary>
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            this.redirectUrlServiceMock = new Mock<IRedirectUrlService>();
            this.mappingEngineMock = new Mock<IMappingEngine>();
            this.localizeTextServiceMock = new Mock<ILocalizedTextService>();
         
            SettingsForTests.ConfigureSettings(SettingsForTests.GenerateMockSettings());
          


            var umbracoContext = this.GetUmbracoContext("http://localhost", -1, new RouteData(), false);

            this.controller = new RedirectsApiController(umbracoContext, this.redirectUrlServiceMock.Object, this.mappingEngineMock.Object, this.Logger, this.localizeTextServiceMock.Object)
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
            this.localizeTextServiceMock = null;

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

        /// <summary>
        /// The delete redirect should return conflict when url tracking is disabled.
        /// </summary>
        [Test]
        public void DeleteRedirectShouldReturnConflictWhenUrlTrackingIsDisabled()
        {
            // arrange
            var guid = Guid.NewGuid();

            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(true);

            this.redirectUrlServiceMock.Setup(x => x.Delete(guid));

            this.localizeTextServiceMock.Setup(
                x => x.Localize(It.IsAny<string>(), It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>()));                            

            // act
            var result = this.controller.DeleteRedirect(guid);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.Delete(guid), Times.Never);
            this.localizeTextServiceMock.Verify(
                x => x.Localize(It.IsAny<string>(), It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>()), Times.Never);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.Conflict, result.StatusCode);
        }

        /// <summary>
        /// The delete redirect should return error responset when delete fails.
        /// </summary>
        [Test]
        public void DeleteRedirectShouldReturnErrorResponsetWhenDeleteFails()
        {
            // arrange
            var guid = Guid.NewGuid();
            var msgKey = "redirectsviewer/deleteError";

            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(false);

            this.redirectUrlServiceMock.Setup(x => x.Delete(guid)).Throws(new Exception("Error during delete of redirect"));
            this.localizeTextServiceMock.Setup(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>())).Returns(msgKey);

            // act
            var result = this.controller.DeleteRedirect(guid);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.Delete(guid), Times.Once);
            this.localizeTextServiceMock.Verify(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>()), Times.Once);


            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<SimpleNotificationModel>(content.Value);

            Assert.AreEqual(msgKey, ((SimpleNotificationModel)content.Value).Message);
        }

        /// <summary>
        /// The delete redirect should return success responset when delete succeeds.
        /// </summary>
        [Test]
        public void DeleteRedirectShouldReturnSuccessResponsetWhenDeleteSucceeds()
        {
            // arrange
            var guid = Guid.NewGuid();
            var msgKey = "redirectsviewer/deleteSuccess";

            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(false);

            this.localizeTextServiceMock.Setup(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>())).Returns(msgKey);

            this.redirectUrlServiceMock.Setup(x => x.Delete(guid));

            // act
            var result = this.controller.DeleteRedirect(guid);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.Delete(guid), Times.Once);

            this.localizeTextServiceMock.Verify(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>()), Times.Once);


            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<SimpleNotificationModel>(content.Value);

            Assert.AreEqual(msgKey, ((SimpleNotificationModel)content.Value).Message);
        }

        /// <summary>
        /// The create redirect should return conflict when url tracking is disabled.
        /// </summary>
        [Test]
        public void CreateRedirectShouldReturnConflictWhenUrlTrackingIsDisabled()
        {
            // arrange
            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(true);

            this.redirectUrlServiceMock.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<Guid>()));

            // act
            var result = this.controller.CreateRedirect(new RedirectSave());

            // assert
            this.redirectUrlServiceMock.Verify(x => x.Register(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.Conflict, result.StatusCode);
        }

        /// <summary>
        /// The create redirect should return bad requestt when input is invalid.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        [Test]
        [TestCaseSource(nameof(CreateRedirectInvalidInputTestData))]
        public void CreateRedirectShouldReturnBadRequestWhenInputIsInvalid(RedirectSave input)
        {
            // arrange
            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(false);

            this.redirectUrlServiceMock.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<Guid>()));

            // act
            var result = this.controller.CreateRedirect(input);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.Register(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// The create redirect should return bad request with message when url is invalid.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        [Test]
        [TestCaseSource(nameof(CreateRedirectInvalidUrlTestData))]
        public void CreateRedirectShouldReturnBadRequestWithMessageWhenUrlIsInvalid(RedirectSave input, string errorMessage)
        {
            // arrange
            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(false);

            this.redirectUrlServiceMock.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<Guid>()));

            this.localizeTextServiceMock.Setup(
                x => x.Localize(errorMessage, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>())).Returns(errorMessage);

            // act
            var result = this.controller.CreateRedirect(input);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.Register(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
            this.localizeTextServiceMock.Verify(
                x => x.Localize(errorMessage, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<SimpleNotificationModel>(content.Value);

            Assert.AreEqual(errorMessage, ((SimpleNotificationModel)content.Value).Message);           
        }

        /// <summary>
        /// The create redirect should return bad request when redirect with url exists.
        /// </summary>
        [Test]
        public void CreateRedirectShouldReturnBadRequestWhenRedirectWithUrlExists()
        {
            // arrange
            var input = new RedirectSave
                            {
                                Url = "/foo",
                                ContentKey = Guid.NewGuid()
                            };

            var msgKey = "redirectsviewer/urlExistsError";

            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(false);

            this.localizeTextServiceMock.Setup(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>())).Returns(msgKey);

            var redirectMock = new Mock<IRedirectUrl>();
            redirectMock.SetupGet(x => x.Url).Returns("/foo");

            long total;
            this.redirectUrlServiceMock.Setup(x => x.GetAllRedirectUrls(0, int.MaxValue, out total)).Returns(new List<IRedirectUrl>
                                                                                                                 {
                                                                                                                     redirectMock.Object
                                                                                                                 });

            this.redirectUrlServiceMock.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<Guid>()));

            // act
            var result = this.controller.CreateRedirect(input);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.Register(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);

            this.localizeTextServiceMock.Verify(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<SimpleNotificationModel>(content.Value);

            Assert.AreEqual(msgKey, ((SimpleNotificationModel)content.Value).Message);
        }

        /// <summary>
        /// The create redirect should return error responset when create fails.
        /// </summary>
        [Test]
        public void CreateRedirectShouldReturnErrorResponsetWhenCreateFails()
        {
            // arrange
            var input = new RedirectSave
                            {
                                Url = "/foo",
                                ContentKey = Guid.NewGuid()
                            };

            var msgKey = "redirectsviewer/createError";

            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(false);

            long total;
            this.redirectUrlServiceMock.Setup(x => x.GetAllRedirectUrls(0, int.MaxValue, out total)).Returns(new List<IRedirectUrl>()); 

            this.redirectUrlServiceMock.Setup(x => x.Register(input.Url, input.ContentKey)).Throws(new Exception("Error during creating of redirect"));

            this.localizeTextServiceMock.Setup(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>())).Returns(msgKey);

            // act
            var result = this.controller.CreateRedirect(input);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.GetAllRedirectUrls(0, int.MaxValue, out total), Times.Once);
            this.redirectUrlServiceMock.Verify(x => x.Register(input.Url, input.ContentKey), Times.Once);
            this.localizeTextServiceMock.Verify(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<SimpleNotificationModel>(content.Value);

            Assert.AreEqual(msgKey, ((SimpleNotificationModel)content.Value).Message);
        }

        /// <summary>
        /// The create redirect should return success responset when create succeeds.
        /// </summary>
        [Test]
        public void CreateRedirectShouldReturnSuccessResponsetWhenCreateSucceeds()
        {
            // arrange
            var input = new RedirectSave
                            {
                                Url = "/foo",
                                ContentKey = Guid.NewGuid()
                            };

            var msgKey = "redirectsviewer/createSuccess";

            // disable it in config
            Mock.Get(UmbracoConfig.For.UmbracoSettings().WebRouting).SetupGet(x => x.DisableRedirectUrlTracking).Returns(false);

            var redirectMock = new Mock<IRedirectUrl>();
            redirectMock.SetupGet(x => x.Url).Returns("/bar");

            long total;
            this.redirectUrlServiceMock.Setup(x => x.GetAllRedirectUrls(0, int.MaxValue, out total)).Returns(new List<IRedirectUrl>
                                                                                                                 {
                                                                                                                     redirectMock.Object
                                                                                                                 });

            this.redirectUrlServiceMock.Setup(x => x.Register(input.Url, input.ContentKey));

            this.localizeTextServiceMock.Setup(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>())).Returns(msgKey);

            // act
            var result = this.controller.CreateRedirect(input);

            // assert
            this.redirectUrlServiceMock.Verify(x => x.GetAllRedirectUrls(0, int.MaxValue, out total), Times.Once);
            this.redirectUrlServiceMock.Verify(x => x.Register(input.Url, input.ContentKey), Times.Once);
            this.localizeTextServiceMock.Verify(
                x => x.Localize(msgKey, It.IsAny<CultureInfo>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            Assert.IsNotNull(result);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var content = (ObjectContent)result.Content;

            Assert.IsNotNull(content.Value);
            Assert.IsInstanceOf<SimpleNotificationModel>(content.Value);

            Assert.AreEqual(msgKey, ((SimpleNotificationModel)content.Value).Message);
        }
    }
}
