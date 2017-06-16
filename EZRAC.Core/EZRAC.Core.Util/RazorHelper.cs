using EZRAC.Core;
using EZRAC.Core.Caching;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.Util
{
    public static class RazorHelper
    {
        public static string ParseTemplate<T>(string templatePath, T model)
        {
            var content = Cache.Get<string>(templatePath);

            if (String.IsNullOrEmpty(content))
            {
                var path = GetAssemblyDirectory();

                var templateFullPath = Path.Combine(path, templatePath);

                content = templateFullPath.ReadTemplateContent();

                Cache.Add(templatePath, content);
            }

            return Razor.Parse(content, model);
        }

        private static string ReadTemplateContent(this string path)
        {
            string content;
            using (var reader = new StreamReader(path))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }

        private static string GetAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var dir = new DirectoryInfo(Uri.UnescapeDataString(uri.Path));
            return dir.Parent.Parent.FullName;
        }
    }
}
