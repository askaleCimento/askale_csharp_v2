using AskalePortal.Data.ReportDataset;
using CustomerPortal.Data.ResponseModels;
using Microsoft.Reporting.WebForms;
using System.Web;

namespace AskalePortal.Report
{
    public class Report
    {
        public ResponseByteArray AracTalepForm(List<AracTalepDataSource> listAracTalepDataSource, List<ReportParameter> listReportParameter)
        {

            ReportViewer rptViewer = new ReportViewer();

            rptViewer.LocalReport.DataSources.Add(new ReportDataSource("AracTalepTableDataset", listAracTalepDataSource));
            rptViewer.LocalReport.ReportPath = "C:\\Users\\dilek.sariyerlioglu\\source\\repos\\askaleportalccore\\AskalePortal.Report\\AracTalepTable.rdl";

            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.AsyncRendering = false;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.FullPage;

            ViewBag.ReportViewer = rptViewer;
        }
    }
}
