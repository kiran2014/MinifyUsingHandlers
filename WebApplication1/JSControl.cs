using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Linq;


namespace WebApplication1
{
    [DefaultProperty("Scripts")]
    [ParseChildren(true, "Scripts")]
    public class JSControl : LiteralControl
    {
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<Script> Scripts { get; set; }

        public JSControl()
        {
            Scripts = new List<Script>();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //if (HttpContext.Current.IsDebuggingEnabled)
            //{
            //    const string format = "<script src='scripts/{0}'></script>";

            //    foreach (Script script in Scripts)
            //        writer.Write(format, script.File);
            //}
            //else
            //{
                IEnumerable<string> scriptsArray = Scripts.Select(s => s.File);
                string scripts = String.Join(",", scriptsArray.ToArray());
                string version = "1.0";
                string format = String.Format("<script src='scripts/JsHandler.ashx?jsfiles={0}&version={1}'></script>", scripts, version);
                writer.Write(format);
            //}
        }
    }

    public class Script
    {
        public string File { get; set; }
    }
}
