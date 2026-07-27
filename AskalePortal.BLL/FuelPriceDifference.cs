using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class FuelPriceDifference : BaseBLL<AskalePortal.Data.Models.FuelPriceDifference>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public FuelPriceDifference(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env; _mapper = mapper;
            }

            public List<Data.Models.FuelPriceDifference> activeMyApprovalList(FilterParam<FuelPriceDifferenceListDtoParameter> filterParam)
            {
                int userId = filterParam.liste?.filterUser ?? 0;
                List<Data.Models.FuelPriceDifference> liste = dal.Get(u => u.enabled && u.currentUserId == userId && u.currentStateId == 1).OrderByDescending(u => u.Id).ToList();
                return liste;

            }

            public int approvalCount(int userId)
            {
                int count = dal.Get(u => u.enabled && u.currentUserId == userId && u.currentStateId == 1).Count();
                return count;
            }

            public List<Data.Models.FuelPriceDifference> listActive(int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? adminUser = bllAdminUsers.GetByID(userId);
                if (adminUser?.roleId == 1)
                {
                    return dal.Get(u => u.enabled && u.approval == null).OrderByDescending(u => u.Id).ToList();
                }
                else
                {
                    return dal.Get(u => u.enabled && u.approval == null && u.userId == userId).OrderByDescending(u => u.Id).ToList();
                }
            }

            public FuelPriceDifferenceRaporDto createReport(DateTime date, int? companyId)
            {
                BLLActions.DieselPrice bllDieselPrice = new BLLActions.DieselPrice(_configuration, _env, _mapper);
                Data.Models.DieselPrice dieselPriceFirst = bllDieselPrice.dieselPriceByDate(date, companyId, true);

                Data.Models.DieselPrice dieselPriceSecond;
                if (dieselPriceFirst == null)
                {
                    FuelPriceDifferenceRaporDto differenceRaporDto = new FuelPriceDifferenceRaporDto();

                    return differenceRaporDto;
                }
                else
                {
                    if (bllDieselPrice.dieselPriceByDate(dieselPriceFirst.girisTarihi.AddDays(-1), companyId,
                            true) != null)
                    {
                        dieselPriceSecond = bllDieselPrice
                                .dieselPriceByDate(dieselPriceFirst.girisTarihi.AddDays(-1), companyId, true);
                    }
                    else
                    {
                        dieselPriceSecond = dieselPriceFirst;
                    }
                    List<Data.Models.FuelPriceDifference> listFuelPriceDifference =
                            findByEnabledAndApprovalAndCompanyId(true, true, companyId, DateTime.Now);
                    List<FuelPriceDifferenceModelDto> listFuelPriceDifferenceDto = new List<FuelPriceDifferenceModelDto>();

                    FuelPriceDifferenceRaporDto fuelPriceDifferenceRaporDto = new FuelPriceDifferenceRaporDto();
                    BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                    ApprovalProcess approvalProcess = bllApprovalProcesses.findByCompanyIdAndTypeIdAndEnabled(companyId ?? 0,
                            (int)CommonConstants.APPROVAL_PROCESSES.YAKITSOZLESME);
                    BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                    List<ApprovalProcessDetail> listFuelPriceDifferenceApprover = bllApprovalProcessDetails
                            .findByProcessIdAndEnabledOrderByDataOrderAsc(approvalProcess.Id);
                    decimal guncelMotorin = dieselPriceFirst.fiyat;
                    decimal oncekiMotorin = dieselPriceSecond.fiyat;
                    decimal firstKdv = (dieselPriceFirst.kdvRate / (100)) + 1;

                    decimal secondKdv = dieselPriceSecond.kdvRate / (100)
                            + ((1));
                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                    Company? company = bllCompanies.GetByID(companyId ?? 0);
                    fuelPriceDifferenceRaporDto.companyName = (company?.companyLongName);
                    foreach (Data.Models.FuelPriceDifference fuelPriceDifference in listFuelPriceDifference)
                    {
                        FuelPriceDifferenceModelDto fuelPriceDifferenceDto = new FuelPriceDifferenceModelDto();
                        fuelPriceDifferenceDto.yukleniciFirma = (fuelPriceDifference.yukleniciFirma);
                        fuelPriceDifferenceDto.isinAdi = (fuelPriceDifference.isinAdi);
                        fuelPriceDifferenceDto.km = (fuelPriceDifference.km ?? 0);
                        fuelPriceDifferenceDto.nevi = (fuelPriceDifference.nevi);
                        fuelPriceDifferenceDto.katsayi = (fuelPriceDifference.katSayi);
                        Data.Models.DieselPrice dieselPriceMain;
                        // HATALI yer burası
                        if (bllDieselPrice.dieselPriceByDate(fuelPriceDifference.fiyatTarih ?? DateTime.Now, companyId,
                                true) != null)
                        {
                            dieselPriceMain = bllDieselPrice.dieselPriceByDate(fuelPriceDifference.fiyatTarih ?? DateTime.Now,
                                    companyId, true);
                        }
                        else
                        {
                            dieselPriceMain = dieselPriceFirst;
                        }
                        decimal mainKdv = (dieselPriceMain.kdvRate) / (100) + (1);
                        decimal motorinFarkiOld = (((oncekiMotorin / (secondKdv)) - (dieselPriceMain.fiyat / (mainKdv))));
                        decimal motorinFarkiNew = (((guncelMotorin / (firstKdv)) - (dieselPriceMain.fiyat / (mainKdv))));

                        decimal eskiFiyat;
                        decimal yeniFiyat;
                        if (fuelPriceDifference.km != 0)
                        {
                            eskiFiyat = (((fuelPriceDifference.katSayi * (motorinFarkiOld))
                                    + (fuelPriceDifference.fiyat)));

                            yeniFiyat = (((fuelPriceDifference.katSayi * (motorinFarkiNew))
                                    + (fuelPriceDifference.fiyat)));
                        }
                        else
                        {
                            eskiFiyat = (((fuelPriceDifference.katSayi * (motorinFarkiOld))
                                    + (fuelPriceDifference.fiyat * (fuelPriceDifference.km)))) ?? 0;
                            yeniFiyat = (((fuelPriceDifference.katSayi * (motorinFarkiNew))
                                    + (fuelPriceDifference.fiyat * (fuelPriceDifference.km)))) ?? 0;
                        }

                        fuelPriceDifferenceDto.eskiFiyat = (eskiFiyat);
                        fuelPriceDifferenceDto.yenifiyat = (yeniFiyat);
                        listFuelPriceDifferenceDto.Add(fuelPriceDifferenceDto);
                    }
                    fuelPriceDifferenceRaporDto.eskiMotorin = (oncekiMotorin);
                    fuelPriceDifferenceRaporDto.liste = (listFuelPriceDifferenceDto);
                    fuelPriceDifferenceRaporDto.tarih = (DateTime.Now);
                    fuelPriceDifferenceRaporDto.yeniMotorin = (guncelMotorin);
                    fuelPriceDifferenceRaporDto.kdvDahil = (guncelMotorin - (oncekiMotorin));

                    decimal firstwithoutkdv = guncelMotorin / (firstKdv);
                    decimal secondwithoutkdv = oncekiMotorin / (secondKdv);
                    fuelPriceDifferenceRaporDto.kdvHaric = (firstwithoutkdv - (secondwithoutkdv));
                    List<FuelDifferenceApproverDto> fuelDifferenceApproverDtos = new List<FuelDifferenceApproverDto>();
                    foreach (ApprovalProcessDetail fuelPriceDifferenceApprover in listFuelPriceDifferenceApprover)
                    {
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        UserByNameEMailDto user = bllAdminUsers.getUserByNameEMailDto(fuelPriceDifferenceApprover.userId);
                        FuelDifferenceApproverDto fuelDifferenceApproverDto = new FuelDifferenceApproverDto();
                        fuelDifferenceApproverDto.siraNo = (fuelPriceDifferenceApprover.dataOrder);
                        fuelDifferenceApproverDto.name = (user.name);
                        fuelDifferenceApproverDtos.Add(fuelDifferenceApproverDto);
                    }
                    fuelPriceDifferenceRaporDto.listOnaylayici = (fuelDifferenceApproverDtos);
                    return fuelPriceDifferenceRaporDto;
                }
            }

            public List<Data.Models.FuelPriceDifference> findByEnabledAndApprovalAndCompanyId(bool enabled, bool approval, int? companyId, DateTime date)
            {
                return dal.Get(u => u.enabled == enabled && u.approval == approval && u.companyId == companyId && u.sozlesmeBitis > date).ToList();
            }

            public List<Data.Models.FuelPriceDifference> listCompleted(int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? currentUser = bllAdminUsers.GetByID(userId);
                if (currentUser?.roleId == 1)
                {
                    List<Data.Models.FuelPriceDifference> representativeExpenseTable = dal.Get(u => u.enabled && u.currentStateId != 1).OrderByDescending(u => u.Id).ToList();
                    return representativeExpenseTable;
                }
                else
                {

                    List<Data.Models.FuelPriceDifference> representativeExpenseTable = dal.Get(u => u.enabled && u.currentStateId != 1 && u.userId == userId).OrderByDescending(u => u.Id).ToList();

                    return representativeExpenseTable;
                }
            }

            public async Task<Data.Models.FuelPriceDifference> saveSozlesmeBitisTarih(Data.ResponseModels.FuelPriceDifferenceDto fuelPriceDifference)
            {

                Data.Models.FuelPriceDifference? saveFuelPriceDifference = await Update(_mapper.Map<Data.Models.FuelPriceDifference>(fuelPriceDifference));
                return saveFuelPriceDifference;
            }

            public async Task<Data.Models.FuelPriceDifference> save(Data.ResponseModels.FuelPriceDifferenceDto entity, int userId)
            {

                BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                ApprovalProcess approvalProcess = bllApprovalProcesses.findByCompanyIdAndTypeIdAndEnabled(
             entity.companyId ?? 0, (int)CommonConstants.APPROVAL_PROCESSES.YAKITSOZLESME);
                BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper       );

                List<ApprovalProcessDetail> listApprovalProcessDetail = bllApprovalProcessDetails
                        .findByProcessIdAndEnabledOrderByDataOrderAsc(approvalProcess.Id);
                int? onaylayici1Id = listApprovalProcessDetail.Find(t => t.dataOrder == 1)?
                        .userId;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? onaylayici1 = bllAdminUsers.GetByID(onaylayici1Id ?? 0);
                if (entity.id == null)
                {

                    entity.createdUserId = (userId);
                    entity.userId = (userId);
                    entity.createdDate = (DateTime.Now.ToString());
                    entity.enabled = (true);
                    entity.currentUserId = (onaylayici1?.Id);
                    entity.onaySirasi = (1);
                    entity.currentStateId = (1);

                    Data.Models.FuelPriceDifference? fuelPriceDifference = await Add(_mapper.Map<Data.Models.FuelPriceDifference>(entity));
                    Data.Models.FuelPriceDifferenceDetail fuelPriceDifferenceDetail = new Data.Models.FuelPriceDifferenceDetail();
                    fuelPriceDifferenceDetail.enabled = (true);
                    fuelPriceDifferenceDetail.approved = (null);
                    fuelPriceDifferenceDetail.fuelId = (fuelPriceDifference?.Id);
                    fuelPriceDifferenceDetail.userId = (onaylayici1?.Id);
                    fuelPriceDifferenceDetail.guid = Guid.NewGuid();
                    fuelPriceDifferenceDetail.createdDate = (DateTime.Now);
                    BLLActions.FuelPriceDifferenceDetail bllFuelPriceDifferenceDetail = new BLLActions.FuelPriceDifferenceDetail(_configuration, _env);
                    await bllFuelPriceDifferenceDetail.Add(fuelPriceDifferenceDetail);

                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = ("Bekleyen Yakıt Fiyat Farkı Kayıt Onayı hk.");
                    emailMessage.toAddress = (onaylayici1?.email);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + onaylayici1?.name +
                    " Yakıt Fiyat Farkı Kayıt Onayı hk.",
                            fuelPriceDifference?.Id.ToString() + " ID'li kayıt onayınızı beklemektedir");
                    emailMessage.emailText = (mailMessage);
                    emailMessage.mailTuru = (1);
                    emailMessage.enabled = (true);
                    emailMessage.isSent = (false);
                    emailMessage.plannedDate = (DateTime.Now);
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    await bllEmailMessages.Add(emailMessage);
                    return fuelPriceDifference!;

                }
                else
                {
                    Data.Models.FuelPriceDifference? fuelPriceDifferenceOld = GetByID(entity.id ?? 0);

                    if (fuelPriceDifferenceOld?.fiyat.CompareTo(entity.fiyat) != 1)
                    {
                        ////////////////////////////
                        //				List<FuelPriceDifferenceDetail> list = fuelPriceDifferenceDetailRepository.findByEnabledAndFuelId(true, entity.getId());
                        //				for (FuelPriceDifferenceDetail fuelPriceDifferenceDetail : list) {
                        //					fuelPriceDifferenceDetailRepository.deleteByIdTemprory(fuelPriceDifferenceDetail.getId());
                        //				}
                        ////////////////////////////
                        // fiyat değişmişse
                        BLLActions.FuelPriceDifferenceDetail bllFuelPriceDifferenceDetail = new BLLActions.FuelPriceDifferenceDetail(_configuration, _env);
                        List<Data.Models.FuelPriceDifferenceDetail> silinecekListe = bllFuelPriceDifferenceDetail
                                .getByActiveFuelId(entity.id ?? 0);
                        foreach (Data.Models.FuelPriceDifferenceDetail fuelPriceDifferenceDetailSil in silinecekListe)
                        {
                            bllFuelPriceDifferenceDetail.Delete(fuelPriceDifferenceDetailSil.Id);
                        }

                        entity.createdUserId = (userId);
                        entity.userId = (userId);
                        entity.createdDate = (DateTime.Now.ToString());
                        entity.enabled = (true);
                        entity.editId = (1);// fiyatdeğişti1
                        entity.currentUserId = (onaylayici1?.Id);
                        entity.onaySirasi = (1);
                        entity.currentStateId = (1);

                        //				FuelPriceDifferenceDetail fuelPriceDifferenceDetailBefore = fuelPriceDifferenceDetailRepository
                        //						.getByActive(entity.getId(), listApprovalProcessDetail.stream()
                        //								.filter(t -> t.getDataOrder() == entity.getOnaySirasi()).findFirst().get().getUserId());
                        //				fuelPriceDifferenceDetailBefore.setEnabled(false);
                        //				fuelPriceDifferenceDetailBefore.setReplyDate(LocalDateTime.now());
                        //				fuelPriceDifferenceDetailRepository.save(fuelPriceDifferenceDetailBefore);

                        Data.Models.FuelPriceDifference fuelPriceDifference = await Update(_mapper.Map<Data.Models.FuelPriceDifference>(entity));
                        Data.Models.FuelPriceDifferenceDetail fuelPriceDifferenceDetail = new Data.Models.FuelPriceDifferenceDetail();
                        fuelPriceDifferenceDetail.enabled = (true);
                        fuelPriceDifferenceDetail.approved = (null);
                        fuelPriceDifferenceDetail.fuelId = (fuelPriceDifference.Id);
                        fuelPriceDifferenceDetail.userId = (onaylayici1?.Id);
                        fuelPriceDifferenceDetail.guid = Guid.NewGuid();
                        fuelPriceDifferenceDetail.createdDate = (DateTime.Now);
                        await bllFuelPriceDifferenceDetail.Add(fuelPriceDifferenceDetail);

                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Yakıt Fiyat Farkı Kayıt Onayı hk.");
                        emailMessage.toAddress = (onaylayici1?.email);

                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + onaylayici1?.name +
                    " Yakıt Fiyat Farkı Kayıt Onayı hk.",
                            fuelPriceDifference.Id.ToString() + " ID'li kayıt onayınızı beklemektedir");
                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (1);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);
                        return fuelPriceDifference;

                    }
                    else if (fuelPriceDifferenceOld.km != (decimal)(entity.km ?? 0))
                    {
                        // km değişmişse
                        BLLActions.FuelPriceDifferenceDetail bllFuelPriceDifferenceDetail = new BLLActions.FuelPriceDifferenceDetail(_configuration, _env);
                        List<Data.Models.FuelPriceDifferenceDetail> silinecekListe = bllFuelPriceDifferenceDetail
                                .getByActiveFuelId(entity.id ?? 0);
                        foreach (Data.Models.FuelPriceDifferenceDetail fuelPriceDifferenceDetailsil in silinecekListe)
                        {
                            bllFuelPriceDifferenceDetail.Delete(fuelPriceDifferenceDetailsil.Id);
                        }

                        entity.createdUserId = (userId);
                        entity.userId = (userId);
                        entity.createdDate = (DateTime.Now.ToString());
                        entity.enabled = (true);
                        entity.editId = (2);// km değişti 2
                        entity.currentUserId = (onaylayici1?.Id);
                        entity.onaySirasi = (1);
                        entity.currentStateId = (1);

                        //				FuelPriceDifferenceDetail fuelPriceDifferenceDetailBefore = fuelPriceDifferenceDetailRepository
                        //						.getByActive(entity.getId(), listApprovalProcessDetail.stream()
                        //								.filter(t -> t.getDataOrder() == entity.getOnaySirasi()).findFirst().get().getUserId());
                        //				fuelPriceDifferenceDetailBefore.setEnabled(false);
                        //				fuelPriceDifferenceDetailBefore.setReplyDate(LocalDateTime.now());
                        //				fuelPriceDifferenceDetailRepository.save(fuelPriceDifferenceDetailBefore);

                        Data.Models.FuelPriceDifference? fuelPriceDifference = await Update(_mapper.Map<Data.Models.FuelPriceDifference>(entity));
                        Data.Models.FuelPriceDifferenceDetail fuelPriceDifferenceDetail = new Data.Models.FuelPriceDifferenceDetail();
                        fuelPriceDifferenceDetail.enabled = (true);
                        fuelPriceDifferenceDetail.approved = (null);
                        fuelPriceDifferenceDetail.fuelId = (fuelPriceDifference.Id);
                        fuelPriceDifferenceDetail.userId = (onaylayici1?.Id);
                        fuelPriceDifferenceDetail.guid = Guid.NewGuid();
                        fuelPriceDifferenceDetail.createdDate = (DateTime.Now);
                        await bllFuelPriceDifferenceDetail.Add(fuelPriceDifferenceDetail);

                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Yakıt Fiyat Farkı Kayıt Onayı hk.");
                        emailMessage.toAddress = (onaylayici1?.email);

                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + onaylayici1?.name +
                    " Yakıt Fiyat Farkı Kayıt Onayı hk.",
                            fuelPriceDifference.Id.ToString() + " ID'li kayıt onayınızı beklemektedir");
                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (1);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);
                        return fuelPriceDifference;
                    }
                    else
                    {
                        entity.updatedUserId = (userId);
                        entity.updateDate = (DateTime.Now.ToString());
                        entity.enabled = (true);
                        return await Update(_mapper.Map<Data.Models.FuelPriceDifference>(entity));
                    }

                }
            }

            public async Task<int> confirmSave(int id, int userId)
            {
                Data.Models.FuelPriceDifference? fuelPriceDifference = GetByID(id);
                BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                ApprovalProcess approvalProcess = bllApprovalProcesses.findByCompanyIdAndTypeIdAndEnabled(
                        fuelPriceDifference?.companyId ?? 0,
                        (int)CommonConstants.APPROVAL_PROCESSES.YAKITSOZLESME);
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? fuelUser = bllAdminUsers.GetByID(fuelPriceDifference?.userId ?? 0);
                BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                List<ApprovalProcessDetail> listApprovalProcessDetail = bllApprovalProcessDetails
                        .findByProcessIdAndEnabledOrderByDataOrderAsc(approvalProcess.Id);
                if (listApprovalProcessDetail.Find
                        (t => t.dataOrder == fuelPriceDifference?.onaySirasi) != null)
                {
                    int deger = (fuelPriceDifference?.onaySirasi + 1) ?? 0;
                    if (listApprovalProcessDetail.FindAll(t => t.dataOrder == deger) == null)
                    {
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Yakıt Fiyat Farkı Kayıt Onayı hk.");
                        emailMessage.toAddress = (fuelUser?.email);

                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + fuelUser?.name +
                    " Yakıt Fiyat Farkı Kayıt Onayı hk.",
                            fuelPriceDifference?.Id.ToString() + " ID'li kayıt onaylanmıştır.");

                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (1);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        await bllEmailMessages.Add(emailMessage);
                        BLLActions.FuelPriceDifferenceDetail bllFuelPriceDifferenceDetail = new BLLActions.FuelPriceDifferenceDetail(_configuration, _env);
                        Data.Models.FuelPriceDifferenceDetail fuelPriceDifferenceDetail = bllFuelPriceDifferenceDetail.getByActive(
                                fuelPriceDifference!.Id,
                                listApprovalProcessDetail.FindAll(t => t.dataOrder == fuelPriceDifference.onaySirasi).First()
                                        .userId);
                        fuelPriceDifferenceDetail.approved = (true);
                        fuelPriceDifferenceDetail.isReplied = (true);
                        fuelPriceDifferenceDetail.replyDate = (DateTime.Now);
                        await bllFuelPriceDifferenceDetail.Update(fuelPriceDifferenceDetail);
                        // fuelPriceDifference.setOnaySirasi(10);
                        fuelPriceDifference.approval = (true);
                        fuelPriceDifference.currentStateId = (4);
                        await Update(fuelPriceDifference);

                        return 3;
                    }
                    else
                    {
                        int sirano = listApprovalProcessDetail.FindAll(t => t.dataOrder == (fuelPriceDifference?.onaySirasi)).First()
                                .userId;
                        BLLActions.FuelPriceDifferenceDetail bllFuelPriceDifferenceDetail = new BLLActions.FuelPriceDifferenceDetail(_configuration, _env);
                        Data.Models.FuelPriceDifferenceDetail fuelPriceDifferenceDetail = bllFuelPriceDifferenceDetail
                                .getByActive(fuelPriceDifference!.Id, sirano);
                        fuelPriceDifferenceDetail.approved = (true);
                        fuelPriceDifferenceDetail.isReplied = (true);
                        fuelPriceDifferenceDetail.replyDate = (DateTime.Now);
                        await bllFuelPriceDifferenceDetail.Update(fuelPriceDifferenceDetail);

                        Data.Models.FuelPriceDifferenceDetail fuelPriceDifferenceDetailnext = new Data.Models.FuelPriceDifferenceDetail();
                        fuelPriceDifferenceDetailnext.fuelId = (fuelPriceDifference.Id);
                        fuelPriceDifferenceDetailnext.createdDate = (DateTime.Now);
                        fuelPriceDifferenceDetailnext.userId = (listApprovalProcessDetail.Find(t => t.dataOrder == (fuelPriceDifference.onaySirasi + 1))!
                                .userId);
                        fuelPriceDifferenceDetailnext.enabled = (true);
                        fuelPriceDifferenceDetailnext.guid = Guid.NewGuid();
                        await bllFuelPriceDifferenceDetail.Add(fuelPriceDifferenceDetailnext);
                        fuelPriceDifference.currentUserId = (listApprovalProcessDetail.Find(t => t.dataOrder == (fuelPriceDifference.onaySirasi + 1))!
                                .userId);
                        fuelPriceDifference.onaySirasi = (fuelPriceDifference.onaySirasi + 1);
                        fuelPriceDifference.currentStateId = (1);
                        await Update(fuelPriceDifference);
                        AdminUser? user2 = bllAdminUsers.GetByID(fuelPriceDifference.currentUserId ?? 0);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Yakıt Fiyat Farkı Kayıt Onayı hk.");
                        emailMessage.toAddress = (user2?.email);

                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user2?.name +
                   " Yakıt Fiyat Farkı Kayıt Onayı hk.",
                           fuelPriceDifference.Id.ToString() + " ID'li kayıt onayınızı beklemektedir.");
                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (1);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);

                        return 1;
                    }

                }
                else
                {
                    return 2;
                }

            }
        }
    }
}
