using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ISGAksiyonTable : BaseBLL<AskalePortal.Data.Models.ISGAksiyonTable>
        {

            public ISGAksiyonTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.ISGAksiyonTable> GetByCompanyID(int userId, int activePage, int pageSize)
            {
                return dal.Get(u => (u.bidirimdeBulunan == userId) && u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.ISGAksiyonTable> GetAllWithPages(int activePage, int pageSize)
            {
                return dal.Get(u => u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.ISGAksiyonTable> GetByUser(int userId, int activePage, int pageSize, int? id)
            {
                string userID = userId.ToString();
                return dal.Get(u => (u.bidirimdeBulunan == userId || u.ISGAksiyonTakipTable.Any(y => y.aksiyonSorumlulari.Contains(userID))) && u.enabled == true && (id.HasValue ? u.Id == id.Value : true)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.ISGAksiyonTable> GetAllWithPages(int[] companies, int activePage, int pageSize)
            {
                return dal.Get(u => u.enabled == true && companies.Contains(u.companyId)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
            }
            public List<AskalePortal.Data.Models.ISGAksiyonTable> GetAllWithPages(int[] companies)
            {
                return dal.Get(u => u.enabled == true && companies.Contains(u.companyId)).OrderByDescending(u => u.Id).ToList();
            }

            public List<AskalePortal.Data.Models.ISGAksiyonTable> GetAllWithPages(int[] companies, int activePage, int pageSize, int? id)
            {
                return dal.Get(u => u.enabled == true && companies.Contains(u.companyId) && (id.HasValue ? u.Id == id.Value : true)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public int approvalCount(int userId)
            {
                var sorumluKisiAcanKisi = (from c in dal.dB.ISGAksiyonTable

                                           join b in dal.dB.ISGAksiyonTakipTable
                                               on c.Id equals b.aksiyonId into takipJoin
                                           from b in takipJoin.DefaultIfEmpty()

                                           join a in dal.dB.ISGUser
                                               on c.companyId equals a.companyId into userJoin
                                           from a in userJoin.DefaultIfEmpty()

                                           where
                                               (c.bittiMi == false) &&
                                               c.enabled &&
                                               (
                                                   (b != null && b.aksiyonSorumlulari.Contains("[" + userId + "]")) ||
                                                   c.bidirimdeBulunan == userId ||
                                                   (a != null && a.userId == userId)
                                               )

                                           select c
    )
    .Distinct()
    .ToList();
                return sorumluKisiAcanKisi.Count();
            }
        }
    }
}