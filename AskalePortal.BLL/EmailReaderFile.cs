using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class EmailReaderFile
        {

            public string BuildEmailTemplate(IConfiguration _configuration, IWebHostEnvironment env, string title, string description)
            {
                
                string? filePath = Path.Combine(env.IsDevelopment() ? _configuration["FilePath:local"]! : env.IsProduction() ? _configuration["FilePath:server"]! :
                    _configuration["FilePath:test"]!, "templates\\Email\\email.html");
                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();
                content = content.Replace("{footer}", "Copyright &copy; 2016 Aşkale Çimento");
                content = content.Replace("{description}", description);
                content = content.Replace("{title}", title);

                return content;

            }

            public string CreditEmailTemplate(IConfiguration _configuration, IWebHostEnvironment env, string title, string firstUser, string kunnr, string name1,
                string klimk, string amount, string description, string ok_link, string no_link)
            {

                string? filePath = Path.Combine(env.IsDevelopment() ? _configuration["FilePath:local"]! : env.IsProduction() ? _configuration["FilePath:server"]! :
                    _configuration["FilePath:test"]!, "templates\\Email\\emailCredit.html");
                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();

                content.Replace("title", title);
                content.Replace("firstUser", firstUser);
                content.Replace("kunnr", kunnr);
                content.Replace("name1", name1);
                content.Replace("klimk", klimk);
                content.Replace("amount", amount);
                content.Replace("description", description);
                content.Replace("ok_link", ok_link);
                content.Replace("no_link", no_link);


                content = content.Replace("{footer}", "Copyright &copy; 2016 Aşkale Çimento");

                return content;

            }


            public string VadeEmailTemplate(IConfiguration _configuration, IWebHostEnvironment env,
               string title, string firstUser, string kunnr, string name1, string belnr, string zfbdt,
            string newValue, string description, string ok_link, string no_link)
            {

                string? filePath = Path.Combine(env.IsDevelopment() ? _configuration["FilePath:local"]! : env.IsProduction() ? _configuration["FilePath:server"]! :
                    _configuration["FilePath:test"]!, "templates\\Email\\emailVade.html");
                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();

              
                content.Replace("title", title);
                content.Replace("firstUser", firstUser);
                content.Replace("kunnr", kunnr);
                content.Replace("name1", name1);
                content.Replace("belnr", belnr);
                content.Replace("zfbdt", zfbdt);
                content.Replace("newValue", newValue);
                content.Replace("description", description);
                content.Replace("ok_link", ok_link);
                content.Replace("no_link", no_link);


                content = content.Replace("{footer}", "Copyright &copy; 2016 Aşkale Çimento");

                return content;

            }

            public string CreateSozlesmeMailString(IConfiguration _configuration, IWebHostEnvironment env, string title, int Id, string VTEXT, string SozlesmeCinsi, string FirmaAdi, string SozlesmeKonusu, string Aciklama, string SozlesmeTutari, string BitisTarihi)
            {
                string? filePath = Path.Combine(env.IsDevelopment() ? _configuration["FilePath:local"]! : env.IsProduction() ? _configuration["FilePath:server"]! :
                _configuration["FilePath:test"]!, "templates\\Email\\emailSozlesme.html");
                //filePath = "C:\\Users\\dilek.sariyerlioglu\\Source\\Repos\\askaleportalccore\\AskalePortal.BLL\\templates\\Email\\emailSozlesme.html";

                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();
                content = content.Replace("{title}", title);
                content = content.Replace("{Id}", Id.ToString());
                content = content.Replace("{VTEXT}", VTEXT);
                content = content.Replace("{SozlesmeCinsi}", SozlesmeCinsi);
                content = content.Replace("{FirmaAdi}", FirmaAdi);
                content = content.Replace("{SozlesmeKonusu}", SozlesmeKonusu);
                content = content.Replace("{Aciklama}", Aciklama);
                content = content.Replace("{SozlesmeTutari}", SozlesmeTutari);
                content = content.Replace("{BitisTarihi}", BitisTarihi);
                content = content.Replace("{footer}", "Copyright &copy; 2016 Aşkale Çimento");
                return content;
            }


            public string CreateIsTakipMailString(IConfiguration _configuration, IWebHostEnvironment env,IMapper mapper, string title, Data.Models.SureliIsTakipTable entity)
            {
                string? filePath = Path.Combine(env.IsDevelopment() ? _configuration["FilePath:local"]! : env.IsProduction() ? _configuration["FilePath:server"]! :
                _configuration["FilePath:test"]!, "templetes\\Email\\emailIsTakip.html");
                filePath = "C:\\Users\\dilek.sariyerlioglu\\Source\\Repos\\askaleportalccore\\AskalePortal.BLL\\templates\\Email\\emailIsTakip.html";

                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();
                content = content.Replace("{title}", title);
                content = content.Replace("{Id}", entity.ToString());
                content = content.Replace("{IsinTanimi}", entity.isinTanimi.ToString());
                content = content.Replace("{BaslamaTarihi}", entity.baslamaTarihi.ToShortDateString());
                string[] idlers = entity.takipSorumlusu.Split(',');
                string InternalAuditor = "";
                BLLActions.AdminUsers bllAdminusers = new BLLActions.AdminUsers(_configuration,env, mapper);
                foreach (var item in idlers)
                {

                    int id = Convert.ToInt32(item);
                    InternalAuditor += bllAdminusers.GetByID(id)?.name + ",";
                }
                InternalAuditor = InternalAuditor.Remove(InternalAuditor.Length - 1);

                content = content.Replace("{TakipSorumlusu}", InternalAuditor);
                content = content.Replace("{SurekliMi}", entity.terminTarihi.ToShortDateString());
                string[] idler = entity.muhattaplar.Split(',');
                string Muhattablar = "";

                foreach (var item in idler)
                {

                    int id = Convert.ToInt32(item);
                    Muhattablar += bllAdminusers.GetByID(id)?.name + ",";
                }
                Muhattablar = Muhattablar.Remove(Muhattablar.Length - 1);
                content = content.Replace("{Muhattaplar}", Muhattablar);
                content = content.Replace("{Tamamlandı}", entity.tamamlandi == true ? "Tamamlandı" : "Tamamlanmadı");
                content = content.Replace("{Aciklama}", entity.aciklama);
                content = content.Replace("{footer}", "Copyright &copy; 2016 Aşkale Çimento");
                return content;
            }

            public string getMusteriEmailText(IConfiguration _configuration, IWebHostEnvironment env,Data.Models.MusteriSikayetForm entity, string companyName, string sikayetTipi,
        string categoryName,    string createdUserName, List<AttachedFile> attachedFiles)
            {
                string? filePath = Path.Combine(env.IsDevelopment() ? _configuration["FilePath:local"]! : env.IsProduction() ? _configuration["FilePath:server"]! :
                _configuration["FilePath:test"]!, "templates\\Email\\emailMusteriSikayet.html");
                //filePath = "C:\\Users\\dilek.sariyerlioglu\\Source\\Repos\\askaleportalccore\\AskalePortal.BLL\\templates\\Email\\emailSozlesme.html";

                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();
                content = content.Replace("{Id}", entity.Id.ToString());
                content = content.Replace("{FabrikaAdi}", companyName.ToString());
                content = content.Replace("{CategoryName}", categoryName);
                content = content.Replace("{MusteriKodu}", entity.musteriKodu);
                content = content.Replace("{MusteriAdi}", entity.musteriAdi);
                content = content.Replace("{SikayetTipi}", sikayetTipi);
                content = content.Replace("{MalzemeTuru}", entity.malzemeTuru);
                content = content.Replace("{MalzemeMiktari}", entity.malzemeMiktari.ToString());
                content = content.Replace("{MusteriTemsilcisi}", entity.musteriTemsilcisi.ToString());
                content = content.Replace("{MusteriTel}", entity.musteriTel.ToString());
                content = content.Replace("{MusteriEmail}", entity.musteriEmail.ToString());
                content = content.Replace("{name}",createdUserName);
                content = content.Replace("{createdDate}", entity.createdDate.ToString("dd.MM.yyyy"));
                content = content.Replace("{description}", entity.description.ToString());
                content = content.Replace("{attachedFiles}", attachedFiles.ToString());
                content = content.Replace("{footer}", "Copyright &copy; 2016 Aşkale Çimento");
                content = content.Replace("{okLink}",CommonConstants.OkNoLinks.OK_LINK);
                content = content.Replace("{noLink}", CommonConstants.OkNoLinks.NO_LINK);
                return content;

            }
        }
    }
}