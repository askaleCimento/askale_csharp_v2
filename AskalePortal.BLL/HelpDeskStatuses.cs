using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
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
        public class HelpDeskStatuses : BaseBLL<AskalePortal.Data.Models.HelpDeskStatus>
        {
            public HelpDeskStatuses(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {

            }
            #region GetAll

            public override List<AskalePortal.Data.Models.HelpDeskStatus> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public List<HelpDeskStatus>? GetAllFilter(FilterParam<HelpDeskStatusListDtoParameter> filterParam)
            {
                string? title = filterParam.liste?.title;
                List<HelpDeskStatus> liste = dal.Get(u => u.enabled && (string.IsNullOrEmpty(title) ? true : u.title.ToLower().Contains(title))).ToList();
                return liste;
            }

            #endregion GetAll
        }
    }    
}