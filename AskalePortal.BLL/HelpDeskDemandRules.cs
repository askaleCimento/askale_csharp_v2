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
        public class HelpDeskDemandRules : BaseBLL<AskalePortal.Data.Models.HelpDeskDemandRule>
        {
            public HelpDeskDemandRules(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            

            public List<AskalePortal.Data.Models.HelpDeskDemandRule> GetAll(string companyID, int helpDeskCategoryID)
            {
                string companies = "[" + companyID + "]";
                string categories = "[" + helpDeskCategoryID + "]";
                var q = dal.Get(k =>k.companies.Contains(companies) && k.helpDeskCategories.Contains(categories) && k.enabled == true).OrderBy(k => k.Id);

                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.HelpDeskDemandRule> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.Id);

                return q.ToList();
            }

            public List<HelpDeskDemandRule>? GetAllFilter(FilterParam<HelpDeskStatusListDtoParameter> filterParam)
            {

                string? title = filterParam.liste?.title;
                List<HelpDeskDemandRule> liste = dal.Get(u => u.enabled && (string.IsNullOrEmpty(title) ? true : u.helpDeskRole.title.ToLower().Contains(title))).ToList();
                return liste;
            }

            public List<HelpDeskDemandRule> findIdByCompanyAndHelpDeskCategory(string? vkorg, string? helpDeskCategoryId)
            {
                List<HelpDeskDemandRule> q = dal.Get(k =>
                k.enabled == true &&
                k.companies.Contains(vkorg ?? "") &&
                k.helpDeskCategories.Contains(helpDeskCategoryId ?? "")).ToList();
                return q;
            }
        }
    }

    
}
