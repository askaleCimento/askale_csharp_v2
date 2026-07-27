
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class MusteriSikayetEmail : BaseBLL<AskalePortal.Data.Models.MusteriSikayetEmail>
        {
            public MusteriSikayetEmail(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll
            public List<AskalePortal.Data.Models.MusteriSikayetEmail> GetAllEmail()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.Id);
                return q.ToList();
            }

            public List<AskalePortal.Data.Models.MusteriSikayetEmail> GetAll(int? Id, int? userId, int? CategoryId, int? createdUserId, DateTime? createdDate, bool seeLog, int pageNumber, int pageSize)
            {
                var q = dal.Get(k => (k.Id == Id) || (Id == null) || (Id == 0)
                && (k.userId == userId || userId == null || userId == 0)
                && (k.categoryId == CategoryId || CategoryId == null || CategoryId == 0)
                && k.enabled == true).OrderByDescending(k => k.createdDate);

                if (seeLog != true) q = q.Where(u => u.createdUserId == createdUserId).OrderByDescending(k => k.createdDate);
                return q.Skip(pageNumber * pageSize).Take(pageSize).ToList();
            }
            #endregion

            public List<int> findUserIdByCategoryIdAndEnabled(int? categoryId, bool enabled)
            {
                List<int> liste = [];
                if (categoryId != null)
                {
                    liste = dal.Get(u => u.enabled == enabled && u.categoryId == categoryId).Select(u => u.userId).ToList();
                }

                return liste;
            }

        }
    }
}
