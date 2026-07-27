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
        public class DahiliYazismaMessage : BaseBLL<AskalePortal.Data.Models.DahiliYazismaMessage>
        {
            public DahiliYazismaMessage(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public bool GetByDahiliIdAndUserId(int dahiliYazismaId, int userId)
            {
               return dal.Get(u => u.enabled == true && u.userId == userId && u.dahiliYazismaId == dahiliYazismaId).Any();
            }
            public List<AskalePortal.Data.Models.DahiliYazismaMessage> GetAllById(int dahiliYazismaId,bool showAll)
            {
                return dal.Get(u => u.enabled == true  && u.dahiliYazismaId == dahiliYazismaId && u.showAll==showAll).OrderBy(u => u.createdDate).ToList();
            }
            public List<AskalePortal.Data.Models.DahiliYazismaMessage> GetAllById(int dahiliYazismaId)
            {
                return dal.Get(u => u.enabled == true && u.dahiliYazismaId == dahiliYazismaId).OrderBy(u=>u.createdDate).ToList();
            }

          
        }
    }
}
