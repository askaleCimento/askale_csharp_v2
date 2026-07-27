using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ActionLogs : BaseBLL<AskalePortal.Data.Models.ActionLog>
        {
            public ActionLogs(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region Get

            public List<AskalePortal.Data.Models.ActionLog> Get(int? moduleID,int? dataID, int? userID)
            {
                var q = dal.Get(k => (k.moduleId == moduleID || moduleID == null) &&
                                     (k.dataId == dataID || dataID == null) &&
                                     (k.userId == userID || userID == null) &&
                                     k.enabled == true);
                return q.ToList();
            }

            #endregion Get

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.ActionLog> GetAllWithPage(int? moduleID, int? dataID, string actionType,int? userID,string ip, DateTime? startDate, DateTime? endDate,
                                                    int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.moduleId == moduleID || moduleID == null) &&
                                     (k.dataId == dataID || dataID == null) &&
                                     (k.actionType.Equals(actionType) || string.IsNullOrEmpty(actionType)) &&
                                     (k.userId == userID || userID == null) &&
                                     (k.actionType.Contains(ip) || string.IsNullOrEmpty(ip)) &&
                                     (k.createdDate >= startDate || startDate == null) &&
                                     (k.createdDate <= endDate || endDate == null) &&
                                     k.enabled == true)
                                     .OrderByDescending(k => k.createdDate)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            #endregion GetAllWithPage

            #region AddList

            public void AddList(List<AskalePortal.Data.Models.ActionLog> lstActionLog)
            {
                lstActionLog.ForEach(k => k.enabled = true);

                dal.AddList(lstActionLog);
            }

            #endregion AddList
        }
    }
}