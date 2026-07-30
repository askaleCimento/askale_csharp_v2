using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class Companies : BaseBLL<AskalePortal.Data.Models.Company>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public Companies(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;   
            }
            #region GetAll

            public List<AskalePortal.Data.Models.Company> GetAll(string VKORG, string VTEXT)
            {
                var q = dal.Get(k => (k.vkorg.Contains(VKORG) || string.IsNullOrEmpty(VKORG)) &&
                                     (k.vtext.Contains(VTEXT) || string.IsNullOrEmpty(VTEXT)) &&
                                     k.enabled == true)
                                     .OrderBy(k=>k.vkorg);
                return q.ToList();
            }
			override
			public List<AskalePortal.Data.Models.Company> GetAll()
			{
				var q = dal.Get(k => k.enabled == true)
									 .OrderBy(k => k.vkorg);
				return q.ToList();
			}
			#endregion GetAll

			public Data.Models.Company? GetCompany(string VKORG)
			{
				return dal.Get(u => u.vkorg == VKORG).FirstOrDefault();
			}
            public List<AskalePortal.Data.Models.Company> GetFabrikaAndHazirBeton()
            {
                List<AskalePortal.Data.Models.Company> q = new List<AskalePortal.Data.Models.Company>();
                foreach (var item in GetAll())
                {
                    Data.Models.Company company = new Data.Models.Company();
                    company.Id = item.Id;
                    company.vtext = item.companySection == "Fabrika" ? item.vtext:"";

                    if(item.companySection == "Fabrika") q.Add(company);
                }
                Data.Models.Company company2 = new Data.Models.Company();
                company2.vtext = "Hazır Betonlar";
                company2.Id = 500;
                q.Add(company2);
                return q.ToList();
            }

            public int? GetCompanyIdByName(string sirket)
            {
                return dal.Get(u => u.vkorg == sirket ).FirstOrDefault()?.Id;
            }

            public Company getByVkorgCompany(string? bukrs)
            {
                return dal.Get(u => u.vkorg == bukrs).First();
            }

            public List<IdandText> GetIdandText()
            {
                return dal.Get(u => u.enabled).Select(u => new IdandText() { id = u.Id, text = u.vtext }).ToList();
            }
            public Company getById(int companyId)
            {
                return dal.Get(u => u.Id == companyId).First();
            }

            public List<Company> getByRoleId(int roleId)
            {
                List<Company> listCompany = new List<Company>();
                BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration,_env, _mapper);
                Role? role = bllRoles.GetByID(roleId);
                string companies = role!=null ? role.companies.Replace("[","").Replace("]", "") : "";
                
                foreach (var item in companies.Split(",").ToList())
                {
                    Company company = getByVkorgCompany(item);
                    if (company == null)
                    {

                    }
                    else
                    {
                        listCompany.Add(getByVkorgCompany(item));
                    }
                }
                return listCompany;

            }

            public string getByUserId(int userId)
            {
                return
         (from c in dal.dB.Company
          join a in dal.dB.AdminUser 
              on c.Id equals a.companyId
          where c.enabled
                && a.Id == userId
          select c.vtext)
         .FirstOrDefault() ??"";
            }

            public List<CompanySaveDto> getAllFilter(Data.RequestParams.FilterParam<CompanyFilterDto> filterParam)
            {
                return dal.Get(u => u.enabled).Select(u=> new CompanySaveDto
                {
                    enabled=u.enabled,
                    companyLongName=u.companyLongName,
                    companySectionId=u.companySectionId,
                    companyShortName=u.companyShortName,
                    companyTitle=u.companyTitle,
                    createdDate=u.createdDate,
                    createdUserId=u.createdUserId,
                    id=u.Id,
                    imgUrl=u.imgUrl,
                    mandt=u.mandt,
                    spras=u.spras,
                    updateDate=u.updatedDate,
                    updatedUserId=u.updatedUserId,
                    vkorg=u.vkorg,
                    vtext=u.vtext
                }).ToList();
            }


            public List<CompanySaveDto>? GetAllFromSAP()
            {
                List<CompanySaveDto> lstData = new();

                try
                {
                    BLLActions.SAPConnectionData bllSapConnection =
                        new BLLActions.SAPConnectionData(
                            _configuration,
                            _env);

                    using SapConnection? sapConnection =
                        bllSapConnection.sapConnection(
                            _configuration,
                            _env);

                    if (sapConnection == null)
                    {
                        return lstData;
                    }

                    using ISapFunction sapFunction =
                        sapConnection.CreateFunction("ZWEBI006");

                    CompanyOutput result =
                        sapFunction.Invoke<CompanyOutput>();

                    lstData = result.listCompanySap?
                        .Select(x => new CompanySaveDto
                        {
                            mandt = x.mandt,
                            spras = x.spras,
                            vkorg = x.vkorg,
                            vtext = x.vtext
                        })
                        .ToList()
                        ?? new List<CompanySaveDto>();
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }

                return lstData;
            }
        }
    }
}