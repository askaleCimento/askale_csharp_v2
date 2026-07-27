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
                var q = dal.Get(k => (k.roleId == roleID || roleID == null) &&
                (a.roleId == 1 || a.role.companies.Contains("[" + k.company.vkorg + "]")) &&
                                     (k.name.Contains(name) || string.IsNullOrEmpty(name)) &&
                                     (k.username.Contains(username) || string.IsNullOrEmpty(username)) &&
                                     (k.sapUser.Contains(sapUser) || string.IsNullOrEmpty(sapUser)) &&
                                     (k.email.Contains(email) || string.IsNullOrEmpty(email)) &&
                                     k.enabled == true).OrderBy(x => x.username).Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
                ;
                return q;
            }

            public Models.AdminUser? GetAllWithPassiveId(int id)
            {
                return dal.Get(u => u.Id == id).FirstOrDefault();
            }

            public List<AskalePortal.Data.Models.AdminUser> GetAllWithPagePasif(int? roleID, string name, string username, string email,
                                                     int activePage, int recordsPerPage, AdminUser a)
            {
                var q = dal.Get(k => (k.roleId == roleID || roleID == null) &&
                (a.roleId == 1 || a.role.companies.Contains("[" + k.company.vkorg + "]")) &&
                                     (k.name.Contains(name) || string.IsNullOrEmpty(name)) &&
                                     (k.username.Contains(username) || string.IsNullOrEmpty(username)) &&
                                     (k.email.Contains(email) || string.IsNullOrEmpty(email)) &&
                                     k.enabled == false).OrderBy(x => x.username)
                                     .Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();

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
                return dal.Get(u => (u.name.Contains(name) || string.IsNullOrEmpty(name)) &&
                (u.username.Contains(username) || string.IsNullOrEmpty(username)) &&
                u.enabled == true).OrderBy(u => u.name).Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
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
                PageReturn<UsersFilterDto>? result = new PageReturn<UsersFilterDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? name = filterPageParam.liste?.filterName;
                int? filterRol = filterPageParam.liste?.filterRol;
                string? filterKullaniciAdi = filterPageParam.liste?.filterKullaniciAdi;
                string? filterEmail = filterPageParam.liste?.filterEmail;
                string? filterSapUser = filterPageParam.liste?.filterSapUserName;
                int? filterCompany = filterPageParam.liste?.filterCompany;
                IQueryable<AdminUser> query = dal.Get(u => u.enabled &&
                name == null ? true :
                u.name == name
                && ((filterRol == null || filterRol == 0) ? true : u.roleId == filterRol)
                && (filterKullaniciAdi == null || filterKullaniciAdi == "" ? true : u.username == filterKullaniciAdi)
                && (filterEmail == null || filterEmail == "" ? true : u.email == filterEmail)
                && (filterSapUser == null || filterSapUser == "" ? true : u.sapUser == filterSapUser)
                && (filterCompany == null || filterCompany == 0 ? true : u.companyId == filterCompany)
                ).Include(u => u.company).Include(u => u.role).OrderByDescending(u => u.Id);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

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

                    }).ToList();
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
                PageReturn<AdminUserDto>? result = new PageReturn<AdminUserDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? name = filterPageParam.liste?.filterName;
                int? filterRol = filterPageParam.liste?.filterRol;
                string? filterKullaniciAdi = filterPageParam.liste?.filterKullaniciAdi;
                string? filterEmail = filterPageParam.liste?.filterEmail;
                string? filterSapUser = filterPageParam.liste?.filterSapUserName;
                int? filterCompany = filterPageParam.liste?.filterCompany;
                IQueryable<AdminUser> query = dal.Get(u => u.enabled == false &&
              name == null ? true :
              u.name == name
              && ((filterRol == null || filterRol == 0) ? true : u.roleId == filterRol)
              && (filterKullaniciAdi == null || filterKullaniciAdi == "" ? true : u.username == filterKullaniciAdi)
              && (filterEmail == null || filterEmail == "" ? true : u.email == filterEmail)
              && (filterSapUser == null || filterSapUser == "" ? true : u.sapUser == filterSapUser)
              && (filterCompany == null || filterCompany == 0 ? true : u.companyId == filterCompany)
              ).Include(u => u.company).Include(u => u.role);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

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


                    }).ToList();
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
                string? name = filterPageParam.liste?.filterName;
                string? filterKullaniciAdi = filterPageParam.liste?.filterUsername;

                var query = from u in dal.Get(a => a.enabled
     && (a.roleId != 1083 || (a.roleId == 1083 && (a.company.vkorg.Contains("AC20") || a.company.vkorg.Contains("AC80"))))
     && (filterKullaniciAdi == null || filterKullaniciAdi == "" ? true : a.username == filterKullaniciAdi)
     && (name == null || name == "" ? true : a.name == name))

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

            public async Task<AdminUser> saveHRUser(AdminUserSaveDto newUserGelen, int userId)
            {
                AdminUser newUser = _mapper.Map<AdminUser>(newUserGelen);
                AdminUser oldUser = GetByID(newUser.Id)!;
                BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
                BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
                BLLActions.HRExpenseDetail bllHRExpenseDetail = new BLLActions.HRExpenseDetail(_configuration, _env);
                BLLActions.HRExpenseTripDetail bllHRExpenseTripDetail = new BLLActions.HRExpenseTripDetail(_configuration, _env);
                BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);
                BLLActions.HRExpenseWithOutDetail bllHRExpenseWithOutDetail = new BLLActions.HRExpenseWithOutDetail(_configuration, _env);
                BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
                BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail = new BLLActions.AnnualLeaveDetail(_configuration, _env);
                BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);
                BLLActions.RepresentativeExpenseDetail bllRepresentativeExpenseDetail = new BLLActions.RepresentativeExpenseDetail(_configuration, _env);
                BLLActions.AracTalepTable bllAracTalepTable = new BLLActions.AracTalepTable(_configuration, _env, _mapper);
                BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLLActions.AracTalepTableDetail(_configuration, _env);

                // employer1 değişmiş ise
                if (oldUser.hremployer1 != newUser.hremployer1)
                {

                    List<Data.Models.HRExpenseTripTable> listHRExpenseTripTable = bllHRExpenseTripTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.hremployer1, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.HRExpenseTripTable hrExpenseTripTable in listHRExpenseTripTable)
                    {
                        hrExpenseTripTable.currentUserId = (newUser.hremployer1 ?? 0);
                        await bllHRExpenseTripTable.Update(hrExpenseTripTable);
                    }
                    List<Data.Models.HRExpenseTable> listHRExpense = bllHRExpenseTable.findByUserIdActive(oldUser.hremployer1,
                            oldUser.Id);
                    foreach (Data.Models.HRExpenseTable hrExpenseTable in listHRExpense)
                    {
                        hrExpenseTable.currentUserId = (newUser.hremployer1 ?? 0);
                        await bllHRExpenseTable.Update(hrExpenseTable);
                    }

                    List<Data.Models.HRExpenseDetail> listHRExpenseDetail = bllHRExpenseDetail
                            .findAllByUserIdActive(oldUser.hremployer1, oldUser.Id);
                    foreach (Data.Models.HRExpenseDetail hrExpenseDetail in listHRExpenseDetail)
                    {
                        hrExpenseDetail.userId = (newUser.hremployer1 ?? 0);
                        await bllHRExpenseDetail.Update(hrExpenseDetail);
                    }

                    List<Data.Models.HRExpenseTripDetail> listHRExpenseTripDetail = bllHRExpenseTripDetail
                            .findByUserIdActive(oldUser.hremployer1, oldUser.Id);
                    foreach (Data.Models.HRExpenseTripDetail hrExpenseTripDetail in listHRExpenseTripDetail)
                    {
                        hrExpenseTripDetail.userId = (newUser.hremployer1 ?? 0);
                        await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);
                    }

                    List<Data.Models.HRExpenseWithOutTable> listHRExpenseWithOut = bllHRExpenseWithOutTable
                            .findByUserIdActive(oldUser.hremployer1, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutTable hrExpenseWithOutTable in listHRExpenseWithOut)
                    {
                        hrExpenseWithOutTable.currentUserId = (newUser.hremployer1 ?? 0);
                        await bllHRExpenseWithOutTable.Update(hrExpenseWithOutTable);
                    }
                    List<Data.Models.HRExpenseWithOutDetail> listHRExpenseWithOutDetail = bllHRExpenseWithOutDetail
                            .findAllByUserIdActive(oldUser.hremployer1, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutDetail hrExpenseWithOutDetail in listHRExpenseWithOutDetail)
                    {
                        hrExpenseWithOutDetail.userId = (newUser.hremployer1 ?? 0);
                        await bllHRExpenseWithOutDetail.Update(hrExpenseWithOutDetail);
                    }

                    List<Data.Models.AnnualLeaveTable> listAnnualLeaveTable = bllAnnualLeaveTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.hremployer1, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.AnnualLeaveTable annualLeaveTable in listAnnualLeaveTable)
                    {
                        annualLeaveTable.currentUserId = (newUser.hremployer1 ?? 0);
                        await bllAnnualLeaveTable.Update(annualLeaveTable);
                    }
                    List<Data.Models.AnnualLeaveDetail> listAnnualLeaveDetail = bllAnnualLeaveDetail
                            .findAllByUserIdActive(oldUser.hremployer1, oldUser.Id);
                    foreach (Data.Models.AnnualLeaveDetail annualLeaveDetail in listAnnualLeaveDetail)
                    {
                        annualLeaveDetail.userId = (newUser.hremployer1 ?? 0);
                        await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                    }

                    List<Data.Models.RepresentativeExpenseTable> listRepresentativeExpenseTable = bllRepresentativeExpenseTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.hremployer1, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseTable representativeExpenseTable in listRepresentativeExpenseTable)
                    {
                        representativeExpenseTable.currentUserId = (newUser.hremployer1 ?? 0);
                        await bllRepresentativeExpenseTable.Update(representativeExpenseTable);
                    }
                    List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                            .findAllByUserIdActive(oldUser.hremployer1, oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseDetail representativeExpenseDetail in listRepresentativeExpenseDetail)
                    {
                        representativeExpenseDetail.userId = (newUser.hremployer1 ?? 0);
                        await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);
                    }

                }
                // manager 1 değişmiş ise
                if (oldUser.manager1 != newUser.manager1)
                {

                    List<Data.Models.HRExpenseTripTable> listHRExpenseTripTable = bllHRExpenseTripTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager1, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.HRExpenseTripTable hrExpenseTripTable in listHRExpenseTripTable)
                    {

                        hrExpenseTripTable.currentUserId = (newUser.manager1 ?? 0);
                        await bllHRExpenseTripTable.Update(hrExpenseTripTable);
                    }
                    List<Data.Models.HRExpenseTripDetail> listHRExpenseTripDetail = bllHRExpenseTripDetail
                            .findByUserIdActive(oldUser.manager1, oldUser.Id);
                    foreach (Data.Models.HRExpenseTripDetail hrExpenseTripDetail in listHRExpenseTripDetail)
                    {
                        hrExpenseTripDetail.userId = (newUser.manager1 ?? 0);
                        await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);

                    }

                    List<Data.Models.HRExpenseTable> listHRExpense = bllHRExpenseTable.findByUserIdActive(oldUser.manager1,
                            oldUser.Id);
                    foreach (Data.Models.HRExpenseTable hrExpenseTable in listHRExpense)
                    {
                        hrExpenseTable.currentUserId = (newUser.manager1 ?? 0);
                        await bllHRExpenseTable.Update(hrExpenseTable);
                    }

                    List<Data.Models.HRExpenseDetail> listHRExpenseDetail = bllHRExpenseDetail
                            .findAllByUserIdActive(oldUser.manager1, oldUser.Id);
                    foreach (Data.Models.HRExpenseDetail hrExpenseDetail in listHRExpenseDetail)
                    {
                        hrExpenseDetail.userId = (newUser.manager1 ?? 0);
                        await bllHRExpenseDetail.Update(hrExpenseDetail);

                    }

                    List<Data.Models.HRExpenseWithOutTable> listHRExpenseWithOut = bllHRExpenseWithOutTable
                            .findByUserIdActive(oldUser.manager1, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutTable hrExpenseWithOutTable in listHRExpenseWithOut)
                    {
                        hrExpenseWithOutTable.currentUserId = (newUser.manager1 ?? 0);
                        await bllHRExpenseWithOutTable.Update(hrExpenseWithOutTable);

                    }
                    List<Data.Models.HRExpenseWithOutDetail> listHRExpenseWithOutDetail = bllHRExpenseWithOutDetail
                            .findAllByUserIdActive(oldUser.manager1, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutDetail hrExpenseWithOutDetail in listHRExpenseWithOutDetail)
                    {
                        hrExpenseWithOutDetail.userId = (newUser.manager1 ?? 0);
                        await bllHRExpenseWithOutDetail.Update(hrExpenseWithOutDetail);

                    }

                    List<Data.Models.AnnualLeaveTable> liAnnualLeaveTables = bllAnnualLeaveTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager1, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.AnnualLeaveTable annualLeaveTable in liAnnualLeaveTables)
                    {
                        annualLeaveTable.currentUserId = (newUser.manager1 ?? 0);
                        await bllAnnualLeaveTable.Update(annualLeaveTable);

                    }
                    List<Data.Models.AnnualLeaveDetail> listAnnualLeaveDetail = bllAnnualLeaveDetail
                            .findAllByUserIdActive(oldUser.manager1, oldUser.Id);
                    foreach (Data.Models.AnnualLeaveDetail annualLeaveDetail in listAnnualLeaveDetail)
                    {

                        annualLeaveDetail.userId = (newUser.manager1 ?? 0);
                        await bllAnnualLeaveDetail.Update(annualLeaveDetail);

                    }

                    List<Data.Models.RepresentativeExpenseTable> listRepresentativeExpenseTable = bllRepresentativeExpenseTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager1, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseTable representativeExpenseTable in listRepresentativeExpenseTable)
                    {
                        representativeExpenseTable.currentUserId = (newUser.manager1 ?? 0);
                        await bllRepresentativeExpenseTable.Update(representativeExpenseTable);
                    }
                    List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                            .findAllByUserIdActive(oldUser.manager1, oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseDetail representativeExpenseDetail in listRepresentativeExpenseDetail)
                    {

                        representativeExpenseDetail.userId = (newUser.manager1 ?? 0);
                        await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);

                    }

                }
                // manager2 değişmiş ise
                if (oldUser.manager2 != newUser.manager2)
                {

                    List<Data.Models.HRExpenseTripTable> listHRExpenseTripTable = bllHRExpenseTripTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager2, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.HRExpenseTripTable hrExpenseTripTable in listHRExpenseTripTable)
                    {

                        if (newUser.manager2 != null)
                        {
                            hrExpenseTripTable.currentUserId = (newUser.manager2 ?? 0);
                            await bllHRExpenseTripTable.Update(hrExpenseTripTable);
                        }
                        else
                        {
                            // bitir
                            hrExpenseTripTable.onaySirasi = (10);
                            hrExpenseTripTable.currentStateId = (4);
                            await bllHRExpenseTripTable.Update(hrExpenseTripTable);
                        }
                    }

                    List<Data.Models.HRExpenseTripDetail> listHRExpenseTripDetail = bllHRExpenseTripDetail
                            .findByUserIdActive(oldUser.manager2, oldUser.Id);
                    foreach (Data.Models.HRExpenseTripDetail hrExpenseTripDetail in listHRExpenseTripDetail)
                    {
                        if (newUser.manager2 != null)
                        {
                            hrExpenseTripDetail.userId = (newUser.manager2 ?? 0);
                            await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);
                        }
                        else
                        {
                            // sil
                            hrExpenseTripDetail.enabled = (false);
                            await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);
                        }

                    }
                    List<Data.Models.HRExpenseTable> listHRExpense = bllHRExpenseTable.findByUserIdActive(oldUser.manager2,
                            oldUser.Id);
                    foreach (Data.Models.HRExpenseTable hrExpenseTable in listHRExpense)
                    {
                        if (newUser.manager2 != null)
                        {
                            hrExpenseTable.currentUserId = (newUser.manager2 ?? 0);
                            await bllHRExpenseTable.Update(hrExpenseTable);
                        }
                        else
                        {
                            // bitir
                            hrExpenseTable.onaySirasi = (10);
                            hrExpenseTable.currentStateId = (4);
                            await bllHRExpenseTable.Update(hrExpenseTable);
                        }
                    }

                    List<Data.Models.HRExpenseDetail> listHRExpenseDetail = bllHRExpenseDetail
                            .findAllByUserIdActive(oldUser.manager2, oldUser.Id);
                    foreach (Data.Models.HRExpenseDetail hrExpenseDetail in listHRExpenseDetail)
                    {
                        if (newUser.manager2 != null)
                        {
                            hrExpenseDetail.userId = (newUser.manager2 ?? 0);
                            await bllHRExpenseDetail.Update(hrExpenseDetail);
                        }
                        else
                        {
                            // sil
                            hrExpenseDetail.enabled = (false);
                            await bllHRExpenseDetail.Update(hrExpenseDetail);
                        }

                    }

                    List<Data.Models.HRExpenseWithOutTable> listHRExpenseWithOut = bllHRExpenseWithOutTable
                            .findByUserIdActive(oldUser.manager2, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutTable hrExpenseWithOutTable in listHRExpenseWithOut)
                    {
                        if (newUser.manager2 != null)
                        {
                            hrExpenseWithOutTable.currentUserId = (newUser.manager2 ?? 0);
                            await bllHRExpenseWithOutTable.Update(hrExpenseWithOutTable);
                        }
                        else
                        {
                            // bitir
                            hrExpenseWithOutTable.onaySirasi = (10);
                            hrExpenseWithOutTable.currentStateId = (4);
                            await bllHRExpenseWithOutTable.Update(hrExpenseWithOutTable);
                        }

                    }
                    List<Data.Models.HRExpenseWithOutDetail> listHRExpenseWithOutDetail = bllHRExpenseWithOutDetail
                            .findAllByUserIdActive(oldUser.manager2, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutDetail hrExpenseWithOutDetail in listHRExpenseWithOutDetail)
                    {
                        if (newUser.manager2 != null)
                        {
                            hrExpenseWithOutDetail.userId = (newUser.manager2 ?? 0);
                            await bllHRExpenseWithOutDetail.Update(hrExpenseWithOutDetail);
                        }
                        else
                        {
                            // sil
                            hrExpenseWithOutDetail.enabled = (false);
                            await bllHRExpenseWithOutDetail.Update(hrExpenseWithOutDetail);
                        }

                    }

                    List<Data.Models.AnnualLeaveTable> listAnnualLeaveTable = bllAnnualLeaveTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager2, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.AnnualLeaveTable annualLeaveTable in listAnnualLeaveTable)
                    {
                        if (newUser.manager2 != null)
                        {
                            annualLeaveTable.currentUserId = (newUser.manager2 ?? 0);
                            await bllAnnualLeaveTable.Update(annualLeaveTable);
                        }
                        else
                        {
                            // bitir
                            // annualLeaveTable.setOnaySirasi(10);
                            annualLeaveTable.currentStateId = (4);
                            await bllAnnualLeaveTable.Update(annualLeaveTable);
                        }
                    }
                    List<Data.Models.AnnualLeaveDetail> listAnnualLeaveDetail = bllAnnualLeaveDetail
                            .findAllByUserIdActive(oldUser.manager2, oldUser.Id);
                    foreach (Data.Models.AnnualLeaveDetail annualLeaveDetail in listAnnualLeaveDetail)
                    {
                        if (newUser.manager2 != null)
                        {
                            annualLeaveDetail.userId = (newUser.manager2 ?? 0);
                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                        }
                        else
                        {
                            // sil
                            annualLeaveDetail.enabled = (false);
                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                        }

                    }

                    List<Data.Models.RepresentativeExpenseTable> listRepresentativeExpenseTable = bllRepresentativeExpenseTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager2, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseTable representativeExpenseTable in listRepresentativeExpenseTable)
                    {
                        if (newUser.manager2 != null)
                        {
                            representativeExpenseTable.currentUserId = (newUser.manager2 ?? 0);
                            await bllRepresentativeExpenseTable.Update(representativeExpenseTable);
                        }
                        else
                        {
                            // bitir
                            representativeExpenseTable.onaySirasi = (10);
                            representativeExpenseTable.currentStateId = (4);
                            await bllRepresentativeExpenseTable.Update(representativeExpenseTable);
                        }
                    }

                    List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                            .findAllByUserIdActive(oldUser.manager2, oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseDetail representativeExpenseDetail in listRepresentativeExpenseDetail)
                    {
                        if (newUser.manager2 != null)
                        {
                            representativeExpenseDetail.userId = (newUser.manager2 ?? 0);
                            await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);
                        }
                        else
                        {
                            // sil
                            representativeExpenseDetail.enabled = (false);
                            await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);
                        }

                    }

                }
                // Manager 3 değişmiş ise
                if (oldUser.manager3 != newUser.manager3)
                {
                    List<Data.Models.HRExpenseTripTable> listHRExpenseTripTable = bllHRExpenseTripTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager3, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.HRExpenseTripTable hrExpenseTripTable in listHRExpenseTripTable)
                    {

                        if (newUser.manager3 != null)
                        {
                            hrExpenseTripTable.currentUserId = (newUser.manager3 ?? 0);
                            await bllHRExpenseTripTable.Update(hrExpenseTripTable);
                        }
                        else
                        {
                            // bitir
                            hrExpenseTripTable.onaySirasi = (10);
                            hrExpenseTripTable.currentStateId = (4);
                            await bllHRExpenseTripTable.Update(hrExpenseTripTable);
                        }
                    }

                    List<Data.Models.HRExpenseTripDetail> listHRExpenseTripDetail = bllHRExpenseTripDetail
                            .findByUserIdActive(oldUser.manager3, oldUser.Id);
                    foreach (Data.Models.HRExpenseTripDetail hrExpenseTripDetail in listHRExpenseTripDetail)
                    {
                        if (newUser.manager3 != null)
                        {
                            hrExpenseTripDetail.userId = (newUser.manager3 ?? 0);
                            await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);
                        }
                        else
                        {
                            // sil
                            hrExpenseTripDetail.enabled = (false);
                            await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);
                        }

                    }

                    List<Data.Models.HRExpenseTable> listHRExpense = bllHRExpenseTable.findByUserIdActive(oldUser.manager3,
                            oldUser.Id);

                    foreach (Data.Models.HRExpenseTable hrExpenseTable in listHRExpense)
                    {
                        if (newUser.manager3 != null)
                        {
                            hrExpenseTable.currentUserId = (newUser.manager3 ?? 0);
                            await bllHRExpenseTable.Update(hrExpenseTable);
                        }
                        else
                        {
                            // bitir
                            hrExpenseTable.onaySirasi = (10);
                            hrExpenseTable.currentStateId = (4);
                            await bllHRExpenseTable.Update(hrExpenseTable);
                        }
                    }

                    List<Data.Models.HRExpenseDetail> listHRExpenseDetail = bllHRExpenseDetail
                            .findAllByUserIdActive(oldUser.manager3, oldUser.Id);
                    foreach (Data.Models.HRExpenseDetail hrExpenseDetail in listHRExpenseDetail)
                    {
                        if (newUser.manager3 != null)
                        {
                            hrExpenseDetail.userId = (newUser.manager3 ?? 0);
                            await bllHRExpenseDetail.Update(hrExpenseDetail);
                        }
                        else
                        {
                            // sil
                            hrExpenseDetail.enabled = (false);
                            await bllHRExpenseDetail.Update(hrExpenseDetail);
                        }

                    }

                    List<Data.Models.HRExpenseWithOutTable> listHRExpenseWithOut = bllHRExpenseWithOutTable
                            .findByUserIdActive(oldUser.manager3, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutTable hrExpenseWithOutTable in listHRExpenseWithOut)
                    {
                        if (newUser.manager3 != null)
                        {
                            hrExpenseWithOutTable.currentUserId = (newUser.manager3 ?? 0);
                            await bllHRExpenseWithOutTable.Update(hrExpenseWithOutTable);
                        }
                        else
                        {
                            // bitir
                            hrExpenseWithOutTable.onaySirasi = (10);
                            hrExpenseWithOutTable.currentStateId = (4);
                            await bllHRExpenseWithOutTable.Update(hrExpenseWithOutTable);
                        }

                    }

                    List<Data.Models.HRExpenseWithOutDetail> listHRExpenseWithOutDetail = bllHRExpenseWithOutDetail
                            .findAllByUserIdActive(oldUser.manager3, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutDetail hrExpenseWithOutDetail in listHRExpenseWithOutDetail)
                    {
                        if (newUser.manager3 != null)
                        {
                            hrExpenseWithOutDetail.userId = (newUser.manager3 ?? 0);
                            await bllHRExpenseWithOutDetail.Update(hrExpenseWithOutDetail);
                        }
                        else
                        {
                            // sil
                            hrExpenseWithOutDetail.enabled = (false);
                            await bllHRExpenseWithOutDetail.Update(hrExpenseWithOutDetail);
                        }

                    }

                    List<Data.Models.AnnualLeaveTable> listAnnualLeaveTables = bllAnnualLeaveTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager3, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.AnnualLeaveTable annualLeaveTable in listAnnualLeaveTables)
                    {
                        if (newUser.manager3 != null)
                        {
                            annualLeaveTable.currentUserId = (newUser.manager3 ?? 0);
                            await bllAnnualLeaveTable.Update(annualLeaveTable);
                        }
                        else
                        {
                            // bitir
                            // annualLeaveTable.setOnaySirasi(10);
                            annualLeaveTable.currentStateId = (4);
                            await bllAnnualLeaveTable.Update(annualLeaveTable);
                        }
                    }

                    List<Data.Models.AnnualLeaveDetail> listAnnualLeaveDetail = bllAnnualLeaveDetail
                            .findAllByUserIdActive(oldUser.manager3, oldUser.Id);
                    foreach (Data.Models.AnnualLeaveDetail annualLeaveDetail in listAnnualLeaveDetail)
                    {
                        if (newUser.manager3 != null)
                        {
                            annualLeaveDetail.userId = (newUser.manager3 ?? 0);
                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                        }
                        else
                        {
                            // sil
                            annualLeaveDetail.enabled = (false);
                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                        }

                    }

                    List<Data.Models.RepresentativeExpenseTable> listRepresentativeExpenseTable = bllRepresentativeExpenseTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager3, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseTable representativeExpenseTable in listRepresentativeExpenseTable)
                    {
                        if (newUser.manager3 != null)
                        {
                            representativeExpenseTable.currentUserId = (newUser.manager3 ?? 0);
                            await bllRepresentativeExpenseTable.Update(representativeExpenseTable);
                        }
                        else
                        {
                            // bitir
                            representativeExpenseTable.onaySirasi = (10);
                            representativeExpenseTable.currentStateId = (4);
                            await bllRepresentativeExpenseTable.Update(representativeExpenseTable);
                        }
                    }
                    List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                            .findAllByUserIdActive(oldUser.manager3, oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseDetail representativeExpenseDetail in listRepresentativeExpenseDetail)
                    {
                        if (newUser.manager3 != null)
                        {
                            representativeExpenseDetail.userId = (newUser.manager3 ?? 0);
                            await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);
                        }
                        else
                        {
                            // sil
                            representativeExpenseDetail.enabled = (false);
                            await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);
                        }

                    }

                }
                // Manager 4 değişmiş ise
                if (oldUser.manager4 != newUser.manager4)
                {

                    List<Data.Models.HRExpenseTripTable> listHRExpenseTripTable = bllHRExpenseTripTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager4, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.HRExpenseTripTable hrExpenseTripTable in listHRExpenseTripTable)
                    {
                        if (newUser.manager4 != null)
                        {
                            hrExpenseTripTable.currentUserId = (newUser.manager4 ?? 0);
                            await bllHRExpenseTripTable.Update(hrExpenseTripTable);
                        }
                        else
                        {
                            hrExpenseTripTable.onaySirasi = (10);
                            hrExpenseTripTable.currentStateId = (4);
                            await bllHRExpenseTripTable.Update(hrExpenseTripTable);
                        }

                    }

                    List<Data.Models.HRExpenseTripDetail> listHRExpenseTripDetail = bllHRExpenseTripDetail
                            .findByUserIdActive(oldUser.manager4, oldUser.Id);
                    foreach (Data.Models.HRExpenseTripDetail hrExpenseTripDetail in listHRExpenseTripDetail)
                    {
                        if (newUser.manager4 != null)
                        {
                            hrExpenseTripDetail.createdUserId = (newUser.manager4);
                            await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);
                        }
                        else
                        {
                            hrExpenseTripDetail.enabled = (false);
                            await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);
                        }

                    }

                    List<Data.Models.HRExpenseTable> listHRExpense = bllHRExpenseTable.findByUserIdActive(oldUser.manager4,
                            oldUser.Id);
                    foreach (Data.Models.HRExpenseTable hrExpenseTable in listHRExpense)
                    {
                        if (newUser.manager4 != null)
                        {
                            hrExpenseTable.currentUserId = (newUser.manager4 ?? 0);
                            await bllHRExpenseTable.Update(hrExpenseTable);
                        }
                        else
                        {
                            hrExpenseTable.onaySirasi = (10);
                            hrExpenseTable.currentStateId = (4);
                            await bllHRExpenseTable.Update(hrExpenseTable);
                        }

                    }

                    List<Data.Models.HRExpenseDetail> listHRExpenseDetail = bllHRExpenseDetail
                            .findAllByUserIdActive(oldUser.manager4, oldUser.Id);
                    foreach (Data.Models.HRExpenseDetail hrExpenseDetail in listHRExpenseDetail)
                    {
                        if (newUser.manager4 != null)
                        {
                            hrExpenseDetail.userId = (newUser.manager4 ?? 0);
                            await bllHRExpenseDetail.Update(hrExpenseDetail);
                        }
                        else
                        {
                            hrExpenseDetail.enabled = (false);
                            await bllHRExpenseDetail.Update(hrExpenseDetail);
                        }
                    }

                    List<Data.Models.HRExpenseWithOutTable> listHRExpenseWithOut = bllHRExpenseWithOutTable
                            .findByUserIdActive(oldUser.manager4, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutTable hrExpenseWithOutTable in listHRExpenseWithOut)
                    {
                        if (newUser.manager4 != null)
                        {
                            hrExpenseWithOutTable.currentUserId = (newUser.manager4 ?? 0);
                            await bllHRExpenseWithOutTable.Update(hrExpenseWithOutTable);
                        }
                        else
                        {
                            hrExpenseWithOutTable.onaySirasi = (10);
                            hrExpenseWithOutTable.currentStateId = (4);
                            await bllHRExpenseWithOutTable.Update(hrExpenseWithOutTable);
                        }
                    }

                    List<Data.Models.HRExpenseWithOutDetail> listHRExpenseWithOutDetail = bllHRExpenseWithOutDetail
                            .findAllByUserIdActive(oldUser.manager4, oldUser.Id);
                    foreach (Data.Models.HRExpenseWithOutDetail hrExpenseWithOutDetail in listHRExpenseWithOutDetail)
                    {
                        if (newUser.manager4 != null)
                        {
                            hrExpenseWithOutDetail.userId = (newUser.manager4 ?? 0);
                            await bllHRExpenseWithOutDetail.Update(hrExpenseWithOutDetail);
                        }
                        else
                        {
                            hrExpenseWithOutDetail.enabled = (false);
                            await bllHRExpenseWithOutDetail.Update(hrExpenseWithOutDetail);
                        }
                    }

                    List<Data.Models.AnnualLeaveTable> listAnnualLeaveTable = bllAnnualLeaveTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager4, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.AnnualLeaveTable annualLeaveTable in listAnnualLeaveTable)
                    {
                        if (newUser.manager4 != null)
                        {
                            annualLeaveTable.currentUserId = (newUser.manager4 ?? 0);
                            await bllAnnualLeaveTable.Update(annualLeaveTable);
                        }
                        else
                        {
                            // annualLeaveTable.setOnaySirasi(10);
                            annualLeaveTable.currentStateId = (4);
                            await bllAnnualLeaveTable.Update(annualLeaveTable);
                        }

                    }
                    List<Data.Models.AnnualLeaveDetail> listAnnualLeaveDetail = bllAnnualLeaveDetail
                            .findAllByUserIdActive(oldUser.manager4, oldUser.Id);
                    foreach (Data.Models.AnnualLeaveDetail annualLeaveDetail in listAnnualLeaveDetail)
                    {
                        if (newUser.manager4 != null)
                        {
                            annualLeaveDetail.userId = (newUser.manager4 ?? 0);
                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                        }
                        else
                        {
                            annualLeaveDetail.enabled = (false);
                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                        }
                    }

                    List<Data.Models.RepresentativeExpenseTable> listRepresentativeExpenseTable = bllRepresentativeExpenseTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.manager4, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseTable representativeExpenseTable in listRepresentativeExpenseTable)
                    {
                        if (newUser.manager4 != null)
                        {
                            representativeExpenseTable.userId = (newUser.manager4 ?? 0);
                            await bllRepresentativeExpenseTable.Update(representativeExpenseTable);
                        }
                        else
                        {
                            representativeExpenseTable.onaySirasi = (10);
                            representativeExpenseTable.currentStateId = (4);
                            await bllRepresentativeExpenseTable.Update(representativeExpenseTable);
                        }
                    }

                    List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                            .findAllByUserIdActive(oldUser.manager4, oldUser.Id);
                    foreach (Data.Models.RepresentativeExpenseDetail representativeExpenseDetail in listRepresentativeExpenseDetail)
                    {
                        if (newUser.manager4 != null)
                        {
                            representativeExpenseDetail.userId = (newUser.manager4 ?? 0);
                            await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);
                        }
                        else
                        {
                            representativeExpenseDetail.enabled = (false);
                            await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);
                        }
                    }

                }

                if (oldUser.izinOnayId != newUser.izinOnayId)
                {

                    List<Data.Models.AnnualLeaveTable> listAnnualLeaveTable = bllAnnualLeaveTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.izinOnayId, 1, true,
                                    oldUser.Id);
                    foreach (Data.Models.AnnualLeaveTable annualLeaveTable in listAnnualLeaveTable)
                    {
                        if (newUser.izinOnayId != null)
                        {
                            annualLeaveTable.currentUserId = (newUser.izinOnayId ?? 0);
                            await bllAnnualLeaveTable.Update(annualLeaveTable);
                        }
                        else
                        {
                            // annualLeaveTable.setOnaySirasi(10);
                            annualLeaveTable.currentStateId = (4);
                            await bllAnnualLeaveTable.Update(annualLeaveTable);
                        }

                    }
                    List<Data.Models.AnnualLeaveDetail> listAnnualLeaveDetail = bllAnnualLeaveDetail
                            .findAllByUserIdActive(oldUser.izinOnayId, oldUser.Id);
                    foreach (Data.Models.AnnualLeaveDetail annualLeaveDetail in listAnnualLeaveDetail)
                    {
                        if (newUser.izinOnayId != null)
                        {
                            annualLeaveDetail.userId = (newUser.izinOnayId ?? 0);
                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                        }
                        else
                        {
                            annualLeaveDetail.enabled = (false);
                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                        }
                    }

                }

                if (oldUser.aracOnayId != newUser.aracOnayId)
                {

                    List<Data.Models.AracTalepTable> listAracTalepTable = bllAracTalepTable
                            .findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(oldUser.aracOnayId, 1, true,
                                    oldUser.Id);
                    List<Data.Models.AracTalepTableDetail> listAracTalepTableDetail = bllAracTalepTableDetail
                            .findAllByUserIdActive(oldUser.aracOnayId, oldUser.Id);
                    foreach (Data.Models.AracTalepTable aracTalepTable in listAracTalepTable)
                    {
                        if (newUser.aracOnayId != null)
                        {
                            aracTalepTable.currentUserId = (newUser.aracOnayId);
                            await bllAracTalepTable.Update(aracTalepTable);
                        }
                        else
                        {
                            // annualLeaveTable.setOnaySirasi(10);
                            aracTalepTable.currentStateId = (4);
                            await bllAracTalepTable.Update(aracTalepTable);
                        }

                    }

                    foreach (Data.Models.AracTalepTableDetail aracTalepTableDetail in listAracTalepTableDetail)
                    {
                        if (newUser.aracOnayId != null)
                        {
                            aracTalepTableDetail.userId = (newUser.aracOnayId);

                            await bllAracTalepTableDetail.Update(aracTalepTableDetail);

                        }
                        else
                        {
                            aracTalepTableDetail.enabled = (false);
                            await bllAracTalepTableDetail.Update(aracTalepTableDetail);
                        }
                    }

                }
                return await Update(newUser);
            }



            #endregion


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
