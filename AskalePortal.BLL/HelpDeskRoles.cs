using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
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
        public class HelpDeskRoles : BaseBLL<AskalePortal.Data.Models.HelpDeskRole>
        {
            public HelpDeskRoles(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.HelpDeskRole>     GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public List<HelpDeskRoleSaveDto>? GetAllFilter(FilterParam<HelpDeskStatusListDtoParameter> filterParam)
            {
                string title = filterParam.liste?.title ?? "";
                return dal.Get(u => u.enabled && u.title.Contains(title)).Select(u=> new HelpDeskRoleSaveDto()
                {
                    title=u.title,
                    enabled=u.enabled,
                    approval=u.approval,
                    createdDate=u.createdDate.ToString(),
                    createdUserId=u.createdUserId,
                    id=u.Id,
                    updateDate=u.updatedDate.ToString(),
                    updatedUserId = u.updatedUserId
                }).ToList();
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.HelpDeskRole> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            public List<IdandText> GetIdandText()
            {
                return dal.Get(u => u.enabled).Select(u => new IdandText() { id = u.Id, text = u.title }).ToList();
            }

            #endregion GetAllWithPage
        }
    }

    
}
