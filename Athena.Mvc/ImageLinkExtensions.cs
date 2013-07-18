using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Ajax;

namespace System.Web.Mvc
{
    public static class ImageLinkExtensions
    {

        /// <summary>
        /// A Simple ActionLink Image
        /// </summary>
        /// <param name="actionName">name of the action in controller</param>
        /// <param name="imgUrl">url of the image</param>
        /// <param name="alt">alt text of the image</param>
        /// <returns></returns>
        public static string ImageLink(this HtmlHelper helper, string actionName, string imgUrl, string alt)
        {
            return ImageLink(helper, actionName, imgUrl, alt, null, null, null);
        }

        public static string ImageActionLink(this AjaxHelper helper, string imageUrl, string altText, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            var link = helper.ActionLink("[replaceme]", actionName, routeValues, ajaxOptions);
            return link.ToString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// A Simple ActionLink Image
        /// </summary>
        /// <param name="actionName">name of the action in controller</param>
        /// <param name="imgUrl">url of the iamge</param>
        /// <param name="alt">alt text of the image</param>
        /// <returns></returns>
        public static string ImageLink(this HtmlHelper helper, string actionName, string imgUrl, string alt, object routeValues)
        {
            return ImageLink(helper, actionName, imgUrl, alt, routeValues, null, null);
        }

        /// <summary>
        /// A Simple ActionLink Image
        /// </summary>
        /// <param name="actioNName">name of the action in controller</param>
        /// <param name="imgUrl">url of the image</param>
        /// <param name="alt">alt text of the image</param>
        /// <param name="linkHtmlAttributes">attributes for the link</param>
        /// <param name="imageHtmlAttributes">attributes for the image</param>
        /// <returns></returns>
        public static string ImageLink(this HtmlHelper helper, string actionName, string imgUrl, string alt, object routeValues, object linkHtmlAttributes, object imageHtmlAttributes)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var url = urlHelper.Action(actionName, routeValues);

            //Create the link
            var linkTagBuilder = new TagBuilder("a");
            linkTagBuilder.MergeAttribute("href", url);
            linkTagBuilder.MergeAttributes(new RouteValueDictionary(linkHtmlAttributes));

            //Create image
            var imageTagBuilder = new TagBuilder("img");
            imageTagBuilder.MergeAttribute("src", urlHelper.Content(imgUrl));
            imageTagBuilder.MergeAttribute("alt", urlHelper.Content(alt));
            imageTagBuilder.MergeAttributes(new RouteValueDictionary(imageHtmlAttributes));

            //Add image to link
            linkTagBuilder.InnerHtml = imageTagBuilder.ToString(TagRenderMode.SelfClosing);

            return linkTagBuilder.ToString();
        }
    }
}
