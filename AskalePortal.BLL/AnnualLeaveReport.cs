using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public sealed class AnnualLeaveReport
            : BaseBLL<AskalePortal.Data.Models.AnnualLeaveTable>
        {
            private const string ReportFileName = "YillikIzin.rdl";
            private const string ReportDataSetName = "dataSetIzin";
            private readonly IWebHostEnvironment _env;

            public AnnualLeaveReport(
                IConfiguration configuration,
                IWebHostEnvironment env)
                : base(configuration, env)
            {
                _env = env;
            }

            public ResponseByteArray CreatePdf(int annualLeaveId)
            {
                AskalePortal.Data.Models.AnnualLeaveTable? annualLeave =
                    dal.Get(item => item.Id == annualLeaveId && item.enabled)
                        .AsNoTracking()
                        .Include(item => item.user)
                        .Include(item => item.vekalet)
                        .SingleOrDefault();

                if (annualLeave == null)
                {
                    throw new KeyNotFoundException(
                        $"{annualLeaveId} numaralı yıllık izin kaydı bulunamadı.");
                }

                Dictionary<int, AskalePortal.Data.Models.AnnualLeaveDetail> approvalDetails =
                    dal.dB.AnnualLeaveDetail
                        .AsNoTracking()
                        .Where(item =>
                            item.anuId == annualLeaveId &&
                            item.enabled &&
                            item.siraNo >= 1 &&
                            item.siraNo <= 5)
                        .OrderByDescending(item => item.createdDate)
                        .AsEnumerable()
                        .GroupBy(item => item.siraNo)
                        .ToDictionary(group => group.Key, group => group.First());

                int[] approverIds = approvalDetails.Values
                    .Select(item => item.userId)
                    .Distinct()
                    .ToArray();

                Dictionary<int, string> approverNames = dal.dB.AdminUser
                    .AsNoTracking()
                    .Where(item => approverIds.Contains(item.Id))
                    .Select(item => new { item.Id, item.name })
                    .ToDictionary(
                        item => item.Id,
                        item => item.name ?? string.Empty);

                DataTable reportData = CreateReportData(
                    annualLeave,
                    approvalDetails,
                    approverNames);

                using LocalReport localReport = new LocalReport
                {
                    ReportPath = ResolveReportPath()
                };

                localReport.DataSources.Add(new ReportDataSource(
                    ReportDataSetName,
                    reportData));

                byte[] pdf = localReport.Render("PDF");

                return new ResponseByteArray
                {
                    file = pdf,
                    fileName = $"YillikIzin_{annualLeave.Id}.pdf",
                    name = "application/pdf"
                };
            }

            private static DataTable CreateReportData(
                AskalePortal.Data.Models.AnnualLeaveTable annualLeave,
                IReadOnlyDictionary<int, AskalePortal.Data.Models.AnnualLeaveDetail> approvalDetails,
                IReadOnlyDictionary<int, string> approverNames)
            {
                DataTable table = new DataTable(ReportDataSetName);
                table.Columns.Add("adsoyad", typeof(string));
                table.Columns.Add("bolumu", typeof(string));
                table.Columns.Add("isegiristarihi", typeof(string));
                table.Columns.Add("gorevunvan", typeof(string));
                table.Columns.Add("type", typeof(int));
                table.Columns.Add("daysleft", typeof(decimal));
                table.Columns.Add("requestday", typeof(decimal));
                table.Columns.Add("startdate", typeof(DateTime));
                table.Columns.Add("enddate", typeof(DateTime));
                table.Columns.Add("address", typeof(string));
                table.Columns.Add("telNo", typeof(string));
                table.Columns.Add("vekalet", typeof(string));
                table.Columns.Add("talepEden", typeof(string));
                table.Columns.Add("birimAmiri", typeof(string));
                table.Columns.Add("amirOnay", typeof(string));
                table.Columns.Add("ikilk", typeof(string));
                table.Columns.Add("ikilkOnay", typeof(string));
                table.Columns.Add("ikson", typeof(string));
                table.Columns.Add("iksonOnay", typeof(string));
                table.Columns.Add("onaylayici4", typeof(string));
                table.Columns.Add("onaylayici4Onay", typeof(string));
                table.Columns.Add("onaylayici5", typeof(string));
                table.Columns.Add("onaylayici5Onay", typeof(string));
                table.Columns.Add("personelNo", typeof(string));
                table.Columns.Add("digerAciklama", typeof(string));
                table.Columns.Add("kalanizin", typeof(decimal));

                decimal remainingLeave = annualLeave.typeId == 2
                    ? annualLeave.dayleft - annualLeave.dayRequested
                    : annualLeave.dayleft;

                table.Rows.Add(
                    annualLeave.user?.name ?? string.Empty,
                    annualLeave.departmanName ?? string.Empty,
                    annualLeave.enteredDate.ToString("dd.MM.yyyy"),
                    annualLeave.job ?? string.Empty,
                    annualLeave.typeId,
                    annualLeave.dayleft,
                    annualLeave.dayRequested,
                    annualLeave.startDate,
                    annualLeave.endDate,
                    HtmlToPlainText(annualLeave.adress),
                    annualLeave.user?.phone ?? string.Empty,
                    annualLeave.vekalet?.name ?? string.Empty,
                    annualLeave.user?.name ?? string.Empty,
                    GetApproverName(1, approvalDetails, approverNames),
                    GetApprovalDate(1, approvalDetails),
                    GetApproverName(2, approvalDetails, approverNames),
                    GetApprovalDate(2, approvalDetails),
                    GetApproverName(3, approvalDetails, approverNames),
                    GetApprovalDate(3, approvalDetails),
                    GetApproverName(4, approvalDetails, approverNames),
                    GetApprovalDate(4, approvalDetails),
                    GetApproverName(5, approvalDetails, approverNames),
                    GetApprovalDate(5, approvalDetails),
                    annualLeave.user?.perNo ?? string.Empty,
                    HtmlToPlainText(annualLeave.digerAciklama),
                    remainingLeave);

                return table;
            }

            private static string GetApproverName(
                int order,
                IReadOnlyDictionary<int, AskalePortal.Data.Models.AnnualLeaveDetail> approvalDetails,
                IReadOnlyDictionary<int, string> approverNames)
            {
                if (!approvalDetails.TryGetValue(
                        order,
                        out AskalePortal.Data.Models.AnnualLeaveDetail? detail))
                {
                    return string.Empty;
                }

                return approverNames.TryGetValue(detail.userId, out string? name)
                    ? name
                    : string.Empty;
            }

            private static string GetApprovalDate(
                int order,
                IReadOnlyDictionary<int, AskalePortal.Data.Models.AnnualLeaveDetail> approvalDetails)
            {
                if (!approvalDetails.TryGetValue(
                        order,
                        out AskalePortal.Data.Models.AnnualLeaveDetail? detail))
                {
                    return string.Empty;
                }

                if (!detail.isReplied)
                {
                    return "Onayda";
                }

                return detail.replyDate.HasValue
                    ? detail.replyDate.Value.ToString("dd.MM.yyyy") +
                      Environment.NewLine +
                      detail.replyDate.Value.ToString("HH:mm:ss")
                    : string.Empty;
            }

            private string ResolveReportPath()
            {
                string[] candidates =
                {
                    Path.Combine(
                        _env.ContentRootPath,
                        "Raporlar",
                        "YillikIzin",
                        ReportFileName),
                    Path.Combine(
                        _env.ContentRootPath,
                        "AskalePortal.BLL",
                        "Raporlar",
                        "YillikIzin",
                        ReportFileName),
                    Path.GetFullPath(Path.Combine(
                        _env.ContentRootPath,
                        "..",
                        "AskalePortal.BLL",
                        "Raporlar",
                        "YillikIzin",
                        ReportFileName)),
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Raporlar",
                        "YillikIzin",
                        ReportFileName),
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "AskalePortal.BLL",
                        "Raporlar",
                        "YillikIzin",
                        ReportFileName)
                };

                string? reportPath = candidates.FirstOrDefault(File.Exists);
                if (reportPath == null)
                {
                    throw new FileNotFoundException(
                        $"{ReportFileName} rapor dosyası bulunamadı. " +
                        $"Kontrol edilen yollar: {string.Join("; ", candidates)}");
                }

                return reportPath;
            }

            private static string HtmlToPlainText(string? html)
            {
                if (string.IsNullOrWhiteSpace(html))
                {
                    return string.Empty;
                }

                string text = Regex.Replace(
                    html,
                    @"<(script|style)\b[^>]*>.*?</\1>",
                    string.Empty,
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);

                text = Regex.Replace(
                    text,
                    @"<br\s*/?>",
                    Environment.NewLine,
                    RegexOptions.IgnoreCase);

                text = Regex.Replace(
                    text,
                    @"</(p|div|li|tr|h[1-6])\s*>",
                    Environment.NewLine,
                    RegexOptions.IgnoreCase);

                text = Regex.Replace(text, @"<[^>]+>", string.Empty);
                return WebUtility.HtmlDecode(text).Trim();
            }
        }
    }
}
