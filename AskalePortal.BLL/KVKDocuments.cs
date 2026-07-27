using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
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
        public class KVKDocuments : BaseBLL<AskalePortal.Data.Models.KVKDocument>
        {
            private readonly IMapper _mapper;
            public KVKDocuments(IConfiguration configuration, IWebHostEnvironment env,IMapper mapper) : base(configuration, env)
            {
                _mapper = mapper;
            }
            #region Get

            public Data.Models.KVKDocument? GetByKVKDocumentId(string documentId)
            {
                var q = dal.Get(k => (k.documentId.ToString() == documentId) &&
                                     k.enabled == true);
                return q.FirstOrDefault();
            }

            #endregion Get

            #region GetFilesByTopId

            public List<AskalePortal.Data.Models.KVKDocument> GetFilesByTopId(int topId, int archiveId)
            {
                var q = dal.Get(k => (k.topId == topId) && (k.typeId !=1) && k.archiveId == archiveId &&
                                     k.enabled == true).OrderBy(d => d.title);
                return q.ToList();
            }

            #endregion GetFilesByTopId

            #region GetDirectoriesByTopId

            public List<AskalePortal.Data.Models.KVKDocument> GetDirectoriesByTopId(int topId, int archiveId)
            {
                var q = dal.Get(k => (k.topId == topId) && (k.typeId == 1) && k.archiveId == archiveId &&
                                     k.enabled == true).OrderBy(d => d.title);
                return q.ToList();
            }

            #endregion GetFilesByTopId

            #region GetDirectoryList

            public List<AskalePortal.Data.Models.KVKDocument> GetDirectoryList(int archiveId)
            {
                var q = dal.Get(k => (k.typeId == 1) && k.archiveId == archiveId &&
                                     k.enabled == true).OrderBy(d => d.title);
                return q.ToList();
            }

            #endregion GetDirectoryList

            #region GetAllByTopId

            public List<AskalePortal.Data.Models.KVKDocument> GetAllByTopId(int topId, int archiveId)
            {
                var q = dal.Get(k => (k.topId == topId) && k.archiveId == archiveId &&
                                     k.enabled == true).OrderBy(d=> d.title);
                return q.ToList();
            }

            #endregion GetAllByTopId

            public List<AskalePortal.Data.Models.KVKDocument> GetWithTree(string title, int archiveId)
            {
                var q = dal.Get(k => k.enabled == true && k.typeId == 1 && k.archiveId == archiveId && (k.title.Contains(title) || string.IsNullOrEmpty(title))).OrderBy(k => k.title).ToList();
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

            public List<KVKDocumentTree> GetAllWithSub(int archiveId)
            {
                List<KVKDocumentTree> ncList = new List<KVKDocumentTree>();
                List<AskalePortal.Data.Models.KVKDocument> dbNcList = dal.Get(k => k.enabled == true && k.typeId == 1 && k.archiveId == archiveId).OrderBy(k => k.Id).ToList();
                List<AskalePortal.Data.Models.KVKDocument> ncTopList = dbNcList.Where(k => k.topId == -1 && k.enabled == true).OrderBy(k => k.title).ToList();

                foreach (var item in ncTopList)
                {
                    KVKDocumentTree nc = new KVKDocumentTree(item.Id.ToString(), item.title);
                    ncList.Add(nc);
                    List<AskalePortal.Data.Models.KVKDocument> ncSubList = dbNcList.Where(k => k.topId == item.Id && k.enabled == true).OrderBy(k => k.title).ToList();
                    foreach (var item2 in ncSubList)
                    {
                        nc = new KVKDocumentTree(item2.Id.ToString(), item.title + " » " + item2.title);
                        ncList.Add(nc);
                    }
                }
                return ncList;
            }

            public List<AskalePortal.Data.Models.KVKDocument> GetWithTreeForCopy(int archiveId)
            {
                var q = dal.Get(k => k.enabled == true && k.typeId == 1 && k.archiveId == archiveId).OrderBy(k => k.title).ToList();
                var s = q.OrderBy(x =>
                {
                    if (x.topId == -1)
                        return x.Id;
                    else
                        return x.topId;
                }).ThenBy(y => y.Id);

                return s.ToList();
            }

            public List<KVKDocument>? getByTopId(int topid)
            {
                return dal.Get(u => u.enabled && u.topId == topid).ToList();
            }

            public async Task<CorporateDocumentsDto> saveFolder(KVKDocument kurumsalDocument)
            {
                KVKDocument? document = await Add(kurumsalDocument);
                CorporateDocumentsDto dto = _mapper.Map<CorporateDocumentsDto>(document);
                return dto;
            }
        }

        public class KVKDocumentTree
        {
            public string Id { get; set; }
            public string title { get; set; }
            public KVKDocumentTree(string Id, string title)
            {
                this.Id = Id;
                this.title = title;
            }
        }
    }

    
}
