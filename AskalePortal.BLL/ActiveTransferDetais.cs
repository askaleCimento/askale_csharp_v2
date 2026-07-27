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
        public class ActiveTransferDetails : BaseBLL<AskalePortal.Data.Models.ActiveTransferDetail>
        {
            public ActiveTransferDetails(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public List<ActiveTransferDetail> GetByAccountTransferId(int transferId)
            {
                return dal.Get(u => u.enabled == true && u.activeTransferId == transferId).ToList();
            }

            public ActiveTransferDetail? GetByAccountTransferId(int id, int userId)
            {
                return dal.Get(u => u.userId == userId && u.activeTransferId == id && u.enabled == true).OrderByDescending(u => u.createdDate).FirstOrDefault();
            }

            internal ActiveTransferDetail? findAllByActiveTransferIdAndApprovedAndUserIdAndEnabled(int id, bool? approved,
            int userId, bool enabled)
            {
                return dal.Get(u => u.activeTransferId == id && u.approved == approved && u.userId == userId && u.enabled == enabled).FirstOrDefault();
            }
        }
    }

}
