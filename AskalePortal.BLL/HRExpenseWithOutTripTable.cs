using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class HRExpenseWithOutTripTable : BaseBLL<AskalePortal.Data.Models.HRExpenseWithOutTripTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public HRExpenseWithOutTripTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            private static (int PageNumber, int PageSize) GetPaging(int? page, int? size)
            {
                int pageNumber = Math.Max(page ?? 0, 0);
                int pageSize = Math.Clamp(size ?? 20, 1, 200);
                return (pageNumber, pageSize);
            }

            private static int GetRequestUserId(int? requestUserId, int? legacyUserId = null)
            {
                int userId = requestUserId.GetValueOrDefault() > 0
                    ? requestUserId!.Value
                    : legacyUserId.GetValueOrDefault();

                return userId > 0 ? userId : 0;
            }

            private static DateTime? ParseOptionalDate(string? value, string parameterName)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                string[] formats = ["yyyy-MM-dd", "dd.MM.yyyy", "dd-MM-yyyy"];
                if (DateTime.TryParseExact(
                    value.Trim(),
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime exactDate))
                {
                    return exactDate.Date;
                }

                if (DateTime.TryParse(
                    value.Trim(),
                    CultureInfo.GetCultureInfo("tr-TR"),
                    DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    return parsedDate.Date;
                }

                throw new ArgumentException(
                    $"Geçersiz tarih değeri: '{value}'.",
                    parameterName);
            }
            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetUserTrip(int userId, int activePage, int pageSize)
            {
                (activePage, pageSize) = GetPaging(activePage, pageSize);
                var q = dal.Get(u => u.userId == userId && u.approval.HasValue && u.enabled)
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetAll(int activePage, int pageSize)
            {
                (activePage, pageSize) = GetPaging(activePage, pageSize);
                var q = dal.Get(u => u.approval.HasValue && u.enabled)
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetLast(int userId)
            {
                return dal.Get(u => u.userId == userId && !u.approval.HasValue && u.enabled)
                    .AsNoTracking()
                    .OrderByDescending(u => u.createdDate)
                    .ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetUserTrip(int userId)
            {
                return dal.Get(u => u.userId == userId && u.enabled)
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id)
                    .ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetAllTamam(int activePage, int pageSize)
            {
                (activePage, pageSize) = GetPaging(activePage, pageSize);
                var q = dal.Get(u => u.enabled)
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetUserTripTamam(int userId, int activePage, int pageSize)
            {
                (activePage, pageSize) = GetPaging(activePage, pageSize);
                var q = dal.Get(u => u.userId == userId && u.enabled)
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetAll(string name, int? destinationLocationGidis, int? destinationLocationDonus, string gidisTarihi, string donusTarihi, string aciklama, int activePage, int pageSize)
            {
                (activePage, pageSize) = GetPaging(activePage, pageSize);
                DateTime? gidis = ParseOptionalDate(gidisTarihi, nameof(gidisTarihi));
                DateTime? donus = ParseOptionalDate(donusTarihi, nameof(donusTarihi));
                string? normalizedName = string.IsNullOrWhiteSpace(name)
                    ? null
                    : name.Trim().ToLower();
                string? normalizedDescription = string.IsNullOrWhiteSpace(aciklama)
                    ? null
                    : aciklama.Trim();

                return dal.Get(u =>
                        u.enabled &&
                        (normalizedName == null || u.user.name.ToLower().Contains(normalizedName)) &&
                        (!destinationLocationGidis.HasValue || u.destinationLocationId == destinationLocationGidis.Value) &&
                        (!destinationLocationDonus.HasValue || u.tripDescriptionId == destinationLocationDonus.Value) &&
                        (!gidis.HasValue || u.gidisTarihi == gidis.Value) &&
                        (!donus.HasValue || u.donusTarihi == donus.Value) &&
                        (normalizedDescription == null || u.tripDesciption.Contains(normalizedDescription)))
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetUserTrip(int userId, string name, int? destinationLocationGidis, int? destinationLocationDonus, string gidisTarihi, string donusTarihi, string aciklama, int activePage, int pageSize)
            {
                (activePage, pageSize) = GetPaging(activePage, pageSize);
                DateTime? gidis = ParseOptionalDate(gidisTarihi, nameof(gidisTarihi));
                DateTime? donus = ParseOptionalDate(donusTarihi, nameof(donusTarihi));
                string? normalizedName = string.IsNullOrWhiteSpace(name)
                    ? null
                    : name.Trim().ToLower();
                string? normalizedDescription = string.IsNullOrWhiteSpace(aciklama)
                    ? null
                    : aciklama.Trim();

                return dal.Get(u =>
                        u.enabled &&
                        u.userId == userId &&
                        u.approval.HasValue &&
                        (normalizedName == null || u.user.name.ToLower().Contains(normalizedName)) &&
                        (!destinationLocationGidis.HasValue || u.destinationLocationId == destinationLocationGidis.Value) &&
                        (!destinationLocationDonus.HasValue || u.tripDescriptionId == destinationLocationDonus.Value) &&
                        (!gidis.HasValue || u.gidisTarihi == gidis.Value) &&
                        (!donus.HasValue || u.donusTarihi == donus.Value) &&
                        (normalizedDescription == null || u.tripDesciption.Contains(normalizedDescription)))
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public List<Data.Models.HRExpenseWithOutTripTable> getFinishedForExpense(int userId)
            {
                List<Data.Models.HRExpenseWithOutTripTable> liste = dal.Get(u =>
                        u.enabled &&
                        u.userId == userId &&
                        u.approval == null &&
                        u.lastApproved == true)
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id)
                    .ToList();
                return liste;
            }

            public PageReturn<Data.Models.HRExpenseWithOutTripTable> listCompleted(
         FilterPageParam<HRExpenseWithOutTripTableDtoParameter> filterPageParam)
            {
                var result = new PageReturn<Data.Models.HRExpenseWithOutTripTable>();

                ArgumentNullException.ThrowIfNull(filterPageParam);
                (int pageNumber, int pageSize) = GetPaging(filterPageParam.page, filterPageParam.size);

                var f = filterPageParam.liste;
                bool hasExplicitRequestUser = filterPageParam.userId is > 0;
                int requestUserId = GetRequestUserId(
                    filterPageParam.userId,
                    f?.filterUserId);

                var bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                var user = bllAdminUsers.GetByID(requestUserId);

                if (user == null)
                    return result;

                var bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                var roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(
                    user.roleId,
                    (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL
                );

                IQueryable<Data.Models.HRExpenseWithOutTripTable> query =
                     dal.Get(u => u.enabled && u.approval == true)
                        .AsNoTracking();


                if (user.roleId != 1 && roleDetail?.canSeeLogs == true)
                {
                    var bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);
                    var role = bllRoles.GetByID(user.roleId);

                    var companyCodes = role?.companies?
                        .Replace("[", "")
                        .Replace("]", "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .ToList() ?? new();

                    var companyIds = dal.dB.Company
                        .AsNoTracking()
                        .Where(company => companyCodes.Contains(company.vkorg))
                        .Select(company => company.Id)
                        .ToList();

                    query = query.Where(u => companyIds.Contains(u.user.companyId));
                }
                else if (user.roleId != 1)
                {
                    query = query.Where(u => u.userId == requestUserId);
                }

                if (hasExplicitRequestUser && f?.filterUserId is > 0)
                    query = query.Where(u => u.userId == f.filterUserId.Value);

                if (!string.IsNullOrWhiteSpace(f?.filterName))
                {
                    string filterName = f.filterName.Trim();
                    query = query.Where(u => u.user.name.Contains(filterName));
                }

                if (!string.IsNullOrWhiteSpace(f?.filterUsername))
                {
                    string filterUsername = f.filterUsername.Trim();
                    query = query.Where(u => u.user.username.Contains(filterUsername));
                }

                if (f?.filterGidisTarihi != null)
                    query = query.Where(u => u.gidisTarihi == f.filterGidisTarihi);

                if (f?.filterDonusTarihi != null)
                    query = query.Where(u => u.donusTarihi == f.filterDonusTarihi);

                if (f?.filterGidisYeriId is > 0)
                    query = query.Where(u => u.destinationLocationId == f.filterGidisYeriId);


                result.totalElements = query.Count();

                result.content = query
                    .OrderByDescending(x => x.Id)
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .ToList();

                result.number = result.content.Count;
                result.size = pageSize;

                return result;
            }
            public PageReturn<HRExpenseTripDto> mylist(FilterPageParam<DieselPriceListDtoParameter> filterPageParam)
            {
                PageReturn<HRExpenseTripDto> result = new PageReturn<HRExpenseTripDto>();

                ArgumentNullException.ThrowIfNull(filterPageParam);
                (int pageNumber, int pageSize) = GetPaging(filterPageParam.page, filterPageParam.size);
                int userId = GetRequestUserId(
                    filterPageParam.userId,
                    filterPageParam.liste?.filterUser);

                if (userId == 0)
                    return result;

                var query = dal.dB.HRExpenseWithOutTripTable
                    .AsNoTracking()
                    .Where(a =>
                        a.enabled &&
                        a.HRExpenseWithOutTable.Any(b =>
                            b.currentStateId == 1 &&
                            b.currentUserId == userId &&
                            b.enabled));

                result.totalElements = query.Count();

                result.content = query
                    .OrderByDescending(u => u.Id)
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .Select(a => new HRExpenseTripDto
                    {
                        id = a.Id,
                        description = a.tripDesciption,
                        destination = a.destinationLocation.destinationLocation,
                        donusTarihi = a.donusTarihi.HasValue
                            ? a.donusTarihi.Value.ToString("dd.MM.yyyy")
                            : null,
                        gidisTarihi = a.gidisTarihi.HasValue
                            ? a.gidisTarihi.Value.ToString("dd.MM.yyyy")
                            : null,
                        kisi = a.user.name,
                        userId = a.userId,
                        whereareyou = a.destinationLocation.destinationLocation
                    })
                    .ToList();

                result.number = result.content.Count;
                result.size = pageSize;

                return result;
            }
            public PageReturn<HRExpenseTripDto> mylistAprovalStatus(FilterPageParam<HRExpenseWitOutTripTableMyListParameter> filterPageParam)
            {
                var result = new PageReturn<HRExpenseTripDto>();
                ArgumentNullException.ThrowIfNull(filterPageParam);
                (int pageNumber, int pageSize) = GetPaging(filterPageParam.page, filterPageParam.size);

                var filters = filterPageParam.liste;
                int requestUserId = GetRequestUserId(
                    filterPageParam.userId,
                    filters?.userId);

                var bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(requestUserId);
                if (user == null)
                    return result;

                var bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(
                    user.roleId,
                    (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);
                bool canSeeAll = user.roleId == 1 || roleDetail?.canSeeLogs == true;

                IQueryable<Data.Models.HRExpenseWithOutTripTable> query = dal.Get(a =>
                        a.enabled &&
                        a.HRExpenseWithOutTable.Any(b =>
                            b.currentStateId == 1 &&
                            b.enabled &&
                            (canSeeAll || b.createdUserId == requestUserId)))
                    .AsNoTracking();

                if (filters?.filterUserId is > 0)
                    query = query.Where(a => a.userId == filters.filterUserId.Value);

                if (filters?.filterDestination is > 0)
                    query = query.Where(a => a.destinationLocationId == filters.filterDestination.Value);

                if (filters?.gidisTarihi != null)
                    query = query.Where(a => a.gidisTarihi == filters.gidisTarihi.Value.Date);

                if (filters?.donusTarihi != null)
                    query = query.Where(a => a.donusTarihi == filters.donusTarihi.Value.Date);

                result.totalElements = query.Count();

                result.content = query
                    .OrderByDescending(u => u.Id)
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .Select(u => new HRExpenseTripDto
                    {
                        description = u.tripDesciption,
                        destination = u.destinationLocation.destinationLocation,
                        donusTarihi = u.donusTarihi.HasValue
                            ? u.donusTarihi.Value.ToString("dd.MM.yyyy")
                            : null,
                        gidisTarihi = u.gidisTarihi.HasValue
                            ? u.gidisTarihi.Value.ToString("dd.MM.yyyy")
                            : null,
                        id = u.Id,
                        kisi = u.user.name,
                        userId = u.userId,
                        whereareyou = u.destinationLocation.destinationLocation
                    })
                    .ToList();

                result.number = result.content.Count;
                result.size = pageSize;
                return result;
            }

            public PageReturn<HRExpenseWithOutTripTableSaveDto> listPageable(FilterPageParam<HRExpenseWithOutTripTableFilterDtoRequest> filterPageParam)
            {
                var result = new PageReturn<HRExpenseWithOutTripTableSaveDto>();
                ArgumentNullException.ThrowIfNull(filterPageParam);
                (int pageNumber, int pageSize) = GetPaging(filterPageParam.page, filterPageParam.size);

                var filters = filterPageParam.liste;
                bool hasExplicitRequestUser = filterPageParam.userId is > 0;
                int requestUserId = GetRequestUserId(
                    filterPageParam.userId,
                    filters?.filterUserId);

                var bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(requestUserId);
                if (user == null)
                    return result;

                IQueryable<Data.Models.HRExpenseWithOutTripTable> query =
                    dal.Get(u => u.enabled).AsNoTracking();

                if (user.roleId != 1)
                    query = query.Where(u => u.userId == requestUserId);

                if (hasExplicitRequestUser && filters?.filterUserId is > 0)
                    query = query.Where(u => u.userId == filters.filterUserId.Value);

                if (!string.IsNullOrWhiteSpace(filters?.filterName))
                {
                    string name = filters.filterName.Trim();
                    query = query.Where(u => u.user.name.Contains(name));
                }

                if (!string.IsNullOrWhiteSpace(filters?.filterUsername))
                {
                    string username = filters.filterUsername.Trim();
                    query = query.Where(u => u.user.username.Contains(username));
                }

                query = query.OrderByDescending(u => u.Id);

                result.content = query
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)

                    .Select(u => new HRExpenseWithOutTripTableSaveDto()
                    {
                        approval = u.approval,
                        enabled = u.enabled,
                        createdDate = u.createdDate.ToString(),
                        createdUserId = u.createdUserId,
                        destinationLocationId = u.destinationLocationId,
                        digerDestination = u.digerDestination,
                        donusTarihi = u.donusTarihi.HasValue
                            ? u.donusTarihi.Value.ToString("dd.MM.yyyy")
                            : null,
                        gidisTarihi = u.gidisTarihi.HasValue
                            ? u.gidisTarihi.Value.ToString("dd.MM.yyyy")
                            : null,
                        id = u.Id,
                        lastApproved = u.lastApproved,
                        onaySirasi = u.onaySirasi,
                        tripDescription = u.tripDescription,
                        tripDescriptionId = u.tripDescriptionId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,
                        userId = u.userId

                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count;
                result.size = pageSize;

                return result;
            }

            public PageReturn<HRExpenseTripDto> activelist(FilterPageParam<HRExpenseWithOutTripTableActiveListDtoRequest> filterPageParam)
            {
                var result = new PageReturn<HRExpenseTripDto>();
                ArgumentNullException.ThrowIfNull(filterPageParam);
                (int pageNumber, int pageSize) = GetPaging(filterPageParam.page, filterPageParam.size);

                var filters = filterPageParam.liste;
                int requestUserId = GetRequestUserId(
                    filterPageParam.userId,
                    filters?.userId);
                DateTime? gidisTarihi = ParseOptionalDate(
                    filters?.filterGidisTarihi,
                    nameof(filters.filterGidisTarihi));
                DateTime? donusTarihi = ParseOptionalDate(
                    filters?.filterDonusTarihi,
                    nameof(filters.filterDonusTarihi));

                var bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(requestUserId);
                if (user == null)
                    return result;

                var bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(
                    user.roleId,
                    (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);
                bool canSeeAll = user.roleId == 1 || roleDetail?.canSeeLogs == true;

                IQueryable<Data.Models.HRExpenseWithOutTripTable> query = dal.Get(a =>
                        a.enabled &&
                        a.HRExpenseWithOutTable.Any(b =>
                            b.currentStateId == 1 &&
                            b.enabled &&
                            (canSeeAll || b.createdUserId == requestUserId)))
                    .AsNoTracking();

                if (filters?.filterUserId is > 0)
                    query = query.Where(a => a.userId == filters.filterUserId.Value);

                if (filters?.filterDestination is > 0)
                    query = query.Where(a => a.destinationLocationId == filters.filterDestination.Value);

                if (gidisTarihi.HasValue)
                    query = query.Where(a => a.gidisTarihi == gidisTarihi.Value);

                if (donusTarihi.HasValue)
                    query = query.Where(a => a.donusTarihi == donusTarihi.Value);

                result.totalElements = query.Count();
                result.content = query
                    .OrderByDescending(a => a.Id)
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .Select(a => new HRExpenseTripDto
                    {
                        description = a.tripDesciption,
                        destination = a.destinationLocation.destinationLocation,
                        donusTarihi = a.donusTarihi.HasValue
                            ? a.donusTarihi.Value.ToString("dd.MM.yyyy")
                            : null,
                        gidisTarihi = a.gidisTarihi.HasValue
                            ? a.gidisTarihi.Value.ToString("dd.MM.yyyy")
                            : null,
                        id = a.Id,
                        kisi = a.user.name,
                        userId = a.userId,
                        whereareyou = a.destinationLocation.destinationLocation
                    })
                    .ToList();

                result.number = result.content.Count;
                result.size = pageSize;
                return result;
            }
        }
    }

}
