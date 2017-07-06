using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Yahoo.Yui.Compressor;


namespace WebApplication1
{
    /// <summary>
    /// Summary description for CssHandler
    /// </summary>
    public class CssHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string[] cssFiles = context.Request.QueryString["cssfiles"].Split(',');

            List<string> files = new List<string>();
            StringBuilder response = new StringBuilder();
            foreach (string cssFile in cssFiles)
            {
                if (!cssFile.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                {
                    //log custom exception
                    context.Response.StatusCode = 403;
                    return;
                }

                try
                {
                    CssCompressor cssCompressor = new CssCompressor();
                    string filePath = context.Server.MapPath(cssFile);
                    string css = File.ReadAllText(filePath);
                    string compressedCss = cssCompressor.Compress(css);
                    response.Append(compressedCss);
                }
                catch (Exception ex)
                {
                    //log exception
                    context.Response.StatusCode = 500;
                    return;
                }
            }

            context.Response.Write(response.ToString());

            string version = "1.0"; //your dynamic version number 

            context.Response.ContentType = "text/css";
            context.Response.AddFileDependencies(files.ToArray());
            HttpCachePolicy cache = context.Response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.VaryByParams["cssfiles"] = true;
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