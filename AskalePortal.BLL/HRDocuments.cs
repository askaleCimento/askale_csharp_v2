
using AskalePortal.Constants;
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
        public class HRDocuments : BaseBLL<Data.Models.HRDocument>
        {
            public HRDocuments(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region Get

            public Data.Models.HRDocument? GetByHRDocumentID(string documentID)
            {
                var q = dal.Get(k => (k.documentId.ToString() == documentID) &&
                                     k.enabled == true);
                return q.FirstOrDefault();
            }

            #endregion Get

            #region GetFilesByTopID

            public List<AskalePortal.Data.Models.HRDocument> GetFilesByTopID(int topID, int archiveID)
            {
                var q = dal.Get(k => (k.topId == topID) && (k.typeId !=1) && k.archiveId == archiveID &&
                                     k.enabled == true).OrderBy(d => d.title);
                return q.ToList();
            }

            #endregion GetFilesByTopID

            #region GetDirectoriesByTopID

            public List<AskalePortal.Data.Models.HRDocument> GetDirectoriesByTopID(int topID, int archiveID)
            {
                var q = dal.Get(k => (k.topId == topID) && (k.typeId == 1) && k.archiveId == archiveID &&
                                     k.enabled == true).OrderBy(d => d.title);
                return q.ToList();
            }

            #endregion GetFilesByTopID

            #region GetDirectoryList

            public List<AskalePortal.Data.Models.HRDocument> GetDirectoryList(int archiveID)
            {
                var q = dal.Get(k => (k.typeId == 1) && k.archiveId == archiveID &&
                                     k.enabled == true).OrderBy(d => d.title);
                return q.ToList();
            }

            #endregion GetDirectoryList

            #region GetAllByTopID

            public List<AskalePortal.Data.Models.HRDocument> GetAllByTopID(int topID, int archiveID)
            {
                var q = dal.Get(k => (k.topId == topID) && k.archiveId == archiveID &&
                                     k.enabled == true).OrderBy(d=> d.title);
                return q.ToList();
            }

            #endregion GetAllByTopID

            public List<AskalePortal.Data.Models.HRDocument> GetWithTree(string title, int archiveID)
            {
                var q = dal.Get(k => k.enabled == true && k.typeId == 1 && k.archiveId == archiveID && (k.title.Contains(title) || string.IsNullOrEmpty(title))).OrderBy(k => k.title).ToList();
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
                            string? mainTitle = q.Where(k => k.Id == item.topId).FirstOrDefault()?.title;
                            item.title = StaticMethods.PlainText(mainTitle??"") + " » " + item.title;
                        }
                    }
                }

                return s.ToList();
            }

            public List<HRDocumentTree> GetAllWithSub(int archiveID)
            {
                List<HRDocumentTree> ncList = new List<HRDocumentTree>();
                List<AskalePortal.Data.Models.HRDocument> dbNcList = dal.Get(k => k.enabled == true && k.typeId == 1 && k.archiveId == archiveID).OrderBy(k => k.Id).ToList();
                List<AskalePortal.Data.Models.HRDocument> ncTopList = dbNcList.Where(k => k.topId == -1 && k.enabled == true).OrderBy(k => k.title).ToList();

                foreach (var item in ncTopList)
                {
                    HRDocumentTree nc = new HRDocumentTree(item.Id.ToString(), item.title);
                    ncList.Add(nc);
                    List<AskalePortal.Data.Models.HRDocument> ncSubList = dbNcList.Where(k => k.topId == item.Id && k.enabled == true).OrderBy(k => k.title).ToList();
                    foreach (var item2 in ncSubList)
                    {
                        nc = new HRDocumentTree(item2.Id.ToString(), item.title + " » " + item2.title);
                        ncList.Add(nc);
                    }
                }
                return ncList;
            }

            public List<AskalePortal.Data.Models.HRDocument> GetWithTreeForCopy(int archiveID)
            {
                var q = dal.Get(k => k.enabled == true && k.typeId == 1 && k.archiveId == archiveID).OrderBy(k => k.title).ToList();
                var s = q.OrderBy(x =>
                {
                    if (x.topId == -1)
                        return x.Id;
                    else
                        return x.topId;
                }).ThenBy(y => y.Id);

                return s.ToList();
            }
        }

        public class HRDocumentTree
        {
            public string ID { get; set; }
            public string title { get; set; }
            public HRDocumentTree(string ID, string title)
            {
                this.ID = ID;
                this.title = title;
            }
        }
    }

    
}
