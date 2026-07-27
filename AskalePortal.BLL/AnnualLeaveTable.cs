
using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.ReportDataset;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Utilities;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.BLL
{

    public partial class BLLActions
    {
        public class AnnualLeaveTable : BaseBLL<AskalePortal.Data.Models.AnnualLeaveTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public AnnualLeaveTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            public List<AskalePortal.Data.Models.AnnualLeaveTable> GetAllActive(int activePage, int pageSize)
            {
                var q = dal.Get(u => u.enabled == true && u.currentStateId == 1).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.AnnualLeaveTable> GetAllActive(int userId, int activePage, int pageSize)
            {
                var q = dal.Get(u => u.enabled == true && u.userId == userId && u.currentStateId == 1).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }
            public List<AskalePortal.Data.Models.AnnualLeaveTable> GetAllByUnApproved(int userId)
            {
                var q = dal.Get(u => u.enabled == true && u.userId == userId && u.currentStateId == 1).OrderByDescending(u => u.Id).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.AnnualLeaveTable> GetAllFinished(int activePage, int pageSize)
            {
                var q = dal.Get(u => u.enabled == true && u.currentStateId != 1).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }
            public List<AskalePortal.Data.Models.AnnualLeaveTable> GetAllFinishedWithUserId(int activePage, int pageSize, int userId, int currentUserId)
            {
                if (userId == 0)
                {
                    var q = dal.Get(u => u.enabled == true && u.currentStateId != 1 && u.currentUserId == currentUserId || u.userId == userId).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return q;
                }
                else
                {
                    var q = dal.Get(u => u.enabled == true && u.currentStateId != 1 && (u.currentUserId == currentUserId || u.userId == userId)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return q;
                }
            }
            public List<AskalePortal.Data.Models.AnnualLeaveTable> GetAllFinished(int userId, int activePage, int pageSize)
            {
                if (userId == 0)
                {
                    var q = dal.Get(u => u.enabled == true && u.currentStateId != 1).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return q;
                }
                else
                {
                    var q = dal.Get(u => u.enabled == true && u.userId == userId && u.currentStateId != 1).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return q;
                }
            }

            public List<AskalePortal.Data.Models.AnnualLeaveTable> GetAllByUnApprovedForApprove(int userId)
            {
                var q = dal.Get(u => u.enabled == true && u.currentUserId == userId && u.currentStateId == 1 && (u.siraNo == 1 || u.siraNo == 2)).OrderByDescending(u => u.Id).ToList();
                return q;
            }
            public List<AskalePortal.Data.Models.AnnualLeaveTable> GetAllByUnApprovedForApproveIK(int userId)
            {
                var q = dal.Get(u => u.enabled == true && u.currentUserId == userId && u.currentStateId == 1 && (u.siraNo == 3)).OrderByDescending(u => u.Id).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.AnnualLeaveTable> GetAllByUserId(int userId)
            {
                var q = dal.Get(u => u.enabled == true && u.userId == userId && u.currentStateId != 2).OrderByDescending(u => u.Id).ToList();
                return q;
            }

            public AnnualLeaveSapModel getAnnualLeaveSap(string perNo)
            {
                if (string.IsNullOrWhiteSpace(perNo))
                {
                    throw new ArgumentException("Personel numarası boş olamaz.", nameof(perNo));
                }

                BLLActions.SAPConnectionData bllSapConnection =
                    new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn =
                    bllSapConnection.sapConnection(_configuration, _env);

                if (sapConn == null)
                {
                    return new AnnualLeaveSapModel
                    {
                        pernr = perNo,
                        quabs = "0",
                        reabs = "0",
                        usabs = "0",
                        tcabs = "0",
                        ecabs = "0",
                        ucabs = "0"
                    };
                }

                try
                {
                    sapConn.Connect();
                    ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI045");
                    AnnualLeaveSap? result = sapFunction.Invoke<AnnualLeaveSap>(
                        input: new AnnualLeaveSapInputParams
                        {
                            perNo = perNo.Trim().PadLeft(8, '0'),
                            gjahr = DateTime.Now.Year
                        });

                    return result?.listAnualLeaveSap?.FirstOrDefault()
                        ?? new AnnualLeaveSapModel
                        {
                            pernr = perNo,
                            quabs = "0",
                            reabs = "0",
                            usabs = "0",
                            tcabs = "0",
                            ecabs = "0",
                            ucabs = "0"
                        };
                }
                finally
                {
                    try
                    {
                        sapConn.Disconnect();
                    }
                    catch
                    {
                        // Asıl SAP hatasını maskelememek için disconnect hatası yutulur.
                    }
                }
            }

            public async Task<AnnualLeaveTableSaveDto> save(AnnualLeaveTableSaveDto entity, int userId)
            {
                try
                {

                    if (entity.id == null)
                    {
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser? loggedUser = bllAdminUsers.GetByID(userId);
                        if (loggedUser == null)
                        {
                            throw new InvalidOperationException("Oturum kullanıcısı bulunamadı.");
                        }
                        if (!loggedUser.izinOnayId.HasValue || loggedUser.izinOnayId.Value <= 0)
                        {
                            throw new InvalidOperationException("Kullanıcının izin onaylayıcısı tanımlı değil.");
                        }
                        entity.createdUserId=userId;
                        entity.createdDate=DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                        entity.enabled=true;
                        Data.Models.AnnualLeaveTable annualLeaveTable = await Add(_mapper.Map<Data.Models.AnnualLeaveTable>(entity));

                        Data.Models.AnnualLeaveDetail annualLeaveDetail = new Data.Models.AnnualLeaveDetail();
                        annualLeaveDetail.isReplied=false;
                        annualLeaveDetail.userId= loggedUser?.izinOnayId??0;
                        annualLeaveDetail.createdDate=DateTime.Now;
                        annualLeaveDetail.anuId=annualLeaveTable.Id;
                        annualLeaveDetail.guid = Guid.NewGuid();
                        annualLeaveDetail.siraNo=1;
                        annualLeaveDetail.enabled=true;
                        BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail = new BLLActions.AnnualLeaveDetail(_configuration, _env);
                        await bllAnnualLeaveDetail.Add(annualLeaveDetail);

                        EmailMessage emailMessage = new EmailMessage();
                        UserByNameEMailDto nextUserEmail = bllAdminUsers
                                .getUserByNameAndEmail(loggedUser?.izinOnayId??0);
                        emailMessage.isSent=false;
                        emailMessage.toAddress=nextUserEmail.email;
                        emailMessage.mailTuru=1;
                        emailMessage.createdDate=DateTime.Now;
                        emailMessage.createdUserId=userId;
                        emailMessage.subject=annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                        string mailString = "<h2>Sayın " + nextUserEmail.name+ "</h2><br/>" + "<h4>"
                                + annualLeaveTable.Id.ToString() + " Id'li izin onayınızı beklemektedir.<br/></h4>";
                        emailMessage.emailText=mailString;
                        emailMessage.plannedDate=DateTime.Now;
                        emailMessage.enabled=true;
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);
                        return _mapper.Map<AnnualLeaveTableSaveDto> (annualLeaveTable);
                    }
                    else
                    {

                        entity.updatedUserId=userId;
                        entity.updateDate=DateTime.Now.ToString();
                        entity.enabled=true;
                        await Update(_mapper.Map<Data.Models.AnnualLeaveTable>(entity));
                        return (entity);
                    }
                }
                catch
                {
                    throw;
                }

            }

            public PageReturn<AnnualLeaveTableResponseDto>? mylist(FilterPageParam<AnnualTableFilterDtoRequest> filterPageParam)
            {
                PageReturn<AnnualLeaveTableResponseDto>? result = new PageReturn<AnnualLeaveTableResponseDto>();
                int pageSize = filterPageParam.size ?? 10;
                int pageNumber = filterPageParam.page ?? 0;
                int? id = filterPageParam?.liste?.id;
                int userId = filterPageParam?.liste?.userId ?? 0;
                int? searchUserId = filterPageParam?.liste?.searchUserId;
                int? currentStateId = filterPageParam?.liste?.currentStateId;

                IQueryable<Data.Models.AnnualLeaveTable> query = dal.Get(u =>
     (id == null || u.Id == id) &&
     (currentStateId == null || u.currentStateId == currentStateId) &&
     u.enabled &&
     u.currentUserId == userId &&
     (searchUserId == null || u.userId == searchUserId) &&
     u.siraNo != 6
 ).OrderByDescending(u => u.Id);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new AnnualLeaveTableResponseDto
                    {
                        id = u.Id,
                        currentStateId = u.currentStateId,
                        endDate = u.endDate.ToString("dd.MM.yyyy HH:ss"),
                        istenenIzin = u.dayRequested,
                        kalanIzin = u.dayleft,
                        startDate = u.startDate.ToString("dd.MM.yyyy HH:ss"),
                        username = u.user.name,

                    }).OrderByDescending(u => u.id).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;

            }

            public int approvalCount(int userId)
            {
                int count = dal.Get(u => u.enabled && u.currentUserId == userId && u.currentStateId == 1 && u.siraNo != 6).Count();
                return count;
            }

            public int approvalCountIk(int userId)
            {
                int count = dal.Get(u => u.enabled && u.currentUserId == userId && u.currentStateId == 1 && u.siraNo == 6).Count();
                return count;
            }

            public List<Data.Models.AnnualLeaveTable>? getAllByUserId(int userId)
            {
                return dal.Get(u => u.userId == userId && u.enabled).ToList();
            }

            public PageReturn<AnnualLeaveTableResponseDto>? list(FilterPageParam<AnnualLeaveFilterDtoRequest>filterPageParam, int adminUserId)
            {

                AdminUsers bllAdminUser = new AdminUsers(_configuration, _env, _mapper);
                Data.Models.AdminUser? adminUser = bllAdminUser.GetByID(adminUserId);

                if (adminUser == null)
                    return null;

                PageReturn<AnnualLeaveTableResponseDto> result = new PageReturn<AnnualLeaveTableResponseDto>();

                int pageSize = filterPageParam?.size ?? 10;
                int pageNumber = filterPageParam?.page ?? 0;

                int? id = filterPageParam?.liste?.id;
                int? userId = filterPageParam?.liste?.userId;
                int? searchUserId = filterPageParam?.liste?.searchUserId;
                int? currentStateId = filterPageParam?.liste?.currentStateId;

                bool canSeeLogs =
                    adminUser.roleId == 1 ||
                    (adminUser.role?.RoleDetail?.Any(r => r.canSeeLogs) ?? false);

                IQueryable<Data.Models.AnnualLeaveTable> query = dal.Get(u =>
                    (id == null || u.Id == id) &&
                    u.enabled &&
                    u.siraNo != 6 &&
                    (currentStateId == null || u.currentStateId == currentStateId) &&
                    (searchUserId == null || u.userId == searchUserId) &&

                    (canSeeLogs || (userId == null || u.currentUserId == userId))
                )
                .OrderByDescending(u => u.Id);

                var pagedData = query
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .Select(u => new AnnualLeaveTableResponseDto
                    {
                        id = u.Id,
                        currentStateId = u.currentStateId,
                        endDate = u.endDate.ToString("dd.MM.yyyy HH:ss"),
                        istenenIzin = u.dayRequested,
                        kalanIzin = u.dayleft,
                        startDate = u.startDate.ToString("dd.MM.yyyy HH:ss"),

                        username = u.user != null ? u.user.name : null
                    })
                    .OrderByDescending(u => u.id)
                    .ToList();

                result.content = pagedData;
                result.totalElements = query.Count();
                result.number = pagedData.Count;
                result.size = pageSize;

                return result;
            }
            public ResponseByteArray? showPdf(int id)
            {


                string? filePath = Path.Combine("D:\\AskalePortal\\AskalePortal.BLL\\Raporlar\\YillikIzin");


                string fileFull = Path.Combine(filePath, "YillikIzin.rdl");



                Data.Models.AnnualLeaveTable? annualLeaveTable = GetByID(id);


                if (annualLeaveTable == null)
                {
                    return null;
                }

                AnnualLeaveDetail bllAnnualLeaveDetail = new AnnualLeaveDetail(_configuration, _env);
                List<Data.Models.AnnualLeaveDetail>? listAnnualLeaveDetails = bllAnnualLeaveDetail.getByAnuId(id);

                // LocalReport nesnesi oluştur
                using (var localReport = new LocalReport())
                {
                    // RDL dosyasını yükle
                    localReport.ReportPath = fileFull;

                    // Parametreleri ayarla


                    // Veri kaynağı ekle (örnek bir DataTable)
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("adsoyad", typeof(string));
                    dataTable.Columns.Add("bolumu", typeof(string));
                    dataTable.Columns.Add("isegiristarihi", typeof(string));
                    dataTable.Columns.Add("gorevunvan", typeof(string));
                    dataTable.Columns.Add("type", typeof(int));
                    dataTable.Columns.Add("daysleft", typeof(string));
                    dataTable.Columns.Add("requestday", typeof(string));
                    dataTable.Columns.Add("startdate", typeof(string));
                    dataTable.Columns.Add("enddate", typeof(string));
                    dataTable.Columns.Add("address", typeof(string));
                    dataTable.Columns.Add("telNo", typeof(string));
                    dataTable.Columns.Add("vekalet", typeof(string));
                    dataTable.Columns.Add("talepEden", typeof(string));
                    dataTable.Columns.Add("name1", typeof(string));
                    dataTable.Columns.Add("tarih1", typeof(string));
                    dataTable.Columns.Add("name2", typeof(string));
                    dataTable.Columns.Add("tarih2", typeof(string));
                    dataTable.Columns.Add("name3", typeof(string));
                    dataTable.Columns.Add("tarih3", typeof(string));
                    dataTable.Columns.Add("name4", typeof(string));
                    dataTable.Columns.Add("tarih4", typeof(string));
                    dataTable.Columns.Add("name5", typeof(string));
                    dataTable.Columns.Add("tarih5", typeof(string));
                    dataTable.Columns.Add("personelNo", typeof(string));
                    dataTable.Columns.Add("digerAciklama", typeof(string));
                    dataTable.Columns.Add("kalanizin", typeof(string));

                    dataTable.Rows.Add(
                       annualLeaveTable.user.name,
                       annualLeaveTable.departmanName,
                       annualLeaveTable.enteredDate,
                          annualLeaveTable.job,
                          annualLeaveTable.typeId,
                          annualLeaveTable.dayleft,
                            annualLeaveTable.dayRequested,
                            annualLeaveTable.startDate,
                            annualLeaveTable.endDate,
                            annualLeaveTable.adress,
                            annualLeaveTable.user.phone,
                            annualLeaveTable.vekalet.name,
                            annualLeaveTable.user.name,
                            onaylayici(listAnnualLeaveDetails, 1),
                            getTarih(listAnnualLeaveDetails, 1),
                           onaylayici(listAnnualLeaveDetails, 2),
                            getTarih(listAnnualLeaveDetails, 2),
                             onaylayici(listAnnualLeaveDetails, 3),
                            getTarih(listAnnualLeaveDetails, 3),
                             onaylayici(listAnnualLeaveDetails, 4),
                            getTarih(listAnnualLeaveDetails, 4),
                             onaylayici(listAnnualLeaveDetails, 5),
                            getTarih(listAnnualLeaveDetails, 5),
                            annualLeaveTable.user.perNo,
                            annualLeaveTable.digerAciklama,
                            annualLeaveTable.dayleft.ToString() + " gün"



                        );



                    localReport.DataSources.Add(new ReportDataSource("dataSetIzin", dataTable)); // DataSet adı RDL'dekiyle eşleşmeli

                    // Raporu PDF olarak render et
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string[] streams;
                    Warning[] warnings;

                    byte[] pdfBytes = localReport.Render(
                        "PDF",
                        null,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);



                    ResponseByteArray responseByteArray = new ResponseByteArray();

                    responseByteArray.file = pdfBytes;
                    responseByteArray.fileName = "Rapor.pdf";
                    return responseByteArray;
                }
            }

            public AnnualLeaveDetailDto? getByAnnualLeaveId(int id)
            {


                AnnualLeaveDetail bllAnnualLeaveDetail = new AnnualLeaveDetail(_configuration, _env);
                List<Data.Models.AnnualLeaveDetail>? listAnnualLeaveDetails = bllAnnualLeaveDetail.getByAnuId(id);
                AnnualLeaveType bllAnnualLeaveType = new AnnualLeaveType(_configuration, _env);
                Data.Models.AnnualLeaveTable? annualLeaveTable = GetByID(id);
                if (annualLeaveTable == null)
                {
                    return null;
                }
                Data.Models.AnnualLeaveType? annualLeaveType = bllAnnualLeaveType.GetByID(annualLeaveTable.typeId);

                AdminUsers bllAdminUser = new AdminUsers(_configuration, _env, _mapper);
                Data.Models.AdminUser? user = bllAdminUser.GetByID(annualLeaveTable.userId);
                Data.Models.AdminUser? vekaletUser = null;
                if (annualLeaveTable.vekaletId != null)
                {
                    vekaletUser = bllAdminUser.GetByID(annualLeaveTable!.vekaletId ?? 0);
                }


                AnnualLeaveDetailDto annualLeaveDetailDto = new AnnualLeaveDetailDto();

                annualLeaveDetailDto.id = id;
                annualLeaveDetailDto.adres = annualLeaveTable?.adress;
                annualLeaveDetailDto.vekaletName = vekaletUser?.name;
                annualLeaveDetailDto.iseGirisTarihi = annualLeaveTable?.enteredDate.ToString("dd.MM.yyyy HH:mm");
                annualLeaveDetailDto.departman = annualLeaveTable?.departmanName;
                annualLeaveDetailDto.digerAciklama = annualLeaveTable?.digerAciklama;
                annualLeaveDetailDto.name = user?.name;
                annualLeaveDetailDto.sicilNo = user?.perNo;
                annualLeaveDetailDto.pozisyon = annualLeaveTable?.job;
                annualLeaveDetailDto.izinTuru = annualLeaveTable?.typeId;
                annualLeaveDetailDto.mevcutIzin = annualLeaveTable?.dayleft.ToString("dd.MM.yyyy HH:mm");
                annualLeaveDetailDto.istenenIzin = annualLeaveTable?.dayRequested.ToString("dd.MM.yyyy HH:mm");
                annualLeaveDetailDto.typeName = annualLeaveType?.typeName ?? "";
                annualLeaveDetailDto.typeNameEn = annualLeaveType?.typeNameEn ?? "";
                annualLeaveDetailDto.kalanIzin = annualLeaveTable?.typeId == 2 ? (annualLeaveTable?.dayleft - annualLeaveTable?.dayRequested).ToString() : annualLeaveTable?.dayleft.ToString();
                annualLeaveDetailDto.startdate = annualLeaveTable?.startDate.ToString("dd.MM.yyyy HH:mm");
                annualLeaveDetailDto.endDate = annualLeaveTable?.endDate.ToString("dd.MM.yyyy HH:mm");
                annualLeaveDetailDto.birinciDurum = getDurum(listAnnualLeaveDetails, 1);
                annualLeaveDetailDto.ikinciDurum = getDurum(listAnnualLeaveDetails, 2);
                annualLeaveDetailDto.ucuncuDurum = getDurum(listAnnualLeaveDetails, 3);
                annualLeaveDetailDto.dorduncuDurum = getDurum(listAnnualLeaveDetails, 4);
                annualLeaveDetailDto.besinciDurum = getDurum(listAnnualLeaveDetails, 5);
                annualLeaveDetailDto.altinciDurum = getDurum(listAnnualLeaveDetails, 6);
                annualLeaveDetailDto.birinciOnaylayici = onaylayici(listAnnualLeaveDetails, 1);
                annualLeaveDetailDto.ikinciOnaylayici = onaylayici(listAnnualLeaveDetails, 2);
                annualLeaveDetailDto.ucuncuOnaylayici = onaylayici(listAnnualLeaveDetails, 3);
                annualLeaveDetailDto.dorduncuOnaylayici = onaylayici(listAnnualLeaveDetails, 4);
                annualLeaveDetailDto.besinciOnaylayici = onaylayici(listAnnualLeaveDetails, 5);
                annualLeaveDetailDto.altinciOnaylayici = onaylayici(listAnnualLeaveDetails, 6);
                annualLeaveDetailDto.birinciOnayTarihi = getTarih(listAnnualLeaveDetails, 1);
                annualLeaveDetailDto.ikinciOnayTarihi = getTarih(listAnnualLeaveDetails, 2);
                annualLeaveDetailDto.ucuncuOnayTarihi = getTarih(listAnnualLeaveDetails, 3);
                annualLeaveDetailDto.dorduncuOnayTarihi = getTarih(listAnnualLeaveDetails, 4);
                annualLeaveDetailDto.besinciOnayTarihi = getTarih(listAnnualLeaveDetails, 5);
                annualLeaveDetailDto.altinciOnayTarihi = getTarih(listAnnualLeaveDetails, 6);
                annualLeaveDetailDto.birinciOnaylayiciFile = getFile(listAnnualLeaveDetails, 1);
                annualLeaveDetailDto.ikinciOnaylayiciFile = getFile(listAnnualLeaveDetails, 2);
                annualLeaveDetailDto.ucuncuOnaylayiciFile = getFile(listAnnualLeaveDetails, 3);
                annualLeaveDetailDto.dorduncuOnaylayiciFile = getFile(listAnnualLeaveDetails, 4);
                annualLeaveDetailDto.besinciOnaylayiciFile = getFile(listAnnualLeaveDetails, 5);
                annualLeaveDetailDto.altinciOnaylayiciFile = getFile(listAnnualLeaveDetails, 6);



                return annualLeaveDetailDto;

            }
            private List<int>? getFile(List<Data.Models.AnnualLeaveDetail>? listAnnualLeaveDetails, int siraNo)
            {
                Data.Models.AnnualLeaveDetail? annualLeaveDetail = listAnnualLeaveDetails?.FirstOrDefault(u => u.siraNo == siraNo);
                string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                       _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "adminusers\\images\\");

                if (annualLeaveDetail == null)
                {
                    return null;
                }
                AdminUsers bllAdminUser = new AdminUsers(_configuration, _env, _mapper);
                Data.Models.AdminUser? user = bllAdminUser.GetByIDAll(annualLeaveDetail.userId);
                if (user == null) { return null; }
                string? fileName = Path.Combine(filePath, user.imageUrl);
                FileStream fs = new FileStream(fileName,
                                   FileMode.Open,
                                   FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                long numBytes = new FileInfo(fileName).Length;


                List<int> listInt = new List<int>();

                byte[] contentInBytes = br.ReadBytes((int)numBytes);

                foreach (byte bite in contentInBytes)
                {
                    int byteSayi = (int)bite;
                    listInt.Add(byteSayi);
                }

                return listInt;


            }
            public string getTarih(List<Data.Models.AnnualLeaveDetail> listAnnualLeaveDetails, int siraNo)
            {
                Data.Models.AnnualLeaveDetail? annualLeaveDetail = listAnnualLeaveDetails.FirstOrDefault(u => u.siraNo == siraNo);
                string tarih;
                if (annualLeaveDetail?.isReplied == false)
                {
                    tarih = "Onayda";
                }
                else
                {
                    tarih = annualLeaveDetail?.replyDate?.ToString("dd.MM.yyyy HH:mm:ss") ?? "";
                }
                return tarih;
            }
            public string? onaylayici(List<Data.Models.AnnualLeaveDetail> listAnnualLeaveDetails, int siraNo)
            {
                Data.Models.AnnualLeaveDetail? annualLeaveDetail = listAnnualLeaveDetails.FirstOrDefault(u => u.siraNo == siraNo);
                AdminUsers bllAdminUser = new AdminUsers(_configuration, _env, _mapper);
                if (annualLeaveDetail == null)
                {
                    return null;
                }
                Data.Models.AdminUser? user = bllAdminUser.GetByIDAll(annualLeaveDetail.userId);

                return user?.name;
            }
            private int getDurum(List<Data.Models.AnnualLeaveDetail> listAnnualLeaveDetails, int siraNo)
            {
                Data.Models.AnnualLeaveDetail? annualLeaveDetail = listAnnualLeaveDetails.FirstOrDefault(u => u.siraNo == siraNo);
                if (annualLeaveDetail == null)
                {
                    return 4;
                }
                switch (annualLeaveDetail.approved)
                {
                    case null:
                        return 3;
                    case true:
                        return 1;
                    case false:
                        return 2;


                }
            }

            public List<Data.Models.AnnualLeaveTable> findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(int? userOld, int currentStateId, bool enabled, int userId)
            {
                List<Data.Models.AnnualLeaveTable> liste = dal.Get(u => u.currentUserId == userOld && u.currentStateId == currentStateId && u.enabled == enabled && u.userId == userId).ToList();
                return liste;
            }

            public async Task<int> approve(int id, int loggedUserId)
            {

                int burakKurkcu = 5758;
                int farukOztas = 6894;
                //DateTimeFormatter dtFormatter = DateTimeFormatter.ofPattern("dd.MM.yyyy");
                BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                Data.Models.CeoTable? ceoTable = bllCeoTable.GetByID(1);
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? ceoUser = bllAdminUsers.GetByID(ceoTable?.userId ?? 0);

                BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail = new BLLActions.AnnualLeaveDetail(_configuration, _env);
                try
                {

                    Data.Models.AnnualLeaveTable? annualLeaveTable = GetByID(id);
                    if (annualLeaveTable != null)
                    {
                        AdminUser? user = bllAdminUsers.GetByID(loggedUserId);
                        if (user != null)
                        {


                            AdminUser? userMain = bllAdminUsers.GetByID(annualLeaveTable.userId);
                            int? nextUser;
                            if (userMain != null)
                            {
                                if (annualLeaveTable.siraNo == 1 && annualLeaveTable.currentUserId.Equals(user.Id))
                                {
                                    nextUser = userMain?.manager1;
                                    if (nextUser == null)
                                    {
                                        return 3;

                                    }
                                    else
                                    {
                                        if (annualLeaveTable.typeId.Equals(4))
                                        {
                                            if (userMain!.manager1.Equals(farukOztas)
                                                    || userMain.manager1.Equals(burakKurkcu))
                                            {
                                                return await onayla1(annualLeaveTable, id, 1, loggedUserId);
                                            }
                                            else
                                            {
                                                annualLeaveTable.currentUserId = nextUser ?? 0;
                                                annualLeaveTable.siraNo = 2;
                                                await Update(annualLeaveTable);

                                                Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail
                                                        .findByAnuIdAndUserIdAndSiraNoAndEnabled(id, user.Id, 1, true);
                                                if (annualLeaveDetail != null)
                                                {
                                                    annualLeaveDetail.replyDate = DateTime.Now;
                                                    annualLeaveDetail.isReplied = true;
                                                    annualLeaveDetail.approved = true;
                                                    annualLeaveDetail.updatedDate = DateTime.Now;
                                                    annualLeaveDetail.updatedUserId = loggedUserId;
                                                    await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                                                }
                                                else
                                                {
                                                    return 2;
                                                }
                                                Data.Models.AnnualLeaveDetail annualLeaveDetailNext = new Data.Models.AnnualLeaveDetail();
                                                annualLeaveDetailNext.userId = nextUser ?? 0;
                                                annualLeaveDetailNext.guid = Guid.NewGuid();
                                                annualLeaveDetailNext.createdDate = DateTime.Now;
                                                annualLeaveDetailNext.siraNo = 2;
                                                annualLeaveDetailNext.anuId = id;
                                                annualLeaveDetailNext.enabled = true;
                                                annualLeaveDetailNext.isReplied = false;
                                                annualLeaveDetailNext.createdDate = DateTime.Now;
                                                annualLeaveDetailNext.createdUserId = loggedUserId;
                                                await bllAnnualLeaveDetail.Add(annualLeaveDetailNext);

                                                if (nextUser.Equals(ceoUser?.Id))
                                                {
                                                    BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                                    SMSMessage smsMessage = new SMSMessage();
                                                    smsMessage.plannedDate = DateTime.Now;
                                                    smsMessage.isSent = false;
                                                    smsMessage.smsText = annualLeaveTable.Id.ToString()
                                                            + "Id'li  yıllık izin onayınızı beklemektedir.";
                                                    smsMessage.toNumbers = ceoUser?.mobile;
                                                    smsMessage.createdUserId = loggedUserId;
                                                    smsMessage.createdDate = DateTime.Now;
                                                    await bllSMSMessages.Add(smsMessage);

                                                }
                                                else
                                                {
                                                    UserByNameEMailDto nextUserEmail = bllAdminUsers.getUserByNameAndEmail(nextUser ?? 0);

                                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                                    EmailMessage emailMessage = new EmailMessage();
                                                    emailMessage.isSent = false;
                                                    emailMessage.toAddress = nextUserEmail.email;
                                                    emailMessage.mailTuru = 1;
                                                    emailMessage.createdDate = DateTime.Now;
                                                    emailMessage.createdUserId = user.Id;
                                                    emailMessage.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                                                    string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                                                            + annualLeaveTable.Id.ToString()
                                                            + " Id'li izin onayınızı beklemektedir.<br/></h4>";
                                                    emailMessage.emailText = mailString;
                                                    emailMessage.plannedDate = DateTime.Now;
                                                    emailMessage.enabled = true;

                                                    await bllEmailMessages.Add(emailMessage);
                                                }
                                                return 1;
                                            }

                                        }
                                        else
                                        {
                                            annualLeaveTable.currentUserId = nextUser ?? 0;
                                            annualLeaveTable.siraNo = 2;
                                            annualLeaveTable.updatedDate = DateTime.Now;
                                            annualLeaveTable.updatedUserId = loggedUserId;
                                            await Update(annualLeaveTable);
                                            Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail
                                                    .findByAnuIdAndUserIdAndSiraNoAndEnabled(id, user.Id, 1, true);
                                            if (annualLeaveDetail != null)
                                            {
                                                annualLeaveDetail.replyDate = DateTime.Now;
                                                annualLeaveDetail.isReplied = true;
                                                annualLeaveDetail.approved = true;
                                                annualLeaveDetail.updatedUserId = loggedUserId;
                                                annualLeaveDetail.updatedDate = DateTime.Now;
                                                await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                                            }
                                            else
                                            {
                                                return 2;
                                            }
                                            Data.Models.AnnualLeaveDetail annualLeaveDetailNext = new Data.Models.AnnualLeaveDetail();
                                            annualLeaveDetailNext.userId = nextUser ?? 0;
                                            annualLeaveDetailNext.guid = Guid.NewGuid();
                                            annualLeaveDetailNext.createdDate = DateTime.Now;
                                            annualLeaveDetailNext.siraNo = 2;
                                            annualLeaveDetailNext.anuId = id;
                                            annualLeaveDetailNext.enabled = true;
                                            annualLeaveDetailNext.isReplied = false;
                                            annualLeaveDetailNext.createdDate = DateTime.Now;
                                            annualLeaveDetailNext.createdUserId = loggedUserId;
                                            await bllAnnualLeaveDetail.Add(annualLeaveDetailNext);
                                            if (nextUser.Equals(ceoUser.Id))
                                            {
                                                BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                                SMSMessage smsMessage = new SMSMessage();
                                                smsMessage.plannedDate = DateTime.Now;
                                                smsMessage.isSent = false;
                                                smsMessage.smsText = annualLeaveTable.Id.ToString()
                                                        + "Id'li  yıllık izin onayınızı beklemektedir.";
                                                smsMessage.toNumbers = ceoUser.mobile;
                                                smsMessage.createdDate = DateTime.Now;
                                                smsMessage.createdUserId = loggedUserId;
                                                await bllSMSMessages.Add(smsMessage);

                                            }
                                            else
                                            {
                                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                                UserByNameEMailDto nextUserEmail = bllAdminUsers.getUserByNameAndEmail(nextUser ?? 0);
                                                EmailMessage emailMessage = new EmailMessage();
                                                emailMessage.isSent = false;
                                                emailMessage.toAddress = nextUserEmail.email;
                                                emailMessage.mailTuru = 1;
                                                emailMessage.createdDate = DateTime.Now;
                                                emailMessage.createdUserId = user.Id;
                                                emailMessage.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                                                string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                                                        + annualLeaveTable.Id.ToString()
                                                        + " Id'li izin onayınızı beklemektedir.<br/></h4>";
                                                emailMessage.emailText = mailString;
                                                emailMessage.plannedDate = DateTime.Now;
                                                emailMessage.enabled = true;

                                                await bllEmailMessages.Add(emailMessage);
                                            }
                                            return 1;
                                        }

                                    }
                                }
                                else if (annualLeaveTable.siraNo == 2 && annualLeaveTable.currentUserId.Equals(user.Id))
                                {
                                    nextUser = userMain.manager2;
                                    if (nextUser == null)
                                    {

                                        if (annualLeaveTable.typeId.Equals(4))
                                        {

                                            if (annualLeaveTable.currentUserId.Equals(farukOztas)
                                                && annualLeaveTable.dayRequested >= 10m)
                                            {
                                                return await onaylaFarukBey(annualLeaveTable, id, 2, loggedUserId);
                                            }
                                            else if (annualLeaveTable.currentUserId.Equals(burakKurkcu))
                                            {
                                                return await onaylaBurakBey(userMain.izinOnayId ?? 0, annualLeaveTable, id, 2, loggedUserId);
                                            }

                                            return await onayla1(annualLeaveTable, id, 2, loggedUserId);

                                        }
                                        else
                                        {
                                            nextUser = userMain.izinOnayId;
                                            annualLeaveTable.currentUserId = nextUser ?? 0;
                                            annualLeaveTable.siraNo = 6;
                                            annualLeaveTable.updatedDate = DateTime.Now;
                                            annualLeaveTable.updatedUserId = loggedUserId;
                                            await Update(annualLeaveTable);
                                            Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail
                                                    .findByAnuIdAndUserIdAndSiraNoAndEnabled(id, user.Id, 2, true);
                                            if (annualLeaveDetail != null)
                                            {
                                                annualLeaveDetail.replyDate = DateTime.Now;
                                                annualLeaveDetail.isReplied = true;
                                                annualLeaveDetail.approved = true;
                                                annualLeaveDetail.updatedUserId = loggedUserId;
                                                annualLeaveDetail.updatedDate = DateTime.Now;
                                                await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                                            }
                                            else
                                            {
                                                return 2;
                                            }
                                            Data.Models.AnnualLeaveDetail annualLeaveDetailNext = new Data.Models.AnnualLeaveDetail();
                                            annualLeaveDetailNext.userId = nextUser ?? 0;
                                            annualLeaveDetailNext.guid = Guid.NewGuid();
                                            annualLeaveDetailNext.createdDate = DateTime.Now;
                                            annualLeaveDetailNext.siraNo = 6;
                                            annualLeaveDetailNext.anuId = id;
                                            annualLeaveDetailNext.enabled = true;
                                            annualLeaveDetailNext.isReplied = false;
                                            annualLeaveDetailNext.createdDate = DateTime.Now;
                                            annualLeaveDetailNext.createdUserId = loggedUserId;
                                            await bllAnnualLeaveDetail.Add(annualLeaveDetailNext);
                                            if (nextUser.Equals(ceoUser?.Id))
                                            {
                                                BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                                SMSMessage smsMessage = new SMSMessage();
                                                smsMessage.plannedDate = DateTime.Now;
                                                smsMessage.isSent = false;
                                                smsMessage.smsText = annualLeaveTable.Id.ToString()
                                                        + "Id'li  yıllık izin onayınızı beklemektedir.";
                                                smsMessage.toNumbers = ceoUser?.mobile;

                                                await bllSMSMessages.Add(smsMessage);

                                            }
                                            else
                                            {
                                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                                UserByNameEMailDto nextUserEmail = bllAdminUsers.getUserByNameAndEmail(nextUser ?? 0);
                                                EmailMessage emailMessage = new EmailMessage();
                                                emailMessage.isSent = false;
                                                emailMessage.toAddress = nextUserEmail.email;
                                                emailMessage.mailTuru = 1;
                                                emailMessage.createdDate = DateTime.Now;
                                                emailMessage.createdUserId = user.Id;
                                                emailMessage.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                                                string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                                                        + annualLeaveTable.Id.ToString()
                                                        + " Id'li izin tamamlanmaya düşmüştür.<br/></h4>";
                                                emailMessage.emailText = mailString;
                                                emailMessage.plannedDate = DateTime.Now;
                                                emailMessage.enabled = true;
                                                await bllEmailMessages.Add(emailMessage);
                                            }
                                            return 1;
                                        }
                                    }
                                    else
                                    {
                                        if (nextUser.Equals(burakKurkcu) && annualLeaveTable.typeId.Equals(4))
                                        {

                                            if (userMain.manager1.Equals(farukOztas)
                                                    && userMain.manager2.Equals(burakKurkcu))
                                            {
                                                return await onaylaFarukBey(annualLeaveTable, id, 2, loggedUserId);

                                            }
                                            else if (annualLeaveTable.dayRequested >= 10m)
                                            {
                                                return await onaylaBurakBey(userMain.izinOnayId ?? 0, annualLeaveTable, id, 2, loggedUserId);
                                            }
                                            else if (userMain.manager2.Equals(farukOztas)
                                                    && annualLeaveTable.dayRequested >= 10m)
                                            {
                                                return await onaylaFarukBey(annualLeaveTable, id, 2, loggedUserId);
                                            }

                                            return await onayla1(annualLeaveTable, id, 3, loggedUserId);
                                        }
                                        annualLeaveTable.currentUserId = nextUser ?? 0;
                                        annualLeaveTable.siraNo = 3;
                                        await Update(annualLeaveTable);
                                        Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail
                                                .findByAnuIdAndUserIdAndSiraNoAndEnabled(id, user.Id, 2, true);
                                        if (annualLeaveDetail != null)
                                        {
                                            annualLeaveDetail.replyDate = DateTime.Now;
                                            annualLeaveDetail.isReplied = true;
                                            annualLeaveDetail.approved = true;
                                            annualLeaveDetail.updatedDate = DateTime.Now;
                                            annualLeaveDetail.updatedUserId = loggedUserId;
                                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                                        }
                                        else
                                        {
                                            return 2;
                                        }
                                        Data.Models.AnnualLeaveDetail annualLeaveDetailNext = new Data.Models.AnnualLeaveDetail();
                                        annualLeaveDetailNext.userId = nextUser ?? 0;
                                        annualLeaveDetailNext.guid = Guid.NewGuid();
                                        annualLeaveDetailNext.createdDate = DateTime.Now;
                                        annualLeaveDetailNext.siraNo = 3;
                                        annualLeaveDetailNext.anuId = id;
                                        annualLeaveDetailNext.isReplied = false;
                                        annualLeaveDetailNext.enabled = true;
                                        annualLeaveDetailNext.createdUserId = loggedUserId;
                                        annualLeaveDetailNext.createdDate = DateTime.Now;
                                        await bllAnnualLeaveDetail.Add(annualLeaveDetailNext);

                                        if (nextUser.Equals(ceoUser?.Id))
                                        {
                                            BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                            SMSMessage smsMessage = new SMSMessage();
                                            smsMessage.plannedDate = DateTime.Now;
                                            smsMessage.isSent = false;
                                            smsMessage.smsText =
                                                    annualLeaveTable.Id.ToString() + "Id'li  yıllık izin onayınızı beklemektedir.";
                                            smsMessage.toNumbers = ceoUser?.mobile;

                                            await bllSMSMessages.Add(smsMessage);

                                        }
                                        else
                                        {
                                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                            UserByNameEMailDto nextUserEmail = bllAdminUsers.getUserByNameAndEmail(nextUser ?? 0);
                                            EmailMessage emailMessage = new EmailMessage();
                                            emailMessage.isSent = false;
                                            emailMessage.toAddress = nextUserEmail.email;
                                            emailMessage.mailTuru = 1;
                                            emailMessage.createdDate = DateTime.Now;
                                            emailMessage.createdUserId = user.Id;
                                            emailMessage.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                                            string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                                                    + annualLeaveTable.Id.ToString()
                                                    + " Id'li izin onayınızı beklemektedir.<br/></h4>";
                                            emailMessage.emailText = mailString;
                                            emailMessage.plannedDate = DateTime.Now;
                                            emailMessage.enabled = true;
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                        return 1;
                                    }

                                }
                                else if (annualLeaveTable.siraNo == 3 && annualLeaveTable.currentUserId.Equals(user.Id))
                                {

                                    if (annualLeaveTable.typeId.Equals(4))
                                    {
                                        if (annualLeaveTable.currentUserId.Equals(farukOztas)
                                                && annualLeaveTable.dayRequested >= 10m)
                                        {
                                            return await onaylaFarukBey(annualLeaveTable, id, 3, loggedUserId);
                                        }
                                        else if (annualLeaveTable.currentUserId.Equals(burakKurkcu))
                                        {
                                            return await onaylaBurakBey(userMain.izinOnayId ?? 0, annualLeaveTable, id, 3, loggedUserId);
                                        }
                                        else if (annualLeaveTable.currentUserId.Equals(farukOztas))
                                        {
                                            return await onaylaBurakBey(userMain.izinOnayId ?? 0, annualLeaveTable, id, 3, loggedUserId);
                                        }
                                        else
                                        {
                                            return await onayla1(annualLeaveTable, id, 3, loggedUserId);
                                        }

                                    }

                                    else
                                    {
                                        nextUser = userMain.izinOnayId;
                                        if (nextUser == null)
                                        {
                                            return 3;
                                        }
                                        else
                                        {
                                            annualLeaveTable.currentUserId = nextUser ?? 0;
                                            annualLeaveTable.siraNo = 6;
                                            await Update(annualLeaveTable);
                                            Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail
                                                    .findByAnuIdAndUserIdAndSiraNoAndEnabled(id, user.Id, 3, true);
                                            if (annualLeaveDetail != null)
                                            {
                                                annualLeaveDetail.replyDate = DateTime.Now;
                                                annualLeaveDetail.isReplied = true;
                                                annualLeaveDetail.approved = true;
                                                annualLeaveDetail.updatedUserId = loggedUserId;
                                                annualLeaveDetail.updatedDate = DateTime.Now;
                                                await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                                            }
                                            else
                                            {
                                                return 2;
                                            }
                                            Data.Models.AnnualLeaveDetail annualLeaveDetailNext = new Data.Models.AnnualLeaveDetail();
                                            annualLeaveDetailNext.userId = nextUser ?? 0;
                                            annualLeaveDetailNext.guid = Guid.NewGuid();
                                            annualLeaveDetailNext.createdDate = DateTime.Now;
                                            annualLeaveDetailNext.createdUserId = loggedUserId;
                                            annualLeaveDetailNext.siraNo = 6;
                                            annualLeaveDetailNext.anuId = id;
                                            annualLeaveDetailNext.isReplied = false;
                                            annualLeaveDetailNext.enabled = true;
                                            await bllAnnualLeaveDetail.Add(annualLeaveDetailNext);

                                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                            UserByNameEMailDto nextUserEmail = bllAdminUsers.getUserByNameAndEmail(nextUser ?? 0);
                                            EmailMessage emailMessage = new EmailMessage();
                                            emailMessage.isSent = false;
                                            emailMessage.toAddress = nextUserEmail.email;
                                            emailMessage.mailTuru = 1;
                                            emailMessage.createdDate = DateTime.Now;
                                            emailMessage.createdUserId = user.Id;
                                            emailMessage.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                                            string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                                                    + annualLeaveTable.Id.ToString() + " Id'li izin tamamlanmaya düşmüştür.<br/></h4>";
                                            emailMessage.emailText = mailString;
                                            emailMessage.plannedDate = DateTime.Now;
                                            emailMessage.enabled = true;
                                            await bllEmailMessages.Add(emailMessage);

                                            AdminUser? nextUserEmailOwner = bllAdminUsers.GetByID(annualLeaveTable.userId);
                                            EmailMessage emailMessageOwner = new EmailMessage();
                                            emailMessageOwner.isSent = false;
                                            emailMessageOwner.toAddress = nextUserEmailOwner?.email;
                                            emailMessageOwner.mailTuru = 1;
                                            emailMessageOwner.createdDate = DateTime.Now;
                                            emailMessageOwner.createdUserId = user.Id;
                                            emailMessageOwner.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                                            string mailStringOwner = "<h2>Sayın " + nextUserEmailOwner?.name + "</h2><br/>" + "<h4>"
                                                    + annualLeaveTable.startDate.ToString("dd.MM.yyyy") + " - "
                                                    + annualLeaveTable.endDate.ToString("dd.MM.yyyy")
                                                    + " tarihleri arasında girmiş olduğunuz izin talebiniz onaylandı.<br/> <p><b>İzninizin geçerli olabilmesi için izin formunun ıslak imzalı halini İnsan Kaynakları birimine teslim etmeniz gerekmektedir.</b></p><br/></h4>";
                                            emailMessageOwner.emailText = mailStringOwner;
                                            emailMessageOwner.plannedDate = DateTime.Now;
                                            emailMessageOwner.enabled = true;
                                            await bllEmailMessages.Add(emailMessageOwner);
                                            return 1;
                                        }

                                    }
                                }
                                else if (annualLeaveTable.siraNo == 4 && annualLeaveTable.currentUserId.Equals(user.Id))
                                {
                                    if (annualLeaveTable.typeId.Equals(4))
                                    {
                                        if (annualLeaveTable.currentUserId.Equals(farukOztas)
                                                && annualLeaveTable.dayRequested >= 10m)
                                        {
                                            return await onaylaFarukBey(annualLeaveTable, id, 4, loggedUserId);
                                        }
                                        else if (annualLeaveTable.currentUserId.Equals(burakKurkcu))
                                        {
                                            return await onaylaBurakBey(userMain.izinOnayId ?? 0, annualLeaveTable, id, 4, loggedUserId);
                                        }
                                        else
                                        {
                                            return await onaylaBurakBey(userMain.izinOnayId ?? 0, annualLeaveTable, id, 4, loggedUserId);
                                        }
                                        //					return 3;
                                    }
                                    else
                                    {
                                        nextUser = userMain.izinOnayId;
                                        if (nextUser == null)
                                        {
                                            return 3;
                                        }
                                        else
                                        {
                                            annualLeaveTable.currentUserId = nextUser ?? 0;
                                            annualLeaveTable.siraNo = 6;
                                            await Update(annualLeaveTable);
                                            Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail
                                                    .findByAnuIdAndUserIdAndSiraNoAndEnabled(id, user.Id, 4, true);
                                            if (annualLeaveDetail != null)
                                            {
                                                annualLeaveDetail.replyDate = DateTime.Now;
                                                annualLeaveDetail.isReplied = true;
                                                annualLeaveDetail.approved = true;
                                                annualLeaveDetail.updatedDate = DateTime.Now;
                                                annualLeaveDetail.updatedUserId = loggedUserId;
                                                await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                                            }
                                            else
                                            {
                                                return 2;
                                            }
                                            Data.Models.AnnualLeaveDetail annualLeaveDetailNext = new Data.Models.AnnualLeaveDetail();
                                            annualLeaveDetailNext.userId = nextUser ?? 0;
                                            annualLeaveDetailNext.guid = Guid.NewGuid();
                                            annualLeaveDetailNext.createdDate = DateTime.Now;
                                            annualLeaveDetailNext.siraNo = 6;
                                            annualLeaveDetailNext.anuId = id;
                                            annualLeaveDetailNext.isReplied = false;
                                            annualLeaveDetailNext.enabled = true;
                                            await bllAnnualLeaveDetail.Add(annualLeaveDetailNext);

                                            UserByNameEMailDto nextUserEmail = bllAdminUsers.getUserByNameAndEmail(nextUser ?? 0);
                                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                            EmailMessage emailMessage = new EmailMessage();
                                            emailMessage.isSent = false;
                                            emailMessage.toAddress = nextUserEmail.email;
                                            emailMessage.mailTuru = 1;
                                            emailMessage.createdDate = DateTime.Now;
                                            emailMessage.createdUserId = user.Id;
                                            emailMessage.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                                            string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                                                    + annualLeaveTable.Id.ToString() + " Id'li izin tamamlanmaya düşmüştür.<br/></h4>";
                                            emailMessage.emailText = mailString;
                                            emailMessage.plannedDate = DateTime.Now;
                                            emailMessage.enabled = true;
                                            await bllEmailMessages.Add(emailMessage);

                                            AdminUser? nextUserEmailOwner = bllAdminUsers.GetByID(annualLeaveTable.userId);
                                            EmailMessage emailMessageOwner = new EmailMessage();
                                            emailMessageOwner.isSent = false;
                                            emailMessageOwner.toAddress = nextUserEmailOwner?.email;
                                            emailMessageOwner.mailTuru = 1;
                                            emailMessageOwner.createdDate = DateTime.Now;
                                            emailMessageOwner.createdUserId = user.Id;
                                            emailMessageOwner.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                                            string mailStringOwner = "<h2>Sayın " + nextUserEmailOwner?.name + "</h2><br/>" + "<h4>"
                                                    + annualLeaveTable.startDate.ToString("dd.MM.yyyy") + " - "
                                                    + annualLeaveTable.endDate.ToString("dd.MM.yyyy")
                                                    + " tarihleri arasında girmiş olduğunuz izin talebiniz onaylandı.<br/> <p><b>İzninizin geçerli olabilmesi için izin formunun ıslak imzalı halini İnsan Kaynakları birimine teslim etmeniz gerekmektedir.</b></p><br/></h4>";
                                            emailMessageOwner.emailText = mailStringOwner;
                                            emailMessageOwner.plannedDate = DateTime.Now;
                                            emailMessageOwner.enabled = true;
                                            await bllEmailMessages.Add(emailMessageOwner);
                                            return 1;
                                        }
                                    }

                                }

                                else if (annualLeaveTable.siraNo == 5 && annualLeaveTable.currentUserId.Equals(user.Id)
                                        && annualLeaveTable.typeId.Equals(4)
                                        && annualLeaveTable.dayRequested >= 10m)
                                {
                                    return await onaylaBurakBey(userMain.izinOnayId ?? 0, annualLeaveTable, id, 5, loggedUserId);

                                }

                                else
                                {
                                    return 4;
                                }
                            }
                            else
                            {
                                return 4;
                            }
                        }
                        else
                        {
                            return 4;
                        }
                    }
                    else
                    {
                        return 4;
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);

                    return 4;
                }
            }


            public async Task<int> onayla1(Data.Models.AnnualLeaveTable annualLeaveTable, int anuId, int siraNo, int loggedUserId)
            {
                // nextUser-> faruk bey
                int nextUser = 6894;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(loggedUserId);
                annualLeaveTable.currentUserId = nextUser;
                annualLeaveTable.siraNo = siraNo + 1;
                annualLeaveTable.updatedDate = DateTime.Now;
                annualLeaveTable.updatedUserId = loggedUserId;
                await Update(annualLeaveTable);

                BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail = new BLLActions.AnnualLeaveDetail(_configuration, _env);
                Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail.findByAnuIdAndUserIdAndSiraNoAndEnabled(anuId,
                        user.Id, siraNo, true);
                if (annualLeaveDetail != null)
                {
                    annualLeaveDetail.replyDate = DateTime.Now;
                    annualLeaveDetail.isReplied = true;
                    annualLeaveDetail.approved = true;
                    annualLeaveDetail.updatedDate = DateTime.Now;
                    annualLeaveDetail.updatedUserId = loggedUserId;
                    await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                }
                else
                {
                    return 2;
                }
                Data.Models.AnnualLeaveDetail annualLeaveDetailNext = new Data.Models.AnnualLeaveDetail();
                annualLeaveDetailNext.userId = nextUser;
                annualLeaveDetailNext.guid = Guid.NewGuid();
                annualLeaveDetailNext.createdDate = DateTime.Now;
                annualLeaveDetailNext.createdUserId = loggedUserId;
                annualLeaveDetailNext.siraNo = siraNo + 1;
                annualLeaveDetailNext.anuId = anuId;
                annualLeaveDetailNext.isReplied = false;
                annualLeaveDetailNext.enabled = true;
                await bllAnnualLeaveDetail.Add(annualLeaveDetailNext);

                UserByNameEMailDto nextUserEmail = bllAdminUsers.getUserByNameAndEmail(nextUser);
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.isSent = false;
                emailMessage.toAddress = nextUserEmail.email;
                emailMessage.mailTuru = 1;
                emailMessage.createdDate = DateTime.Now;
                emailMessage.createdUserId = user.Id;
                emailMessage.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                        + annualLeaveTable.Id.ToString() + " Id'li izin onayınızı beklemektedir.<br/></h4>";
                emailMessage.emailText = mailString;
                emailMessage.plannedDate = DateTime.Now;
                emailMessage.enabled = true;
                await bllEmailMessages.Add(emailMessage);
                return 1;
            }

            public async Task<int> onaylaFarukBey(Data.Models.AnnualLeaveTable annualLeaveTable, int anuId, int siraNo, int loggedUserId)
            {
                int nextUser = 5758;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(loggedUserId);
                annualLeaveTable.currentUserId = nextUser;
                annualLeaveTable.siraNo = siraNo + 1;
                annualLeaveTable.updatedDate = DateTime.Now;
                annualLeaveTable.updatedUserId = loggedUserId;
                await Update(annualLeaveTable);
                BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail = new BLLActions.AnnualLeaveDetail(_configuration, _env);
                Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail.findByAnuIdAndUserIdAndSiraNoAndEnabled(anuId,
                        user.Id, siraNo, true);
                if (annualLeaveDetail != null)
                {
                    annualLeaveDetail.replyDate = DateTime.Now;
                    annualLeaveDetail.isReplied = true;
                    annualLeaveDetail.approved = true;
                    annualLeaveDetail.updatedUserId = loggedUserId;
                    annualLeaveDetail.updatedDate = DateTime.Now;
                    await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                }
                else
                {
                    return 2;
                }
                Data.Models.AnnualLeaveDetail annualLeaveDetailNext = new Data.Models.AnnualLeaveDetail();
                annualLeaveDetailNext.userId = nextUser;
                annualLeaveDetailNext.guid = Guid.NewGuid();
                annualLeaveDetailNext.createdDate = DateTime.Now;
                annualLeaveDetailNext.createdUserId = loggedUserId;
                annualLeaveDetailNext.siraNo = siraNo + 1;
                annualLeaveDetailNext.anuId = anuId;
                annualLeaveDetailNext.isReplied = false;
                annualLeaveDetailNext.enabled = true;
                await bllAnnualLeaveDetail.Add(annualLeaveDetailNext);

                UserByNameEMailDto nextUserEmail = bllAdminUsers.getUserByNameAndEmail(nextUser);
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.isSent = false;
                emailMessage.toAddress = nextUserEmail.email;
                emailMessage.mailTuru = 1;
                emailMessage.createdDate = DateTime.Now;
                emailMessage.createdUserId = user.Id;
                emailMessage.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                        + annualLeaveTable.Id.ToString() + " Id'li izin onayınızı beklemektedir.<br/></h4>";
                emailMessage.emailText = mailString;
                emailMessage.plannedDate = DateTime.Now;
                emailMessage.enabled = true;
                await bllEmailMessages.Add(emailMessage);
                return 1;
            }

            public async Task<int> onaylaBurakBey(int izinonayId, Data.Models.AnnualLeaveTable annualLeaveTable, int anuId, int siraNo, int loggedUserId)
            {
                int nextUser = izinonayId;
                // nextUser -> izinonayId
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(loggedUserId);
                //DateTimeFormatter dtFormatter = DateTimeFormatter.ofPattern("dd.MM.yyyy");
                annualLeaveTable.currentUserId = nextUser;
                annualLeaveTable.siraNo = 6;
                annualLeaveTable.updatedDate = DateTime.Now;
                annualLeaveTable.updatedUserId = loggedUserId;
                await Update(annualLeaveTable);

                BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail = new BLLActions.AnnualLeaveDetail(_configuration, _env);

                Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail.findByAnuIdAndUserIdAndSiraNoAndEnabled(anuId,
                        user.Id, siraNo, true);
                if (annualLeaveDetail != null)
                {
                    annualLeaveDetail.replyDate = DateTime.Now;
                    annualLeaveDetail.isReplied = true;
                    annualLeaveDetail.approved = true;
                    annualLeaveDetail.updatedDate = DateTime.Now;
                    annualLeaveDetail.updatedUserId = loggedUserId;
                    await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                }
                else
                {
                    return 2;
                }
                Data.Models.AnnualLeaveDetail annualLeaveDetailNext = new Data.Models.AnnualLeaveDetail();
                annualLeaveDetailNext.userId = nextUser;
                annualLeaveDetailNext.guid = Guid.NewGuid();
                annualLeaveDetailNext.createdDate = DateTime.Now;
                annualLeaveDetailNext.createdUserId = loggedUserId;
                annualLeaveDetailNext.siraNo = 6;
                annualLeaveDetailNext.anuId = anuId;
                annualLeaveDetailNext.isReplied = false;
                annualLeaveDetailNext.enabled = true;
                await bllAnnualLeaveDetail.Add(annualLeaveDetailNext);

                UserByNameEMailDto nextUserEmail = bllAdminUsers.getUserByNameAndEmail(nextUser);
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.isSent = false;
                emailMessage.toAddress = nextUserEmail.email;
                emailMessage.mailTuru = 1;
                emailMessage.createdDate = DateTime.Now;
                emailMessage.createdUserId = user.Id;
                emailMessage.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                        + annualLeaveTable.Id.ToString() + " Id'li izin tamamlanmaya düşmüştür.<br/></h4>";
                emailMessage.emailText = mailString;
                emailMessage.plannedDate = DateTime.Now;
                emailMessage.enabled = true;
                await bllEmailMessages.Add(emailMessage);

                AdminUser? nextUserEmailOwner = bllAdminUsers.GetByID(annualLeaveTable.userId);
                EmailMessage emailMessageOwner = new EmailMessage();
                emailMessageOwner.isSent = false;
                emailMessageOwner.toAddress = nextUserEmailOwner?.email;
                emailMessageOwner.mailTuru = 1;
                emailMessageOwner.createdDate = DateTime.Now;
                emailMessageOwner.createdUserId = user.Id;
                emailMessageOwner.subject = annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                string mailStringOwner = "<h2>Sayın " + nextUserEmailOwner?.name + "</h2><br/>" + "<h4>"
                        + annualLeaveTable.startDate.ToString("dd.MM.yyyy") + " - "
                        + annualLeaveTable.endDate.ToString("dd.MM.yyyy")
                        + " tarihleri arasında girmiş olduğunuz izin talebiniz onaylandı.<br/> <p><b>İzninizin geçerli olabilmesi için izin formunun ıslak imzalı halini İnsan Kaynakları birimine teslim etmeniz gerekmektedir.</b></p><br/></h4>";
                emailMessageOwner.emailText = mailStringOwner;
                emailMessageOwner.plannedDate = DateTime.Now;
                emailMessageOwner.enabled = true;
                await bllEmailMessages.Add(emailMessageOwner);
                return 1;

            }

            public PageReturn<AnnualLeaveTableDto>? iklist(FilterPageParam<AnnualLeaveFilterDtoRequest> filterPageParam)
            {

                PageReturn<AnnualLeaveTableDto> result = new PageReturn<AnnualLeaveTableDto>();

                int pageSize = filterPageParam?.size ?? 10;
                int pageNumber = filterPageParam?.page ?? 0;

                int? id = filterPageParam?.liste?.id;
                int? userId = filterPageParam?.liste?.userId;
                int? searchUserId = filterPageParam?.liste?.searchUserId;
                int? currentStateId = filterPageParam?.liste?.currentStateId;


                IQueryable<Data.Models.AnnualLeaveTable> query = dal.Get(a =>
                   ( userId == null || a.currentUserId ==userId) && 
			 ( searchUserId == null || a.userId ==searchUserId) && 
			( id == null || a.Id ==id ) &&
			a.currentStateId ==currentStateId && (a.siraNo== 6) && a.enabled ==true
                )
                .OrderByDescending(u => u.Id);

                var pagedData = query
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .Select(u => new AnnualLeaveTableDto
                    {
                        id = u.Id,
                        currentStateId = u.currentStateId,
                        endDate = u.endDate.ToString("dd.MM.yyyy HH:mm"),
                        istenenIzin = Convert.ToDouble(u.dayRequested),
                        kalanIzin = Convert.ToDouble(u.dayleft),
                        startDate = u.startDate.ToString("dd.MM.yyyy HH:mm"),
                        username = u.user != null ? u.user.name : null
                    })
                    .OrderByDescending(u => u.id)
                    .ToList();

                result.content = pagedData;
                result.totalElements = query.Count();
                result.number = pagedData.Count;
                result.size = pageSize;

                return result;
            }

            public PageReturn<AnnualLeaveTableDto>? listFinished(
      FilterPageParam<AnnualLeaveFilterDtoRequest> filterPageParam,
      int adminUserId)
            {
                AdminUsers bllAdminUser = new AdminUsers(_configuration, _env, _mapper);
                Data.Models.AdminUser? adminUser = bllAdminUser.GetByID(adminUserId);

                if (adminUser == null)
                    return null;

                PageReturn<AnnualLeaveTableDto> result = new();

                int pageSize = filterPageParam?.size ?? 10;
                int pageNumber = filterPageParam?.page ?? 0;

                int? id = filterPageParam?.liste?.id;
                int? userId = filterPageParam?.liste?.userId;
                int? searchUserId = filterPageParam?.liste?.searchUserId;

                DateTime today = DateTime.Now;

                IQueryable<Data.Models.AnnualLeaveTable> query;

                bool annualLeaveCanSeeLogs =
                    adminUser.role?.RoleDetail?.Any(x =>
                        x.moduleId == (int)CommonConstants.MODULES.ANNUALLEAVE &&
                        x.canSeeLogs) ?? false;

                bool annualCalendarCanSeeLogs =
                    adminUser.role?.RoleDetail?.Any(x =>
                        x.moduleId == (int) CommonConstants.MODULES.ANNUALCALENDAR &&
                        x.canSeeLogs) ?? false;

                if (adminUser.roleId == 1)
                {
                    query = dal.Get(x =>
                        x.enabled &&
                        x.currentStateId != 1 &&
                        (id == null || x.Id == id) &&
                        (searchUserId == null || x.userId == searchUserId));
                }
                else if (annualLeaveCanSeeLogs)
                {
                    List<int> companyIds = adminUser.role.companies
                        .Replace("[", "")
                        .Replace("]", "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToList();

                    query = dal.Get(x =>
                        x.enabled &&
                        x.currentStateId != 1 &&
                        (id == null || x.Id == id) &&
                        (searchUserId == null || x.userId == searchUserId) &&
                        companyIds.Contains(x.user.companyId));
                }
                else if (annualCalendarCanSeeLogs)
                {
                    query = dal.Get(x =>
                        x.enabled &&
                        x.currentStateId != 1 &&
                        (id == null || x.Id == id) &&
                        (userId == null ||
                         x.userId == userId ||
                         x.currentUserId == userId));
                }
                else
                {
                    query = dal.Get(x =>

                        (
                            (userId == null || x.userId == userId) &&
                            (id == null || x.Id == id) &&
                            x.currentStateId != 1 &&
                            x.enabled
                        )

                        ||

                        (

                            x.AnnualLeaveDetail.Any(d => d.userId == userId) &&
                            today >= x.startDate &&
                            today <= x.endDate

                        )
                    );
                }

                int totalCount = query.Count();

                var pagedData = query
                    .OrderByDescending(x => x.Id)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .Select(x => new AnnualLeaveTableDto
                    {
                        id = x.Id,
                        currentStateId = x.currentStateId,
                        startDate = x.startDate.ToString("dd.MM.yyyy HH:mm"),
                        endDate = x.endDate.ToString("dd.MM.yyyy HH:mm"),
                        istenenIzin = Convert.ToDouble(x.dayRequested),
                        kalanIzin = Convert.ToDouble(x.dayleft),
                        username = x.user.name
                    })
                    .ToList();

                result.content = pagedData;
                result.totalElements = totalCount;
                result.number = pageNumber;
                result.size = pageSize;

                return result;
            }

            public async Task<int> reject(int id, int loggedUserId)
            {
                try
                {

                    Data.Models.AnnualLeaveTable? annualLeaveTable = GetByID(id);
                    if (annualLeaveTable != null) {
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser user = bllAdminUsers.GetByID(loggedUserId);
                        annualLeaveTable.currentStateId=2;
                        await Update(annualLeaveTable);
                        BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail = new BLLActions.AnnualLeaveDetail(_configuration, _env);
                        Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail
                                .findByAnuIdAndUserIdAndSiraNoAndEnabled(id, user.Id, annualLeaveTable.siraNo, true);
                        if (annualLeaveDetail != null)
                        {
                            annualLeaveDetail.replyDate=DateTime.Now;
                            annualLeaveDetail.isReplied=true;
                            annualLeaveDetail.approved=false;
                            await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                        }
                        else
                        {
                            return 2;
                        }

                        UserByNameEMailDto nextUserEmail = bllAdminUsers
                                .getUserByNameAndEmail(annualLeaveTable.createdUserId ??0);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.isSent=false;
                        emailMessage.toAddress=nextUserEmail.email;
                        emailMessage.mailTuru=1;
                        emailMessage.createdDate=DateTime.Now;
                        emailMessage.createdUserId=user.Id;
                        emailMessage.subject=annualLeaveTable.Id.ToString() + " Nolu İzin hk.";
                        string mailString = "<h2>Sayın " + nextUserEmail.name + "</h2><br/>" + "<h4>"
                                + annualLeaveTable.Id.ToString() + " Id'li izin red edilmiştir.<br/></h4>";
                        emailMessage.emailText=mailString;
                        emailMessage.plannedDate=DateTime.Now;
                        emailMessage.enabled=true;
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);

                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                
                }
                catch (Exception e)
                {
                    
                    return 2;
                }
            
          
            }

            public async Task<int> onaylaIK(int id, int loggedUserId)
            {

                try
                {

                    Data.Models.AnnualLeaveTable? annualLeaveTable = GetByID(id);
                    if (annualLeaveTable != null)
                    {
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser user = bllAdminUsers.GetByID(loggedUserId);
                        if (annualLeaveTable.siraNo == 6 && annualLeaveTable.currentUserId.Equals(user.Id))
                        {
                            annualLeaveTable.currentStateId = 4;
                            annualLeaveTable.siraNo = 10;
                            await Update(annualLeaveTable);
                            BLLActions.AnnualLeaveDetail bllAnnualLeaveDetail = new BLLActions.AnnualLeaveDetail(_configuration, _env);
                            Data.Models.AnnualLeaveDetail annualLeaveDetail = bllAnnualLeaveDetail
                                    .findByAnuIdAndUserIdAndSiraNoAndEnabled(id, user.Id, 6, true);
                            if (annualLeaveDetail != null)
                            {
                                annualLeaveDetail.replyDate = DateTime.Now;
                                annualLeaveDetail.isReplied = true;
                                annualLeaveDetail.approved = true;
                                annualLeaveDetail.updatedDate = DateTime.Now;
                                annualLeaveDetail.updatedUserId = loggedUserId;
                                await bllAnnualLeaveDetail.Update(annualLeaveDetail);
                            }
                            else
                            {
                                return 2;
                            }
                            AdminUser nextUserEmail = bllAdminUsers.GetByID(annualLeaveTable.userId);

                            decimal bigDecimal = new decimal(7.5);
                            string saat = (annualLeaveTable.dayRequested * bigDecimal).ToString();
                            BLLActions.AnnualLeaveType bllAnnualLeaveType = new BLLActions.AnnualLeaveType(_configuration, _env);
                            Data.Models.AnnualLeaveType? annualLeaveType = bllAnnualLeaveType.GetByID(annualLeaveTable.typeId);
                            string? izinTuru = annualLeaveType?.sapCode;
                            if (izinTuru != null)
                            {
                                DateTime? endoftime = null;

                                switch (annualLeaveTable.typeId)
                                {
                                    case 2:
                                    case 3:
                                    case 4:
                                        {
                                            var tempDate = annualLeaveTable.endDate.AddDays(-1);

                                            switch (tempDate.DayOfWeek)
                                            {
                                                case DayOfWeek.Sunday:
                                                    endoftime = annualLeaveTable.endDate.AddDays(-2);
                                                    break;

                                                default:
                                                    endoftime = tempDate;
                                                    break;
                                            }
                                            break;
                                        }

                                    case 6:
                                    case 7:
                                    case 8:
                                        endoftime = annualLeaveTable.endDate.AddDays(-1);
                                        break;

                                    default:
                                        endoftime = DateTime.Now;
                                        break;
                                }

                                Data.Models.HRAnnualSapIntegration annualSapIntegration = new Data.Models.HRAnnualSapIntegration();
                                annualSapIntegration.approval = false;
                                annualSapIntegration.userId = user.Id;
                                annualSapIntegration.duzeltme = annualLeaveTable.dayRequested.ToString().Replace(".", ",");
                                annualSapIntegration.izinbaslangici = annualLeaveTable.startDate;
                                annualSapIntegration.izinbitisi = endoftime;
                                annualSapIntegration.saat = saat.Replace(".", ",");
                                annualSapIntegration.izinturu = izinTuru;
                                annualSapIntegration.yillikIzinId = annualLeaveTable.Id;
                                annualSapIntegration.perno = nextUserEmail?.perNo;
                                annualSapIntegration.enabled = true;
                                BLLActions.HRAnnualSapIntegration bllHRAnnualSapIntegration = new BLLActions.HRAnnualSapIntegration(_configuration, _env);
                                await bllHRAnnualSapIntegration.Add(annualSapIntegration);
                            }

                            return 1;
                        }
                        else
                        {
                            return 4;
                        }
                    }
                  
                
                    else
                    {
                        return 4;

                    }
                }
                catch (Exception e)
                {
                    return 4;
                }
            }
        }
    }



}
