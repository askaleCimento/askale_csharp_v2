using System.IO;
using System.Web.Mvc;
using GleamTech.DocumentUltimate;
using GleamTech.DocumentUltimate.Web;

namespace DocumentOpen.Controllers
{
    public class DocumentController : Controller
    {
        private static readonly string DocumentRoot = @"E:\IIS\SAP Portal\live\files";

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Open(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return new HttpStatusCodeResult(400, "filename zorunludur.");
            }

            var safeFileName = Path.GetFileName(filename);
            var fullPath = Path.Combine(DocumentRoot, safeFileName);

            if (!System.IO.File.Exists(fullPath))
            {
                return HttpNotFound("Dosya bulunamadı.");
            }

            var documentViewer = new DocumentViewer
            {
                Resizable = true,
                Document = fullPath,
                HighlightedKeywords = new[] { safeFileName },
                DownloadFileName = safeFileName
            };

            return View(documentViewer);
        }
    }
}
