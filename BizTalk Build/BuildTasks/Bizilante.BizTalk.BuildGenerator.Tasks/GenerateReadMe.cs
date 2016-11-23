using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Web.UI;

namespace bizilante.BuildGenerator.Tasks
{
    public class GenerateReadMe : Task
    {
        /// <summary>
        /// Name of the application
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }
        /// <summary>
        /// Application version
        /// </summary>
        [Required]
        public string Version { get; set; }
        /// <summary>
        /// File path of the notes file to include
        /// </summary>
        [Required]
        public string NotesPath { get; set; }
        /// <summary>
        /// File Path of the ReadMe file
        /// (this file get's created with this task)
        /// </summary>
        [Required]
        public string ReadMePath { get; set; }

        private const string color_header = "#668040";
        private const string color_title = "#86BF30";
        private const string color_detail = "#DCE6CF";

        public override bool Execute()
        {
            try
            {
                if (string.IsNullOrEmpty(this.ApplicationName))
                    throw new ArgumentException("ApplicationName argument cannot be empty");
                if (string.IsNullOrEmpty(this.NotesPath))
                    throw new ArgumentException("NotesPath argument cannot be empty");
                if (string.IsNullOrEmpty(this.ReadMePath))
                    throw new ArgumentException("ReadMePath argument cannot be empty");

                if (!File.Exists(this.NotesPath))
                    throw new Exception(string.Format("'{0}' does not exist!", this.NotesPath));

                Log.LogMessage(MessageImportance.Normal, string.Format("Create {0}: Application={1}, Version={2}, ", ReadMePath, ApplicationName, Version));

                HtmlTextWriter writer = new HtmlTextWriter(new StreamWriter(this.ReadMePath, false));
                writer.RenderBeginTag(HtmlTextWriterTag.Html);

                writer.RenderBeginTag(HtmlTextWriterTag.Head);
                writer.RenderBeginTag(HtmlTextWriterTag.Title);
                writer.Write(ApplicationName);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Body);

                writer.AddAttribute("boder", "0");
                writer.AddAttribute("width", "500px");
                writer.RenderBeginTag(HtmlTextWriterTag.Table);

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddAttribute("bgcolor", color_header);
                writer.AddAttribute("colspan", "2");
                writer.AddAttribute("align", "center");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Application Info");
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddAttribute("width", "70px");
                writer.AddAttribute("bgcolor", color_title);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Application Name");
                writer.RenderEndTag();
                writer.AddAttribute("align", "right");
                writer.AddAttribute("bgcolor", color_detail);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(ApplicationName);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddAttribute("bgcolor", color_title);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Version");
                writer.RenderEndTag();
                writer.AddAttribute("align", "right");
                writer.AddAttribute("bgcolor", color_detail);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Version);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddAttribute("bgcolor", color_title);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Generated on");
                writer.RenderEndTag();
                writer.AddAttribute("align", "right");
                writer.AddAttribute("bgcolor", color_detail);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddAttribute("bgcolor", color_title);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Built on");
                writer.RenderEndTag();
                writer.AddAttribute("align", "right");
                writer.AddAttribute("bgcolor", color_detail);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Environment.MachineName);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddAttribute("bgcolor", color_title);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("By");
                writer.RenderEndTag();
                writer.AddAttribute("align", "right");
                writer.AddAttribute("bgcolor", color_detail);
                writer.AddAttribute("valign", "top");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Environment.UserName);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddAttribute("colspan", "2");
                writer.AddAttribute("bgcolor", color_detail);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write("Release note:");
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.I);

                // Insert the release note here
                StreamReader r = new StreamReader(NotesPath);
                writer.Write(r.ReadToEnd());
                r.Close();

                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.Close();

                return true;
            }
            catch (ArgumentException argEx)
            {
                Log.LogError(argEx.Message, new object[] { ApplicationName, Version, NotesPath, ReadMePath });
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, new object[] { ApplicationName, Version, NotesPath, ReadMePath });
            }
            return false;
        }
    }
}
