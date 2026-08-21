using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Reporting.NETCore;
using System.Data;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public sealed class HRExpenseReport : BaseBLL<AskalePortal.Data.Models.HRExpenseTable>
        {
            private const string ReportFileName = "HRExpenseDetail.rdl";
            private readonly IWebHostEnvironment _env;

            public HRExpenseReport(IConfiguration configuration, IWebHostEnvironment env)
                : base(configuration, env)
            {
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

                DataTable reportData = CreateReportDataTable();
                foreach (AskalePortal.Data.Models.HRExpenseTable expense in expenses)
                {
                    reportData.Rows.Add(
                        trip.Id,
                        trip.user.name ?? string.Empty,
                        DbValue(trip.gidisTarihi),
                        DbValue(trip.donusTarihi),
                        GetDestination(trip),
                        GetTripDescription(trip),
                        expense.expenseType?.expenseTypeName ?? string.Empty,
                        expense.amount,
                        DbValue(expense.spendingTime),
                        expense.expenseDescription ?? string.Empty,
                        expense.createdUser?.name ?? string.Empty,
                        DbValue(expense.approval),
                        GetStatus(trip.approval),
                        trip.user.company?.companyTitle ?? string.Empty,
                        department,
                        expense.approvedAmount,
                        expense.totalLimitAmount ?? 0m,
                        trip.user.plstx ?? string.Empty,
                        FirstNotEmpty(trip.user.orgtx, trip.user.btext, trip.user.sstxt),
                        expense.kalinanGunSayisi);
                }

                using LocalReport localReport = new LocalReport
                {
                    ReportPath = ResolveReportPath()
                };

                localReport.DataSources.Add(new ReportDataSource("DataSet3", reportData));
                return localReport.Render("PDF");
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
                table.Columns.Add("expenseType", typeof(string));
                table.Columns.Add("amount", typeof(decimal));
                table.Columns.Add("spendingTime", typeof(DateTime));
                table.Columns.Add("expensedescription", typeof(string));
                table.Columns.Add("createdUser", typeof(string));
                table.Columns.Add("approval", typeof(bool));
                table.Columns.Add("status", typeof(string));
                table.Columns.Add("company", typeof(string));
                table.Columns.Add("Departman", typeof(string));
                table.Columns.Add("approvedamount", typeof(decimal));
                table.Columns.Add("harcamalimiti", typeof(decimal));
                table.Columns.Add("gorevtanimi", typeof(string));
                table.Columns.Add("birimi", typeof(string));
                table.Columns.Add("gunsayisi", typeof(int));
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

            private static string FirstNotEmpty(params string?[] values)
            {
                return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
            }
        }
    }
}
