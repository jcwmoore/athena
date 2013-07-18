using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;

namespace Athena.Mvc.Tests
{
    [TestFixture]
    public class ImageLinkExtensionsFixture
    {
        [Test]
        public void ImageLinkTest()
        {
            var target = CreateHtmlHelper(new ViewDataDictionary());
            var res = target.ImageLink("ActionName", "test://image", "alt");
            Assert.That(res, Is.EqualTo(@"<a href=""""><img alt=""alt"" src=""test://image"" /></a>")); // Route Data not returned
        }

        [Test]
        public void ImageActionLinkTest()
        {
            var target = CreateAjaxHelper(new ViewDataDictionary());
            var res = target.ImageActionLink("test://image", "alt", "Action", null, new System.Web.Mvc.Ajax.AjaxOptions { UpdateTargetId="Target" });
            Assert.That(res, Is.EqualTo(@"<a href="""" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: &#39;Target&#39; });""><img alt=""alt"" src=""test://image"" /></a>")); // Route Data not returned
        }

        public static HtmlHelper CreateHtmlHelper(ViewDataDictionary vd)
        {
            var rd = new RouteData();
            rd.Route = new Route("controller/action", new MvcRouteHandler());
            rd.RouteHandler = new MvcRouteHandler();
            var mockViewContext = new Mock<ViewContext>(
              new ControllerContext(
                Mock.Of<HttpContextBase>(),
                rd,
                Mock.Of<ControllerBase>()),
              Mock.Of<IView>(),
              vd,
              new TempDataDictionary(),
              Mock.Of<System.IO.TextWriter>());

            var mockViewDataContainer = new Mock<IViewDataContainer>();
            mockViewDataContainer.Setup(v => v.ViewData).Returns(vd);

            return new HtmlHelper(mockViewContext.Object, mockViewDataContainer.Object);
        }

        public static AjaxHelper CreateAjaxHelper(ViewDataDictionary vd)
        {
            var rd = new RouteData();
            rd.Route = new Route("controller/action", new MvcRouteHandler());
            rd.RouteHandler = new MvcRouteHandler();
            var mockViewContext = new Mock<ViewContext>(
              new ControllerContext(
                Mock.Of<HttpContextBase>(),
                rd,
                Mock.Of<ControllerBase>()),
              Mock.Of<IView>(),
              vd,
              new TempDataDictionary(),
              Mock.Of<System.IO.TextWriter>());

            var mockViewDataContainer = new Mock<IViewDataContainer>();
            mockViewDataContainer.Setup(v => v.ViewData).Returns(vd);

            return new AjaxHelper(mockViewContext.Object, mockViewDataContainer.Object);
        }
    }


}
