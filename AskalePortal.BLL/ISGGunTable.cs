using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public class ISGGunTable : BaseBLL<AskalePortal.Data.Models.ISGGunTable>
        {
            private readonly IMapper _mapper;
            public ISGGunTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _mapper = mapper;
            }
            public List<AskalePortal.Data.Models.ISGGunTable> GetAll(int companyId = 0, DateTime? timeofoccurence = null)
            {
                return dal.Get(u => (u.enabled == true) && (companyId == 0 ? true : u.companyId == companyId) && (timeofoccurence == null ? true : u.timeofoccurence == timeofoccurence)).ToList();
            }

            public List<AskalePortal.Data.Models.ISGGunTable> Fabrikalar()
            {
                List<AskalePortal.Data.Models.ISGGunTable> liste = new List<AskalePortal.Data.Models.ISGGunTable>();
                var listem = dal.Get(u => (u.enabled == true) && u.company.companySection == "Fabrika").GroupBy(u => u.companyId).Select(u => new
                {
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

            public List<AskalePortal.Data.Models.ISGGunTable> Santraller()
            {

                List<AskalePortal.Data.Models.ISGGunTable> liste = new List<AskalePortal.Data.Models.ISGGunTable>();
                var listem = dal.Get(u => (u.enabled == true) && u.company.companySection == "Hazır Beton").GroupBy(u => u.companyId).Select(u => new
                {
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


                    value = DateTime.Now.Subtract(u.timeofoccurence).Days,
                    text = u.company.companyShortName,
                    sectionId = u.company.companySectionId,
                }).ToList();
                return liste;
            }
            public List<ISGGunTableSaveDto> getAllFilter(
                FilterParam<IsgGunTableListParameterDto> filterParam)
            {
                int? companyId = filterParam.liste?.filterCompanyId;

                List<Data.Models.ISGGunTable> records = dal
                    .Get(u =>
                        u.enabled &&
                        (!companyId.HasValue ||
                         companyId.Value == 0 ||
                         u.companyId == companyId.Value))
                    .OrderByDescending(u => u.Id)
                    .ToList();

                return records
                    .Select(u => new ISGGunTableSaveDto
                    {
                        id = u.Id,
                        companyId = u.companyId,
                        timeofoccurence = u.timeofoccurence.ToString("dd.MM.yyyy"),
                        enabled = u.enabled,
                        createdDate = u.createdDate,
                        createdUserId = u.createdUserId,
                        updateDate = u.updatedDate,
                        updatedUserId = u.updatedUserId,
                    })
                    .ToList();
            }
            public async Task<ActionResult<object>> save(
                ISGGunTableSaveDto entity,
                int userId)
            {
            
                if (entity.companyId == null)
                {
                    return new BadRequestObjectResult(
                        "Firma bilgisi boş olamaz.");
                }

                if (string.IsNullOrWhiteSpace(entity.timeofoccurence))
                {
                    return new BadRequestObjectResult(
                        "Tarih bilgisi boş olamaz.");
                }

                if (!DateTime.TryParse(
                        entity.timeofoccurence,
                        out DateTime timeOfOccurrence))
                {
                    return new BadRequestObjectResult(
                        "Tarih formatı geçersiz.");
                }


                if (entity.id.HasValue)
                {
                    Data.Models.ISGGunTable? existingRecord =
                        dal.Get(x =>
                                x.Id == entity.id.Value)
                            .FirstOrDefault();

                    if (existingRecord == null)
                    {
                        return new NotFoundObjectResult(
                            $"Kayıt bulunamadı. Id: {entity.id.Value}");
                    }

                    // Gelen değerlerle mevcut kaydı güncelle.
                    existingRecord.companyId =
                        entity.companyId.Value;

                    existingRecord.timeofoccurence =
                        timeOfOccurrence;

                    existingRecord.updatedUserId =
                        userId;

                    existingRecord.updatedDate =
                        DateTime.Now;

                    existingRecord.enabled =
                        true;

                    return await Update(existingRecord);
                }


                Data.Models.ISGGunTable? existingCompanyRecord =
                    dal.Get(x =>
                            x.enabled &&
                            x.companyId == entity.companyId.Value)
                        .FirstOrDefault();


                if (existingCompanyRecord != null)
                {
                    existingCompanyRecord.timeofoccurence =
                        timeOfOccurrence;

                    existingCompanyRecord.updatedUserId =
                        userId;

                    existingCompanyRecord.updatedDate =
                        DateTime.Now;

                    existingCompanyRecord.enabled =
                        true;

                    return await Update(existingCompanyRecord);
                }

                Data.Models.ISGGunTable newRecord =
                    new Data.Models.ISGGunTable
                    {
                        companyId =
                            entity.companyId.Value,

                        timeofoccurence =
                            timeOfOccurrence,

                        createdUserId =
                            userId,

                        createdDate =
                            DateTime.Now,

                        enabled =
                            true
                    };

                return await Add(newRecord);
            }
        }
    }
}
