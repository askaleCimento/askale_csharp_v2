using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Reporting.NETCore;
using System.Data;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public sealed class HRExpenseTripReport : BaseBLL<AskalePortal.Data.Models.HRExpenseTripTable>
        {
            private const string ReportFileName = "HRExpenseTripReport.rdl";
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;

            public HRExpenseTripReport(IConfiguration configuration, IWebHostEnvironment env)
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
                    .Include(item => item.hereLocation)
                    .Include(item => item.tripDescriptionNavigation)
                    .SingleOrDefault(item => item.Id == tripId && item.enabled);

                if (trip == null)
                {
                    throw new KeyNotFoundException($"{tripId} numaralı seyahat bulunamadı.");
                }

                string department = trip.user?.departmanId.HasValue == true
                    ? dal.dB.HRDepartmanTable
                        .AsNoTracking()
                        .Where(item => item.Id == trip.user.departmanId.Value && item.enabled)
                        .Select(item => item.departmanAdi)
                        .FirstOrDefault() ?? string.Empty
                    : string.Empty;

                DataTable tripData = CreateTripDataTable();
                tripData.Rows.Add(
                    trip.Id,
                    trip.user?.name ?? string.Empty,
                    trip.user?.company?.companyTitle ?? string.Empty,
                    department,
                    trip.hereLocation?.destinationLocation ?? string.Empty,
                    GetDestination(trip),
                    DbValue(trip.gidisTarihi),
                    DbValue(trip.donusTarihi),
                    trip.avans,
                    GetTripDescription(trip));

                using LocalReport localReport = new LocalReport
                {
                    ReportPath = ResolveReportPath()
                };

                localReport.DataSources.Add(new ReportDataSource("TripDataSet", tripData));
                localReport.DataSources.Add(new ReportDataSource(
                    "ApprovalDataSet",
                    CreateApprovalDataTable(tripId)));

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

                List<AskalePortal.Data.Models.HRExpenseTripDetail> approvals =
                    dal.dB.HRExpenseTripDetail
                        .AsNoTracking()
                        .Include(item => item.user)
                            .ThenInclude(user => user.company)
                        .Where(item => item.tripId == tripId && item.enabled)
                        .OrderBy(item => item.createdDate)
                        .ThenBy(item => item.Id)
                        .ToList();

                int[] proxyUserIds = approvals
                    .Where(item => item.vekaletUserId.HasValue)
                    .Select(item => item.vekaletUserId!.Value)
                    .Distinct()
                    .ToArray();

                Dictionary<int, string> proxyUsers = dal.dB.AdminUser
                    .AsNoTracking()
                    .Where(item => proxyUserIds.Contains(item.Id))
                    .ToDictionary(item => item.Id, item => item.name ?? string.Empty);

                foreach (AskalePortal.Data.Models.HRExpenseTripDetail approval in approvals)
                {
                    (string statusText, string statusColor) = GetApprovalStatus(approval);
                    (byte[]? photo, string photoMimeType) = ReadUserPhoto(approval.user?.imageUrl);

                    string approverName = string.IsNullOrWhiteSpace(approval.user?.company?.vtext)
                        ? approval.user?.name ?? string.Empty
                        : $"{approval.user?.name} ({approval.user.company.vtext})";

                    if (approval.vekaletUserId.HasValue &&
                        proxyUsers.TryGetValue(approval.vekaletUserId.Value, out string? proxyName) &&
                        !string.IsNullOrWhiteSpace(proxyName))
                    {
                        approverName += $" - Vekaleten: {proxyName}";
                    }

                    DataRow row = table.NewRow();
                    row["Photo"] = photo == null ? DBNull.Value : photo;
                    row["PhotoMimeType"] = photoMimeType;
                    row["UserName"] = approverName;
                    row["StatusText"] = statusText;
                    row["StatusColor"] = statusColor;
                    table.Rows.Add(row);
                }

                return table;
            }

            private static (string StatusText, string StatusColor) GetApprovalStatus(
                AskalePortal.Data.Models.HRExpenseTripDetail approval)
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

            private static DataTable CreateTripDataTable()
            {
                DataTable table = new DataTable("TripDataSet");
                table.Columns.Add("tripID", typeof(int));
                table.Columns.Add("user", typeof(string));
                table.Columns.Add("company", typeof(string));
                table.Columns.Add("department", typeof(string));
                table.Columns.Add("hereLocation", typeof(string));
                table.Columns.Add("destination", typeof(string));
                table.Columns.Add("startDate", typeof(DateTime));
                table.Columns.Add("endDate", typeof(DateTime));
                table.Columns.Add("advance", typeof(decimal));
                table.Columns.Add("tripDescription", typeof(string));
                return table;
            }

            private static object DbValue<T>(T? value) where T : struct
            {
                return value.HasValue ? value.Value : DBNull.Value;
            }

            private static string GetDestination(
                AskalePortal.Data.Models.HRExpenseTripTable trip)
            {
                return !string.IsNullOrWhiteSpace(trip.digerDestination)
                    ? trip.digerDestination
                    : trip.destinationLocation?.destinationLocation ?? string.Empty;
            }

            private static string GetTripDescription(
                AskalePortal.Data.Models.HRExpenseTripTable trip)
            {
                return !string.IsNullOrWhiteSpace(trip.tripDescription)
                    ? trip.tripDescription
                    : trip.tripDescriptionNavigation?.tripDescription ?? string.Empty;
            }
        }
    }
}
