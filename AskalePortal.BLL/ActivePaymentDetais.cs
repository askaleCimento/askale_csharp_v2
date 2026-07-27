using AskalePortal.Data.Models;
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
        public class ActivePaymentDetails : BaseBLL<Data.Models.ActivePaymentDetail>
        {
            public ActivePaymentDetails(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public List<Data.Models.ActivePaymentDetail> GetByAccountPaymentId(int paymentId)
            {
                return dal.Get(u => u.enabled == true && u.activePaymentId == paymentId).ToList();
            }

            public Data.Models.ActivePaymentDetail? GetByAccountPaymentId(int id, int userId)
            {
                return dal.Get(u => u.userId == userId && u.activePaymentId == id && u.enabled == true).OrderByDescending(u => u.createdDate).FirstOrDefault();
            }

            public ActivePaymentDetail findAllByActivePaymentIdAndApprovedAndUserId(int id, bool? approved,int userId)
            {

                return dal.Get(u => u.activePaymentId == id && u.approved == approved && u.userId == userId).First();
            }
        }
    }

}
