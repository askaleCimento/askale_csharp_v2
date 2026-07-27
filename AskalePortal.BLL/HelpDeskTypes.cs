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
        public class HelpDeskTypes : BaseBLL<AskalePortal.Data.Models.HelpDeskType>
        {
            public HelpDeskTypes(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.HelpDeskType> GetAll(string title)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrEmpty(title)) && k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.HelpDeskType> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public List<HelpDeskType> getAllFilter(FilterParam<HelpDeskStatusListDtoParameter> filterParam)
            {
                string title = filterParam.liste?.title ??"";
                List<HelpDeskType> liste = dal.Get(u => u.enabled && u.title.Contains(title)).ToList();
                return liste;
            }

            #endregion GetAll
        }
    }
}