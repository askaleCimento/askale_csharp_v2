using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public sealed class HRExpenseReport : BaseBLL<AskalePortal.Data.Models.HRExpenseTable>
        {
            private const string ReportFileName = "HRExpenseDetail.rdl";
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;

            public HRExpenseReport(IConfiguration configuration, IWebHostEnvironment env)
                : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }

            public byte[] CreatePdf(int tripId)
            {
                AskalePortal.Data.Models.HRExpenseTripTable? trip = dal.dB.HRExpenseTripTable
                    .AsNoTracking()
                    .Include(item => item.user)
                        .ThenInclude(user => user.company)
                    .Include(item => item.destinationLocation)
                    .Include(item => item.tripDescriptionNavigation)
                    .SingleOrDefault(item => item.Id == tripId && item.enabled);

                if (trip == null)
                {
                    throw new KeyNotFoundException($"{tripId} numaralı görev bulunamadı.");
                }

                List<AskalePortal.Data.Models.HRExpenseTable> expenses = dal.Get(item => item.tripId == tripId && item.enabled)
                    .AsNoTracking()
                    .Include(item => item.expenseType)
                    .Include(item => item.createdUser)
                    .Include(item => item.currentUser)
                    .OrderBy(item => item.spendingTime)
                    .ThenBy(item => item.Id)
                    .ToList();

                if (expenses.Count == 0)
                {
                    throw new KeyNotFoundException($"{tripId} numaralı göreve ait harcama bulunamadı.");
                }

                string department = trip.user.departmanId.HasValue
                    ? dal.dB.HRDepartmanTable
                        .AsNoTracking()
                        .Where(item => item.Id == trip.user.departmanId.Value && item.enabled)
                        .Select(item => item.departmanAdi)
                        .FirstOrDefault() ?? string.Empty
                    : string.Empty;

                List<ExpenseReportRow> reportRows = BuildReportRows(expenses, trip);
                DataTable reportData = CreateReportDataTable();
                foreach (ExpenseReportRow row in reportRows)
                {
                    reportData.Rows.Add(
                        trip.Id,
                        trip.user.name ?? string.Empty,
                        DbValue(trip.gidisTarihi),
                        DbValue(trip.donusTarihi),
                        GetDestination(trip),
                        GetTripDescription(trip),
                        GetStatus(trip.approval),
                        trip.user.company?.companyTitle ?? string.Empty,
                        department,
                        trip.user.plstx ?? string.Empty,
                        FirstNotEmpty(trip.user.orgtx, trip.user.btext, trip.user.sstxt),
                        GetTripDayCount(trip),
                        row.ExpenseType,
                        row.GroupMode,
                        row.SpendingTime,
                        row.RecordCount,
                        row.DayCount,
                        row.DailyLimitBreakdown,
                        row.DayCountBreakdown,
                        row.AmountBreakdown,
                        row.DescriptionBreakdown,
                        row.ClaimedAmount,
                        row.ApprovedAmount,
                        row.ExpenseLimit,
                        row.DifferenceAmount,
                        row.Description,
                        row.CreatedBy,
                        row.DocumentInfo,
                        row.ProcessStatus,
                        row.ProcessColor,
                        row.AmountColor);
                }

                using LocalReport localReport = new LocalReport
                {
                    ReportPath = ResolveReportPath()
                };

                localReport.DataSources.Add(new ReportDataSource("DataSet3", reportData));
                localReport.DataSources.Add(new ReportDataSource(
                    "ApprovalDataSet",
                    CreateApprovalDataTable(tripId)));
                return localReport.Render("PDF");
            }

            private static List<ExpenseReportRow> BuildReportRows(
                List<AskalePortal.Data.Models.HRExpenseTable> expenses,
                AskalePortal.Data.Models.HRExpenseTripTable trip)
            {
                return expenses
                    .GroupBy(expense => expense.expenseType?.toplamaNo == true
                        && expense.spendingTime.HasValue
                            ? $"date:{expense.expenseTypeId}:{expense.spendingTime.Value:yyyyMMdd}"
                            : $"expense:{expense.Id}")
                    .Select(group =>
                    {
                        AskalePortal.Data.Models.HRExpenseTable first = group
                            .OrderBy(item => item.Id)
                            .First();
                        bool isDateGrouped = first.expenseType?.toplamaNo == true;
                        decimal claimedAmount = group.Sum(item => item.amount);
                        decimal expenseLimit = group.Max(item => item.totalLimitAmount ?? 0m);

                        // toplamaNo türlerinde aynı tarih grubunun onaylanan toplamı
                        // gruptaki her kayda yazılır. Sum kullanmak tutarı katlar.
                        decimal approvedAmount = isDateGrouped
                            ? group.Max(item => item.approvedAmount)
                            : first.approvedAmount;

                        List<AskalePortal.Data.Models.HRExpenseTable> orderedItems = group
                            .OrderBy(item => item.Id)
                            .ToList();

                        string processStatus;
                        string processColor;
                        AskalePortal.Data.Models.HRExpenseTable? pending =
                            group.FirstOrDefault(item => item.currentStateId == 1);
                        AskalePortal.Data.Models.HRExpenseTable? rejected =
                            group.FirstOrDefault(item => item.approval == false);

                        if (pending != null)
                        {
                            string currentUser = pending.currentUser?.name ?? string.Empty;
                            processStatus = string.IsNullOrWhiteSpace(currentUser)
                                ? "Onayda bekliyor"
                                : $"{currentUser} onayında bekliyor";
                            processColor = "#EAF4FB";
                        }
                        else if (rejected != null)
                        {
                            string currentUser = rejected.currentUser?.name ?? string.Empty;
                            processStatus = string.IsNullOrWhiteSpace(currentUser)
                                ? "Reddedildi"
                                : $"{currentUser} tarafından reddedildi";
                            processColor = "#FDECEC";
                        }
                        else
                        {
                            processStatus = "Bitti";
                            processColor = "#EAF7EF";
                        }

                        List<string> descriptions = group
                            .Select(item => CleanText(item.expenseDescription))
                            .Where(item => !string.IsNullOrWhiteSpace(item))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        List<string> createdBy = group
                            .Select(item => item.createdUser?.name?.Trim())
                            .Where(item => !string.IsNullOrWhiteSpace(item))
                            .Select(item => item!)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        List<string> documents = group
                            .SelectMany(item => (item.fileNames ?? string.Empty)
                                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        return new ExpenseReportRow
                        {
                            SortId = first.Id,
                            ExpenseType = first.expenseType?.expenseTypeName ?? string.Empty,
                            GroupMode = isDateGrouped
                                ? "Aynı tarih kayıtları birlikte hesaplanır"
                                : "Kayıt bazında hesaplanır",
                            SpendingTime = first.spendingTime
                                ?? trip.gidisTarihi
                                ?? first.createdDate
                                ?? trip.createdDate,
                            RecordCount = group.Count(),
                            DayCount = group.Max(item => item.kalinanGunSayisi),
                            DailyLimitBreakdown = string.Join(
                                Environment.NewLine,
                                orderedItems.Select(item => GetDailyLimit(item).ToString(
                                    "N2", CultureInfo.GetCultureInfo("tr-TR")) + " TL")),
                            DayCountBreakdown = string.Join(
                                Environment.NewLine,
                                orderedItems.Select(item => item.kalinanGunSayisi.ToString(
                                    CultureInfo.GetCultureInfo("tr-TR")))),
                            AmountBreakdown = string.Join(
                                Environment.NewLine,
                                orderedItems.Select(item => item.amount.ToString(
                                        "N2", CultureInfo.GetCultureInfo("tr-TR")) + " TL")),
                            DescriptionBreakdown = string.Join(
                                Environment.NewLine,
                                orderedItems.Select(item =>
                                {
                                    string value = CleanText(item.expenseDescription);
                                    return string.IsNullOrWhiteSpace(value) ? "-" : value;
                                })),
                            ClaimedAmount = claimedAmount,
                            ApprovedAmount = approvedAmount,
                            ExpenseLimit = expenseLimit,
                            DifferenceAmount = Math.Abs(expenseLimit - approvedAmount),
                            Description = string.Join("; ", descriptions),
                            CreatedBy = string.Join(", ", createdBy),
                            DocumentInfo = documents.Count == 0
                                ? "Belge yok"
                                : documents.Count == 1
                                    ? $"1 belge: {documents[0]}"
                                    : $"{documents.Count} belge: {string.Join(", ", documents)}",
                            ProcessStatus = processStatus,
                            ProcessColor = processColor,
                            AmountColor = expenseLimit > 0m && claimedAmount > expenseLimit
                                ? "#FDECEC"
                                : expenseLimit > 0m
                                    ? "#EAF7EF"
                                    : "#F8FAFC"
                        };
                    })
                    .OrderBy(item => item.ExpenseType, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(item => item.SpendingTime)
                    .ThenBy(item => item.SortId)
                    .ToList();
            }

            private static decimal GetDailyLimit(
                AskalePortal.Data.Models.HRExpenseTable expense)
            {
                decimal totalLimit = expense.totalLimitAmount ?? 0m;
                int dayCount = Math.Max(1, expense.kalinanGunSayisi);
                return totalLimit / dayCount;
            }

            private DataTable CreateApprovalDataTable(int tripId)
            {
                DataTable table = new DataTable("ApprovalDataSet");
                table.Columns.Add("Photo", typeof(byte[]));
                table.Columns.Add("PhotoMimeType", typeof(string));
                table.Columns.Add("UserName", typeof(string));
                table.Columns.Add("StatusText", typeof(string));
                table.Columns.Add("StatusColor", typeof(string));

                List<AskalePortal.Data.Models.HRExpenseDetail> approvals =
                    dal.dB.HRExpenseDetail
                        .AsNoTracking()
                        .Include(item => item.user)
                            .ThenInclude(user => user.company)
                        .Include(item => item.vekaletUser)
                        .Where(item => item.tripId == tripId && item.enabled)
                        .OrderBy(item => item.createdDate)
                        .ThenBy(item => item.Id)
                        .ToList();

                foreach (AskalePortal.Data.Models.HRExpenseDetail approval in approvals)
                {
                    (string statusText, string statusColor) = GetApprovalStatus(approval);
                    (byte[]? photo, string photoMimeType) = ReadUserPhoto(approval.user?.imageUrl);

                    DataRow row = table.NewRow();
                    row["Photo"] = photo == null ? DBNull.Value : photo;
                    row["PhotoMimeType"] = photoMimeType;
                    string approverName = string.IsNullOrWhiteSpace(approval.user?.company?.vtext)
                        ? approval.user?.name ?? string.Empty
                        : $"{approval.user?.name} ({approval.user.company.vtext})";
                    if (approval.vekaletUser != null)
                    {
                        approverName += $" - Vekaleten: {approval.vekaletUser.name}";
                    }

                    row["UserName"] = approverName;
                    row["StatusText"] = statusText;
                    row["StatusColor"] = statusColor;
                    table.Rows.Add(row);
                }

                return table;
            }

            private static (string StatusText, string StatusColor) GetApprovalStatus(
                AskalePortal.Data.Models.HRExpenseDetail approval)
            {
                if (!approval.isReplied || !approval.approved.HasValue)
                {
                    return ("ONAYI BEKLENİYOR...", "#D9EDF7");
                }

                string date = approval.replyDate?.ToString("dd.MM.yyyy HH:mm") ?? string.Empty;
                return approval.approved.Value
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
                    Path.Combine(_env.ContentRootPath, "Raporlar", ReportFileName),
                    Path.Combine(_env.ContentRootPath, "AskalePortal.BLL", "Raporlar", ReportFileName),
                    Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "AskalePortal.BLL", "Raporlar", ReportFileName)),
                    Path.Combine(AppContext.BaseDirectory, "Raporlar", ReportFileName),
                    Path.Combine(AppContext.BaseDirectory, "AskalePortal.BLL", "Raporlar", ReportFileName)
                };

                string? reportPath = candidates.FirstOrDefault(File.Exists);
                if (reportPath == null)
                {
                    throw new FileNotFoundException(
                        $"{ReportFileName} rapor dosyası bulunamadı. Kontrol edilen yollar: {string.Join("; ", candidates)}");
                }

                return reportPath;
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

            private static object DbValue<T>(T? value) where T : struct
            {
                return value.HasValue ? value.Value : DBNull.Value;
            }

            private static string GetDestination(AskalePortal.Data.Models.HRExpenseTripTable trip)
            {
                return !string.IsNullOrWhiteSpace(trip.digerDestination)
                    ? trip.digerDestination
                    : trip.destinationLocation?.destinationLocation ?? string.Empty;
            }

            private static string GetTripDescription(AskalePortal.Data.Models.HRExpenseTripTable trip)
            {
                return !string.IsNullOrWhiteSpace(trip.tripDescription)
                    ? trip.tripDescription
                    : trip.tripDescriptionNavigation?.tripDescription ?? string.Empty;
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

            private static int GetTripDayCount(
                AskalePortal.Data.Models.HRExpenseTripTable trip)
            {
                if (!trip.gidisTarihi.HasValue || !trip.donusTarihi.HasValue)
                {
                    return 1;
                }

                return Math.Max(1, (trip.donusTarihi.Value.Date -
                    trip.gidisTarihi.Value.Date).Days);
            }

            private static string FirstNotEmpty(params string?[] values)
            {
                return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
            }

            private static string CleanText(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return string.Empty;
                }

                string withoutHtml = Regex.Replace(value, "<[^>]+>", " ");
                string decoded = WebUtility.HtmlDecode(withoutHtml).Replace('\u00A0', ' ');
                return Regex.Replace(decoded, @"\s+", " ").Trim();
            }

            private sealed class ExpenseReportRow
            {
                public int SortId { get; init; }
                public string ExpenseType { get; init; } = string.Empty;
                public string GroupMode { get; init; } = string.Empty;
                public DateTime SpendingTime { get; init; }
                public int RecordCount { get; init; }
                public int DayCount { get; init; }
                public string DailyLimitBreakdown { get; init; } = string.Empty;
                public string DayCountBreakdown { get; init; } = string.Empty;
                public string AmountBreakdown { get; init; } = string.Empty;
                public string DescriptionBreakdown { get; init; } = string.Empty;
                public decimal ClaimedAmount { get; init; }
                public decimal ApprovedAmount { get; init; }
                public decimal ExpenseLimit { get; init; }
                public decimal DifferenceAmount { get; init; }
                public string Description { get; init; } = string.Empty;
                public string CreatedBy { get; init; } = string.Empty;
                public string DocumentInfo { get; init; } = string.Empty;
                public string ProcessStatus { get; init; } = string.Empty;
                public string ProcessColor { get; init; } = string.Empty;
                public string AmountColor { get; init; } = string.Empty;
            }
        }
    }
}
