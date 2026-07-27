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

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class EArsivFatura : BaseBLL<AskalePortal.Data.Models.EArsivFatura>
        {
            private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;

            public EArsivFatura(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public async Task<int> finished(string ettn)
            {
                try
                {

                    Data.Models.EArsivFatura eArsivFatura = dal.Get(u => u.ettn == ettn).First();
                    eArsivFatura.bittiMi = true;
                    await Update(eArsivFatura);
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }

            }

            public List<EArsivFaturaResponseDto> listMyIncoices(int userId, bool enabled, bool bittiMi)
            {
                BLLActions.EArsivFaturaYetkiler bllEArsivFaturaYetkiler = new BLLActions.EArsivFaturaYetkiler(_configuration, _env, _mapper);
                List<int> listCompanyId = bllEArsivFaturaYetkiler.findCompanyIdByUserIdAndEnabled(userId, true);
                

                if (listCompanyId.Count==0)
                {
                    return [];
                }
                else
                {
                    List<EArsivFaturaResponseDto> result =
 (from a in dal.Get(x => x.enabled == enabled && x.bittiMi == bittiMi)
      join b in dal.dB.AdminUser on a.userId equals b.Id into users
  from b in users.DefaultIfEmpty()
  join c in dal.dB.Company on a.companyId equals c.Id into companies
  from c in companies.DefaultIfEmpty()
  where (a.companyId == null && a.userId == null)
        ||  listCompanyId.Contains(a.companyId ?? 0)
  orderby a.belgeTarihi
  select new EArsivFaturaResponseDto
  {
      ettn = a.ettn,
      belgeNumarasi = a.belgeNumarasi,
      saticiVknTckn = a.saticiVknTckn,
      saticiUnvanAdSoyad = a.saticiUnvanAdSoyad,
      belgeTarihi = (a.belgeTarihi ?? DateTime.Now).ToString("dd.MM.yyyy"),
      belgeTuru = a.belgeTuru,
      onayDurumu = a.onayDurumu,
      companyId = a.companyId,
      companyName = c.vtext??"",
      username = b.name ??"",
      bittiMi = a.bittiMi
  }).ToList();

                    return result;
                }
             
            }

            public async Task<EArsivFaturaSaveDto> save(EArsivFaturaSaveDto entity)
            {
                if (entity.enabled == null)
                {
                    entity.bittiMi=false;
                    entity.enabled=true;
                }
                entity.pullTime =DateTime.Now.ToString();
                Data.Models.EArsivFatura? saved = await Add(_mapper.Map<Data.Models.EArsivFatura>(entity));
                return _mapper.Map<EArsivFaturaSaveDto>(saved);
            }
        }
    }
}
