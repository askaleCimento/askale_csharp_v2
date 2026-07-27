using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetUserTrip(int userId, int activePage, int pageSize)
            {
                var q = dal.Get(u => u.userId == userId && u.approval.HasValue && u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetAll(int activePage, int pageSize)
            {
                var q = dal.Get(u => u.approval.HasValue && u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetLast(int userId)
            {
                return dal.Get(u => u.userId == userId && !u.approval.HasValue && u.enabled == true).OrderByDescending(u => u.createdDate).ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetUserTrip(int userId)
            {
                return dal.Get(u => u.userId == userId && u.enabled == true).OrderByDescending(u => u.Id).ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetAllTamam(int activePage, int pageSize)
            {
                var q = dal.Get(u => u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetUserTripTamam(int userId, int activePage, int pageSize)
            {
                var q = dal.Get(u => u.userId == userId && u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetAll(string name, int? destinationLocationGidis, int? destinationLocationDonus, string gidisTarihi, string donusTarihi, string aciklama, int activePage, int pageSize)
            {
                DateTime? gidisT = null;
                if (!string.IsNullOrEmpty(gidisTarihi))
                {
                    gidisT = DateTime.Parse(gidisTarihi);
                }
                DateTime? gidisD = null;
                if (!string.IsNullOrEmpty(donusTarihi))
                {
                    gidisD = DateTime.Parse(donusTarihi);
                }
                var q = dal.Get(u => (string.IsNullOrEmpty(name) ? true : u.user.name.ToLower().Contains(name)) && (destinationLocationGidis.HasValue ? u.destinationLocationId == destinationLocationGidis : true)
                 && (destinationLocationDonus.HasValue ? u.tripDescriptionId == destinationLocationDonus : true) && (gidisT.HasValue ? u.gidisTarihi == gidisT : true)
                 && (gidisD.HasValue ? u.gidisTarihi == gidisD : true) && (string.IsNullOrEmpty(aciklama) ? true : u.tripDesciption.Contains(aciklama))
                 && (u.enabled == true)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTripTable> GetUserTrip(int userId, string name, int? destinationLocationGidis, int? destinationLocationDonus, string gidisTarihi, string donusTarihi, string aciklama, int activePage, int pageSize)
            {
                DateTime? gidisT = null;
                if (!string.IsNullOrEmpty(gidisTarihi))
                {
                    gidisT = DateTime.Parse(gidisTarihi);
                }
                DateTime? gidisD = null;
                if (!string.IsNullOrEmpty(donusTarihi))
                {
                    gidisT = DateTime.Parse(donusTarihi);
                }
                var q = dal.Get(u => (u.userId == userId) && (string.IsNullOrEmpty(name) ? true : u.user.name.ToLower().Contains(name)) && (u.userId == userId) && (u.approval == true) && (destinationLocationGidis.HasValue ? u.destinationLocationId == destinationLocationGidis : true)
                 && (destinationLocationDonus.HasValue ? u.tripDescriptionId == destinationLocationDonus : true) && (gidisT.HasValue ? u.gidisTarihi == gidisT : true)
                 && (gidisD.HasValue ? u.gidisTarihi == gidisD : true) && (string.IsNullOrEmpty(aciklama) ? true : u.tripDesciption.Contains(aciklama))
                 && (u.enabled == true)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<Data.Models.HRExpenseWithOutTripTable> getFinishedForExpense(int userId)
            {
                List<Data.Models.HRExpenseWithOutTripTable> liste = dal.Get(u => u.enabled && u.userId == userId && u.approval == null).ToList();
                return liste;
            }

            public PageReturn<Data.Models.HRExpenseWithOutTripTable> listCompleted(
         FilterPageParam<HRExpenseWithOutTripTableDtoParameter> filterPageParam)
            {
                var result = new PageReturn<Data.Models.HRExpenseWithOutTripTable>();

                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int userId = filterPageParam?.liste?.filterUserId ?? 0;
                var f = filterPageParam?.liste;

                var bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                var user = bllAdminUsers.GetByID(userId);

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


                if (user.roleId == 1)
                {
                    
                }
                else if (roleDetail?.canSeeLogs == true)
                {
                    var bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);
                    var role = bllRoles.GetByID(user.roleId);

                    var bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);

                    var companyIds = role?.companies?
                        .Replace("[", "")
                        .Replace("]", "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => bllCompanies.getByVkorgCompany(x.Trim()).Id)
                        .ToList() ?? new();

                    query = query.Where(u => companyIds.Contains(u.user.companyId));
                }
                else
                {
                    query = query.Where(u => u.userId == userId);
                }

             

                if (!string.IsNullOrWhiteSpace(f?.filterName))
                    query = query.Where(u => u.user.name.Contains(f.filterName));

                if (!string.IsNullOrWhiteSpace(f?.filterUsername))
                    query = query.Where(u => u.user.username.Contains(f.filterUsername));

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

                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? userId = filterPageParam.liste?.filterUser;

                var baseQuery =
                    from a in dal.dB.HRExpenseWithOutTripTable
                        .Include(x => x.user)
                        .Include(x => x.destinationLocation)
                    join b in dal.dB.HRExpenseWithOutTable
                        on a.Id equals b.tripId
                    where a.enabled
                          && b.currentStateId == 1
                          && b.currentUserId == userId
                          && b.enabled
                    select a ;

                var distinctQuery = baseQuery.Distinct();
                result.totalElements = distinctQuery.Count();

                var pagedData = distinctQuery
                    .OrderByDescending(u=>u.Id)
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .ToList();
                result.content = pagedData.Select(a => new HRExpenseTripDto
                {
                    id = a.Id,
                    description = a.tripDesciption,
                    destination = a.destinationLocation != null
                        ? a.destinationLocation.destinationLocation
                        : null,
                    donusTarihi = a.donusTarihi.HasValue
                        ? a.donusTarihi.Value.ToString("dd.MM.yyyy")
                        : null,
                    gidisTarihi = a.gidisTarihi.HasValue
                        ? a.gidisTarihi.Value.ToString("dd.MM.yyyy")
                        : null,
                    kisi = a.user != null ? a.user.name : null,
                    userId = a.userId,
                    whereareyou = a.destinationLocation != null
                        ? a.destinationLocation.destinationLocation
                        : null
                }).ToList();

                result.number = result.content.Count;
                result.size = pageSize;

                return result;
            }
            public PageReturn<HRExpenseTripDto> mylistAprovalStatus(FilterPageParam<HRExpenseWitOutTripTableMyListParameter> filterPageParam)
            {
                PageReturn<HRExpenseTripDto>? result = new PageReturn<HRExpenseTripDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? userId = filterPageParam?.liste?.userId;
                DateTime? gidisTarihi = filterPageParam?.liste?.gidisTarihi;
                DateTime? donusTarihi = filterPageParam?.liste?.donusTarihi;
                int? filterDestination = filterPageParam?.liste?.filterDestination;
                int? filterUserId = filterPageParam?.liste?.filterUserId;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId ?? 0);

                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user!.roleId, (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);
                IQueryable<Data.Models.HRExpenseWithOutTripTable> query;
                if (user.roleId == 1 || (roleDetail != null && roleDetail.canSeeLogs))
                {
                    query = dal.Get(a =>
    a.enabled &&
    a.HRExpenseWithOutTable.Any(b => b.currentStateId == 1 && b.enabled) &&
    (filterUserId == null || a.userId == filterUserId) &&
    (filterDestination == null || a.destinationLocationId == filterDestination) &&
    (gidisTarihi == null || a.gidisTarihi == gidisTarihi) &&
    (donusTarihi == null || a.donusTarihi == donusTarihi)
);
                }
                else
                {

                    query = dal.Get(a =>
     a.enabled &&
     a.HRExpenseWithOutTable.Any(b => b.currentStateId == 1
                                        && b.createdUserId == userId
                                        && b.enabled) &&
     (filterUserId == null || a.userId == filterUserId) &&
     (filterDestination == null || a.destinationLocationId == filterDestination) &&
     (gidisTarihi == null || a.gidisTarihi == gidisTarihi) &&
     (donusTarihi == null || a.donusTarihi == donusTarihi)
 );

                }

                result.content = query.OrderByDescending(u=>u.Id)
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new HRExpenseTripDto()
                    {
                        description = u.tripDescription,
                        destination = u.destinationLocation.destinationLocation,
                        donusTarihi = (u.donusTarihi ?? DateTime.Now).ToString("dd.MM.yyyy"),
                        gidisTarihi = (u.gidisTarihi ??DateTime.Now).ToString("dd.MM.yyyy"),
                        id = u.Id,
                        kisi = u.user.name,
                        userId = u.userId,
                        whereareyou = u.destinationLocation.destinationLocation,

                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;


                return result;

            }

            public PageReturn<HRExpenseWithOutTripTableSaveDto> listPageable(FilterPageParam<HRExpenseWithOutTripTableFilterDtoRequest> filterPageParam)
            {
                PageReturn<HRExpenseWithOutTripTableSaveDto>? result = new PageReturn<HRExpenseWithOutTripTableSaveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? name = filterPageParam.liste?.filterName;
                int? filterUserId = filterPageParam.liste?.filterUserId;
                string? filterKullaniciAdi = filterPageParam.liste?.filterUsername;


                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(filterUserId ?? 0);
                IQueryable<Data.Models.HRExpenseWithOutTripTable> query;
                if (user?.roleId == 1)
                {
                    query = dal.Get(u => u.enabled &&
                    (name == null || name == "" ? true : u.user.name.Contains(name)) &&
                    (filterKullaniciAdi == null || filterKullaniciAdi == "" ? true : u.user.username.Contains(filterKullaniciAdi))
                    ).OrderByDescending(u => u.Id);
                }
                else
                {
                    query = dal.Get(u => u.enabled &&
                   (name == null || name == "" ? true : u.user.name.Contains(name)) &&
                   (filterKullaniciAdi == null || filterKullaniciAdi == "" ? true : u.user.username.Contains(filterKullaniciAdi)) &&
                   (filterUserId == null || filterUserId == 0 ? true : u.userId == filterUserId)
                   ).OrderByDescending(u => u.Id);
                }

                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new HRExpenseWithOutTripTableSaveDto()
                    {
                        approval = u.approval,
                        enabled = u.enabled,
                        createdDate = u.createdDate.ToString(),
                        createdUserId = u.createdUserId,
                        destinationLocationId = u.destinationLocationId,
                        digerDestination = u.digerDestination,
                        donusTarihi = u.donusTarihi.ToString(),
                        gidisTarihi = u.gidisTarihi.ToString(),
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
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public PageReturn<HRExpenseTripDto> activelist(FilterPageParam<HRExpenseWithOutTripTableActiveListDtoRequest> filterPageParam)
            {
                PageReturn<HRExpenseTripDto>? result = new PageReturn<HRExpenseTripDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? userId = filterPageParam.liste?.userId;
                DateTime? gidisTarihi = filterPageParam.liste?.filterGidisTarihi != null ? DateTime.Parse(filterPageParam.liste?.filterGidisTarihi!) : null;
                DateTime? donusTarihi = filterPageParam.liste?.filterDonusTarihi != null ? DateTime.Parse(filterPageParam.liste?.filterDonusTarihi!) : null;
                int? filterDestination = filterPageParam.liste?.filterDestination;
                int? filterUserId = filterPageParam.liste?.filterUserId;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId ?? 0);

                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user!.roleId, (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);

                if (user.roleId == 1 || (roleDetail != null && roleDetail.canSeeLogs))
                {

                    var query =
                 from a in dal.dB.HRExpenseWithOutTripTable
                 join b in dal.dB.HRExpenseWithOutTable on a.Id equals b.tripId
                 where a.enabled
                       && b.currentStateId == 1
                       && b.enabled == true
                       && (filterUserId == null || a.userId == filterUserId)
                       && (filterDestination == null || a.destinationLocationId == filterDestination)
                       && (gidisTarihi == null || a.gidisTarihi == gidisTarihi)
                       && (donusTarihi == null || a.donusTarihi == donusTarihi)
                
                 select a; 

                    result.totalElements = query.Count();

                    result.content = query
                        .OrderByDescending(u=>u.Id)
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .Select(a => new HRExpenseTripDto
                        {
                            description = a.tripDesciption,
                            destination = a.destinationLocation.destinationLocation,
                            donusTarihi =(a.donusTarihi ?? DateTime.Now).ToString("dd.MM.yyyy"),
                            gidisTarihi = (a.gidisTarihi ?? DateTime.Now).ToString("dd.MM.yyyy"),
                            id = a.Id,
                            kisi = a.user.name,
                            userId = a.userId,
                            whereareyou = a.destinationLocation.destinationLocation,

                        })
                        .Distinct() // aynı trip birden fazla b kaydından dolayı tekrar etmesin
                        .ToList();

                    result.number = result.content.Count();
                    result.size = pageSize;
                }
                else
                {

                    var query =
                 from a in dal.dB.HRExpenseWithOutTripTable
                 join b in dal.dB.HRExpenseWithOutTable on a.Id equals b.tripId
                 where a.enabled
                       && b.currentStateId == 1
                       && b.enabled == true
                       && b.createdUserId == userId
                       && (filterUserId == null || a.userId == filterUserId)
                       && (filterDestination == null || a.destinationLocationId == filterDestination)
                       && (gidisTarihi == null || a.gidisTarihi == gidisTarihi)
                       && (donusTarihi == null || a.donusTarihi == donusTarihi)
                 orderby b.tripId descending
                 select a; // şimdilik entity dön

                    // toplam kayıt sayısı
                    result.totalElements = query.Count();

                    // sayfalama + DTO dönüşüm
                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .Select(a => new HRExpenseTripDto
                        {
                            description = a.tripDesciption,
                            destination = a.destinationLocation.destinationLocation,
                            donusTarihi = (a.donusTarihi ?? DateTime.Now).ToString("dd.MM.yyyy"),
                            gidisTarihi = (a.gidisTarihi ??DateTime.Now).ToString("dd.MM.yyyy"),
                            id = a.Id,
                            kisi = a.user.name,
                            userId = a.userId,
                            whereareyou = a.destinationLocation.destinationLocation,

                        })
                        .Distinct() // aynı trip birden fazla b kaydından dolayı tekrar etmesin
                        .ToList();

                    result.number = result.content.Count();
                    result.size = pageSize;
                }




                return result;
            }
        }
    }

}
