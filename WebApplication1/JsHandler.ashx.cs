using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Yahoo.Yui.Compressor;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for JsHandler
    /// </summary>
    public class JsHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string[] jsFiles = context.Request.QueryString["jsfiles"].Split(',');

            List<string> files = new List<string>();
            StringBuilder response = new StringBuilder();

            foreach (string jsFile in jsFiles)
            {
                if (!jsFile.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                {
                    //log custom exception
                    context.Response.StatusCode = 403;
                    return;
                }

                try
                {
                    JavaScriptCompressor javaScriptCompressor = new JavaScriptCompressor();
                    string filePath = context.Server.MapPath(jsFile);
                    string js = File.ReadAllText(filePath);
                    string compressedJS = javaScriptCompressor.Compress(js);
                    response.Append(compressedJS);
                }
                catch (Exception ex)
                {
                    //log exception
                    context.Response.StatusCode = 500;
                    return;
                }
            }

            context.Response.Write(response.ToString());

            string version = "1.0"; //your dynamic version number here

            context.Response.ContentType = "application/javascript";
            context.Response.AddFileDependencies(files.ToArray());
            HttpCachePolicy cache = context.Response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.VaryByParams["jsfiles"] = true;
            cache.VaryByParams["version"] = true;
            cache.SetETag(version);
            cache.SetLastModifiedFromFileDependencies();
            cache.SetMaxAge(TimeSpan.FromDays(14));
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}