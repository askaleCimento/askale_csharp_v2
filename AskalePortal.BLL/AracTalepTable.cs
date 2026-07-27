using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.ReportDataset;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class AracTalepTable : BaseBLL<Data.Models.AracTalepTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public AracTalepTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public int approvalCount(int userId)
            {
                int deger = dal.Get(u => u.enabled && u.currentUserId == userId && u.currentStateId == 1).Count();
                return deger;
            }

            public PageReturn<AracTalepTableDto>? mylistDto(FilterPageParam<AracTalepTableParamsDto> filterPageParam)
            {
                PageReturn<AracTalepTableDto>? result = new PageReturn<AracTalepTableDto>();
                int pageSize = filterPageParam.size ?? 10;
                int pageNumber = filterPageParam.page ?? 0;

                int? createdUser = filterPageParam.liste?.filterCreatedUser;
                int? userId = filterPageParam.liste?.userId;
                IQueryable<Data.Models.AracTalepTable> query = dal.Get(u => u.enabled && u.currentStateId == 1 &&
                userId == null ? true :
                u.currentUserId == userId
                && ((createdUser == null || createdUser == 0) ? true : u.createdUserId == createdUser)).Include(u => u.createdUser)
                    .Include(u => u.destinationLocation);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)
                  .OrderByDescending(u => u.Id)
                    .Select(u => new AracTalepTableDto()
                    {
                        id = u.Id,
                        aciklama = u.aciklama,
                        baslangicTarihi = u.baslangicTarihi.ToString(),
                        createdUser = u.createdUser.name,
                        destinationLocation = u.destinationLocation.destinationLocation,
                        createdUserId = u.createdUserId,
                        onaySirasi = u.onaySirasi,
                        plaka = u.plaka,
                        teslimTarihi = u.teslimTarihi.ToString(),

                    }).OrderByDescending(u => u.id).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public PageReturn<AracTalepTableDto>? activeListdto(FilterPageParam<AracTalepTableParamsDto> filterPageParam, int userRoleId)
            {
                PageReturn<AracTalepTableDto>? result = new PageReturn<AracTalepTableDto>();
                int pageSize = filterPageParam.size ?? 10;
                int pageNumber = filterPageParam.page ?? 0;

                int? createdUser = filterPageParam.liste?.filterCreatedUser;
                int? userId = filterPageParam.liste?.userId;

                BLL.BLLActions.RoleDetails bllRoleDetails = new BLL.BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(userRoleId, (int)CommonConstants.MODULES.ARACTALEP);
                if (userRoleId == 1 || (roleDetail != null && roleDetail.canSeeLogs))
                {

                    IQueryable<Data.Models.AracTalepTable> query = dal.Get(u => u.enabled && u.currentStateId == 1
                    && ((createdUser == null || createdUser == 0) ? true : u.createdUserId == createdUser)).Include(u => u.createdUser)
                        .Include(u => u.destinationLocation);
                    result.content = query
                      .Skip(pageSize * pageNumber).Take(pageSize)
                      .OrderByDescending(u => u.Id)
                        .Select(u => new AracTalepTableDto()
                        {
                            id = u.Id,
                            aciklama = u.aciklama,
                            baslangicTarihi = u.baslangicTarihi.ToString(),
                            createdUser = u.createdUser.name,
                            destinationLocation = u.destinationLocation.destinationLocation,
                            createdUserId = u.createdUserId,
                            onaySirasi = u.onaySirasi,
                            plaka = u.plaka,
                            teslimTarihi = u.teslimTarihi.ToString(),

                        }).OrderByDescending(u => u.id).ToList();
                    result.totalElements = query.Count();
                    result.number = result.content.Count();
                    result.size = pageSize;
                }
                else
                {
                    IQueryable<Data.Models.AracTalepTable> query = dal.Get(u => u.enabled && u.currentStateId == 1 &&
                    userId == null ? true :
                    (u.createdUserId == userId || u.createdUser.aracOnayId == userId)
                    && ((createdUser == null || createdUser == 0) ? true : u.createdUserId == createdUser)).Include(u => u.createdUser)
                        .Include(u => u.destinationLocation);
                    result.content = query
                      .Skip(pageSize * pageNumber).Take(pageSize)
                      .OrderByDescending(u => u.Id)
                        .Select(u => new AracTalepTableDto()
                        {
                            id = u.Id,
                            aciklama = u.aciklama,
                            baslangicTarihi = u.baslangicTarihi.ToString(),
                            createdUser = u.createdUser.name,
                            destinationLocation = u.destinationLocation.destinationLocation,
                            createdUserId = u.createdUserId,
                            onaySirasi = u.onaySirasi,
                            plaka = u.plaka,
                            teslimTarihi = u.teslimTarihi.ToString(),

                        }).OrderByDescending(u => u.id).ToList();
                    result.totalElements = query.Count();
                    result.number = result.content.Count();
                    result.size = pageSize;
                }




                return result;
            }



            public PageReturn<AracTalepTableDto>? completedListdto(FilterPageParam<AracTalepTableParamsDto> filterPageParam, int userRoleId)
            {
                PageReturn<AracTalepTableDto>? result = new PageReturn<AracTalepTableDto>();
                int pageSize = filterPageParam.size ?? 10;
                int pageNumber = filterPageParam.page ?? 0;

                int? createdUser = filterPageParam.liste?.filterCreatedUser;
                int? userId = filterPageParam.liste?.userId;

                BLL.BLLActions.RoleDetails bllRoleDetails = new BLL.BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(userRoleId, (int)CommonConstants.MODULES.ARACTALEP);
                if (userRoleId == 1 || (roleDetail != null && roleDetail.canSeeLogs))
                {

                    IQueryable<Data.Models.AracTalepTable> query = dal.Get(u => u.enabled && u.currentStateId == 4
                    && ((createdUser == null || createdUser == 0) ? true : u.createdUserId == createdUser)).Include(u => u.createdUser)
                        .Include(u => u.destinationLocation);
                    result.content = query
                      .Skip(pageSize * pageNumber).Take(pageSize)
                   .OrderByDescending(u => u.Id)
                        .Select(u => new AracTalepTableDto()
                        {
                            id = u.Id,
                            aciklama = u.aciklama,
                            baslangicTarihi = u.baslangicTarihi.ToString(),
                            createdUser = u.createdUser.name,
                            destinationLocation = u.destinationLocation.destinationLocation,
                            createdUserId = u.createdUserId,
                            onaySirasi = u.onaySirasi,
                            plaka = u.plaka,
                            teslimTarihi = u.teslimTarihi.ToString(),

                        }).OrderByDescending(u => u.id).ToList();
                    result.totalElements = query.Count();
                    result.number = result.content.Count();
                    result.size = pageSize;
                }
                else
                {
                    IQueryable<Data.Models.AracTalepTable> query = dal.Get(u => u.enabled && u.currentStateId == 4 &&
                    userId == null ? true :
                    (u.createdUserId == userId || u.createdUser.aracOnayId == userId)
                    && ((createdUser == null || createdUser == 0) ? true : u.createdUserId == createdUser)).Include(u => u.createdUser)
                        .Include(u => u.destinationLocation);
                    result.content = query
                      .Skip(pageSize * pageNumber).Take(pageSize)
                      .OrderByDescending(u => u.Id)
                        .Select(u => new AracTalepTableDto()
                        {
                            id = u.Id,
                            aciklama = u.aciklama,
                            baslangicTarihi = u.baslangicTarihi.ToString(),
                            createdUser = u.createdUser.name,
                            destinationLocation = u.destinationLocation.destinationLocation,
                            createdUserId = u.createdUserId,
                            onaySirasi = u.onaySirasi,
                            plaka = u.plaka,
                            teslimTarihi = u.teslimTarihi.ToString(),

                        }).OrderByDescending(u => u.id).ToList();
                    result.totalElements = query.Count();
                    result.number = result.content.Count();
                    result.size = pageSize;
                }




                return result;
            }

            public async Task<int> reject(int talepId, int userId)
            {
                try
                {
                    BLL.BLLActions.AdminUsers bllAdminUsers = new BLL.BLLActions.AdminUsers(_configuration, _env, _mapper);
                    Data.Models.AracTalepTable aracTalepTable = GetByID(talepId)!;
                    Data.Models.AdminUser? user = bllAdminUsers.GetByID(aracTalepTable?.createdUserId??0);
                    
                    int donenDeger = 0;
                    try
                    {
                        aracTalepTable!.approval=false;
                        aracTalepTable.currentStateId=2;
                        await Update(aracTalepTable);
                        donenDeger = 1;

                    }
                    catch (Exception)
                    {
                        donenDeger = 2;
                    }

                    try
                    {
                        BLL.BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLL.BLLActions.AracTalepTableDetail(_configuration,_env);

                        Data.Models.AracTalepTableDetail aracTalepTableDetail = bllAracTalepTableDetail.getByActiveNull(talepId,
                                userId);
                        aracTalepTableDetail.approved=false;
                        aracTalepTableDetail.isReplied=true;
                        aracTalepTableDetail.replyDate = DateTime.Now;
                        await bllAracTalepTableDetail.Update(aracTalepTableDetail);
                        donenDeger = 1;
                    }
                    catch (Exception )
                    {
                        donenDeger = 2;
                    }
                    BLL.BLLActions.EmailMessages bllEmailMessages = new BLL.BLLActions.EmailMessages(_configuration,_env);
                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject="Bekleyen Araç Talebi hk.";
                    emailMessage.toAddress=user?.email;
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration,_env,"Sayın "+user?.name+" Araç Talebi hk.",
                            talepId.ToString() + " ID'li araç talebiniz reddedilmiştir.");
                    emailMessage.emailText=(mailMessage);
                    emailMessage.mailTuru=(1);
                    emailMessage.enabled=(true);
                    emailMessage.isSent=(false);
                    emailMessage.plannedDate=DateTime.Now;
                    await bllEmailMessages.Add(emailMessage);
                    return donenDeger;
                }
                catch (Exception e)
                {
                    Console.WriteLine(talepId.ToString()
                            + " id'li araç talebi red edemiyor. Hata: " + e.Message);
                    return 2;
                }
            }

            public async Task<int> confirm(int talepId, int userId)
            {
                try
                {

                    int donenDeger = 0;
                    Data.Models.AracTalepTable aracTalepTable = GetByID(talepId)!;
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration,_env, _mapper);
                    AdminUser createdUser = bllAdminUsers.GetByID(aracTalepTable.createdUserId??0)!;

                    if (userId.Equals(createdUser.Id))
                    {
                        if (createdUser.manager1 == null)
                        {
                            // hata
                            donenDeger = 2;

                        }
                        else
                        {
                            // kendi onaylıyor manager 1den devam
                            aracTalepTable.currentUserId=(createdUser.manager1);
                            aracTalepTable.onaySirasi=1;
                            aracTalepTable.currentStateId=1;
                            await Update(aracTalepTable);

                            AdminUser manager1 = bllAdminUsers.GetByID(createdUser.manager1 ??0)!;
                            BLL.BLLActions.EmailMessages bllEmailMessages = new BLL.BLLActions.EmailMessages(_configuration,_env);
                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject="Bekleyen Araç Talebi hk.";
                            emailMessage.toAddress=manager1.email;
                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager1.name +  " Araç Talebi hk.",
                                    talepId.ToString() + " ID'li araç talebi onayınızı beklemektedir.");

                            emailMessage.emailText=mailMessage;
                            emailMessage.mailTuru=1;
                            emailMessage.enabled=true;
                            emailMessage.isSent=false;
                            emailMessage.plannedDate=DateTime.Now;
                            await bllEmailMessages.Add(emailMessage);

                            BLL.BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLL.BLLActions.AracTalepTableDetail(_configuration, _env);

                            Data.Models.AracTalepTableDetail aracTalepTableDetail = bllAracTalepTableDetail.getByActiveNull(talepId,
                                    aracTalepTable.createdUserId??0);
                            aracTalepTableDetail.approved=(true);
                            aracTalepTableDetail.isReplied=(true);
                            aracTalepTableDetail.replyDate=(DateTime.Now);
                            await bllAracTalepTableDetail.Update(aracTalepTableDetail);

                            Data.Models.AracTalepTableDetail aracTalepTableDetailNext = new Data.Models.AracTalepTableDetail();
                            aracTalepTableDetailNext.talepId=(talepId);
                            aracTalepTableDetailNext.createdDate=(DateTime.Now);
                            aracTalepTableDetailNext.userId=(createdUser.manager1);
                            aracTalepTableDetailNext.enabled=(true);
                            aracTalepTableDetailNext.isReplied=(false);
                            aracTalepTableDetailNext.guid= Guid.NewGuid();
                            await bllAracTalepTableDetail.Add(aracTalepTableDetailNext);
                            donenDeger = 1;

                        }

                    }
                    else if (userId.Equals(createdUser.manager1))
                    {
                        if (createdUser.aracOnayId == null)
                        {
                            // hata
                            donenDeger = 2;
                        }
                        else
                        {
                            // manager 1 onayladı araconaylayicidan devam
                            if (createdUser.aracOnayId.Equals(createdUser.manager1))
                            {
                                aracTalepTable.currentUserId=createdUser.hrmanager;
                                aracTalepTable.onaySirasi=3;
                                aracTalepTable.currentStateId=(1);
                                await Update(aracTalepTable);
                                AdminUser hrManager = bllAdminUsers.GetByID(createdUser.hrmanager??0)!;
                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration,_env);
                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject="Bekleyen Araç Talebi hk.";
                                emailMessage.toAddress=hrManager.email;

                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + hrManager.name+ " Araç Talebi hk.",
                                        talepId.ToString() + " ID'li araç talebi onayınızı beklemektedir.");
                                emailMessage.emailText=mailMessage;
                                emailMessage.mailTuru=1;
                                emailMessage.enabled=true;
                                emailMessage.isSent=false;
                                emailMessage.plannedDate=DateTime.Now;
                                await bllEmailMessages.Add(emailMessage);

                                BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLLActions.AracTalepTableDetail(_configuration,_env); 
                                Data.Models.AracTalepTableDetail aracTalepTableDetail = bllAracTalepTableDetail.getByActiveNull(talepId,
                                        createdUser.manager1??0);
                                aracTalepTableDetail.approved=true;
                                aracTalepTableDetail.isReplied=true;
                                aracTalepTableDetail.replyDate=DateTime.Now;
                                await bllAracTalepTableDetail.Update(aracTalepTableDetail);

                                Data.Models.AracTalepTableDetail aracTalepTableDetailnext = new Data.Models.AracTalepTableDetail();
                                aracTalepTableDetailnext.talepId=talepId;
                                aracTalepTableDetailnext.createdDate = DateTime.Now;
                                aracTalepTableDetailnext.userId=createdUser.hrmanager;
                                aracTalepTableDetailnext.enabled=true;
                                aracTalepTableDetailnext.isReplied=false;
                                aracTalepTableDetailnext.guid = Guid.NewGuid();
                                await bllAracTalepTableDetail.Add(aracTalepTableDetailnext);

                                donenDeger = 1;
                            }
                            else
                            {
                                aracTalepTable.currentUserId=createdUser.aracOnayId;
                                aracTalepTable.onaySirasi=2;
                                aracTalepTable.currentStateId=1;
                                await Update(aracTalepTable);

                                AdminUser aracOnaylayici = bllAdminUsers.GetByID(createdUser.aracOnayId??0)!;
                                BLL.BLLActions.EmailMessages bllEmailMessages = new BLL.BLLActions.EmailMessages(_configuration, _env);

                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject="Bekleyen Araç Talebi hk.";
                                emailMessage.toAddress=aracOnaylayici.email;
                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                                string mailMessage =bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + aracOnaylayici.name+  " Araç Talebi hk.",
                                        talepId.ToString() + " ID'li araç talebi onayınızı beklemektedir.");

                                emailMessage.emailText=mailMessage;
                                emailMessage.mailTuru=1;
                                emailMessage.enabled=true;
                                emailMessage.isSent=false;
                                emailMessage.plannedDate=DateTime.Now;
                                await bllEmailMessages.Add(emailMessage);
                                BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLLActions.AracTalepTableDetail(_configuration, _env);

                                Data.Models.AracTalepTableDetail aracTalepTableDetail = bllAracTalepTableDetail.getByActiveNull(talepId,
                                        createdUser.manager1??0);
                                aracTalepTableDetail.approved=true;
                                aracTalepTableDetail.isReplied=true;
                                aracTalepTableDetail.replyDate=DateTime.Now;
                                await bllAracTalepTableDetail.Update(aracTalepTableDetail);

                                Data.Models.AracTalepTableDetail aracTalepTableDetailnext = new Data.Models.AracTalepTableDetail();
                                aracTalepTableDetailnext.talepId=talepId;
                                aracTalepTableDetailnext.createdDate=DateTime.Now;
                                aracTalepTableDetailnext.userId=createdUser.aracOnayId;
                                aracTalepTableDetailnext.enabled=true;
                                aracTalepTableDetailnext.isReplied=false;
                                aracTalepTableDetailnext.guid=Guid.NewGuid();
                                await bllAracTalepTableDetail.Add(aracTalepTableDetailnext);

                                donenDeger = 1;
                            }
                        }
                    }
                    else if (userId.Equals(createdUser.aracOnayId))
                    {
                        if (createdUser.hrmanager == null)
                        {
                            // hata
                            createdUser.hrmanager=6894;
                            await bllAdminUsers.Update(createdUser);
                            donenDeger = 2;
                        }
                        else
                        {
                            // arac onaylayıcı onaylıyor ik müdüründen devam

                            if (createdUser.hrmanager.Equals(createdUser.Id))
                            {
                                aracTalepTable.approval=true;
                                aracTalepTable.onaySirasi=10;
                                aracTalepTable.currentStateId=4;
                                await Update(aracTalepTable);
                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject="Bekleyen Araç Talebi hk.";
                                emailMessage.toAddress=createdUser.email;
                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + createdUser.name + " Araç Talebi hk.",
                                        talepId.ToString() + " ID'li araç talebiniz onaylanmıştır.");
                                emailMessage.emailText=mailMessage;
                                emailMessage.mailTuru=1;
                                emailMessage.enabled=true;
                                emailMessage.isSent=false;
                                emailMessage.plannedDate=DateTime.Now;
                                await bllEmailMessages.Add(emailMessage);

                                BLLActions.AracTalepTableDetail bllAracTableDetail = new BLLActions.AracTalepTableDetail(_configuration, _env);
                                Data.Models.AracTalepTableDetail aracTalepTableDetail = bllAracTableDetail.getByActiveNull(talepId,
                                        createdUser.izinOnayId??0);
                                aracTalepTableDetail.approved=true;
                                aracTalepTableDetail.isReplied=true;
                                aracTalepTableDetail.replyDate=DateTime.Now;
                                await bllAracTableDetail.Update(aracTalepTableDetail);

                                donenDeger = 3;
                            }
                            else
                            {
                                if (createdUser.aracOnayId.Equals(createdUser.hrmanager))
                                {
                                    // ik müdürü onaylıyor biticek
                                    aracTalepTable.approval=true;
                                    aracTalepTable.onaySirasi=10;
                                    aracTalepTable.currentStateId=4;
                                    await Update(aracTalepTable);
                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject="Bekleyen Araç Talebi hk.";
                                    emailMessage.toAddress=createdUser.email;
                                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + createdUser.name + " Araç Talebi hk.",
                                            talepId.ToString() + " ID'li araç talebiniz onaylanmıştır.");
                                    emailMessage.emailText=mailMessage;
                                    emailMessage.mailTuru=1;
                                    emailMessage.enabled=true;
                                    emailMessage.isSent=false;
                                    emailMessage.plannedDate=DateTime.Now;
                                    await bllEmailMessages.Add(emailMessage);

                                    BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLLActions.AracTalepTableDetail(_configuration, _env);
                                    Data.Models.AracTalepTableDetail aracTalepTableDetail = bllAracTalepTableDetail
                                            .getByActiveNull(talepId, createdUser.hrmanager??0);
                                    aracTalepTableDetail.approved=true;
                                    aracTalepTableDetail.isReplied=true;
                                    aracTalepTableDetail.replyDate=DateTime.Now;
                                    await bllAracTalepTableDetail.Update(aracTalepTableDetail);

                                    donenDeger = 3;
                                }
                                else
                                {
                                    aracTalepTable.currentUserId=createdUser.hrmanager;
                                    aracTalepTable.onaySirasi=3;
                                    aracTalepTable.currentStateId=1;
                                    await Update(aracTalepTable);

                                    Data.Models.AdminUser hrManager = bllAdminUsers.GetByID(createdUser.hrmanager??0)!;
                                    BLLActions.EmailMessages bllEmailMessages=new BLLActions.EmailMessages(_configuration, _env);   
                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject="Bekleyen Araç Talebi hk.";
                                    emailMessage.toAddress=hrManager.email;
                                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + hrManager.name + " Araç Talebi hk.",
                                            talepId.ToString() + " ID'li araç talebi onayınızı beklemektedir.");
                                    emailMessage.emailText=mailMessage;
                                    emailMessage.mailTuru=1;
                                    emailMessage.enabled=true;
                                    emailMessage.isSent=false;
                                    emailMessage.plannedDate=DateTime.Now;
                                    await bllEmailMessages.Add(emailMessage);

                                    BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLLActions.AracTalepTableDetail(_configuration,_env);
                                    Data.Models.AracTalepTableDetail aracTalepTableDetail = bllAracTalepTableDetail
                                            .getByActiveNull(talepId, createdUser.aracOnayId??0);
                                    aracTalepTableDetail.approved=true;
                                    aracTalepTableDetail.isReplied=true;
                                    aracTalepTableDetail.replyDate=DateTime.Now;
                                    await bllAracTalepTableDetail.Update(aracTalepTableDetail);

                                    Data.Models.AracTalepTableDetail aracTalepTableDetailnext = new Data.Models.AracTalepTableDetail();
                                    aracTalepTableDetailnext.talepId=talepId;
                                    aracTalepTableDetailnext.createdDate=DateTime.Now;
                                    aracTalepTableDetailnext.userId=createdUser.hrmanager;
                                    aracTalepTableDetailnext.enabled=true;
                                    aracTalepTableDetailnext.isReplied=false;
                                    aracTalepTableDetailnext.guid=Guid.NewGuid();
                                    await bllAracTalepTableDetail.Add(aracTalepTableDetailnext);

                                    donenDeger = 1;
                                }
                            }

                        }
                    }

                    else if (userId.Equals(createdUser.hrmanager))
                    {

                        // ik müdürü onaylıyor biticek
                        aracTalepTable.approval=true;
                        aracTalepTable.onaySirasi=10;
                        aracTalepTable.currentStateId=4;
                        await Update(aracTalepTable);

                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject="Bekleyen Araç Talebi hk.";
                        emailMessage.toAddress=createdUser.email;
                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + createdUser.name + " Araç Talebi hk.",
                                talepId.ToString() + " ID'li araç talebiniz onaylanmıştır.");
                        emailMessage.emailText=mailMessage;
                        emailMessage.mailTuru=1;
                        emailMessage.enabled=true;
                        emailMessage.isSent=false;
                        emailMessage.plannedDate=DateTime.Now;
                        await bllEmailMessages.Add(emailMessage);

                        BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLLActions.AracTalepTableDetail(_configuration, _env);

                        Data.Models.AracTalepTableDetail aracTalepTableDetail = bllAracTalepTableDetail.getByActiveNull(talepId,
                                createdUser.hrmanager??0);
                        aracTalepTableDetail.approved=true;
                        aracTalepTableDetail.isReplied=true;
                        aracTalepTableDetail.replyDate=DateTime.Now;
                        await bllAracTalepTableDetail.Update(aracTalepTableDetail);

                        donenDeger = 3;

                    }
                    return donenDeger;
                }
                catch (Exception e)
                {
                    Console.WriteLine(talepId.ToString()
                            + " id'li araç talebi onaylayamıyor. Hata: " + e.Message);
                    
                    return 4;
                }
            }

            public List<Data.Models.AracTalepTable> findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(int? userOld,
            int currentStateId, Boolean enabled, int userId)
            {
                List<Data.Models.AracTalepTable> liste = dal.Get(u => u.currentUserId == userOld && u.currentStateId == currentStateId && u.enabled == enabled && u.createdUserId == userId).ToList();
                return liste;
            }
        }
    }

}
