using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseParams;
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
        public class AnnualCalendarTable : BaseBLL<AskalePortal.Data.Models.AnnualCalenderTable>
        {
            public AnnualCalendarTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public List<AnnualCalenderTable> GetAllByPage(int activePage,int pageSize)
            {
              return dal.Get(u=>u.enabled==true).OrderBy(u=>u.baslangic).Skip(activePage*pageSize).Take(pageSize).ToList();
            }

            public AnnualCalenderTable? getDay(DateTime date)
            {
                return dal.Get(u => u.baslangic == date.Date && u.enabled == true).FirstOrDefault();
            }

            public bool GetByTarih(DateTime baslangic)
            {
                return dal.Get(u => u.baslangic == baslangic && u.enabled == true).Any();
            }

            public PageReturn<AnnualCalenderTable> filterPageable(FilterPageParam<AnnualCalenderDtoRequest> filterPageParam)
            {
                PageReturn<AnnualCalenderTable>? result = new PageReturn<AnnualCalenderTable>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                DateTime? baslangic = filterPageParam.liste?.baslangic;

                IQueryable<AnnualCalenderTable> query = dal.Get(u =>
                    u.enabled &&
                    (baslangic == null ? true : u.baslangic == baslangic)
                );

                result.content = query
                    .OrderByDescending(u => u.baslangic)
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .ToList();

                int count = query.Count();

                result.totalPages = (int)Math.Ceiling((double)count / pageSize);
                result.totalElements = count;
                result.size = pageSize;

                return result;
            }
        }
    }
}
