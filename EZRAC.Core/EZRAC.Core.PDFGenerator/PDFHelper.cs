using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.FileGenerator
{
    public class PDFHelper
    {
        public static Byte[] PdfSharpConvertBytes(String html)
        {
            Byte[] res = null;
            using (MemoryStream ms = new MemoryStream())
            {

                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                pdf.Save(ms);
                res = ms.ToArray();
            }
            return res;
        }
    }

}
