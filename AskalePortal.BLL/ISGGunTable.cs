using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ISGGunTable : BaseBLL<AskalePortal.Data.Models.ISGGunTable>
        {
            public ISGGunTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.ISGGunTable> GetAll(int companyId = 0, DateTime? timeofoccurence = null)
            {
                return dal.Get(u => (u.enabled == true) && (companyId == 0 ? true : u.companyId == companyId) && (timeofoccurence == null ? true : u.timeofoccurence == timeofoccurence)).ToList();
            }

            public List<AskalePortal.Data.Models.ISGGunTable> Fabrikalar()
            {
                List<AskalePortal.Data.Models.ISGGunTable> liste = new List<AskalePortal.Data.Models.ISGGunTable>();
                var listem= dal.Get(u => (u.enabled == true) && u.company.companySection == "Fabrika").GroupBy(u => u.companyId).Select(u => new  {
                    Id=u.FirstOrDefault().Id,
                    status=true,
                    timeofoccurence=u.Max(y=>y.timeofoccurence),
                    companyId=u.Key,
                    createdTime=u.FirstOrDefault().createdDate,
                    createdUserId=u.FirstOrDefault().createdUserId,
                    Company=u.FirstOrDefault().company,
                    AdminUser=u.FirstOrDefault().createdUser
                }).OrderBy(u => u.Company.vkorg).ToList();

                foreach (var item in listem)
                {
                    AskalePortal.Data.Models.ISGGunTable iSGGunTable = new AskalePortal.Data.Models.ISGGunTable()
                    {
                        Id=item.Id,
                        createdUser=item.AdminUser,
                        companyId=item.companyId,
                        company=item.Company,
                        createdDate=item.createdTime,
                        createdUserId=item.createdUserId,
                        enabled=item.status,
                        timeofoccurence=item.timeofoccurence
                    };
                    liste.Add(iSGGunTable);
                }
                return liste;
            }
           
            public List<AskalePortal.Data.Models.ISGGunTable> Santraller()
            {
               
                List<AskalePortal.Data.Models.ISGGunTable> liste = new List<AskalePortal.Data.Models.ISGGunTable>();
                var listem = dal.Get(u => (u.enabled == true) && u.company.companySection == "Hazır Beton").GroupBy(u => u.companyId).Select(u => new {
                    Id = u.FirstOrDefault().Id,
                    status = true,
                    timeofoccurence = u.Max(y => y.timeofoccurence),
                    companyId = u.Key,
                    createdTime = u.FirstOrDefault().createdDate,
                    createdUserId = u.FirstOrDefault().createdUserId,
                    Company = u.FirstOrDefault().company,
                    AdminUser = u.FirstOrDefault().createdUser
                }).OrderBy(u => u.Company.vkorg).ToList();

                foreach (var item in listem)
                {
                    AskalePortal.Data.Models.ISGGunTable iSGGunTable = new AskalePortal.Data.Models.ISGGunTable()
                    {
                        Id = item.Id,
                        createdUser = item.AdminUser,
                        companyId = item.companyId,
                        company = item.Company,
                        createdDate = item.createdTime,
                        createdUserId = item.createdUserId,
                        enabled = item.status,
                        timeofoccurence = item.timeofoccurence
                    };
                    liste.Add(iSGGunTable);
                }
                return liste;
            }

            public List<ISGGunTableGraphDto>? NumberOfAccidentFreeDays()
            {
                List<ISGGunTableGraphDto>? liste = dal.Get(k => k.enabled).OrderByDescending(u => u.Id).Select(u => new ISGGunTableGraphDto
                {
                    

                     value=DateTime.Now.Subtract(u.timeofoccurence).Days,
                     text=u.company.companyShortName,
                     sectionId=u.company.companySectionId,
                }).ToList();
                return liste;
            }
        }
    }
}
