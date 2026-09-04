using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.ReportDataset;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Net;
using System.Data;
using Microsoft.Reporting.NETCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class RepresentativeExpenseTable : BaseBLL<AskalePortal.Data.Models.RepresentativeExpenseTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public RepresentativeExpenseTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            public List<AskalePortal.Data.Models.RepresentativeExpenseTable> GetAllByUnApproved(int userId)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId == 1 && u.currentUserId == userId).ToList();
            }
            public List<AskalePortal.Data.Models.RepresentativeExpenseTable> GetAllByUserId(int userId)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId == 1 && u.userId == userId).ToList();
            }

            public List<AskalePortal.Data.Models.RepresentativeExpenseTable> GetAllByFinished(int userId)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId != 1 && u.userId == userId).ToList();
            }

            public List<AskalePortal.Data.Models.RepresentativeExpenseTable> GetAllActive()
            {
                return dal.Get(u => u.enabled == true && u.currentStateId == 1).ToList();
            }

            public List<AskalePortal.Data.Models.RepresentativeExpenseTable> GetAllFinished()
            {
                return dal.Get(u => u.enabled == true && u.currentStateId != 1).ToList();
            }

            public int myApprovalCount(int userId)
            {
                int deger = dal.Get(k => k.enabled == true && k.currentUserId == userId && k.currentStateId == 1).Count();
                return deger;
            }

            public List<Data.Models.RepresentativeExpenseTable> findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(int? userOld, int currentStateId, bool enabled, int userId)
            {
                List<Data.Models.RepresentativeExpenseTable> liste = dal.Get(u => u.currentUserId == userOld && u.currentStateId == currentStateId && u.enabled == enabled && u.userId == userId).ToList();
                return liste;
            }

            public async Task<Data.Models.RepresentativeExpenseTable> Save(RepresentativeExpenseTableSaveDto entity, int userId)
            {
                try
                {


                    if (entity.id == null)
                    {
                        entity.createdUserId = (userId);
                        entity.createdDate = DateTime.Now.ToString();
                        entity.enabled = true;

                        Data.Models.RepresentativeExpenseTable? representativeExpenseTable = await Add(_mapper.Map<Data.Models.RepresentativeExpenseTable>(entity));
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        Data.Models.AdminUser user = bllAdminUsers.GetByID(representativeExpenseTable!.userId)!;
                        Data.Models.AdminUser manager1 = bllAdminUsers.GetByID(user.manager1 ?? 0)!;

                        BLLActions.RepresentativeExpenseDetail bllRepresentativeExpenseDetail = new BLLActions.RepresentativeExpenseDetail(_configuration, _env);
                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = new Data.Models.RepresentativeExpenseDetail();
                        representativeExpenseDetail.enabled = true;
                        representativeExpenseDetail.approved = null;
                        representativeExpenseDetail.isReplied = false;
                        representativeExpenseDetail.repId = representativeExpenseTable.Id;
                        representativeExpenseDetail.userId = user.manager1 ?? 0;
                        representativeExpenseDetail.guid = Guid.NewGuid();
                        representativeExpenseDetail.createdDate = (DateTime.Now);
                        await bllRepresentativeExpenseDetail.Add(representativeExpenseDetail);

                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = "Bekleyen Harcama Onayı hk.";
                        emailMessage.toAddress = manager1.email;

                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);


                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager1.name +
                        " Harcama Onayı hk.",
                                    representativeExpenseTable.Id.ToString() + " ID'li harcama onayınızı beklemektedir.");

                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = 3;
                        emailMessage.enabled = true;
                        emailMessage.isSent = false;
                        emailMessage.plannedDate = DateTime.Now;
                        await bllEmailMessages.Add(emailMessage);
                        return representativeExpenseTable;

                    }
                    else
                    {

                        entity.updatedUserId = userId;
                        entity.updateDate = DateTime.Now.ToString();
                        entity.enabled = true;
                        return await Update(_mapper.Map<Data.Models.RepresentativeExpenseTable>(entity));
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            public PageReturn<RepresentativeExpenseTableSaveDto> listByUserIdActive(FilterPageParam<RepresentativeExpenseTableDtoParameter> filterPageParam)
            {
                PageReturn<RepresentativeExpenseTableSaveDto>? result = new PageReturn<RepresentativeExpenseTableSaveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;


                int? filterUser = filterPageParam.liste?.filterUser;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(filterUser ?? 0);
                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user!.roleId, (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);

                IQueryable<Data.Models.RepresentativeExpenseTable> query;
                if (user.roleId == 1 || (roleDetail != null && roleDetail.canSeeLogs))
                {
                    query = dal.Get(u => u.enabled && u.currentStateId == 1).OrderByDescending(u => u.Id);
                }
                else
                {
                    query = dal.Get(u => u.enabled && u.currentStateId == 1 && (u.createdUserId == filterUser || u.userId == filterUser)).OrderByDescending(u => u.Id);
                }

                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new RepresentativeExpenseTableSaveDto()
                    {
                        userId = u.userId,
                        createdUserId = u.createdUserId,
                        createdDate = u.createdDate.ToString(),
                        amount = u.amount,
                        approval = u.approval,
                        approvedAmount = u.approvedAmount,
                        currentStateId = u.currentStateId,
                        currentUserId = u.currentUserId,
                        description = u.description,
                        disaproveCondition = u.disaproveCondition,
                        enabled = u.enabled,
                        fileNames = u.fileNames,
                        id = u.Id,
                        onaySirasi = u.onaySirasi,
                        spendingTime = u.spendingTime.ToString(),
                        typeId = u.typeId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,



                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public PageReturn<RepresentativeExpenseTableSaveDto> activeMyApprovalList(FilterPageParam<RepresentativeExpenseTableDtoParameter> filterPageParam)
            {
                PageReturn<RepresentativeExpenseTableSaveDto>? result = new PageReturn<RepresentativeExpenseTableSaveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;


                int? filterUser = filterPageParam.liste?.filterUser;


                IQueryable<Data.Models.RepresentativeExpenseTable> query = dal.Get(u => u.enabled && u.currentStateId == 1 && u.currentUserId == filterUser).OrderByDescending(u => u.Id);


                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new RepresentativeExpenseTableSaveDto()
                    {
                        userId = u.userId,
                        createdUserId = u.createdUserId,
                        createdDate = u.createdDate.ToString(),
                        amount = u.amount,
                        approval = u.approval,
                        approvedAmount = u.approvedAmount,
                        currentStateId = u.currentStateId,
                        currentUserId = u.currentUserId,
                        description = u.description,
                        disaproveCondition = u.disaproveCondition,
                        enabled = u.enabled,
                        fileNames = u.fileNames,
                        id = u.Id,
                        onaySirasi = u.onaySirasi,
                        spendingTime = u.spendingTime.ToString(),
                        typeId = u.typeId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,



                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public PageReturn<RepresentativeExpenseTableSaveDto> listCompleted(FilterPageParam<RepresentativeExpenseTableDtoParameter> filterPageParam)
            {
                PageReturn<RepresentativeExpenseTableSaveDto>? result = new PageReturn<RepresentativeExpenseTableSaveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;


                int? filterUser = filterPageParam.liste?.filterUser;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(filterUser ?? 0);
                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user!.roleId, (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);

                IQueryable<Data.Models.RepresentativeExpenseTable> query;
                if (user.roleId == 1)
                {
                    query = dal.Get(u => u.enabled && u.currentStateId != 1).OrderByDescending(u => u.Id);
                }
                else if (roleDetail != null && roleDetail.canSeeLogs)
                {
                    BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);
                    Role? role = bllRoles.GetByID(user.roleId);
                    string[] listCompanyIds = role?.companies.Replace("[", "").Replace("]", "").Split(",") ?? [];
                    List<int> listCompanyIdsint = new List<int>();
                    foreach (string str in listCompanyIds)
                    {
                        BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                        Company company = bllCompanies.getByVkorgCompany(str);
                        listCompanyIdsint.Add(company.Id);
                    }
                    query = dal.Get(c => c.enabled && c.currentStateId != 1 && listCompanyIdsint.Contains(c.user.companyId)).OrderByDescending(u => u.Id);
                }
                else
                {
                    query = dal.Get(u => u.enabled && u.currentStateId != 1 && u.userId == filterUser).OrderByDescending(u => u.Id);
                }

                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new RepresentativeExpenseTableSaveDto()
                    {
                        userId = u.userId,
                        createdUserId = u.createdUserId,
                        createdDate = u.createdDate.ToString(),
                        amount = u.amount,
                        approval = u.approval,
                        approvedAmount = u.approvedAmount,
                        currentStateId = u.currentStateId,
                        currentUserId = u.currentUserId,
                        description = u.description,
                        disaproveCondition = u.disaproveCondition,
                        enabled = u.enabled,
                        fileNames = u.fileNames,
                        id = u.Id,
                        onaySirasi = u.onaySirasi,
                        spendingTime = u.spendingTime.ToString(),
                        typeId = u.typeId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,

                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public async Task<int> reject(int repId, int userId)
            {
                try
                {
                    Data.Models.RepresentativeExpenseTable? representativeExpenseTable = GetByID(repId);
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    Data.Models.AdminUser? user = bllAdminUsers.GetByID(representativeExpenseTable?.userId ?? 0);
                    int donenDeger = 0;
                    try
                    {
                        representativeExpenseTable!.approval = (false);
                        representativeExpenseTable.currentStateId = (2);
                        await Update(representativeExpenseTable);
                        donenDeger = 1;

                    }
                    catch
                    {
                        donenDeger = 2;
                    }

                    try
                    {
                        BLLActions.RepresentativeExpenseDetail bllRepresentativeExpenseDetail = new BLLActions.RepresentativeExpenseDetail(_configuration, _env);
                        List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                                .getByActive(repId, userId);
                        for (int j = 1; j < listRepresentativeExpenseDetail.Count; j++)
                        {
                            int id = listRepresentativeExpenseDetail[j].Id;
                            bllRepresentativeExpenseDetail.Delete(id);
                        }
                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = listRepresentativeExpenseDetail[0];
                        representativeExpenseDetail.approved = (false);
                        representativeExpenseDetail.isReplied = (true);
                        representativeExpenseDetail.replyDate = (DateTime.Now);
                        await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);
                        donenDeger = 1;
                    }
                    catch
                    {
                        donenDeger = 2;
                    }

                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                    emailMessage.toAddress = (user?.email);

                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();


                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user?.name +
                    " RED Harcama Onayı hk.",
                          repId.ToString() + " ID'li Harcamanız reddedilmiştir.");

                    emailMessage.emailText = (mailMessage);
                    emailMessage.mailTuru = (3);
                    emailMessage.enabled = (true);
                    emailMessage.isSent = (false);
                    emailMessage.plannedDate = (DateTime.Now);
                    await bllEmailMessages.Add(emailMessage);
                    return donenDeger;
                }
                catch
                {

                    return 2;
                }
            }
            // 1->onaylandı
            // 2-> onaylayıcıları kontrol edin
            // 3->bitti
            // 4->hata
            public async Task<int> confirm(int repId, int userId)
            {

                BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                Data.Models.CeoTable ceo = bllCeoTable.GetByID(1)!;
                Data.Models.RepresentativeExpenseTable representativeExpenseTable = GetByID(repId)!;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                Data.Models.AdminUser? repUser = bllAdminUsers.GetByID(representativeExpenseTable.userId);
                int ceoId = ceo.userId;
                Data.Models.AdminUser? ceoUser = bllAdminUsers.GetByID(ceoId);

                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                BLLActions.RepresentativeExpenseDetail bllRepresentativeExpenseDetail = new BLLActions.RepresentativeExpenseDetail(_configuration, _env);
                if (userId == repUser?.manager1 && userId != ceoUser?.Id)
                {
                    if (repUser.manager2 == null)
                    {
                        if (repUser.manager3 == null && repUser.manager4 == null)
                        {
                            representativeExpenseTable.currentUserId = (ceo.userId);
                            representativeExpenseTable.onaySirasi = (1);
                            representativeExpenseTable.currentStateId = (1);
                            await Update(representativeExpenseTable);
                            // ceo devam

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                            emailMessage.toAddress = (ceoUser?.email);



                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + ceoUser?.name +
          " Harcama Onayı hk.",
                      repId.ToString() + " ID'li harcama onayınızı beklemektedir.");

                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = (DateTime.Now);
                            await bllEmailMessages.Add(emailMessage);


                            List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                                    .getByActive(repId, repUser.manager1);

                            for (int j = 1; j < listRepresentativeExpenseDetail.Count; j++)
                            {
                                int id = listRepresentativeExpenseDetail[j].Id;
                                bllRepresentativeExpenseDetail.Delete(id);
                            }
                            Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = listRepresentativeExpenseDetail[0];
                            representativeExpenseDetail.approved = (true);
                            representativeExpenseDetail.isReplied = (true);
                            representativeExpenseDetail.replyDate = (DateTime.Now);
                            await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);

                            Data.Models.RepresentativeExpenseDetail representativeExpenseDetailnext = new Data.Models.RepresentativeExpenseDetail();
                            representativeExpenseDetailnext.repId = (repId);
                            representativeExpenseDetailnext.createdDate = (DateTime.Now);
                            representativeExpenseDetailnext.userId = (ceo.userId);
                            representativeExpenseDetailnext.enabled = (true);
                            representativeExpenseDetailnext.isReplied = (false);
                            representativeExpenseDetailnext.guid = (Guid.NewGuid());
                            await bllRepresentativeExpenseDetail.Add(representativeExpenseDetailnext);
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        representativeExpenseTable.currentUserId = (repUser.manager2 ?? 0);
                        representativeExpenseTable.onaySirasi = (1);
                        representativeExpenseTable.currentStateId = (1);
                        await Update(representativeExpenseTable);
                        // manager 2den devam
                        AdminUser? manager2 = bllAdminUsers.GetByID(repUser.manager2 ?? 0);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                        emailMessage.toAddress = (manager2?.email);


                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager2?.name +
          " Harcama Onayı hk.",
                      repId.ToString() + " ID'li harcama onayınızı beklemektedir.");
                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (3);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        await bllEmailMessages.Add(emailMessage);

                        List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                                .getByActive(repId, repUser.manager1);
                        for (int j = 1; j < listRepresentativeExpenseDetail.Count; j++)
                        {
                            int id = listRepresentativeExpenseDetail[0].Id;
                            bllRepresentativeExpenseDetail.Delete(id);
                        }
                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = listRepresentativeExpenseDetail[0];
                        representativeExpenseDetail.approved = (true);
                        representativeExpenseDetail.isReplied = (true);
                        representativeExpenseDetail.replyDate = (DateTime.Now);
                        await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);

                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetailnext = new Data.Models.RepresentativeExpenseDetail();
                        representativeExpenseDetailnext.repId = (repId);
                        representativeExpenseDetailnext.createdDate = (DateTime.Now);
                        representativeExpenseDetailnext.userId = (repUser.manager2 ?? 0);
                        representativeExpenseDetailnext.enabled = (true);
                        representativeExpenseDetailnext.isReplied = (false);
                        representativeExpenseDetailnext.guid = Guid.NewGuid();
                        await bllRepresentativeExpenseDetail.Add(representativeExpenseDetailnext);

                        return 1;

                    }
                }
                else if (userId == repUser?.manager2 && userId != ceoUser?.Id)
                {
                    if (repUser.manager3 != null)
                    {
                        representativeExpenseTable.currentUserId = (repUser.manager3 ?? 0);
                        representativeExpenseTable.onaySirasi = (2);
                        representativeExpenseTable.currentStateId = (1);
                        await Update(representativeExpenseTable);
                        // manager 3den devam
                        AdminUser? manager3 = bllAdminUsers.GetByID(repUser.manager3 ?? 0);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                        emailMessage.toAddress = (manager3?.email);
                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager3?.name +
          " Harcama Onayı hk.",
                      repId.ToString() + " ID'li harcama onayınızı beklemektedir.");

                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (3);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        await bllEmailMessages.Add(emailMessage);

                        List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                                .getByActive(repId, repUser.manager2);
                        for (int j = 1; j < listRepresentativeExpenseDetail.Count; j++)
                        {
                            int id = listRepresentativeExpenseDetail[j].Id;
                            bllRepresentativeExpenseDetail.Delete(id);
                        }
                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = listRepresentativeExpenseDetail[0];
                        representativeExpenseDetail.approved = (true);
                        representativeExpenseDetail.isReplied = (true);
                        representativeExpenseDetail.replyDate = (DateTime.Now);
                        await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);

                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetailnext = new Data.Models.RepresentativeExpenseDetail();
                        representativeExpenseDetailnext.repId = (repId);
                        representativeExpenseDetailnext.createdDate = (DateTime.Now);
                        representativeExpenseDetailnext.userId = (repUser.manager3 ?? 0);
                        representativeExpenseDetailnext.enabled = (true);
                        representativeExpenseDetailnext.isReplied = (false);
                        representativeExpenseDetailnext.guid = Guid.NewGuid();
                        await bllRepresentativeExpenseDetail.Add(representativeExpenseDetailnext);

                        return 1;

                    }
                    else if (repUser.manager4 == null)
                    {
                        representativeExpenseTable.currentUserId = (ceo.userId);
                        representativeExpenseTable.onaySirasi = (2);
                        representativeExpenseTable.currentStateId = (1);
                        await Update(representativeExpenseTable);
                        // ceo devam
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                        emailMessage.toAddress = (ceoUser?.email);
                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + ceoUser?.name +
          " Harcama Onayı hk.",
                      repId.ToString() + " ID'li harcama onayınızı beklemektedir.");

                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (3);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        await bllEmailMessages.Add(emailMessage);

                        List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                                .getByActive(repId, repUser.manager2);

                        for (int j = 1; j < listRepresentativeExpenseDetail.Count; j++)
                        {
                            int id = listRepresentativeExpenseDetail[0].Id;
                            bllRepresentativeExpenseDetail.Delete(id);
                        }
                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = listRepresentativeExpenseDetail[0];
                        representativeExpenseDetail.approved = (true);
                        representativeExpenseDetail.isReplied = (true);
                        representativeExpenseDetail.replyDate = (DateTime.Now);
                        await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);

                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetailnext = new Data.Models.RepresentativeExpenseDetail();
                        representativeExpenseDetailnext.repId = (repId);
                        representativeExpenseDetailnext.createdDate = (DateTime.Now);
                        representativeExpenseDetailnext.userId = (ceo.userId);
                        representativeExpenseDetailnext.enabled = (true);
                        representativeExpenseDetailnext.isReplied = (false);
                        representativeExpenseDetailnext.guid = Guid.NewGuid();
                        await bllRepresentativeExpenseDetail.Add(representativeExpenseDetailnext);

                        return 1;
                    }
                    else
                    {
                        return 4;
                    }

                }
                else if (userId == repUser?.manager3 && userId != ceoUser?.Id)
                {
                    if (repUser.manager4 != null)
                    {
                        // manager4den devam
                        representativeExpenseTable.currentUserId = (repUser.manager4 ?? 0);
                        representativeExpenseTable.onaySirasi = (3);
                        representativeExpenseTable.currentStateId = (1);
                        await Update(representativeExpenseTable);
                        // manager 3den devam
                        AdminUser? manager4 = bllAdminUsers.GetByID(repUser.manager4 ?? 0);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                        emailMessage.toAddress = (manager4?.email);

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager4?.name +
          " Harcama Onayı hk.",
                      repId.ToString() + " ID'li harcama onayınızı beklemektedir.");


                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (3);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        await bllEmailMessages.Add(emailMessage);

                        List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                                .getByActive(repId, repUser.manager3);
                        for (int j = 1; j < listRepresentativeExpenseDetail.Count; j++)
                        {
                            int id = listRepresentativeExpenseDetail[j].Id;
                            bllRepresentativeExpenseDetail.Delete(id);
                        }
                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = listRepresentativeExpenseDetail[0];
                        representativeExpenseDetail.approved = (true);
                        representativeExpenseDetail.isReplied = (true);
                        representativeExpenseDetail.replyDate = (DateTime.Now);
                        await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);

                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetailnext = new Data.Models.RepresentativeExpenseDetail();
                        representativeExpenseDetailnext.repId = (repId);
                        representativeExpenseDetailnext.createdDate = (DateTime.Now);
                        representativeExpenseDetailnext.userId = (repUser.manager4 ?? 0);
                        representativeExpenseDetailnext.enabled = true;
                        representativeExpenseDetailnext.isReplied = (false);
                        representativeExpenseDetailnext.guid = Guid.NewGuid();
                        await bllRepresentativeExpenseDetail.Add(representativeExpenseDetailnext);
                        return 1;
                    }
                    else
                    {
                        representativeExpenseTable.currentUserId = (ceo.userId);
                        representativeExpenseTable.onaySirasi = (3);
                        representativeExpenseTable.currentStateId = (1);
                        await Update(representativeExpenseTable);
                        // ceo devam
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                        emailMessage.toAddress = (ceoUser?.email);
                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + ceoUser?.name +
         " Harcama Onayı hk.",
                     repId.ToString() + " ID'li harcama onayınızı beklemektedir.");
                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (3);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        await bllEmailMessages.Add(emailMessage);

                        List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                                .getByActive(repId, repUser.manager3);
                        for (int j = 1; j < listRepresentativeExpenseDetail.Count; j++)
                        {
                            int id = listRepresentativeExpenseDetail[j].Id;
                            bllRepresentativeExpenseDetail.Delete(id);
                        }
                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = listRepresentativeExpenseDetail[0];
                        representativeExpenseDetail.approved = (true);
                        representativeExpenseDetail.isReplied = (true);
                        representativeExpenseDetail.replyDate = (DateTime.Now);
                        await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);

                        Data.Models.RepresentativeExpenseDetail representativeExpenseDetailnext = new Data.Models.RepresentativeExpenseDetail();
                        representativeExpenseDetailnext.repId = (repId);
                        representativeExpenseDetailnext.createdDate = (DateTime.Now);
                        representativeExpenseDetailnext.userId = (ceo.userId);
                        representativeExpenseDetailnext.enabled = (true);
                        representativeExpenseDetailnext.isReplied = (false);
                        representativeExpenseDetailnext.guid = Guid.NewGuid();
                        await bllRepresentativeExpenseDetail.Add(representativeExpenseDetailnext);
                        return 1;
                    }

                }
                else if (userId == repUser?.manager4 && userId != ceoUser?.Id)
                {
                    representativeExpenseTable.currentUserId = (ceo.userId);
                    representativeExpenseTable.onaySirasi = (4);
                    representativeExpenseTable.currentStateId = (1);
                    await Update(representativeExpenseTable);
                    // ceo devam
                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                    emailMessage.toAddress = (ceoUser?.email);
                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + ceoUser?.name +
         " Harcama Onayı hk.",
                     repId.ToString() + " ID'li harcama onayınızı beklemektedir.");
                    emailMessage.emailText = (mailMessage);
                    emailMessage.mailTuru = (4);
                    emailMessage.enabled = (true);
                    emailMessage.isSent = (false);
                    emailMessage.plannedDate = (DateTime.Now);
                    await bllEmailMessages.Add(emailMessage);

                    List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                            .getByActive(repId, repUser.manager4);
                    for (int j = 1; j < listRepresentativeExpenseDetail.Count; j++)
                    {
                        int id = listRepresentativeExpenseDetail[j].Id;
                        bllRepresentativeExpenseDetail.Delete(id);
                    }
                    Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = listRepresentativeExpenseDetail[0];
                    representativeExpenseDetail.approved = (true);
                    representativeExpenseDetail.isReplied = (true);
                    representativeExpenseDetail.replyDate = (DateTime.Now);
                    await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);

                    Data.Models.RepresentativeExpenseDetail representativeExpenseDetailnext = new Data.Models.RepresentativeExpenseDetail();
                    representativeExpenseDetailnext.repId = (repId);
                    representativeExpenseDetailnext.createdDate = (DateTime.Now);
                    representativeExpenseDetailnext.userId = (ceo.userId);
                    representativeExpenseDetailnext.isReplied = (false);
                    representativeExpenseDetailnext.enabled = (true);
                    representativeExpenseDetailnext.guid = Guid.NewGuid();
                    await bllRepresentativeExpenseDetail.Add(representativeExpenseDetailnext);
                    return 1;
                }
                else if (userId == ceo.userId)
                {
                    // bitiş
                    representativeExpenseTable.onaySirasi = (10);
                    representativeExpenseTable.approval = (true);
                    representativeExpenseTable.currentStateId = (4);
                    await Update(representativeExpenseTable);
                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                    emailMessage.toAddress = (repUser?.email);

                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + repUser?.name +
        " Harcama Onayı hk.",
                    repId.ToString() + " ID'li Harcamanız onaylanmıştır.");
                    emailMessage.emailText = (mailMessage);
                    emailMessage.mailTuru = (3);
                    emailMessage.enabled = (true);
                    emailMessage.isSent = (false);
                    emailMessage.plannedDate = (DateTime.Now);
                    await bllEmailMessages.Add(emailMessage);

                    List<Data.Models.RepresentativeExpenseDetail> listRepresentativeExpenseDetail = bllRepresentativeExpenseDetail
                            .getByActive(repId, ceo.userId);
                    for (int j = 1; j < listRepresentativeExpenseDetail.Count; j++)
                    {
                        int id = listRepresentativeExpenseDetail[j].Id;
                        bllRepresentativeExpenseDetail.Delete(id);
                    }
                    Data.Models.RepresentativeExpenseDetail representativeExpenseDetail = listRepresentativeExpenseDetail[0];
                    representativeExpenseDetail.approved = (true);
                    representativeExpenseDetail.isReplied = (true);
                    representativeExpenseDetail.replyDate = (DateTime.Now);
                    await bllRepresentativeExpenseDetail.Update(representativeExpenseDetail);
                    return 3;
                }
                return 4;
            }

            public byte[] CreatePdf(int repId)
            {
                try
                {
                    Data.Models.RepresentativeExpenseTable? expense = dal.Get(u =>
                            u.Id == repId && u.enabled)
                        .AsNoTracking()
                        .Include(u => u.user)
                        .Include(u => u.type)
                        .SingleOrDefault();

                    if (expense == null)
                    {
                        throw new Exception($"{repId} numaralı temsili harcama kaydı bulunamadı.");
                    }

                    DataTable approvalData = CreateRepresentativeApprovalDataTable(repId);
                    string reportPath = ResolveRepresentativeReportPath();

                    using LocalReport localReport = new LocalReport
                    {
                        ReportPath = reportPath
                    };

                    localReport.DataSources.Clear();
                    localReport.DataSources.Add(
                        new ReportDataSource("ApprovalDataSet", approvalData));

                    localReport.SetParameters(new[]
                    {
                        new ReportParameter("username", expense.user?.name ?? string.Empty),
                        new ReportParameter("perno", expense.user?.perNo ?? string.Empty),
                        new ReportParameter("harcamaId", expense.Id.ToString()),
                        new ReportParameter("harcamaTuru", expense.type?.typeName ?? string.Empty),
                        new ReportParameter("harcamaZamani", expense.spendingTime?.ToString("dd.MM.yyyy HH:mm") ?? string.Empty),
                        new ReportParameter("harcamaAciklamasi", SanitizeRepresentativeExpenseHtml(expense.description)),
                        new ReportParameter("harcamaTutari", FormatRepresentativeMoney(expense.amount)),
                        new ReportParameter("onaylananTutar", FormatRepresentativeMoney(expense.approvedAmount))
                    });

                    byte[] pdf = localReport.Render("PDF");
                    if (pdf == null || pdf.Length == 0)
                    {
                        throw new Exception("LocalReport PDF oluşturdu ancak çıktı boş geldi.");
                    }

                    return pdf;
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"RepresentativeExpense PDF oluşturulurken hata oluştu: {ex.Message}",
                        ex);
                }
            }

            private DataTable CreateRepresentativeApprovalDataTable(int repId)
            {
                DataTable table = new DataTable("ApprovalDataSet");
                table.Columns.Add("Photo", typeof(byte[]));
                table.Columns.Add("PhotoMimeType", typeof(string));
                table.Columns.Add("UserName", typeof(string));
                table.Columns.Add("StatusText", typeof(string));
                table.Columns.Add("StatusColor", typeof(string));

                // Tamamlanmış harcamalarda geçmiş onay adımlarının tamamını göstermeliyiz.
                // Bu yüzden enabled filtresi uygulanmıyor. Eski kayıtlarda enabled=false olsa bile
                // onay geçmişi rapordan kaybolmuyor.

                List<Data.Models.RepresentativeExpenseDetail> approvals =
                    dal.dB.RepresentativeExpenseDetail
                        .AsNoTracking()
                        .Include(item => item.user)
                        .Where(item => item.repId == repId)
                        .OrderBy(item => item.createdDate)
                        .ThenBy(item => item.Id)
                        .ToList();

                List<int> proxyUserIds = approvals
                    .Where(item => item.vekaletUserId.HasValue)
                    .Select(item => item.vekaletUserId!.Value)
                    .Distinct()
                    .ToList();

                Dictionary<int, Data.Models.AdminUser> proxyUsers =
                    proxyUserIds.Count == 0
                        ? new Dictionary<int, Data.Models.AdminUser>()
                        : dal.dB.AdminUser
                            .AsNoTracking()
                            .Where(item => proxyUserIds.Contains(item.Id))
                            .ToDictionary(item => item.Id);

                foreach (Data.Models.RepresentativeExpenseDetail approval in approvals)
                {
                    Data.Models.AdminUser? proxyUser = null;
                    if (approval.vekaletUserId.HasValue)
                    {
                        proxyUsers.TryGetValue(approval.vekaletUserId.Value, out proxyUser);
                    }

                    Data.Models.AdminUser? photoUser = proxyUser ?? approval.user;
                    (byte[]? photo, string mimeType) =
                        ReadRepresentativeUserPhoto(photoUser?.imageUrl);

                    (string statusText, string statusColor) =
                        GetRepresentativeApprovalStatus(approval);

                    DataRow row = table.NewRow();
                    row["Photo"] = photo == null ? DBNull.Value : photo;
                    row["PhotoMimeType"] = mimeType;
                    row["UserName"] = GetRepresentativeApproverName(approval, proxyUser);
                    row["StatusText"] = statusText;
                    row["StatusColor"] = statusColor;
                    table.Rows.Add(row);
                }

                return table;
            }

            private string ResolveRepresentativeReportPath()
            {
                const string reportFileName = "RepresentativeExpenseReport.rdl";

                string[] candidates =
                {
                    Path.Combine(_env.ContentRootPath, "Raporlar", reportFileName),
                    Path.Combine(_env.ContentRootPath, "AskalePortal.BLL", "Raporlar", reportFileName),
                    Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "AskalePortal.BLL", "Raporlar", reportFileName)),
                    Path.Combine(AppContext.BaseDirectory, "Raporlar", reportFileName),
                    Path.Combine(AppContext.BaseDirectory, "AskalePortal.BLL", "Raporlar", reportFileName)
                };

                string? reportPath = candidates.FirstOrDefault(File.Exists);
                if (reportPath == null)
                {
                    throw new FileNotFoundException(
                        $"{reportFileName} rapor dosyası bulunamadı. Kontrol edilen yollar: {string.Join("; ", candidates)}");
                }

                return reportPath;
            }

            private (byte[]? Photo, string MimeType) ReadRepresentativeUserPhoto(string? fileName)
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

            private static string GetRepresentativeApproverName(
                Data.Models.RepresentativeExpenseDetail approval,
                Data.Models.AdminUser? proxyUser)
            {
                string approver = approval.user?.name ?? string.Empty;

                if (proxyUser == null)
                {
                    return approver;
                }

                string proxy = proxyUser.name ?? string.Empty;
                return string.IsNullOrWhiteSpace(approver)
                    ? $"{proxy} (Vekaleten)"
                    : $"{approver} / Vekaleten: {proxy}";
            }

            private static (string StatusText, string StatusColor)
                GetRepresentativeApprovalStatus(Data.Models.RepresentativeExpenseDetail approval)
            {
                if (!approval.isReplied && !approval.approved.HasValue)
                {
                    return ("ONAYI BEKLENİYOR...", "#D9EDF7");
                }

                string date = approval.replyDate?.ToString("dd.MM.yyyy HH:mm") ?? string.Empty;

                return approval.approved switch
                {
                    true => ($"Onaylandı - {date}", "#DFF0D8"),
                    false => ($"Reddedildi - {date}", "#F2DEDE"),
                    null => ("YANITLANDI", "#FCF8E3")
                };
            }

            private static string FormatRepresentativeMoney(decimal value)
            {
                return $"{value:N2} TL";
            }

            private static string SanitizeRepresentativeExpenseHtml(string? html)
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

                text = Regex.Replace(text, @"<\s*strong\b[^>]*>", "<b>", RegexOptions.IgnoreCase);
                text = Regex.Replace(text, @"<\s*/\s*strong\s*>", "</b>", RegexOptions.IgnoreCase);
                text = Regex.Replace(text, @"<\s*b\b[^>]*>", "<b>", RegexOptions.IgnoreCase);
                text = Regex.Replace(text, @"<\s*br\b[^>]*\/?>", "<br/>", RegexOptions.IgnoreCase);
                text = Regex.Replace(text, @"<\s*p\b[^>]*>", "<p>", RegexOptions.IgnoreCase);
                text = Regex.Replace(
                    text,
                    @"<(?!/?(?:b|br|p)\b)[^>]+>",
                    string.Empty,
                    RegexOptions.IgnoreCase);

                return text.Trim();
            }


        }


    }

}