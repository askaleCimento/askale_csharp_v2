
using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class IcraDocuments : BaseBLL<AskalePortal.Data.Models.IcraDocument>
        {
            public IcraDocuments(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region Get

            public Data.Models.IcraDocument? GetByIcraDocumentID(string documentID)
            {
                var q = dal.Get(k => (k.documentID.ToString() == documentID) &&
                                     k.enabled == true);
                return q.FirstOrDefault();
            }

            #endregion Get

            #region GetFilesByTopID

            public List<AskalePortal.Data.Models.IcraDocument> GetFilesByTopID(int topID, int archiveID)
            {
                var q = dal.Get(k => (k.topId == topID) && (k.typeId !=1) && k.archiveId == archiveID &&
                                     k.enabled == true).OrderBy(d => d.title);
                return q.ToList();
            }

            #endregion GetFilesByTopID

            #region GetDirectoriesByTopID

            public List<AskalePortal.Data.Models.IcraDocument> GetDirectoriesByTopID(int topID, int archiveID)
            {
                var q = dal.Get(k => (k.topId == topID) && (k.typeId == 1) && k.archiveId == archiveID &&
                                     k.enabled == true).OrderBy(d => d.title);
                return q.ToList();
            }

            #endregion GetFilesByTopID

            #region GetDirectoryList

            public List<AskalePortal.Data.Models.IcraDocument> GetDirectoryList(int archiveID)
            {
                var q = dal.Get(k => (k.typeId == 1) && k.archiveId == archiveID &&
                                     k.enabled == true).OrderBy(d => d.title);
                return q.ToList();
            }

            #endregion GetDirectoryList

            #region GetAllByTopID

            public List<AskalePortal.Data.Models.IcraDocument> GetAllByTopID(int topID, int archiveID)
            {
                var q = dal.Get(k => (k.topId == topID) && k.archiveId == archiveID &&
                                     k.enabled == true).OrderBy(d=> d.title);
                return q.ToList();
            }

            #endregion GetAllByTopID

            public List<AskalePortal.Data.Models.IcraDocument> GetWithTree(string title, int archiveID)
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
                            string mainTitle = q.Where(k => k.Id == item.topId).FirstOrDefault().title;
                            item.title = StaticMethods.PlainText(mainTitle) + " » " + item.title;
                        }
                    }
                }

                return s.ToList();
            }

            public List<IcraDocumentTree> GetAllWithSub(int archiveID)
            {
                List<IcraDocumentTree> ncList = new List<IcraDocumentTree>();
                List<AskalePortal.Data.Models.IcraDocument> dbNcList = dal.Get(k => k.enabled == true && k.typeId == 1 && k.archiveId == archiveID).OrderBy(k => k.Id).ToList();
                List<AskalePortal.Data.Models.IcraDocument> ncTopList = dbNcList.Where(k => k.topId == -1 && k.enabled == true).OrderBy(k => k.title).ToList();

                foreach (var item in ncTopList)
                {
                    IcraDocumentTree nc = new IcraDocumentTree(item.Id.ToString(), item.title);
                    ncList.Add(nc);
                    List<AskalePortal.Data.Models.IcraDocument> ncSubList = dbNcList.Where(k => k.topId == item.Id && k.enabled == true).OrderBy(k => k.title).ToList();
                    foreach (var item2 in ncSubList)
                    {
                        nc = new IcraDocumentTree(item2.Id.ToString(), item.title + " » " + item2.title);
                        ncList.Add(nc);
                    }
                }
                return ncList;
            }

            public List<AskalePortal.Data.Models.IcraDocument> GetWithTreeForCopy(int archiveID)
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

            public List<ExecutiveDocumentsDto> getByTopId(int topid)
            {
                List<ExecutiveDocumentsDto> liste = dal.Get(k => k.enabled && k.topId == topid).Select(k => new ExecutiveDocumentsDto()
                {
                    createdDate = k.createdDate,
                    documentId = k.documentID.ToString(),
                    filename = k.filename,
                    fileSize = k.fileSize ?? 0,
                    id = k.Id,
                    title = k.title,
                    topID = k.topId,
                    typeID = k.typeId,
                    typeName = k.typeName,
                }).ToList();
                return liste;
            }

            public ExecutiveDocumentsDto? getById(int id)
            {
                ExecutiveDocumentsDto? dto = dal.Get(k => k.enabled && k.Id == id).Select(k => new ExecutiveDocumentsDto()
                {
                    createdDate = k.createdDate,
                    documentId = k.documentID.ToString(),
                    filename = k.filename,
                    fileSize = k.fileSize ?? 0,
                    id = k.Id,
                    title = k.title,
                    topID = k.topId,
                    typeID = k.typeId,
                    typeName = k.typeName,
                }).FirstOrDefault();
                return dto;

            }

            public async Task<IcraDocument?> save(IcraDocument icraDocument, int userId)
            {
                if (icraDocument != null)
                {
                    
                    if (icraDocument?.Id != 0)
                    {

                        icraDocument!.updatedDate = DateTime.Now;
                        icraDocument.updatedUserId = userId == 0 ? null : userId;
                        await Update(icraDocument);
                        return (icraDocument);
                    }
                    else
                    {

                        icraDocument.createdDate = DateTime.Now;
                        icraDocument.createdUserId = userId;
                        icraDocument.enabled = true;
                        await Add(icraDocument);
                        return (icraDocument);
                    }
                }
                return null;
            }
        }

        public class IcraDocumentTree
        {
            public string ID { get; set; }
            public string title { get; set; }
            public IcraDocumentTree(string ID, string title)
            {
                this.ID = ID;
                this.title = title;
            }
        }
    }

    
}
