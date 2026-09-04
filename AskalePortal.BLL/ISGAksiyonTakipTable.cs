using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ISGAksiyonTakipTable : BaseBLL<AskalePortal.Data.Models.ISGAksiyonTakipTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;

            public ISGAksiyonTakipTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper=mapper;
            }
            public List<AskalePortal.Data.Models.ISGAksiyonTakipTable> GetByUserID(int userID, int? id)
            {
                string userId = userID.ToString();

                return dal.Get(u => (u.aksiyon.bidirimdeBulunan == userID || u.aksiyonSorumlulari.Contains(userId)) && (id.HasValue ? u.aksiyonId == id.Value : true) && u.enabled == true).ToList();
            }

            public string SetSAP(string kisaAciklama, int priority, string username, string alinmasiGerekenOnlemler, string sirket)
            {
                string time = DateTime.Now.ToString("HH:mm:ss");
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                string result = string.Empty;
                if (sapConn != null)
                {
                    sapConn.Connect();
                    ISapFunction sapFunction = sapConn.CreateFunction("ZWEBIISGAKSIYON");
                    result = sapFunction.Invoke<string>(input: new ISGAksiyonTakipTableInputParams
                    {
                        SHORT_TEXT = Constants.CommonConstants.HTMLDonusum(kisaAciklama),
                        PRIORITY = priority.ToString(),
                        NOTIFTIME = time,
                        NOTIF_DATE = DateTime.Now.ToString(),
                        PLANPLANT = sirket,
                        REPORTEDBY = username,
                        TEXT_LINE = Constants.CommonConstants.HTMLDonusum(alinmasiGerekenOnlemler)

                    }


                    );
                    sapConn.Disconnect();
                    return result;
                }
                else
                {
                    return "";
                }







            }

            public List<AskalePortal.Data.Models.ISGAksiyonTakipTable> GetByUser(int userId, int? Id)
            {
                return dal.Get(u => (Id.HasValue ? u.aksiyonId == Id.Value : true) && u.aksiyon.bidirimdeBulunan == userId && u.enabled == true).ToList();
            }

            public List<AskalePortal.Data.Models.ISGAksiyonTakipTable> GetAllWithCompanies(int[] companyIds, int? id)
            {
                return dal.Get(u => companyIds.Contains(u.aksiyon.companyId) && (id.HasValue ? u.aksiyonId == id.Value : true) && u.enabled == true).ToList();
            }

            public List<Data.Models.ISGAksiyonTakipTable> listAllByAksiyonIdAndEnabled(int aksiyonId, bool enabled)
            {
                List<Data.Models.ISGAksiyonTakipTable> deger = dal.Get(u=>u.enabled==enabled && u.aksiyonId==aksiyonId).ToList();
                foreach (Data.Models.ISGAksiyonTakipTable isg in deger)
                {
                    string[] liste = isg.aksiyonSorumlulari.Split(",");
                    string aksiyonSorumlusu = "";
                    foreach (string item in liste)
                    {
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        aksiyonSorumlusu += bllAdminUsers.GetByID(int.Parse(item.Replace("[", "").Replace("]", "")))?.name + ",";
                    }
                    if (!string.IsNullOrEmpty(aksiyonSorumlusu))
                    {
                        aksiyonSorumlusu = aksiyonSorumlusu.Substring(0, aksiyonSorumlusu.Count() - 1);
                    }
                    isg.aksiyonSorumlulari=aksiyonSorumlusu;

                }
                return deger;
            }

            public async Task<Data.ResponseModels.ISGAksiyonTakipTableSaveDto> saveTakip(ISGAksiyonTakipTableSaveDto? entity, int? sapBildirim, int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration,_env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(userId);
                Data.Models.ISGAksiyonTakipTable entitysave;
                if (entity.id == null)
                {
                    entity.createdUserId=user.Id;
                    entity.createdDate=DateTime.Now.ToString();
                    entity.enabled=true;
                    entitysave = await Add(_mapper.Map<Data.Models.ISGAksiyonTakipTable>(entity));
                }
                else
                {
                    entity.updatedUserId=user.Id;
                    entity.updateDate=DateTime.Now.ToString();
                    entity.enabled = true;
                    entitysave = await Update(_mapper.Map<Data.Models.ISGAksiyonTakipTable>(entity));
                }
                
                string[] listUserId = entitysave.aksiyonSorumlulari.Split(",");

                foreach (string userIdIsg in listUserId)
                {
                    if (!string.IsNullOrEmpty(userIdIsg))
                    {
                        string userIdForISG = userIdIsg.Replace("[", "").Replace("]", "");
                        int idISG = int.Parse(userIdForISG);
                        AdminUser userISG =bllAdminUsers.GetByID(idISG);

                        EmailMessage msg1 = new EmailMessage();
                        msg1.subject=(entitysave.aksiyonId.ToString() + " nolu İSG talebine açılmış İSG İş Bildirimi");
                        msg1.toAddress=userISG.email;
                        msg1.plannedDate=DateTime.Now;

                        BLLActions.EmailReaderFile
                                   bllEmailReaderFile =
                                       new BLLActions.EmailReaderFile();

                        msg1.emailText =
                            bllEmailReaderFile
                                .BuildEmailTemplate(_configuration, _env,userISG.name,
                                entitysave.aksiyonId.ToString() + " No'lu isg talebine aksiyon girilmiştir.<br /><br /> Açıklama:"
                                + entitysave.kisaAciklama + "<br /><br />Alınması Gereken Önlemler:" + entitysave.alinmasiGerekenOnlemler +
                                " <br /><br />Saygılarımızla.");

                
                        msg1.isSent=false;

                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(msg1);
                    }
                }
                if (sapBildirim == 1)
                {
                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                    Company company = bllCompanies.getById(user.companyId);
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBIISGAKSIYON");

                        sapFunction.Invoke(
                           input: new ISGAksiyonTakipTableInputParams
                           {
                               SHORT_TEXT = HtmlToText(entitysave.kisaAciklama ?? string.Empty),
                               PRIORITY = entitysave.oncelik.ToString(),
                               NOTIF_DATE = DateTime.Now.ToString(),
                               NOTIFTIME = DateTime.Now.ToString("HH:mm:ss"),
                               REPORTEDBY = user.username,
                               PLANPLANT = company.vkorg,
                               TEXT_LINE = HtmlToText(entitysave.alinmasiGerekenOnlemler)

                           });
                    }
                    
                }
                return _mapper.Map< ISGAksiyonTakipTableSaveDto >(entitysave);


            }


            private static string HtmlToText(string? html)
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

                text = Regex.Replace(
                    text,
                    @"<br\s*/?>",
                    Environment.NewLine,
                    RegexOptions.IgnoreCase);

                text = Regex.Replace(
                    text,
                    @"</p\s*>",
                    Environment.NewLine,
                    RegexOptions.IgnoreCase);

                text = Regex.Replace(
                    text,
                    @"<[^>]+>",
                    string.Empty);

                text = WebUtility.HtmlDecode(text);

                return text.Trim();
            }
        }
    }
}