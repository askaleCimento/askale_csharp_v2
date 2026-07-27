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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class SozlesmeTable : BaseBLL<AskalePortal.Data.Models.SozlesmeTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public SozlesmeTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public async Task<Data.Models.SozlesmeTable> completedSozlesme(int sozlesmeTableId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);

                Data.Models.SozlesmeTable? sozlesmeTable = GetByID(sozlesmeTableId);
                sozlesmeTable!.tamamMi = (true);
                List<string> bildirimKisiler = (sozlesmeTable.bildirimYapilacakKisiler ?? "").Split(',').ToList();

                // bildirim yapılacak kişiler
                if (!(sozlesmeTable.bildirimYapilacakKisiler ?? "").Equals(""))
                {
                    foreach (string kisiId in bildirimKisiler)
                    {
                        int id = int.Parse(kisiId);
                        UserByNameEMailDto user = bllAdminUsers.getUserByNameAndEmail(id);
                        createMailComplete(user, sozlesmeTable);
                    }
                }

                // oluşturan kişi
                UserByNameEMailDto createdUser = bllAdminUsers.getUserByNameAndEmail(sozlesmeTable.createdUserId ?? 0);
                createMailComplete(createdUser, sozlesmeTable);

                // direktör
                BLLActions.SozlesmeMailTable bllSozlesmeMailTable = new BLLActions.SozlesmeMailTable(_configuration, _env);
                List<Data.Models.SozlesmeMailTable> listSozlesmeMailTable = bllSozlesmeMailTable.GetAll();
                int direktorId = listSozlesmeMailTable.Find(p => p.satinAlmaGrubu.Contains("Direktör"))?.userId ?? 0;
                if (direktorId != 0)
                {
                    UserByNameEMailDto direktorUser = bllAdminUsers.getUserByNameAndEmail(direktorId);
                    createMailComplete(direktorUser, sozlesmeTable);
                }


                // mudur
                if (sozlesmeTable.satinAlmaGrubu.Contains("Satinalma"))
                {
                    int mudurId = listSozlesmeMailTable.Find(p => p.satinAlmaGrubu.Contains("Satınalma"))?.userId ?? 0;
                    if (mudurId != 0)
                    {
                        UserByNameEMailDto mudurUser = bllAdminUsers.getUserByNameAndEmail(mudurId);
                        createMailComplete(mudurUser, sozlesmeTable);
                    }


                }
                else if (sozlesmeTable.satinAlmaGrubu.Contains("Lojistik"))
                {
                    int mudurId = listSozlesmeMailTable.Find(p => p.satinAlmaGrubu.Contains("Lojistik"))?.userId ?? 0;
                    if (mudurId != 0)
                    {
                        UserByNameEMailDto mudurUser = bllAdminUsers.getUserByNameAndEmail(mudurId);
                        createMailComplete(mudurUser, sozlesmeTable);
                    }


                }
                else if (sozlesmeTable.satinAlmaGrubu.Contains("Hazırbeton"))
                {
                    int mudurId = listSozlesmeMailTable.Find(p => p.satinAlmaGrubu.Contains("Hazırbeton"))?.userId ?? 0;
                    if (mudurId != 0)
                    {
                        UserByNameEMailDto mudurUser = bllAdminUsers.getUserByNameAndEmail(mudurId);
                        createMailComplete(mudurUser, sozlesmeTable);
                    }

                }

                return await Update(sozlesmeTable);
            }

            private async void createMailComplete(UserByNameEMailDto user, Data.Models.SozlesmeTable entity)
            {
                BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                Company company = bllCompanies.getById(entity.companyId);

                BLLActions.SozlesmeCinsiTable bllSozlesmeCinsiTable = new BLLActions.SozlesmeCinsiTable(_configuration, _env);
                Data.Models.SozlesmeCinsiTable? sozlesmeCinsiTable =  bllSozlesmeCinsiTable.GetByID(entity.sozlesmeTuruId);

                BLLActions.SaticiFirmalarTable bllSaticiFirmalarTable = new BLLActions.SaticiFirmalarTable(_configuration, _env);
                List<Data.Models.SaticiFirmalarTable> listSaticiFirmalarTable = bllSaticiFirmalarTable.findByFirmaAdiCompany(entity.firmaKodu, company.Id);

                EmailMessage emailMessage = new EmailMessage();
                emailMessage.subject = (entity.Id.ToString() + " Nolu Sözleşme Tamamlama");
                emailMessage.toAddress = (user.email);
                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                string mailMessage = bllEmailReaderFile.CreateSozlesmeMailString(_configuration, _env, "Sayın" + " " + user.name, entity.Id, company.vtext,
                        sozlesmeCinsiTable?.sozlesmeCinsi ??"", listSaticiFirmalarTable.FirstOrDefault()?.firmaAdi ?? "", entity.sozlesmeKonusu,
                        entity.aciklama, entity.sozlesmeTutari.ToString(), entity.bitisTarihi.ToString());
                emailMessage.emailText = (mailMessage);
                emailMessage.mailTuru = (1);
                emailMessage.enabled = (true);
                emailMessage.isSent = (false);
                emailMessage.plannedDate = (DateTime.Now);
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                await bllEmailMessages.Add(emailMessage);
            }

            public PageReturn<SozlesmeTableDto>? FilterPageableDto(FilterPageParam<SozlesmeTableListDtoParameter> filterPageParam)
            {

                PageReturn<SozlesmeTableDto>? result = new PageReturn<SozlesmeTableDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? filterSozlesmeNo = filterPageParam?.liste?.filterSozlesmeNo;
                int? filterCompanyId = filterPageParam?.liste?.filterCompanyId;
                int? filterSozlesmeCinsiId = filterPageParam?.liste?.filterSozlesmeCinsiId;
                string? filterSozlesmeKonusu = filterPageParam?.liste?.filterSozlesmeKonusu;
                string? filterAciklama = filterPageParam?.liste?.filterAciklama;
                string? filterSozlesmeTutari = filterPageParam?.liste?.filterSozlesmeTutari;
                string? filterFirmaAdi = filterPageParam?.liste?.filterFirmaAdi;
                DateTime? filterBaslangicTarih = filterPageParam?.liste?.filterBaslangicTarih;
                DateTime? filterBitisTarih = filterPageParam?.liste?.filterBitisTarih;
                bool? filterTamamlandimi = filterPageParam?.liste?.filterTamamlandimi;

                var query = from a in dal.Get(a => a.enabled &&
                   (filterSozlesmeNo == null || filterSozlesmeNo == 0 || a.Id == filterSozlesmeNo) &&
                   (filterCompanyId == null || filterCompanyId == 0 || a.company.Id == filterCompanyId) &&
                   (filterSozlesmeCinsiId == null || filterSozlesmeCinsiId == 0 || a.sozlesmeTuru.Id == filterSozlesmeCinsiId) &&
                   (string.IsNullOrEmpty(filterSozlesmeKonusu) || a.sozlesmeKonusu.Contains(filterSozlesmeKonusu)) &&
                   (string.IsNullOrEmpty(filterAciklama) || a.aciklama.Contains(filterAciklama)) &&
                   (string.IsNullOrEmpty(filterSozlesmeTutari) || a.sozlesmeTutari.ToString().Contains(filterSozlesmeTutari)) &&
                   (filterBaslangicTarih == null || a.baslangicTarihi == filterBaslangicTarih) &&
                   (filterBitisTarih == null || a.bitisTarihi == filterBitisTarih) &&
                   (filterTamamlandimi == null || a.tamamMi == filterTamamlandimi)
               ).OrderByDescending(u => u.Id)
                            join f in dal.dB.SaticiFirmalarTable on a.firmaKodu equals f.firmaKodu into firmaJoin
                            from firma in firmaJoin.Take(1).DefaultIfEmpty() // left join
                            select new SozlesmeTableDto()
                            {
                                aciklama = a.aciklama,
                                bitisTarihi = a.bitisTarihi,
                                company = a.company.vtext,
                                firmaAdi = firma.firmaAdi ?? "",  // ilişkisiz tablodan gelen veri
                                firmaKodu = a.firmaKodu,
                                id = a.Id,
                                paraBirimi = a.sozlesmeTutarBirimi.paraBirimi,
                                picture = null,
                                sozlesmeCinsi = a.sozlesmeTuru.sozlesmeCinsi,
                                sozlesmeKonusu = a.sozlesmeKonusu,
                                tamamMi = a.tamamMi,
                                tutar = (double)a.sozlesmeTutari
                            };

                result.content = query.Skip(pageSize * pageNumber).Take(pageSize).ToList();

                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                foreach (SozlesmeTableDto sozlesme in result.content)
                {
                    var bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);

                    var attachedFiles = bllAttachedFiles
                        .getByModuleIdAndTargetId((int)CommonConstants.MODULES.SOZLESMEGIRIS, sozlesme.id ?? 0);
                    List<string> listfile = new List<string>();
                    foreach (AttachedFile file in attachedFiles)
                    {
                        string filename = file.title;
                        listfile.Add(filename);
                    }

                    sozlesme.picture = listfile;
                }

                return result;
            }

            public List<AskalePortal.Data.Models.SozlesmeTable> Get(int? id, string sirket, int sozlesmecinsi, string firmaadi,
                string sozlesmekonusu, string aciklama, decimal tutar1, decimal tutar2, string baslangictarihi1, string baslangictarihi2, string bitistarihi1, string bitistarihi2, string sam)
            {
                List<AskalePortal.Data.Models.SozlesmeTable> l = new List<AskalePortal.Data.Models.SozlesmeTable>();
                int sirketId = 0;
                try
                {
                    DateTime time1 = string.IsNullOrEmpty(baslangictarihi1) ? Convert.ToDateTime("01.01.1900") : Convert.ToDateTime(baslangictarihi1);
                    DateTime time2 = string.IsNullOrEmpty(baslangictarihi2) ? Convert.ToDateTime("01.01.1900") : Convert.ToDateTime(baslangictarihi2);
                    DateTime time3 = string.IsNullOrEmpty(bitistarihi1) ? Convert.ToDateTime("01.01.1900") : Convert.ToDateTime(bitistarihi1);
                    DateTime time4 = string.IsNullOrEmpty(bitistarihi2) ? Convert.ToDateTime("01.01.1900") : Convert.ToDateTime(bitistarihi2);
                    bool? tamamMi;

                    if (sam == "1")
                    {
                        tamamMi = false;
                    }
                    else if (sam == "2")
                    {
                        tamamMi = true;
                    }
                    else
                    {
                        tamamMi = null;
                    }

                    l = dal.Get(k => tamamMi.HasValue ? k.tamamMi == tamamMi : true &&
                                      k.enabled == true).ToList();
                    if (!string.IsNullOrEmpty(sirket))
                    {
                        BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                        sirketId = bllCompanies.GetCompanyIdByName(sirket) ?? 0;
                        l = l.Where(u => u.companyId == sirketId).ToList();
                    }
                    if (!string.IsNullOrEmpty(baslangictarihi1))
                    {
                        l = l.Where(u => u.baslangicTarihi >= time1).ToList();
                    }
                    if (!string.IsNullOrEmpty(baslangictarihi2))
                    {
                        l = l.Where(u => u.baslangicTarihi <= time2).ToList();
                    }
                    if (tutar1 != 0)
                    {
                        l = l.Where(u => u.sozlesmeTutari >= tutar1).ToList();
                    }
                    if (tutar2 != 0)
                    {
                        l = l.Where(u => u.sozlesmeTutari <= tutar2).ToList();
                    }

                    if (!string.IsNullOrEmpty(bitistarihi1))
                    {
                        l = l.Where(u => u.bitisTarihi >= time3).ToList();
                    }
                    if (!string.IsNullOrEmpty(bitistarihi2))
                    {
                        l = l.Where(u => u.bitisTarihi <= time4).ToList();
                    }
                    if (!string.IsNullOrEmpty(aciklama))
                    {
                        l = l.Where(u => u.aciklama.Contains(aciklama)).ToList();
                    }
                    if (sozlesmecinsi != 0)
                    {
                        l = l.Where(u => u.sozlesmeTuruId == sozlesmecinsi).ToList();
                    }
                    if (!string.IsNullOrEmpty(sozlesmekonusu))
                    {
                        l = l.Where(u => u.sozlesmeKonusu.Contains(sozlesmekonusu)).ToList();
                    }
                    if (id != 0)
                    {
                        l = l.Where(k => k.Id == id).ToList();
                    }
                    if (!string.IsNullOrEmpty(firmaadi))
                    {
                        BLLActions.SaticiFirmalarTable bllSaticiFirmalarTable = new BLLActions.SaticiFirmalarTable(_configuration, _env);
                        List<string> listSatici = bllSaticiFirmalarTable.GetByFirmaAdiLike(firmaadi);
                        l = l.Where(u => listSatici.Contains(u.firmaKodu)).ToList();
                    }
                    if (!string.IsNullOrEmpty(sozlesmekonusu))
                    {

                        l = l.Where(u => u.sozlesmeKonusu.Contains(sozlesmekonusu)).ToList();
                    }
                    if (!string.IsNullOrEmpty(aciklama))
                    {

                        l = l.Where(u => u.aciklama.Contains(aciklama)).ToList();
                    }

                }
                catch (Exception)
                {

                    return l;
                }
                return l;

            }

            public async Task<Data.ResponseModels.SozlesmeTableSaveDto> save(Data.ResponseModels.SozlesmeTableSaveDto table, int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                if (table.id == null)
                {
                    table.createdUserId = (userId);
                    table.createdDate = (DateTime.Now.ToString());
                    table.enabled = (true);
                    table.tamamMi = table.tamamMi ?? false;
                    List<string> bildirimKisiler = (table.bildirimYapilacakKisiler ?? "").Split(',').ToList();
                    //Data.Models.SozlesmeTable table = _mapper.Map<Data.Models.SozlesmeTable>(entity);
                    try
                    {
                        Data.Models.SozlesmeTable? sozlesmeTable = await Add(_mapper.Map<Data.Models.SozlesmeTable>(table));


                        // bildirim yapılacak kişiler
                        if (!sozlesmeTable!.bildirimYapilacakKisiler.Equals(""))
                        {
                            foreach (string kisiId in bildirimKisiler)
                            {
                                int id = Int32.Parse(kisiId);
                                if (kisiId != null)
                                {
                                    UserByNameEMailDto user = bllAdminUsers.getUserByNameAndEmail(id);
                                    createMail(user, _mapper.Map<Data.ResponseModels.SozlesmeTableSaveDto>(sozlesmeTable));
                                }

                            }
                        }
                        // oluşturan kişi
                        UserByNameEMailDto createdUser = bllAdminUsers.getUserByNameAndEmail(table.createdUserId ?? 0);
                        if (createdUser != null)
                        {
                            createMail(createdUser, _mapper.Map<Data.ResponseModels.SozlesmeTableSaveDto>(sozlesmeTable));
                        }
                        // direktör
                        BLLActions.SozlesmeMailTable bllSozlesmeMailTable = new BLLActions.SozlesmeMailTable(_configuration, _env);
                        List<Data.Models.SozlesmeMailTable> listSozlesmeMailTable = bllSozlesmeMailTable.GetAll();
                        int direktorId = listSozlesmeMailTable.Find(p => p.satinAlmaGrubu.Contains("Direktör"))?.userId ?? 0;

                        if (direktorId != 0)
                        {
                            UserByNameEMailDto direktorUser = bllAdminUsers.getUserByNameAndEmail(direktorId);
                            createMail(direktorUser, _mapper.Map<Data.ResponseModels.SozlesmeTableSaveDto>(sozlesmeTable));
                        }


                        // mudur
                        if (sozlesmeTable.satinAlmaGrubu.Contains("Satınalma"))
                        {
                            int mudurId = listSozlesmeMailTable.Find(p => p.satinAlmaGrubu.Contains("Satınalma"))?.userId ?? 0;
                            if (mudurId != 0)
                            {
                                UserByNameEMailDto mudurUser = bllAdminUsers.getUserByNameAndEmail(mudurId);
                                createMail(mudurUser, _mapper.Map<Data.ResponseModels.SozlesmeTableSaveDto>(sozlesmeTable));
                            }


                        }
                        else if (sozlesmeTable.satinAlmaGrubu.Contains("Lojistik"))
                        {
                            int mudurId = listSozlesmeMailTable.Find(p => p.satinAlmaGrubu.Contains("Lojistik"))?.userId ?? 0;
                            if (mudurId != 0)
                            {
                                UserByNameEMailDto mudurUser = bllAdminUsers.getUserByNameAndEmail(mudurId);
                                createMail(mudurUser, _mapper.Map<Data.ResponseModels.SozlesmeTableSaveDto>(sozlesmeTable));
                            }


                        }
                        else if (sozlesmeTable.satinAlmaGrubu.Contains("Hazırbeton"))
                        {
                            int mudurId = listSozlesmeMailTable.Find(p => p.satinAlmaGrubu.Contains("Hazırbeton"))?.userId ?? 0;
                            if (mudurId != 0)
                            {
                                UserByNameEMailDto mudurUser = bllAdminUsers.getUserByNameAndEmail(mudurId);
                                createMail(mudurUser, _mapper.Map<Data.ResponseModels.SozlesmeTableSaveDto>(sozlesmeTable));
                            }
                        }
                        return _mapper.Map<Data.ResponseModels.SozlesmeTableSaveDto>(sozlesmeTable);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                    }
                    return new Data.ResponseModels.SozlesmeTableSaveDto();

                }
                else
                {

                    table.updatedUserId = (userId);
                    table.updateDate = (DateTime.Now.ToString());
                    table.enabled = (true);
                    await Update(_mapper.Map<Data.Models.SozlesmeTable>(table));
                    return table;
                }

            }

            private async void createMail(UserByNameEMailDto user, Data.ResponseModels.SozlesmeTableSaveDto entity)
            {
                BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                Company company = bllCompanies.getById(entity.companyId ?? 0);
                BLLActions.SozlesmeCinsiTable bllSozlesmeCinsiTable = new BLLActions.SozlesmeCinsiTable(_configuration, _env);
                Data.Models.SozlesmeCinsiTable? sozlesmeCinsiTable = bllSozlesmeCinsiTable.GetByID(entity.sozlesmeTuruId ?? 0);
                BLLActions.SaticiFirmalarTable bllSaticiFirmalarTable = new BLLActions.SaticiFirmalarTable(_configuration, _env);
                Data.Models.SaticiFirmalarTable listSaticiFirmalarTable = bllSaticiFirmalarTable.GetByKod(entity.firmaKodu ?? "", company.Id);
                string bitisTarihiString = entity.bitisTarihi ?? ""; // String olarak alınıyor
                DateTime bitisTarihi;

                if (DateTime.TryParseExact(bitisTarihiString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out bitisTarihi))
                {
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = (entity.id.ToString() + " Nolu sözleşme hatırlatma");
                    emailMessage.toAddress = (user.email);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    string mailMessage = bllEmailReaderFile.CreateSozlesmeMailString(_configuration, _env, "Sayın" + " " + user.name, entity.id ?? 0, company.vtext,
                            sozlesmeCinsiTable?.sozlesmeCinsi ??"", listSaticiFirmalarTable.firmaAdi,
                            entity.sozlesmeKonusu ?? "", entity.aciklama ?? "", entity.sozlesmeTutari.ToString() ?? "",
                            entity.bitisTarihi ?? "");
                    emailMessage.emailText = (mailMessage);
                    emailMessage.mailTuru = (1);
                    emailMessage.enabled = (true);
                    emailMessage.isSent = (false);
                    emailMessage.plannedDate = DateTime.Parse(entity.bitisTarihi ?? "").AddMonths(-3).Date;
                    await bllEmailMessages.Add(emailMessage);
                }

                if (DateTime.Parse(entity.bitisTarihi ?? "").AddMonths(-1) > DateTime.Today)
                {
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);

                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = (entity.id.ToString() + " Nolu sözleşme hatırlatma");
                    emailMessage.toAddress = (user.email);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    string mailMessage = bllEmailReaderFile.CreateSozlesmeMailString(_configuration, _env, "Sayın" + " " + user.name, entity.id ?? 0, company.vtext,
                            sozlesmeCinsiTable?.sozlesmeCinsi??"", listSaticiFirmalarTable.firmaAdi,
                            entity.sozlesmeKonusu ?? "", entity.aciklama ?? "", entity.sozlesmeTutari.ToString() ?? "",
                            entity.bitisTarihi?.ToString() ?? "");
                    emailMessage.emailText = mailMessage;
                    emailMessage.mailTuru = 1;
                    emailMessage.enabled = true;
                    emailMessage.isSent = false;
                    emailMessage.plannedDate = DateTime.Parse(entity.bitisTarihi ?? "").AddMonths(-1).Date;
                    await bllEmailMessages.Add(emailMessage);
                }

                if (DateTime.Parse(entity.bitisTarihi ?? "").AddDays(-15) > DateTime.Now)
                {
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);

                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = (entity.id.ToString() + " Nolu sözleşme hatırlatma");
                    emailMessage.toAddress = (user.email);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    string mailMessage = bllEmailReaderFile.CreateSozlesmeMailString(_configuration, _env, "Sayın" + " " + user.name, entity.id ?? 0, company.vtext,
                            sozlesmeCinsiTable?.sozlesmeCinsi??"", listSaticiFirmalarTable.firmaAdi,
                            entity.sozlesmeKonusu ?? "", entity.aciklama ?? "", entity.sozlesmeTutari.ToString() ?? "",
                            entity.bitisTarihi?.ToString() ?? "");


                    emailMessage.emailText = mailMessage;
                    emailMessage.mailTuru = 1;
                    emailMessage.enabled = true;
                    emailMessage.isSent = false;
                    emailMessage.plannedDate = DateTime.Parse(entity.bitisTarihi ?? "").AddDays(-15).Date;
                    await bllEmailMessages.Add(emailMessage);
                }
                else
                {
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);

                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = (entity.id.ToString() + " Nolu Sözleşme Hatırlatma");
                    emailMessage.toAddress = (user.email);

                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    string mailMessage = bllEmailReaderFile.CreateSozlesmeMailString(_configuration, _env, "Sayın" + " " + user.name ?? "", entity.id ?? 0, company.vtext ?? "",
                            sozlesmeCinsiTable?.sozlesmeCinsi ?? "", listSaticiFirmalarTable.firmaAdi ?? "",
                            entity.sozlesmeKonusu ?? "", entity.aciklama ?? "", (entity.sozlesmeTutari ?? 0).ToString(),
                            (entity.bitisTarihi ?? "").ToString());
                    emailMessage.emailText = (mailMessage);
                    emailMessage.mailTuru = (1);
                    emailMessage.enabled = (true);
                    emailMessage.isSent = (false);
                    emailMessage.plannedDate = (DateTime.Now);
                    await bllEmailMessages.Add(emailMessage);
                }

            }

            public List<SozlesmeTablePdfDto> getPdfList()
            {
                List<Data.Models.SozlesmeTable> listSozlesmeTables = GetAll();
                BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                List<Company> listCompanies = bllCompanies.GetAll();
                BLLActions.ParaBirimiTable bllParaBirimiTable = new BLLActions.ParaBirimiTable(_configuration, _env);
                List<Data.Models.ParaBirimiTable> listBirimiTables = bllParaBirimiTable.GetAll();
                List<SozlesmeTablePdfDto> listPdfDto = new List<SozlesmeTablePdfDto>();
                foreach (Data.Models.SozlesmeTable sozlesmeTable in listSozlesmeTables)
                {
                    SozlesmeTablePdfDto dto = new SozlesmeTablePdfDto();
                    dto.aciklama = (sozlesmeTable.aciklama.Replace("\\<.*?>", ""));
                    dto.baslangicTarihi = (sozlesmeTable.baslangicTarihi);
                    dto.bitisTarihi = (sozlesmeTable.bitisTarihi);
                    dto.damgaVergisiOdemesi = (sozlesmeTable.damgaVergisiOdemesi);
                    dto.firmaKodu = (sozlesmeTable.firmaKodu);
                    dto.firmaYetkisi = (sozlesmeTable.firmaYetkilisi);
                    dto.id = (sozlesmeTable.Id);
                    dto.iletisim = (sozlesmeTable.iletisim);
                    dto.teminatBaslangic = (sozlesmeTable.teminatBaslangic);
                    dto.teminatBitis = (sozlesmeTable.teminatBitis);
                    dto.teminatTutari = (sozlesmeTable.teminatTutari);
                    dto.teminatTutariBirimi = (
                            listBirimiTables.Find(t => t.Id == sozlesmeTable.teminatTutariParaBirimId)?.paraBirimi ?? "");
                    dto.teminatVarmi = (sozlesmeTable.teminatVarmi);
                    dto.uyariTarihi = (sozlesmeTable.uyariTarihi);
                    dto.vkorg = (listCompanies.Find(t => t.Id == sozlesmeTable.companyId)?.vkorg ?? "");
                    dto.vtext = (listCompanies.Find(t => t.Id == sozlesmeTable.companyId)?.vtext ?? "");
                    dto.imzalananTarih = (sozlesmeTable.imzalananTarih);
                    dto.odemeAvansBirimi = (listBirimiTables.Find(t => t.Id == sozlesmeTable.odemeAvansBirimiId)?.paraBirimi ?? "");
                    dto.odemeAvansTutari = (sozlesmeTable.odemeAvansTutari);
                    dto.odemeAvansYuzdesi = (sozlesmeTable.odemeAvansYuzdesi);
                    dto.satinAlmaGrubu = (sozlesmeTable.satinAlmaGrubu);
                    dto.sozlesmeOdemeVadesi = (sozlesmeTable.sozlesmeOdemeVadesi);
                    dto.sozlesmeTutarBirimi = (
                            listBirimiTables.Find(t => t.Id == sozlesmeTable.sozlesmeTutarBirimiId)?.paraBirimi);
                    dto.sozlesmeTutari = (sozlesmeTable.sozlesmeTutari);
                    dto.tamammi = (sozlesmeTable.tamamMi);
                    listPdfDto.Add(dto);
                }

                return listPdfDto;
            }
        }
    }
}
