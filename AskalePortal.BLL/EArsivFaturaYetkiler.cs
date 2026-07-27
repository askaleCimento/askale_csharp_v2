using AskalePortal.Data.ResponseModels;
using AutoMapper;
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
        public class EArsivFaturaYetkiler : BaseBLL<AskalePortal.Data.Models.EArsivFaturaYetkiler>
        {
            private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;

            public EArsivFaturaYetkiler(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public async Task<int> deletebyuserid(int userId)
            {
                try
                {
                    var kayit = dal.Get(u => u.userId == userId).ToList();
                    foreach (Data.Models.EArsivFaturaYetkiler yetki in kayit)
                    {
                        yetki.enabled = false;
                        await Update(yetki);
                    }
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }
              
            }

            public List<int> findCompanyIdByUserIdAndEnabled(int userId, bool enabled)
            {
                List<int> companyIds = dal.Get(u => u.enabled == enabled
                           && u.userId == userId)
                    .Select(u => u.companyId ??0)
                    .ToList();
                return companyIds ?? [];

            }

            public EArsivFaturaYetkilerResponseDto getByUserId(int userId)
            {
                var query =
     from a in dal.Get(a=>a.enabled == true && a.userId == userId)
     join b in dal.dB.AdminUser on a.userId equals b.Id into ub
     from b in ub.DefaultIfEmpty()
     join c in dal.dB.Company on a.companyId equals c.Id into ac
     from c in ac.DefaultIfEmpty()
     
     select new EArsivFaturaYetkilerDto
     {
         userId = b != null ? b.Id : null,
         userName = b != null ? b.name : null,
         companyName = c != null ? c.vtext : null,
         companyId = c != null ? c.Id : null
     };

                var liste = query.ToList();

                var returnList = new List<EArsivFaturaYetkilerResponseDto>();

                var grouped = liste
                    .GroupBy(w => w.userName);

                foreach (var group in grouped)
                {
                    var dto = new EArsivFaturaYetkilerResponseDto();

                    var companyNames = new HashSet<string>();
                    var companyIds = new HashSet<int>();

                    foreach (var entity in group)
                    {
                        if (!string.IsNullOrEmpty(entity.companyName))
                        {
                            companyNames.Add(entity.companyName);
                        }

                        if (entity.companyId.HasValue)
                        {
                            companyIds.Add(entity.companyId.Value);
                        }
                    }

                    var first = group.FirstOrDefault();

                    if (first != null)
                    {
                        dto.userName = first.userName;
                        dto.userId = first.userId;
                    }

                    if (companyNames.Count > 0)
                    {
                        dto.companyNames = companyNames;
                        dto.companyIds = companyIds;
                    }

                    returnList.Add(dto);
                }

                return returnList.FirstOrDefault();
            }

            public List<EArsivFaturaYetkilerResponseDto> listDtoByEnabled(bool enabled)
            {
                List<EArsivFaturaYetkilerDto> liste = (
    from a in dal.dB.EArsivFaturaYetkiler
    join b in dal.dB.AdminUser on a.userId equals b.Id into users
    from b in users.DefaultIfEmpty()
    join c in dal.dB.Company on a.companyId equals c.Id into companies
    from c in companies.DefaultIfEmpty()
    where a.enabled == enabled
    select new EArsivFaturaYetkilerDto
    {
        userId = b != null ? b.Id : 0,
        userName = b != null ? b.name : null,
        companyName = c != null ? c.vtext : null,
        companyId = c != null ? c.Id : 0
    }
).ToList(); ;
                List<EArsivFaturaYetkilerResponseDto> returnList = new List<EArsivFaturaYetkilerResponseDto> ();
                var studlistGrouped = liste
     .GroupBy(w => w.userName)
     .ToDictionary(g => g.Key, g => g.ToList());
                foreach (var entrySet in studlistGrouped)
                {
                    var key = entrySet.Key;
                    var valueList = entrySet.Value;

                    EArsivFaturaYetkilerResponseDto arsivFaturaYetkilerDto = new EArsivFaturaYetkilerResponseDto();

                    HashSet<string> listCompanyNames = new HashSet<string>();
                    HashSet<int> listCompanyIds = new HashSet<int>();
                    foreach (EArsivFaturaYetkilerDto entity in entrySet.Value)
                    {
                        if (entity.companyName != null)
                        {
                            listCompanyNames.Add(entity.companyName);
                            if (entity.companyId.HasValue)
                            {
                                listCompanyIds.Add(entity.companyId.Value);
                            }
                           
                        }
                    }
                    arsivFaturaYetkilerDto.userName = entrySet.Value[0].userName;
                    arsivFaturaYetkilerDto.userId = entrySet.Value[0].userId;
                    if (listCompanyNames.Count !=0 )
                    {
                        arsivFaturaYetkilerDto.companyNames=listCompanyNames;
                        
                        arsivFaturaYetkilerDto.companyIds=listCompanyIds;
                    }

                    returnList.Add(arsivFaturaYetkilerDto);
                }
                return returnList;
            }

            public async Task<string> saveTotal(int userId, int selectedUserId, List<int> selectedCompanyIds)
            {
                try
                {
                    List<int> unselectedList = selectedCompanyIds;
                    List<Data.Models.EArsivFaturaYetkiler> listData = dal.Get(u => u.userId == selectedUserId).ToList();
                    List<Data.Models.EArsivFaturaYetkiler> listDataContain = listData
     .Where(u => u.companyId.HasValue && selectedCompanyIds.Contains(u.companyId.Value))
     .ToList();
                    List<Data.Models.EArsivFaturaYetkiler> listDataNotContain =
     listData.Where(u => u.companyId.HasValue && !selectedCompanyIds.Contains(u.companyId.Value))
             .ToList();

                    foreach (Data.Models.EArsivFaturaYetkiler eArsivFaturaYetkiler in listDataContain)
                    {
                        if (eArsivFaturaYetkiler.companyId.HasValue)
                        {
                            unselectedList.Remove(eArsivFaturaYetkiler.companyId.Value);
                        }
                    
                        if (!eArsivFaturaYetkiler.enabled)
                        {
                            eArsivFaturaYetkiler.enabled=true;
                            await Update(eArsivFaturaYetkiler);
                        }
                    }

                    foreach (Data.Models.EArsivFaturaYetkiler eArsivFaturaYetkiler in listDataNotContain)
                    {

                        if (eArsivFaturaYetkiler.enabled)
                        {
                            eArsivFaturaYetkiler.enabled=false;
                           await Update(eArsivFaturaYetkiler);
                        }
                    }

                    foreach (int companyId in unselectedList)
                    {
                        Data.Models.EArsivFaturaYetkiler eArsivFaturaYetkiler = new Data.Models.EArsivFaturaYetkiler();
                        eArsivFaturaYetkiler.companyId=companyId;
                        eArsivFaturaYetkiler.userId=selectedUserId;
                        eArsivFaturaYetkiler.enabled=true;
                        eArsivFaturaYetkiler.createdDate=DateTime.Now;
                        eArsivFaturaYetkiler.createdUserId=userId;
                        await Update(eArsivFaturaYetkiler);
                    }

                    return "0";
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
            }
        }
    }
}
