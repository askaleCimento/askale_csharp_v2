
using AskalePortal.Constants;
using AskalePortal.Data.Models;
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
        public class HelpDeskCategories : BaseBLL<AskalePortal.Data.Models.HelpDeskCategory>
        {
            public HelpDeskCategories(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.HelpDeskCategory> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public List<HelpDeskCategoryTree> GetAllWithSub()
            {
                List<HelpDeskCategoryTree> ncList = new List<HelpDeskCategoryTree>();
                List<AskalePortal.Data.Models.HelpDeskCategory> dbNcList = dal.Get(k => k.enabled == true).OrderBy(k => k.Id).ToList();
                List<AskalePortal.Data.Models.HelpDeskCategory> ncTopList = dbNcList.Where(k => k.topId == -1 &&  k.enabled == true).OrderBy(k => k.title).ToList();

                foreach (var item in ncTopList)
                {
                    HelpDeskCategoryTree nc = new HelpDeskCategoryTree(item.Id.ToString(), item.title);
                    ncList.Add(nc);
                    List<AskalePortal.Data.Models.HelpDeskCategory> ncSubList = dbNcList.Where(k => k.topId == item.Id && k.enabled == true).OrderBy(k => k.title).ToList();
                    foreach (var item2 in ncSubList)
                    {
                        nc = new HelpDeskCategoryTree(item2.Id.ToString(), item.title + " » " + item2.title);
                        ncList.Add(nc);
                    }
                }
                return ncList;
            }

            public List<AskalePortal.Data.Models.HelpDeskCategory> GetWithTree()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title).ToList();
                var s = q.OrderBy(x =>
                {
                    if (x.topId == -1)
                        return x.Id;
                    else
                        return x.topId;
                }).ThenBy(y => y.Id);

                var tempCount = q.Where(x => x.topId != -1).Count();

                if (tempCount > 0)
                {
                    foreach (var item in s)
                    {
                        if (item.topId == -1)
                        {
                            item.title = "<strong>" + item.title + "</strong>";
                        }
                        else
                        {
                            string mainTitle = q.Where(k => k.Id == item.topId).FirstOrDefault()?.title ??"";
                            item.title = StaticMethods.PlainText(mainTitle) + " » " + item.title;
                        }
                    }
                }

                return s.ToList();
            }

            #endregion GetAll

            #region GetAll

            public List<AskalePortal.Data.Models.HelpDeskCategory> GetByTopID(int topID)
            {
                var q = dal.Get(k => k.topId == topID && k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.HelpDeskCategory> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            public List<HelpDeskCategorySaveDto>? GetAllFilter(FilterParam<HelpDeskStatusListDtoParameter> filterParam)
            {
                string title = filterParam.liste?.title ?? "";
                return dal.Get(k => k.enabled == true &&k.title.Contains(title)).Select(u=> new HelpDeskCategorySaveDto()
                {
                    title=u.title,
                    createdDate=u.createdDate,
                    createdUserId=u.createdUserId,
                    enabled= u.enabled,
                    id=u.Id,
                    topId = u.topId,
                    updateDate = u.updatedDate,
                    updatedUserId = u.updatedUserId
                }).ToList();

               
            }

            #endregion GetAllWithPage
        }

        public class HelpDeskCategoryTree
        {
            public string ID { get; set; }
            public string title { get; set; }
            public HelpDeskCategoryTree(string ID, string title)
            {
                this.ID = ID;
                this.title = title;
            }
        }
    }

    
}
