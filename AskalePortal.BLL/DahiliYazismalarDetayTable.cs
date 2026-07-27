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
        public class DahiliYazismalarDetayTable : BaseBLL<AskalePortal.Data.Models.DahiliYazismalarDetayTable>
        {
            public DahiliYazismalarDetayTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public List<AskalePortal.Data.Models.DahiliYazismalarDetayTable> GetAllNotApprovedByUserId(int ID)
            {
                return dal.Get(u => u.approved == null && u.enabled == true && u.userId == ID).ToList();
            }

            public List<AskalePortal.Data.Models.DahiliYazismalarDetayTable> GetAllNotApprovedBySuperUser()
            {
                return dal.Get(u => u.approved == null && u.enabled == true).ToList();
            }


            public List<AskalePortal.Data.Models.DahiliYazismalarDetayTable> GetByDahiliYazismaID(int Id)
            {
                return dal.Get(u => u.enabled == true && u.dahiliYazismaId == Id).ToList();

            }
            public AskalePortal.Data.Models.DahiliYazismalarDetayTable GetUnApprovedByUserIdAndDahiliId(int id, int userId)
            {
                return dal.Get(u => u.userId == userId && u.dahiliYazismaId == id && u.kanalBittiMi == false && u.approved == null && u.enabled == true).FirstOrDefault() ?? new AskalePortal.Data.Models.DahiliYazismalarDetayTable();
            }
            public AskalePortal.Data.Models.DahiliYazismalarDetayTable GetMyDetail(int id, int userId)
            {
                return dal.Get(u => u.userId == userId && u.dahiliYazismaId == id && u.enabled == true).FirstOrDefault() ?? new AskalePortal.Data.Models.DahiliYazismalarDetayTable();
            }

            public AskalePortal.Data.Models.DahiliYazismalarDetayTable GetByIdAndUser(int Id, int userId)
            {
                return dal.Get(u => u.enabled == true && u.userId == userId && u.dahiliYazismaId == Id).OrderByDescending(u => u.createdDate).FirstOrDefault() ?? new AskalePortal.Data.Models.DahiliYazismalarDetayTable();
            }

            public List<AskalePortal.Data.Models.DahiliYazismalarDetayTable> GetAllForSuperUser()
            {
                return dal.Get(u => u.approved == null && u.enabled == true).ToList();
            }

            public int approvalCount(int userId)
            {
                return dal.Get(u => u.approved == null && u.userId == userId && u.dahiliYazisma.onaylandiMi == false && u.dahiliYazisma.bittiMi == false
                && u.dahiliYazisma.enabled == true && u.enabled == true).Count();
            }

            internal List<string> getByLastUserName(int? dahiliYazismaId)
            {
                if (dahiliYazismaId != null)
                {
                    List<string> liste = new List<string>();
                    liste = dal.Get(u => u.dahiliYazismaId == dahiliYazismaId && u.approved == null && u.enabled)
                        .OrderByDescending(u => u.createdDate)
                        .Select(u => u.user.name).ToList();
                    return liste;
                }
                else
                {
                    return [];
                }
            }

            internal List<Data.Models.DahiliYazismalarDetayTable> findAllByEnabledAndDahiliYazismaId(bool enabled, int? id)
            {
                return dal.Get(u => u.enabled == enabled && u.dahiliYazismaId == id).ToList();
            }

            internal List<Data.Models.DahiliYazismalarDetayTable> findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(int? dahiliYazismaId,
            bool? approved, int userId, bool enabled)

            {
                if (userId != 0)
                {
                    return dal.Get(u => u.enabled == enabled && u.dahiliYazismaId == dahiliYazismaId && u.approved == approved && u.userId == userId).ToList();
                }
                else
                {
                    return [];
                }
            }


        }
    }

}
