using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class HRExpenseAmount : BaseBLL<AskalePortal.Data.Models.HRExpenseAmount>
        {
            public HRExpenseAmount(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public int GetByName(int calisanTuruId, int harcamaTuruId)
            {
                return dal.Get(u => u.calisanTuruId == calisanTuruId && u.harcamaTuruId == harcamaTuruId && u.enabled == true).Count();
            }
            public int GetByNameClass(AskalePortal.Data.Models.HRExpenseAmount entity)
            {
                return dal.Get(u => u.calisanTuruId == entity.calisanTuruId && u.harcamaTuruId == entity.harcamaTuruId && u.Id!=entity.Id && u.enabled == true).Count();
            }

            public List<AskalePortal.Data.Models.HRExpenseAmount> GetByCalAndHarca(int calisanTuruId, int harcamaTuruId, int activePage, int recordsPerPage)
            {
                return dal.Get(u => (calisanTuruId == 0 ? true : u.calisanTuruId == calisanTuruId) && (harcamaTuruId == 0 ? true : u.harcamaTuruId == harcamaTuruId) && u.enabled == true).OrderBy(u=>u.calisanTuru.calisanTuru).Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
            }

            public AskalePortal.Data.Models.HRExpenseAmount GetAmount(Func<object, object> p)
            {
                throw new NotImplementedException();
            }

            public AskalePortal.Data.Models.HRExpenseAmount GetAmount(int calisanId, int harcamaId)
            {
                return dal.Get(u => u.calisanTuruId == calisanId && u.harcamaTuruId == harcamaId && u.enabled == true).FirstOrDefault() ?? new AskalePortal.Data.Models.HRExpenseAmount();
            }

            public AskalePortal.Data.Models.HRExpenseAmount GetByCalIdAndHarca(int calisanTuruId, int harcamaTuruId)
            {
                return dal.Get(u => (calisanTuruId == 0 ? true : u.calisanTuruId == calisanTuruId) && (harcamaTuruId == 0 ? true : u.harcamaTuruId == harcamaTuruId) && u.enabled == true).OrderBy(u => u.calisanTuru.calisanTuru).FirstOrDefault() ?? new AskalePortal.Data.Models.HRExpenseAmount();
            }

            public PageReturn<HRExpenseAmountDto>? listExpenseAmount(FilterPageParam<HRExpenseAmountRequestDto> filterPageParam)
            {

                PageReturn<HRExpenseAmountDto>? result = new PageReturn<HRExpenseAmountDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? filterCalisanTuru = filterPageParam.liste?.filterCalisanTuru;
                int? filterHarcamaTuru = filterPageParam.liste?.filterHarcamaTuru;

                IQueryable<Data.Models.HRExpenseAmount> query = dal.Get(u => u.enabled && u.calisanTuru.enabled && u.harcamaTuru.enabled

                && ((filterCalisanTuru == null || filterCalisanTuru == 0) ? true : u.calisanTuruId == filterCalisanTuru)
               && ((filterHarcamaTuru == null || filterHarcamaTuru == 0) ? true : u.harcamaTuruId == filterHarcamaTuru)
                ).OrderByDescending(u=>u.Id);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new HRExpenseAmountDto()
                    {
                      id=u.Id,
                      calisanTuru=u.calisanTuru.calisanTuru,
                      harcamaTuru=u.harcamaTuru.expenseTypeName,
                      enabled = u.enabled,
                      harcirahMiktari = u.harcirahMiktari,

                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            //public Data.Models.HRExpenseAmount getByCalisanTuruIdAndHarcamaTuruId(int calisanTuruId, int harcamaTuruId)
            //{
            //    Data.Models.HRExpenseAmount amount = dal.Get(u => u.enabled && u.calisanTuruId == calisanTuruId && u.harcamaTuruId == harcamaTuruId).OrderByDescending(u => u.gecerlilikTarihi).First();
            //    return amount;
            //}

            public Data.Models.HRExpenseAmount getbycalisanturuidandharcamaturuid(int? calisanTuruId, int? expenseTypeId, string? spendingTime)
            {
                Data.Models.HRExpenseAmount hRExpenseAmount;
                if (spendingTime != null)
                {
                    DateTime spendingDate = DateTime.Parse(spendingTime);

                    hRExpenseAmount = dal.Get(u =>
                         u.gecerlilikTarihi <= spendingDate &&
                         u.calisanTuruId == calisanTuruId &&
                         u.harcamaTuruId == expenseTypeId
                     ).OrderByDescending(u => u.gecerlilikTarihi).First();
                }
                else
                {

                    hRExpenseAmount = dal.Get(u =>
                        
                         u.calisanTuruId == calisanTuruId &&
                         u.harcamaTuruId == expenseTypeId
                     ).OrderByDescending(u => u.gecerlilikTarihi).First();
                }
               
                return hRExpenseAmount;
            }
        }
    }
}
