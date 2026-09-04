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
        public sealed class HRExpenseWithOutReport : BaseBLL<AskalePortal.Data.Models.HRExpenseWithOutTable>
        {
            private const string ReportFileName = "HRExpenseDetail.rdl";

            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;

            public HRExpenseWithOutReport(
                IConfiguration configuration,
                IWebHostEnvironment env)
                : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }

            public byte[] CreatePdf(int tripId)
            {
                AskalePortal.Data.Models.HRExpenseWithOutTripTable? trip =
                    dal.dB.HRExpenseWithOutTripTable
                        .AsNoTracking()
                        .Include(item => item.user)
                            .ThenInclude(user => user.company)
                        .Include(item => item.destinationLocation)
                        .Include(item => item.tripDescriptionNavigation)
                        .SingleOrDefault(item => item.Id == tripId && item.enabled);

                if (trip == null)
                {
                    throw new KeyNotFoundException(
                        $"{tripId} numaralı görevsiz harcama kaydı bulunamadı.");
                }

                List<AskalePortal.Data.Models.HRExpenseWithOutTable> expenses =
                    dal.Get(item => item.tripId == tripId && item.enabled)
                        .AsNoTracking()
                        .Include(item => item.expenseType)
                        .Include(item => item.createdUser)
                        .OrderBy(item => item.spendingTime)
                        .ThenBy(item => item.Id)
                        .ToList();

                if (expenses.Count == 0)
                {
                    throw new KeyNotFoundException(
                        $"{tripId} numaralı kayda ait harcama bulunamadı.");
                }

                string department = trip.user?.departmanId.HasValue == true
                    ? dal.dB.HRDepartmanTable
                        .AsNoTracking()
                        .Where(item =>
                            item.Id == trip.user.departmanId.Value &&
                            item.enabled)
                        .Select(item => item.departmanAdi)
                        .FirstOrDefault() ?? string.Empty
                    : string.Empty;

                DataTable reportData = CreateReportDataTable();

                foreach (AskalePortal.Data.Models.HRExpenseWithOutTable expense in expenses)
                {
                    decimal expenseLimit = expense.totalLimitAmount ?? 0m;
                    int dayCount = expense.kalinanGunSayisi;
                    decimal dailyLimit = dayCount > 0
                        ? expenseLimit / dayCount
                        : expenseLimit;

                    string processStatus = GetStatus(expense.approval);
                    string description = HtmlToPlainText(expense.tripDesciption);

                    DataRow row = reportData.NewRow();
                    row["tripID"] = trip.Id;
                    row["user"] = trip.user?.name ?? string.Empty;
                    row["gidisTarihi"] = DbValue(trip.gidisTarihi);
                    row["donusTarihi"] = DbValue(trip.donusTarihi);
                    row["destination"] = GetDestination(trip);
                    row["tripDesciption"] = GetTripDescription(trip);
                    row["status"] = GetStatus(trip.approval);
                    row["company"] = trip.user?.company?.companyTitle ?? string.Empty;
                    row["Departman"] = department;
                    row["gorevtanimi"] = trip.user?.plstx ?? string.Empty;
                    row["birimi"] = FirstNotEmpty(
                        trip.user?.orgtx,
                        trip.user?.btext,
                        trip.user?.sstxt);
                    row["gunsayisi"] = dayCount;

                    row["expenseType"] = expense.expenseType?.expenseTypeName ?? string.Empty;
                    row["groupMode"] = GetGroupMode(expense);
                    row["spendingTime"] = DbValue(expense.spendingTime);
                    row["recordCount"] = 1;
                    row["dayCount"] = dayCount;
                    row["dailyLimitBreakdown"] = FormatMoney(dailyLimit);
                    row["dayCountBreakdown"] = dayCount > 0 ? dayCount.ToString() : "-";
                    row["amountBreakdown"] = FormatMoney(expense.amount);
                    row["descriptionBreakdown"] = description;
                    row["claimedAmount"] = expense.amount;
                    row["approvedAmount"] = expense.approvedAmount;
                    row["expenseLimit"] = expenseLimit;
                    row["differenceAmount"] = expense.approvedAmount - expense.amount;
                    row["description"] = description;
                    row["createdBy"] = expense.createdUser?.name ?? string.Empty;
                    row["documentInfo"] = expense.fileNames ?? string.Empty;
                    row["processStatus"] = processStatus;
                    row["processColor"] = GetProcessColor(expense.approval);
                    row["amountColor"] = GetAmountColor(expense.amount, expenseLimit);

                    reportData.Rows.Add(row);
                }

                DataTable approvalData = CreateApprovalDataTable(tripId);

                using LocalReport localReport = new LocalReport
                {
                    ReportPath = ResolveReportPath()
                };

                localReport.DataSources.Add(
                    new ReportDataSource("DataSet3", reportData));
                localReport.DataSources.Add(
                    new ReportDataSource("ApprovalDataSet", approvalData));

                return localReport.Render("PDF");
            }

            private DataTable CreateApprovalDataTable(int tripId)
            {
                DataTable table = new DataTable("ApprovalDataSet");
                table.Columns.Add("Photo", typeof(byte[]));
                table.Columns.Add("PhotoMimeType", typeof(string));
                table.Columns.Add("UserName", typeof(string));
                table.Columns.Add("StatusText", typeof(string));
                table.Columns.Add("StatusColor", typeof(string));

                List<AskalePortal.Data.Models.HRExpenseWithOutDetail> approvals =
                    dal.dB.HRExpenseWithOutDetail
                        .AsNoTracking()
                        .Include(item => item.user)
                        .Include(item => item.vekaletUser)
                        .Where(item => item.tripId == tripId && item.enabled)
                        .OrderBy(item => item.createdDate)
                        .ThenBy(item => item.Id)
                        .ToList();

                foreach (AskalePortal.Data.Models.HRExpenseWithOutDetail approval in approvals)
                {
                    AskalePortal.Data.Models.AdminUser? photoUser =
                        approval.vekaletUser ?? approval.user;

                    (byte[]? photo, string mimeType) =
                        ReadUserPhoto(photoUser?.imageUrl);

                    (string statusText, string statusColor) =
                        GetApprovalStatus(approval);

                    DataRow row = table.NewRow();
                    row["Photo"] = photo == null ? DBNull.Value : photo;
                    row["PhotoMimeType"] = mimeType;
                    row["UserName"] = GetApproverName(approval);
                    row["StatusText"] = statusText;
                    row["StatusColor"] = statusColor;
                    table.Rows.Add(row);
                }

                return table;
            }

            private static DataTable CreateReportDataTable()
            {
                DataTable table = new DataTable("DataSet3");

                table.Columns.Add("tripID", typeof(int));
                table.Columns.Add("user", typeof(string));
                table.Columns.Add("gidisTarihi", typeof(DateTime));
                table.Columns.Add("donusTarihi", typeof(DateTime));
                table.Columns.Add("destination", typeof(string));
                table.Columns.Add("tripDesciption", typeof(string));
                table.Columns.Add("status", typeof(string));
                table.Columns.Add("company", typeof(string));
                table.Columns.Add("Departman", typeof(string));
                table.Columns.Add("gorevtanimi", typeof(string));
                table.Columns.Add("birimi", typeof(string));
                table.Columns.Add("gunsayisi", typeof(int));

                table.Columns.Add("expenseType", typeof(string));
                table.Columns.Add("groupMode", typeof(string));
                table.Columns.Add("spendingTime", typeof(DateTime));
                table.Columns.Add("recordCount", typeof(int));
                table.Columns.Add("dayCount", typeof(int));
                table.Columns.Add("dailyLimitBreakdown", typeof(string));
                table.Columns.Add("dayCountBreakdown", typeof(string));
                table.Columns.Add("amountBreakdown", typeof(string));
                table.Columns.Add("descriptionBreakdown", typeof(string));
                table.Columns.Add("claimedAmount", typeof(decimal));
                table.Columns.Add("approvedAmount", typeof(decimal));
                table.Columns.Add("expenseLimit", typeof(decimal));
                table.Columns.Add("differenceAmount", typeof(decimal));
                table.Columns.Add("description", typeof(string));
                table.Columns.Add("createdBy", typeof(string));
                table.Columns.Add("documentInfo", typeof(string));
                table.Columns.Add("processStatus", typeof(string));
                table.Columns.Add("processColor", typeof(string));
                table.Columns.Add("amountColor", typeof(string));

                return table;
            }

            private string ResolveReportPath()
            {
                string[] candidates =
                {
                    Path.Combine(_env.ContentRootPath, "Raporlar", ReportFileName),
                    Path.Combine(
                        _env.ContentRootPath,
                        "AskalePortal.BLL",
                        "Raporlar",
                        ReportFileName),
                    Path.GetFullPath(Path.Combine(
                        _env.ContentRootPath,
                        "..",
                        "AskalePortal.BLL",
                        "Raporlar",
                        ReportFileName)),
                    Path.Combine(AppContext.BaseDirectory, "Raporlar", ReportFileName),
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "AskalePortal.BLL",
                        "Raporlar",
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

            private (byte[]? Photo, string MimeType) ReadUserPhoto(string? fileName)
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return (null, "image/png");
                }

                string basePath;

                if (_env.EnvironmentName == "Development")
                {
                    basePath = _configuration["FilePath:local"] ?? string.Empty;
                }
                else if (_env.EnvironmentName == "Production")
                {
                    basePath = _configuration["FilePath:server"] ?? string.Empty;
                }
                else
                {
                    basePath = _configuration["FilePath:test"] ?? string.Empty;
                }

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

            private static string GetApproverName(
                AskalePortal.Data.Models.HRExpenseWithOutDetail approval)
            {
                string approver = approval.user?.name ?? string.Empty;

                if (approval.vekaletUser == null)
                {
                    return approver;
                }

                string proxy = approval.vekaletUser.name ?? string.Empty;
                return string.IsNullOrWhiteSpace(approver)
                    ? $"{proxy} (Vekaleten)"
                    : $"{approver} / Vekaleten: {proxy}";
            }

            private static (string StatusText, string StatusColor) GetApprovalStatus(
                AskalePortal.Data.Models.HRExpenseWithOutDetail approval)
            {
                if (!approval.isReplied && !approval.approved.HasValue)
                {
                    return ("ONAYI BEKLENİYOR...", "#D9EDF7");
                }

                string date = approval.replyDate?.ToString("dd.MM.yyyy HH:mm")
                    ?? string.Empty;

                return approval.approved switch
                {
                    true => ($"Onaylama Tarihi: {date}", "#DFF0D8"),
                    false => ($"Reddetme Tarihi: {date}", "#F2DEDE"),
                    null => ("YANITLANDI", "#FCF8E3")
                };
            }

            private static string GetDestination(
                AskalePortal.Data.Models.HRExpenseWithOutTripTable trip)
            {
                return !string.IsNullOrWhiteSpace(trip.digerDestination)
                    ? trip.digerDestination
                    : trip.destinationLocation?.destinationLocation ?? string.Empty;
            }

            private static string GetTripDescription(
                AskalePortal.Data.Models.HRExpenseWithOutTripTable trip)
            {
                return FirstNotEmpty(
                    trip.tripDesciption,
                    trip.tripDescription,
                    trip.tripDescriptionNavigation?.tripDescription);
            }

            private static string GetStatus(bool? approval)
            {
                return approval switch
                {
                    true => "Onaylandı",
                    false => "Red Edildi",
                    null => "Onayda"
                };
            }

            private static string GetProcessColor(bool? approval)
            {
                return approval switch
                {
                    true => "#DFF0D8",
                    false => "#F2DEDE",
                    null => "#D9EDF7"
                };
            }

            private static string GetAmountColor(decimal amount, decimal limit)
            {
                return limit > 0m && amount > limit
                    ? "#FFF4E5"
                    : "#FFFFFF";
            }

            private static string GetGroupMode(
                AskalePortal.Data.Models.HRExpenseWithOutTable expense)
            {
                if (expense.expenseType?.harcamaBoyu == true)
                {
                    return "TOTAL";
                }

                if (expense.expenseType?.toplamaNo == true)
                {
                    return "DAILY";
                }

                return "DETAIL";
            }

            private static string FormatMoney(decimal value)
            {
                return $"{value:N2} TL";
            }

            private static object DbValue<T>(T? value) where T : struct
            {
                return value.HasValue ? value.Value : DBNull.Value;
            }

            private static string FirstNotEmpty(params string?[] values)
            {
                return values.FirstOrDefault(
                    value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
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

                return text.Trim();
            }
        }
    }
}
