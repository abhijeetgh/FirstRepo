using EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Risk.UI.Web.DocumentGenerator
{
    public interface IDocumentGenerator
    {
        Task<byte[]> GetBytesAsync(DocumentGeneratorViewModel request);

        //Task<PdfResult> GetPdfResultAsync(DocumentGeneratorViewModel model);
    }
}
