using System;
using System.Collections.Generic;
using System.IO;


namespace AskalePortal.Report1
{
    public class Class1
    {
        public ResponseByteArray AracTalepForm(List<AracTalepDataSource> listAracTalepDataSource, List<ReportParameter> listReportParameter)
        {

            // RDL dosyasının yolunu al
            string reportPath = Path.Combine(_env.ContentRootPath, "Reports", "SampleReport.rdl");

            // LocalReport nesnesi oluştur
            LocalReport localReport = new LocalReport(reportPath);

            // Parametreleri ayarla
            var parameters = new List<ReportParameter>
            {
                new ReportParameter("ReportParameter1", param1), // RDL'deki parametre adıyla eşleşmeli
                new ReportParameter("ReportParameter2", param2.ToString())
            };
            localReport.SetParameters(parameters);

            // Veri kaynağı ekle (örnek bir veri kaynağı, kendi verinizi buraya ekleyin)
            var data = new List<object> { new { Name = param1, Value = param2 } };
            localReport.AddDataSource("DataSet1", data); // DataSet adı RDL dosyanızdakiyle eşleşmeli

            // Raporu PDF olarak render et
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string[] streams = null;
            Warning[] warnings = null;

            byte[] pdfBytes = localReport.Render("PDF", null, out mimeType, out encoding, out extension, out streams, out warnings);

            // PDF dosyasını response olarak döndür
            return File(pdfBytes, "application/pdf", $"Report_{param1}.pdf");
        }
}
