using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Azure;
using BCrypt.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Models = AskalePortal.Data.Models;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class AdminUsers : BaseBLL<AskalePortal.Data.Models.AdminUser>
        {
            private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;

            public AdminUsers(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            public List<IdandText> GetIdandText()
            {
                return dal.Get(u => u.enabled).Select(u => new IdandText() { id = u.Id, text = u.name }).ToList();
            }
            public Models.AdminUser? GetByIdWithPassive(int Id)
            {
                return dal.Get(u => u.Id == Id && u.enabled == false).FirstOrDefault();
            }
            public Models.AdminUser? GetByIdWithPassiveAndActive(int Id)
            {
                return dal.Get(u => u.Id == Id).FirstOrDefault();
            }


            public AdminUser? getUser(string username, string password)
            {

                AdminUser? adminUser = dal.Get(k => k.username == username && k.enabled).FirstOrDefault();
                if (adminUser != null &&
                    BCrypt.Net.BCrypt.Verify(password, adminUser.password))
                {
                    return adminUser;
                }
                else
                {
                    return null;
                }

            }
            #region GetByUsernamePass

            public Models.AdminUser? GetByUsernamePass(string username, string password)
            {
                var q = dal.Get(k => k.username.Equals(username) &&
                                     k.password.Equals(password) &&
                                     k.approval == true &&
                                     k.enabled == true);

                return q.FirstOrDefault();
            }

            #endregion GetByUsernamePass

            #region GetByUsername

            public Models.AdminUser? GetByUsername(string username)
            {
                var q = dal.Get(k => k.username.Equals(username) &&
                                     k.enabled == true);

                return q.FirstOrDefault();
            }

            #endregion GetByUsername

            #region GetByTCKimlik

            public Models.AdminUser? GetByTCKimlik(string tc)
            {
                var q = dal.Get(k => k.merni.Equals(tc) &&
                                     k.enabled == true);

                return q.FirstOrDefault();
            }

            public List<AskalePortal.Data.Models.AdminUser> GetByTCKimlikList(string tc)
            {
                var q = dal.Get(k => (k.merni.Contains(tc) || string.IsNullOrEmpty(tc)) &&
                                     k.enabled == true);

                return q.ToList();
            }

            #endregion GetByTCKimlik

            #region GetByBirthDay


            public List<AskalePortal.Data.Models.AdminUser> GetByBirthDay()
            {
                var dt = DateTime.Today;

                var q = dal.Get(k =>
                               k.bdate.HasValue ? k.bdate.Value.Day == dt.Day : false &&
                                k.bdate.HasValue ? k.bdate.Value.Month == dt.Month : false &&
                                k.approval == true &&
                                k.enabled == true);

                return q.ToList();
            }


            #endregion

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.AdminUser> GetAllWithPage(int? roleID, string name, string username, string email, string sapUser,
                                                         int activePage, int recordsPerPage, AdminUser a)
            {
                name = name?.Trim() ?? string.Empty;
                username = username?.Trim() ?? string.Empty;
                email = email?.Trim() ?? string.Empty;
                sapUser = sapUser?.Trim() ?? string.Empty;

                var q = dal.Get(k =>
                        k.enabled == true &&
                        (roleID == null || k.roleId == roleID) &&
                (a.roleId == 1 || a.role.companies.Contains("[" + k.company.vkorg + "]")) &&
                        (string.IsNullOrEmpty(name) || (k.name != null && k.name.Contains(name))) &&
                        (string.IsNullOrEmpty(username) || (k.username != null && k.username.Contains(username))) &&
                        (string.IsNullOrEmpty(sapUser) || (k.sapUser != null && k.sapUser.Contains(sapUser))) &&
                        (string.IsNullOrEmpty(email) || (k.email != null && k.email.Contains(email))))
                    .OrderBy(x => x.username)
                    .Skip(activePage * recordsPerPage)
                    .Take(recordsPerPage)
                    .ToList();

                return q;
            }

            public Models.AdminUser? GetAllWithPassiveId(int id)
            {
                return dal.Get(u => u.Id == id).FirstOrDefault();
            }

            public List<AskalePortal.Data.Models.AdminUser> GetAllWithPagePasif(int? roleID, string name, string username, string email,
                                                     int activePage, int recordsPerPage, AdminUser a)
            {
                name = name?.Trim() ?? string.Empty;
                username = username?.Trim() ?? string.Empty;
                email = email?.Trim() ?? string.Empty;

                var q = dal.Get(k =>
                        k.enabled == false &&
                        (roleID == null || k.roleId == roleID) &&
                (a.roleId == 1 || a.role.companies.Contains("[" + k.company.vkorg + "]")) &&
                        (string.IsNullOrEmpty(name) || (k.name != null && k.name.Contains(name))) &&
                        (string.IsNullOrEmpty(username) || (k.username != null && k.username.Contains(username))) &&
                        (string.IsNullOrEmpty(email) || (k.email != null && k.email.Contains(email))))
                    .OrderBy(x => x.username)
                    .Skip(activePage * recordsPerPage)
                    .Take(recordsPerPage)
                    .ToList();

                return q;
            }
            #endregion GetAllWithPage

            #region GetDocumentIDList

            public List<int> GetDocumentIDList(int userID)
            {
                var q = dal.Get(k => k.documentUserId.Contains("[" + userID + "]")).Select(d => d.Id);
                return q.ToList();
            }

            #endregion GetDocumentIDList

            #region GetMyPersonelIDList

            public List<int> GetMyPersonelIDList(int myUserID)
            {
                var q = dal.Get(k => k.Id == myUserID || k.manager1 == myUserID || k.manager2 == myUserID).Select(d => d.Id);
                return q.ToList();
            }

            public List<AdminUser> GetByFabrikaId(string vKORG)
            {
                return dal.Get(u => u.company.vkorg == vKORG && u.enabled == true).ToList();
            }

            public string GetByEmail(int mailUserId)
            {
                return dal.Get(u => u.Id == mailUserId && u.enabled == true).First().name;
            }


            #endregion GetMyPersonelIDList

            #region GetByNameAndUserName
            public List<AdminUser> GetByNameAndUserName(string name, string username, int activePage, int recordsPerPage)
            {
                name = name?.Trim() ?? string.Empty;
                username = username?.Trim() ?? string.Empty;

                return dal.Get(u =>
                        u.enabled == true &&
                        (string.IsNullOrEmpty(name) || (u.name != null && u.name.Contains(name))) &&
                        (string.IsNullOrEmpty(username) || (u.username != null && u.username.Contains(username))))
                    .OrderBy(u => u.name)
                    .Skip(activePage * recordsPerPage)
                    .Take(recordsPerPage)
                    .ToList();
            }

            public List<AdminUser> GetAllByID(List<int> ids)
            {
                return dal.Get(u => ids.Contains(u.Id) && u.enabled == true).ToList();
            }

            public List<AdminUser> GetAllWithPassive()
            {
                return dal.Get(u => u.enabled == true || u.enabled == false).ToList();
            }

            public PageReturn<UsersFilterDto>? FilterPageableDto(FilterPageParam<UserFilterDtoRequest> filterPageParam)
            {
                PageReturn<UsersFilterDto> result = new PageReturn<UsersFilterDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? name = filterPageParam.liste?.filterName?.Trim();
                int? filterRol = filterPageParam.liste?.filterRol;
                string? filterKullaniciAdi = filterPageParam.liste?.filterKullaniciAdi?.Trim();
                string? filterEmail = filterPageParam.liste?.filterEmail?.Trim();
                string? filterSapUser = filterPageParam.liste?.filterSapUserName?.Trim();
                int? filterCompany = filterPageParam.liste?.filterCompany;

                IQueryable<AdminUser> query = dal.Get(u =>
                        u.enabled &&
                        (string.IsNullOrEmpty(name) || (u.name != null && u.name.Contains(name))) &&
                        (!filterRol.HasValue || filterRol.Value == 0 || u.roleId == filterRol.Value) &&
                        (string.IsNullOrEmpty(filterKullaniciAdi) || (u.username != null && u.username.Contains(filterKullaniciAdi))) &&
                        (string.IsNullOrEmpty(filterEmail) || (u.email != null && u.email.Contains(filterEmail))) &&
                        (string.IsNullOrEmpty(filterSapUser) || (u.sapUser != null && u.sapUser.Contains(filterSapUser))) &&
                        (!filterCompany.HasValue || filterCompany.Value == 0 || u.companyId == filterCompany.Value))
                    .Include(u => u.company)
                    .Include(u => u.role)
                    .OrderByDescending(u => u.Id);

                result.totalElements = query.Count();
                result.content = query
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .Select(u => new UsersFilterDto()
                    {
                        companyId = u.companyId,
                        approval = u.approval,
                        email = u.email,
                        id = u.Id,
                        name = u.name,
                        role = u.role.title,
                        roleId = u.roleId,
                        sapUserName = u.sapUser,
                        userName = u.username,
                        vkorg = u.company.vkorg,
                    })
                    .ToList();


                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public UserByNameEMailDto getUserByNameEMailDto(int id)
            {
                UserByNameEMailDto dto = dal.Get(k => k.enabled && k.Id == id).Select(u => new UserByNameEMailDto()
                {

                    email = u.email,
                    id = u.Id,
                    name = u.name,
                }).First();
                return dto;
            }

            public PageReturn<AdminUserDto> listPassivePageableDto(FilterPageParam<UserFilterDtoRequest> filterPageParam)
            {
                PageReturn<AdminUserDto> result = new PageReturn<AdminUserDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? name = filterPageParam.liste?.filterName?.Trim();
                int? filterRol = filterPageParam.liste?.filterRol;
                string? filterKullaniciAdi = filterPageParam.liste?.filterKullaniciAdi?.Trim();
                string? filterEmail = filterPageParam.liste?.filterEmail?.Trim();
                string? filterSapUser = filterPageParam.liste?.filterSapUserName?.Trim();
                int? filterCompany = filterPageParam.liste?.filterCompany;

                IQueryable<AdminUser> query = dal.Get(u =>
                        u.enabled == false &&
                        (string.IsNullOrEmpty(name) || (u.name != null && u.name.Contains(name))) &&
                        (!filterRol.HasValue || filterRol.Value == 0 || u.roleId == filterRol.Value) &&
                        (string.IsNullOrEmpty(filterKullaniciAdi) || (u.username != null && u.username.Contains(filterKullaniciAdi))) &&
                        (string.IsNullOrEmpty(filterEmail) || (u.email != null && u.email.Contains(filterEmail))) &&
                        (string.IsNullOrEmpty(filterSapUser) || (u.sapUser != null && u.sapUser.Contains(filterSapUser))) &&
                        (!filterCompany.HasValue || filterCompany.Value == 0 || u.companyId == filterCompany.Value))
                    .Include(u => u.company)
                    .Include(u => u.role)
                    .OrderByDescending(u => u.Id);

                result.totalElements = query.Count();
                result.content = query
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .Select(u => new AdminUserDto()
                    {
                        companyId = u.companyId,
                        approval = u.approval,
                        email = u.email,
                        id = u.Id,
                        name = u.name,
                        role = u.role.title,
                        roleId = u.roleId,
                        sapUserName = u.sapUser,
                        userName = u.username,
                        vkorg = u.company.vkorg,
                    })
                    .ToList();


                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public List<AdminUser> listAllUser()
            {
                List<AdminUser> liste = dal.Get(k => k.enabled == true || k.enabled == false).ToList();

                return liste;
            }
            public List<UserNameCompanyDto> listAllUserDto()
            {
                List<UserNameCompanyDto> liste = dal.Get(k => k.enabled == true).OrderBy(u => u.name).Select(u => new UserNameCompanyDto()
                {
                    userId = u.Id,
                    username = u.name,
                    vtext = u.company.vtext

                }).ToList();

                return liste;
            }
            public List<AdminUser> listAllByEnabled()
            {
                List<AdminUser> liste = dal.Get(k => k.enabled == true).ToList();

                return liste;
            }


            public AdminUser findHelpDeskRoleId(int helpDeskRoleId)
            {
                AdminUser adminUser = dal.Get(k => k.helpDeskRoleId == helpDeskRoleId && k.enabled == true).First();
                return adminUser;
            }

            public List<UserByNameEMailDto> getUserByNameEMailDtoAll()
            {

                List<UserByNameEMailDto> dto = dal.Get(u => u.enabled).Select(u => new UserByNameEMailDto()
                {

                    email = u.email,
                    id = u.Id,
                    name = u.name,
                }).ToList();
                return dto;
            }

            public UserByNameEMailDto getUserByNameAndEmail(int id)
            {
                AdminUser dto = dal.Get(u => u.Id == id).First();
                UserByNameEMailDto mapper = new UserByNameEMailDto { email = dto.email, id = dto.Id, name = dto.name };
                return mapper;

            }

            public PageReturn<HrUserDto> hrUserList(FilterPageParam<HRUserListDtoParameter> filterPageParam, int userId)
            {
                PageReturn<HrUserDto>? result = new PageReturn<HrUserDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;
                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);

                AdminUser? user = GetByID(userId);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user!.roleId, (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);
                string? companies = user?.role?.companies;
                string[] listCompanies = companies?.Replace("\\[|\\]", "").Split(",") ?? [];
                string? name = filterPageParam.liste?.filterName?.Trim();
                string? filterKullaniciAdi = filterPageParam.liste?.filterUsername?.Trim();

                var query = from u in dal.Get(a =>
     a.enabled
     && (a.roleId != 1083 || (a.roleId == 1083 && (a.company.vkorg.Contains("AC20") || a.company.vkorg.Contains("AC80"))))
     && (string.IsNullOrEmpty(filterKullaniciAdi) || (a.username != null && a.username.Contains(filterKullaniciAdi)))
     && (string.IsNullOrEmpty(name) || (a.name != null && a.name.Contains(name))))

                                // Departman için join
                            join d in dal.dB.HRDepartmanTable
                                on u.departmanId equals d.Id into deptGroup
                            from dept in deptGroup.DefaultIfEmpty()

                                //Kullanıcı türü için
                            join k in dal.dB.HREmployeeType
                                on u.calisanTuruId equals k.Id into calisanTuruGroup
                            from calisanTuru in calisanTuruGroup.DefaultIfEmpty()

                                // Employer1 için join
                            join e1 in dal.dB.AdminUser
                                on u.hremployer1 equals e1.Id into emply1Group
                            from emp1 in emply1Group.DefaultIfEmpty()

                                // Manager1 için join
                            join m1 in dal.dB.AdminUser
                                on u.manager1 equals m1.Id into mgr1Group
                            from mgr1 in mgr1Group.DefaultIfEmpty()

                                // Manager2 için join
                            join m2 in dal.dB.AdminUser
                                on u.manager2 equals m2.Id into mgr2Group
                            from mgr2 in mgr2Group.DefaultIfEmpty()

                                // Manager3 için join
                            join m3 in dal.dB.AdminUser
                                on u.manager3 equals m3.Id into mgr3Group
                            from mgr3 in mgr3Group.DefaultIfEmpty()

                                // Manager4 için join
                            join m4 in dal.dB.AdminUser
                                on u.manager4 equals m4.Id into mgr4Group
                            from mgr4 in mgr4Group.DefaultIfEmpty()


                            orderby u.Id descending
                            select new HrUserDto()
                            {
                                id = u.Id,
                                name = u.name,
                                username = u.username,
                                departmanAdi = dept != null ? dept.departmanAdi : "",
                                perNo = u.perNo,
                                kullaniciTuru = calisanTuru != null ? calisanTuru.calisanTuru : "",
                                hrEmployer1name = emp1 != null ? emp1.name : "",
                                hrEmployer1change = u.hrchanger1,
                                manager1 = mgr1 != null ? mgr1.name : "",
                                manager1change = u.hrchanger2,
                                manager2 = mgr2 != null ? mgr2.name : "",
                                manager2change = u.hrchanger3,
                                manager3 = mgr3 != null ? mgr3.name : "",
                                manager3change = u.hrchanger4,
                                manager4 = mgr4 != null ? mgr4.name : "",
                                manager4change = u.hrchanger5,
                                company = u.company.vkorg
                            };
                result.content = query
    .Skip(pageSize * pageNumber)
    .Take(pageSize)
    .ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            #endregion


            public async Task<AdminUser> saveHRUser(
                AdminUserSaveDto newUserGelen,
                int userId)
            {
                AdminUser newUser =
                    _mapper.Map<AdminUser>(newUserGelen);


                // ============================================================
                // ESKİ KAYDI SADECE KARŞILAŞTIRMA İÇİN AL
                //
                // DİKKAT:
                // GetByID() kullanırsak EF Core oldUser'ı track eder.
                //
                // Daha sonra:
                //
                //     Update(newUser)
                //
                // içinde newUser Attach edilmeye çalışıldığında aynı Id'ye sahip
                // oldUser zaten track edildiği için:
                //
                // "another instance with the same key is already being tracked"
                //
                // hatası oluşur.
                //
                // Bu yüzden oldUser mutlaka AsNoTracking alınmalıdır.
                // ============================================================

                AdminUser oldUser = dal
                    .Get(x => x.Id == newUser.Id)
                    .AsNoTracking()
                    .FirstOrDefault()
                    ?? throw new InvalidOperationException(
                        $"Güncellenecek AdminUser bulunamadı. Id: {newUser.Id}");


                // ============================================================
                // HR EMPLOYER
                // ============================================================

                if (ApproverChanged(
                    oldUser.hremployer1,
                    newUser.hremployer1))
                {
                    await UpdateCommonApprovalUserAsync(
                        oldUser.Id,
                        oldUser.hremployer1!.Value,
                        newUser.hremployer1,
                        finishFlowWhenRemoved: false);
                }


                // ============================================================
                // MANAGER 1
                // ============================================================

                if (ApproverChanged(
                    oldUser.manager1,
                    newUser.manager1))
                {
                    await UpdateCommonApprovalUserAsync(
                        oldUser.Id,
                        oldUser.manager1!.Value,
                        newUser.manager1,
                        finishFlowWhenRemoved: false);
                }


                // ============================================================
                // MANAGER 2
                // ============================================================

                if (ApproverChanged(
                    oldUser.manager2,
                    newUser.manager2))
                {
                    await UpdateCommonApprovalUserAsync(
                        oldUser.Id,
                        oldUser.manager2!.Value,
                        newUser.manager2,
                        finishFlowWhenRemoved: true);
                }


                // ============================================================
                // MANAGER 3
                // ============================================================

                if (ApproverChanged(
                    oldUser.manager3,
                    newUser.manager3))
                {
                    await UpdateCommonApprovalUserAsync(
                        oldUser.Id,
                        oldUser.manager3!.Value,
                        newUser.manager3,
                        finishFlowWhenRemoved: true);
                }


                // ============================================================
                // MANAGER 4
                // ============================================================

                if (ApproverChanged(
                    oldUser.manager4,
                    newUser.manager4))
                {
                    await UpdateCommonApprovalUserAsync(
                        oldUser.Id,
                        oldUser.manager4!.Value,
                        newUser.manager4,
                        finishFlowWhenRemoved: true);
                }


                // ============================================================
                // İZİN ONAYLAYICI
                // ============================================================

                if (ApproverChanged(
                    oldUser.izinOnayId,
                    newUser.izinOnayId))
                {
                    await UpdateAnnualLeaveApproverAsync(
                        oldUser.Id,
                        oldUser.izinOnayId!.Value,
                        newUser.izinOnayId);
                }


                // ============================================================
                // ARAÇ ONAYLAYICI
                // ============================================================

                if (ApproverChanged(
                    oldUser.aracOnayId,
                    newUser.aracOnayId))
                {
                    await UpdateVehicleApproverAsync(
                        oldUser.Id,
                        oldUser.aracOnayId!.Value,
                        newUser.aracOnayId);
                }


                // Eğer AdminUser modelinde updatedUserId varsa kullanabilirsin:
                //
                // newUser.updatedUserId = userId;


                // oldUser AsNoTracking olduğu için burada artık aynı Id
                // tracking çakışması oluşmaz.
                return await Update(newUser);
            }


            // ================================================================
            // APPROVER CHANGED
            // ================================================================

            private static bool ApproverChanged(
                int? oldApproverId,
                int? newApproverId)
            {
                /*
                 * old = null, new = null
                 *      -> false
                 *
                 * old = null, new = 15
                 *      -> false
                 *      -> ilk atama
                 *
                 * old = 15, new = 15
                 *      -> false
                 *
                 * old = 15, new = 20
                 *      -> true
                 *
                 * old = 15, new = null
                 *      -> true
                 */

                return oldApproverId.HasValue &&
                       oldApproverId != newApproverId;
            }


            // ================================================================
            // ORTAK ONAYLAYICI DEĞİŞİKLİĞİ
            //
            // hremployer1
            // manager1
            // manager2
            // manager3
            // manager4
            //
            // için kullanılır.
            // ================================================================

            private async Task UpdateCommonApprovalUserAsync(
                int employeeId,
                int oldApproverId,
                int? newApproverId,
                bool finishFlowWhenRemoved)
            {
                BLLActions.HRExpenseTripTable bllHRExpenseTripTable =
                    new BLLActions.HRExpenseTripTable(
                        _configuration,
                        _env,
                        _mapper);

                BLLActions.HRExpenseTable bllHRExpenseTable =
                    new BLLActions.HRExpenseTable(
                        _configuration,
                        _env,
                        _mapper);

                BLLActions.HRExpenseDetail bllHRExpenseDetail =
                    new BLLActions.HRExpenseDetail(
                        _configuration,
                        _env);

                BLLActions.HRExpenseTripDetail bllHRExpenseTripDetail =
                    new BLLActions.HRExpenseTripDetail(
                        _configuration,
                        _env);

                BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable =
                    new BLLActions.HRExpenseWithOutTable(
                        _configuration,
                        _env,
                        _mapper);

                BLLActions.HRExpenseWithOutDetail bllHRExpenseWithOutDetail =
                    new BLLActions.HRExpenseWithOutDetail(
                        _configuration,
                        _env);

                BLLActions.AnnualLeaveTable bllAnnualLeaveTable =
                    new BLLActions.AnnualLeaveTable(
                        _configuration,
                        _env,
                        _mapper);

                BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail =
                    new BLLActions.AnnualLeaveDetail(
                        _configuration,
                        _env);

                BLLActions.RepresentativeExpenseTable
                    bllRepresentativeExpenseTable =
                        new BLLActions.RepresentativeExpenseTable(
                            _configuration,
                            _env,
                            _mapper);

                BLLActions.RepresentativeExpenseDetail
                    bllRepresentativeExpenseDetail =
                        new BLLActions.RepresentativeExpenseDetail(
                            _configuration,
                            _env);


                // ============================================================
                // HR EXPENSE TRIP TABLE
                // ============================================================

                List<Data.Models.HRExpenseTripTable> tripTables =
                    bllHRExpenseTripTable
                        .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(
                            oldApproverId,
                            1,
                            true,
                            employeeId);

                foreach (Data.Models.HRExpenseTripTable item in tripTables)
                {
                    if (newApproverId.HasValue)
                    {
                        item.currentUserId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        item.onaySirasi = 10;
                        item.currentStateId = 4;
                    }
                    else
                    {
                        item.currentUserId = 0;
                    }

                    await bllHRExpenseTripTable.Update(item);
                }


                // ============================================================
                // HR EXPENSE TRIP DETAIL
                // ============================================================

                List<Data.Models.HRExpenseTripDetail> tripDetails =
                    bllHRExpenseTripDetail
                        .findByUserIdActive(
                            oldApproverId,
                            employeeId);

                foreach (Data.Models.HRExpenseTripDetail item in tripDetails)
                {
                    if (newApproverId.HasValue)
                    {
                        /*
                         * Eski manager4 kodunda burada:
                         *
                         * createdUserId
                         *
                         * değiştiriliyordu.
                         *
                         * Diğer manager bloklarıyla aynı olacak şekilde userId
                         * kullanıyoruz.
                         */
                        item.userId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        item.enabled = false;
                    }
                    else
                    {
                        item.userId = 0;
                    }

                    await bllHRExpenseTripDetail.Update(item);
                }


                // ============================================================
                // HR EXPENSE TABLE
                // ============================================================

                List<Data.Models.HRExpenseTable> expenseTables =
                    bllHRExpenseTable
                        .findByUserIdActive(
                            oldApproverId,
                            employeeId);

                foreach (Data.Models.HRExpenseTable item in expenseTables)
                {
                    if (newApproverId.HasValue)
                    {
                        item.currentUserId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        item.onaySirasi = 10;
                        item.currentStateId = 4;
                    }
                    else
                    {
                        item.currentUserId = 0;
                    }

                    await bllHRExpenseTable.Update(item);
                }


                // ============================================================
                // HR EXPENSE DETAIL
                // ============================================================

                List<Data.Models.HRExpenseDetail> expenseDetails =
                    bllHRExpenseDetail
                        .findAllByUserIdActive(
                            oldApproverId,
                            employeeId);

                foreach (Data.Models.HRExpenseDetail item in expenseDetails)
                {
                    if (newApproverId.HasValue)
                    {
                        item.userId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        item.enabled = false;
                    }
                    else
                    {
                        item.userId = 0;
                    }

                    await bllHRExpenseDetail.Update(item);
                }


                // ============================================================
                // HR EXPENSE WITHOUT TABLE
                // ============================================================

                List<Data.Models.HRExpenseWithOutTable> withoutTables =
                    bllHRExpenseWithOutTable
                        .findByUserIdActive(
                            oldApproverId,
                            employeeId);

                foreach (Data.Models.HRExpenseWithOutTable item in withoutTables)
                {
                    if (newApproverId.HasValue)
                    {
                        item.currentUserId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        item.onaySirasi = 10;
                        item.currentStateId = 4;
                    }
                    else
                    {
                        item.currentUserId = 0;
                    }

                    await bllHRExpenseWithOutTable.Update(item);
                }


                // ============================================================
                // HR EXPENSE WITHOUT DETAIL
                // ============================================================

                List<Data.Models.HRExpenseWithOutDetail> withoutDetails =
                    bllHRExpenseWithOutDetail
                        .findAllByUserIdActive(
                            oldApproverId,
                            employeeId);

                foreach (Data.Models.HRExpenseWithOutDetail item in withoutDetails)
                {
                    if (newApproverId.HasValue)
                    {
                        item.userId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        item.enabled = false;
                    }
                    else
                    {
                        item.userId = 0;
                    }

                    await bllHRExpenseWithOutDetail.Update(item);
                }


                // ============================================================
                // ANNUAL LEAVE TABLE
                // ============================================================

                List<Data.Models.AnnualLeaveTable> annualLeaveTables =
                    bllAnnualLeaveTable
                        .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(
                            oldApproverId,
                            1,
                            true,
                            employeeId);

                foreach (Data.Models.AnnualLeaveTable item in annualLeaveTables)
                {
                    if (newApproverId.HasValue)
                    {
                        item.currentUserId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        /*
                         * Mevcut kodunda AnnualLeave tarafında onaySirasi
                         * değiştirilmeden currentStateId = 4 yapılıyordu.
                         */
                        item.currentStateId = 4;
                    }
                    else
                    {
                        item.currentUserId = 0;
                    }

                    await bllAnnualLeaveTable.Update(item);
                }


                // ============================================================
                // ANNUAL LEAVE DETAIL
                // ============================================================

                List<Data.Models.AnnualLeaveDetail> annualLeaveDetails =
                    bllAnnualLeaveDetail
                        .findAllByUserIdActive(
                            oldApproverId,
                            employeeId);

                foreach (Data.Models.AnnualLeaveDetail item in annualLeaveDetails)
                {
                    if (newApproverId.HasValue)
                    {
                        item.userId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        item.enabled = false;
                    }
                    else
                    {
                        item.userId = 0;
                    }

                    await bllAnnualLeaveDetail.Update(item);
                }


                // ============================================================
                // REPRESENTATIVE EXPENSE TABLE
                // ============================================================

                List<Data.Models.RepresentativeExpenseTable> representativeTables =
                    bllRepresentativeExpenseTable
                        .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(
                            oldApproverId,
                            1,
                            true,
                            employeeId);

                foreach (
                    Data.Models.RepresentativeExpenseTable item
                    in representativeTables)
                {
                    if (newApproverId.HasValue)
                    {
                        /*
                         * Eski manager4 kodunda userId değiştiriliyordu.
                         *
                         * Diğer manager blokları ve sorgu currentUserId kullandığı
                         * için currentUserId olarak düzenlendi.
                         */
                        item.currentUserId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        item.onaySirasi = 10;
                        item.currentStateId = 4;
                    }
                    else
                    {
                        item.currentUserId = 0;
                    }

                    await bllRepresentativeExpenseTable.Update(item);
                }


                // ============================================================
                // REPRESENTATIVE EXPENSE DETAIL
                // ============================================================

                List<Data.Models.RepresentativeExpenseDetail> representativeDetails =
                    bllRepresentativeExpenseDetail
                        .findAllByUserIdActive(
                            oldApproverId,
                            employeeId);

                foreach (
                    Data.Models.RepresentativeExpenseDetail item
                    in representativeDetails)
                {
                    if (newApproverId.HasValue)
                    {
                        item.userId =
                            newApproverId.Value;
                    }
                    else if (finishFlowWhenRemoved)
                    {
                        item.enabled = false;
                    }
                    else
                    {
                        item.userId = 0;
                    }

                    await bllRepresentativeExpenseDetail.Update(item);
                }
            }


            // ================================================================
            // YILLIK İZİN ONAYLAYICI
            // ================================================================

            private async Task UpdateAnnualLeaveApproverAsync(
                int employeeId,
                int oldApproverId,
                int? newApproverId)
            {
                BLLActions.AnnualLeaveTable bllAnnualLeaveTable =
                    new BLLActions.AnnualLeaveTable(
                        _configuration,
                        _env,
                        _mapper);

                BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail =
                    new BLLActions.AnnualLeaveDetail(
                        _configuration,
                        _env);


                // ============================================================
                // ANNUAL LEAVE TABLE
                // ============================================================

                List<Data.Models.AnnualLeaveTable> annualLeaveTables =
                    bllAnnualLeaveTable
                        .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(
                            oldApproverId,
                            1,
                            true,
                            employeeId);

                foreach (Data.Models.AnnualLeaveTable item in annualLeaveTables)
                {
                    if (newApproverId.HasValue)
                    {
                        item.currentUserId =
                            newApproverId.Value;
                    }
                    else
                    {
                        item.currentStateId = 4;
                    }

                    await bllAnnualLeaveTable.Update(item);
                }


                // ============================================================
                // ANNUAL LEAVE DETAIL
                // ============================================================

                List<Data.Models.AnnualLeaveDetail> annualLeaveDetails =
                    bllAnnualLeaveDetail
                        .findAllByUserIdActive(
                            oldApproverId,
                            employeeId);

                foreach (Data.Models.AnnualLeaveDetail item in annualLeaveDetails)
                {
                    if (newApproverId.HasValue)
                    {
                        item.userId =
                            newApproverId.Value;
                    }
                    else
                    {
                        item.enabled = false;
                    }

                    await bllAnnualLeaveDetail.Update(item);
                }
            }


            // ================================================================
            // ARAÇ ONAYLAYICI
            // ================================================================

            private async Task UpdateVehicleApproverAsync(
                int employeeId,
                int oldApproverId,
                int? newApproverId)
            {
                BLLActions.AracTalepTable bllAracTalepTable =
                    new BLLActions.AracTalepTable(
                        _configuration,
                        _env,
                        _mapper);

                BLLActions.AracTalepTableDetail bllAracTalepTableDetail =
                    new BLLActions.AracTalepTableDetail(
                        _configuration,
                        _env);


                // ============================================================
                // ARAÇ TALEP TABLE
                // ============================================================

                List<Data.Models.AracTalepTable> vehicleTables =
                    bllAracTalepTable
                        .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(
                            oldApproverId,
                            1,
                            true,
                            employeeId);

                foreach (Data.Models.AracTalepTable item in vehicleTables)
                {
                    if (newApproverId.HasValue)
                    {
                        item.currentUserId =
                            newApproverId.Value;
                    }
                    else
                    {
                        item.currentStateId = 4;
                    }

                    await bllAracTalepTable.Update(item);
                }


                // ============================================================
                // ARAÇ TALEP DETAIL
                // ============================================================

                List<Data.Models.AracTalepTableDetail> vehicleDetails =
                    bllAracTalepTableDetail
                        .findAllByUserIdActive(
                            oldApproverId,
                            employeeId);

                foreach (Data.Models.AracTalepTableDetail item in vehicleDetails)
                {
                    if (newApproverId.HasValue)
                    {
                        item.userId =
                            newApproverId.Value;
                    }
                    else
                    {
                        item.enabled = false;
                    }

                    await bllAracTalepTableDetail.Update(item);
                }
            }


            public List<AdminUser> getUserByCompanyVkorg(List<int> listEmails, string vkorg, bool enabled)
            {
                List<AdminUser> liste = dal.Get(u => u.enabled == enabled && u.role.companies.Contains(vkorg) && listEmails.Contains(u.Id)).ToList();


                return liste;
            }

            public List<AdminUser> getRoleIdList(HashSet<int> listRoleId)
            {
                List<AdminUser> liste = dal.Get(u => u.enabled == true && listRoleId.Contains(u.roleId) && u.role.enabled == true).ToList();
                return liste;
            }
        }
    }


}
