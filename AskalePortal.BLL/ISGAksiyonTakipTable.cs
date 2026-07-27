using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AskalePortal.Data.Models;
using AskalePortal.Data.SAP.InputParams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SapNwRfc;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ISGAksiyonTakipTable : BaseBLL<AskalePortal.Data.Models.ISGAksiyonTakipTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;

            public ISGAksiyonTakipTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
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
        }
    }
}