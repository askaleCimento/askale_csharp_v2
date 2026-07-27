
using AskalePortal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ApprovalProcessTypes : BaseBLL<AskalePortal.Data.Models.ApprovalProcessType>
        {
            public ApprovalProcessTypes(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.ApprovalProcessType> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.ApprovalProcessType> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            

            #endregion GetAllWithPage
        }
    }

    
}
