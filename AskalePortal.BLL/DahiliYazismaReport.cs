using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public sealed class DahiliYazismaReport
            : BaseBLL<AskalePortal.Data.Models.DahiliYazismaTable>
        {
            private const string ReportFileName = "DahiliYazisma.rdl";
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;

            public DahiliYazismaReport(
                IConfiguration configuration,
                IWebHostEnvironment env)
                : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }

            public ResponseByteArray CreatePdf(int dahiliYazismaId)
            {
                AskalePortal.Data.Models.DahiliYazismaTable? correspondence =
                    dal.Get(item => item.Id == dahiliYazismaId && item.enabled)
                        .AsNoTracking()
                        .Include(item => item.kanal)
                        .SingleOrDefault();

                if (correspondence == null)
                {
                    throw new KeyNotFoundException(
                        $"{dahiliYazismaId} numaralı dahili yazışma bulunamadı.");
                }

                DateTime reportDate = correspondence.tarih ?? correspondence.createdDate;

                using LocalReport localReport = new LocalReport
                {
                    ReportPath = ResolveReportPath()
                };

                localReport.DataSources.Add(new ReportDataSource(
                    "ApprovalDataSet",
                    CreateApprovalDataTable(correspondence)));

                localReport.SetParameters(new[]
                {
                    new ReportParameter(
                        "ReportParameter1",
                        correspondence.servisi ?? string.Empty),
                    new ReportParameter(
                        "ReportParameter2",
                        correspondence.konu ?? string.Empty),
                    new ReportParameter(
                        "ReportParameter3",
                        reportDate.ToString("dd.MM.yyyy")),
                    new ReportParameter(
                        "ReportParameter4",
                        correspondence.Id.ToString()),
                    new ReportParameter(
                        "ReportParameter5",
                        correspondence.kanal?.bolumAdi ?? string.Empty),
                    new ReportParameter(
                        "ReportParameter6",
                        HtmlToPlainText(correspondence.icerik))
                });

                byte[] pdf = localReport.Render("PDF");

                return new ResponseByteArray
                {
                    file = pdf,
                    fileName = $"DahiliYazisma_{correspondence.Id}.pdf",
                    name = "application/pdf"
                };
            }

            private DataTable CreateApprovalDataTable(
                AskalePortal.Data.Models.DahiliYazismaTable correspondence)
            {
                DataTable table = new DataTable("ApprovalDataSet");
                table.Columns.Add("Photo", typeof(byte[]));
                table.Columns.Add("PhotoMimeType", typeof(string));
                table.Columns.Add("UserName", typeof(string));
                table.Columns.Add("StatusText", typeof(string));
                table.Columns.Add("StatusColor", typeof(string));

                int? ceoUserId = dal.dB.CeoTable
                    .AsNoTracking()
                    .Where(item => item.Id == 1 && item.enabled)
                    .Select(item => (int?)item.userId)
                    .FirstOrDefault();

                List<(int UserId, bool SonOnayMi, bool IsCeo)> approvalSteps = [];

                AddApprovalStep(approvalSteps, correspondence.onaylayici1, false);
                AddApprovalStep(approvalSteps, correspondence.onaylayici2, false);
                AddApprovalStep(approvalSteps, correspondence.onaylayici3, false);
                AddApprovalStep(approvalSteps, correspondence.onaylayici4, false);
                AddApprovalStep(approvalSteps, correspondence.kanal?.userId, false);

                if (ceoUserId.HasValue)
                {
                    approvalSteps.Add((ceoUserId.Value, false, true));
                }

                AddApprovalStep(approvalSteps, correspondence.kanalGorusuUserId, true);
                AddApprovalStep(approvalSteps, correspondence.lastUserId, true);
                AddApprovalStep(approvalSteps, correspondence.lastUserId2, true);

                if (approvalSteps.Count == 0)
                {
                    return table;
                }

                int[] userIds = approvalSteps
                    .Select(item => item.UserId)
                    .Distinct()
                    .ToArray();

                Dictionary<int, AskalePortal.Data.Models.AdminUser> users = dal.dB.AdminUser
                    .AsNoTracking()
                    .Include(item => item.company)
                    .Where(item => userIds.Contains(item.Id))
                    .ToDictionary(item => item.Id);

                List<AskalePortal.Data.Models.DahiliYazismalarDetayTable> details =
                    dal.dB.DahiliYazismalarDetayTable
                        .AsNoTracking()
                        .Where(item =>
                            item.enabled &&
                            item.dahiliYazismaId == correspondence.Id &&
                            userIds.Contains(item.userId))
                        .OrderByDescending(item => item.createdDate)
                        .ToList();

                foreach ((int userId, bool sonOnayMi, bool isCeo) in approvalSteps)
                {
                    if (!users.TryGetValue(userId, out AskalePortal.Data.Models.AdminUser? user))
                    {
                        continue;
                    }

                    AskalePortal.Data.Models.DahiliYazismalarDetayTable? detail = isCeo
                        ? details.FirstOrDefault(item => item.userId == userId)
                        : details.FirstOrDefault(item =>
                            item.userId == userId &&
                            item.sonOnayMi == sonOnayMi);

                    (string statusText, string statusColor) = GetApprovalStatus(detail);
                    (byte[]? photo, string photoMimeType) = ReadUserPhoto(user.imageUrl);

                    DataRow row = table.NewRow();
                    row["Photo"] = photo == null ? DBNull.Value : photo;
                    row["PhotoMimeType"] = photoMimeType;
                    row["UserName"] = string.IsNullOrWhiteSpace(user.company?.vtext)
                        ? user.name ?? string.Empty
                        : $"{user.name} ({user.company.vtext})";
                    row["StatusText"] = statusText;
                    row["StatusColor"] = statusColor;
                    table.Rows.Add(row);
                }

                return table;
            }

            private static void AddApprovalStep(
                ICollection<(int UserId, bool SonOnayMi, bool IsCeo)> steps,
                int? userId,
                bool sonOnayMi)
            {
                if (userId.HasValue && userId.Value > 0)
                {
                    steps.Add((userId.Value, sonOnayMi, false));
                }
            }

            private static (string StatusText, string StatusColor) GetApprovalStatus(
                AskalePortal.Data.Models.DahiliYazismalarDetayTable? detail)
            {
                if (detail == null)
                {
                    return (string.Empty, "#FCF8E3");
                }

                if (!detail.approved.HasValue)
                {
                    return ("ONAYI BEKLENİYOR...", "#D9EDF7");
                }

                string date = detail.replyDate?.ToString("dd.MM.yyyy HH:mm") ?? string.Empty;
                return detail.approved.Value
                    ? ($"Onaylama Tarihi: {date}", "#DFF0D8")
                    : ($"Reddetme Tarihi: {date}", "#F2DEDE");
            }

            private (byte[]? Photo, string MimeType) ReadUserPhoto(string? fileName)
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return (null, "image/png");
                }

                string basePath = _env.IsDevelopment()
                    ? _configuration["FilePath:local"] ?? string.Empty
                    : _env.IsProduction()
                        ? _configuration["FilePath:server"] ?? string.Empty
                        : _configuration["FilePath:test"] ?? string.Empty;

                string fullPath = Path.Combine(
                    basePath,
                    "adminusers",
                    "images",
                    Path.GetFileName(fileName));

                if (!File.Exists(fullPath))
                {
                    return (null, "image/png");
                }

                string mimeType = Path.GetExtension(fullPath).ToLowerInvariant() switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    _ => "image/png"
                };

                return (File.ReadAllBytes(fullPath), mimeType);
            }

            private string ResolveReportPath()
            {
                string[] candidates =
                {
                    Path.Combine(
                        _env.ContentRootPath,
                        "Raporlar",
                        "DahiliYazisma",
                        ReportFileName),
                    Path.Combine(
                        _env.ContentRootPath,
                        "AskalePortal.BLL",
                        "Raporlar",
                        "DahiliYazisma",
                        ReportFileName),
                    Path.GetFullPath(Path.Combine(
                        _env.ContentRootPath,
                        "..",
                        "AskalePortal.BLL",
                        "Raporlar",
                        "DahiliYazisma",
                        ReportFileName)),
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Raporlar",
                        "DahiliYazisma",
                        ReportFileName),
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "AskalePortal.BLL",
                        "Raporlar",
                        "DahiliYazisma",
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
                text = WebUtility.HtmlDecode(text);
                text = Regex.Replace(
                    text,
                    @"(\r?\n){3,}",
                    Environment.NewLine + Environment.NewLine);

                return text.Trim();
            }
        }
    }
}
