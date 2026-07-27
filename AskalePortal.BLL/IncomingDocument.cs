using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;


namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class IncomingDocuments : BaseBLL<AskalePortal.Data.Models.IncomingDocument>
        {
            private readonly IWebHostEnvironment _env;
            private readonly IConfiguration _configuration;
            private readonly IMapper _mapper;
            public IncomingDocuments(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }


            public List<AskalePortal.Data.Models.IncomingDocument> GetAllIncoming(AskalePortal.Data.Models.AdminUser USER, DateTime? incomingDate, int? sourceID, string title, int? documentOrder, int? userID, int pageNumber, int pageSize, bool newRole)
            {
                BLLActions.AdminUsers bllUsers = new BLLActions.AdminUsers(_configuration, _env,_mapper);
                var idList = bllUsers.GetByID(USER.Id)?.documentUserId.ToUserIDIntList();

                if (incomingDate.HasValue)
                {
                    var date1 = incomingDate.Value.Date;
                    var date2 = incomingDate.Value.Date.AddDays(1);

                    var q = dal.Get(k => k.isOutgoing == false
                                        && (idList!.Contains(k.createdUserId!.Value) || k.createdUserId == USER.Id || USER.roleId == 1 || newRole == true)
                                        && (k.sourceId == sourceID || sourceID == null)
                                        && (k.title.Contains(title) || string.IsNullOrEmpty(title))
                                        && (k.documentOrder == documentOrder || documentOrder == null)
                                        && (k.userId == userID || userID == null)
                                        && (k.incomingDate > date1 && k.incomingDate < date2)
                                        && k.enabled == true
                                        ).OrderByDescending(k => k.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();
                    return q;
                }
                else
                {
                    var q = dal.Get(k => k.isOutgoing == false
                                        && (idList!.Contains(k.createdUserId!.Value) || k.createdUserId == USER.Id || USER.roleId == 1 || newRole == true)
                                        && (k.title.Contains(title) || string.IsNullOrEmpty(title))
                                        && (k.documentOrder == documentOrder || documentOrder == null)
                                        && (k.userId == userID || userID == null)
                                        && (k.sourceId == sourceID || sourceID == null)
                                        && k.enabled == true
                                        ).OrderByDescending(k => k.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();
                    return q;
                }
            }

            public List<AskalePortal.Data.Models.IncomingDocument> GetAllOutgoing(AskalePortal.Data.Models.AdminUser USER, DateTime? incomingDate, int? sourceID, string title, int? documentOrder, int? userID, int pageNumber, int pageSize, bool newRole)
            {

                BLLActions.AdminUsers bllUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                var idList = bllUsers.GetByID(USER.Id)?.documentUserId.ToUserIDIntList()??[];

                if (incomingDate.HasValue)
                {
                    var date1 = incomingDate.Value.Date;
                    var date2 = incomingDate.Value.Date.AddDays(1);
                    var q = dal.Get(k => k.isOutgoing == true
                                        && (idList.Contains(k.createdUserId!.Value) || k.createdUserId == USER.Id || USER.roleId == 1 || newRole == true)
                                        && (k.sourceId == sourceID || sourceID == null)
                                        && (k.incomingDate > date1 && k.incomingDate < date2)
                                        && (k.title.Contains(title) || string.IsNullOrEmpty(title))
                                        && (k.documentOrder == documentOrder || documentOrder == null)
                                        && (k.userId == userID || userID == null)
                                        && k.enabled == true
                                        ).OrderByDescending(k => k.incomingDate).Skip(pageNumber * pageSize).Take(pageSize).ToList();
                    return q;
                }
                else
                {
                    var q = dal.Get(k => k.isOutgoing == true
                                        && (idList.Contains(k.createdUserId!.Value) || k.createdUserId == USER.Id || USER.roleId == 1 || newRole == true)
                                        && (k.sourceId == sourceID || sourceID == null)
                                        && (k.title.Contains(title) || string.IsNullOrEmpty(title))
                                        && (k.documentOrder == documentOrder || documentOrder == null)
                                        && (k.userId == userID || userID == null)
                                        && k.enabled == true
                                        ).OrderByDescending(k => k.incomingDate).Skip(pageNumber * pageSize).Take(pageSize).ToList();
                    return q;
                }
            }

            public IncomingDocumentMyEditDto getMyEdit(int id, bool isOutgoing)
            {
                IncomingDocumentMyEditDto incomingDocumentMyEditDto = dal.Get(u => u.enabled
                && u.Id == id && u.isOutgoing == isOutgoing).Select(u => new IncomingDocumentMyEditDto()
                {
                    id = u.Id,
                    createdCompanyName = "",
                    createdUserName = "",
                    documentDate = u.documentDate,
                    documentSpecialNumber = u.documentSpecialNumber,
                    files = null,
                    incomingDate = u.incomingDate,
                    isCompleted = u.isCompleted,
                    notes = u.notes,
                    sourceTitle = u.source.title,
                    title = u.title,
                    typeTitle = u.type.title,
                    userCompanyName = "",
                    userName = ""


                }).First();
                BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);
                List<AttachedFile> listFiles = bllAttachedFiles.getByModuleIdAndTargetId((int)CommonConstants.MODULES.INCOMING_DOCUMENTS, id);
                List<string> listFilesName = new List<string>();
                foreach (AttachedFile item in listFiles)
                {
                    listFilesName.Add(item.filePath);
                }
                incomingDocumentMyEditDto.files = listFilesName;
                return incomingDocumentMyEditDto;
            }

            public List<AskalePortal.Data.Models.IncomingDocument> GetMyIncomingDocuments(AskalePortal.Data.Models.AdminUser USER, string title, int pageNumber, int pageSize)
            {
                BLLActions.AdminUsers bllUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                var idList = bllUsers.GetByID(USER.Id)?.documentUserId.ToUserIDIntList()??[];

                var q = dal.Get(k => k.isOutgoing == false
                                    && (idList.Contains(k.userId!.Value) || k.userId == USER.Id || k.userIds.Contains("[" + USER.Id + "]"))
                                    && (k.title.Contains(title) || string.IsNullOrEmpty(title))
                                    && k.enabled == true).OrderByDescending(k => k.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.IncomingDocument> GetMyOutgoingDocuments(AskalePortal.Data.Models.AdminUser USER, string title, int pageNumber, int pageSize)
            {
                BLLActions.AdminUsers bllUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                var idList = bllUsers.GetByID(USER.Id)?.documentUserId.ToUserIDIntList()??[];

                var q = dal.Get(k => k.isOutgoing == true
                                    && (idList.Contains(k.userId!.Value) || k.userId == USER.Id || k.userIds.Contains("[" + USER.Id + "]"))
                                    && (k.title.Contains(title) || string.IsNullOrEmpty(title))
                                    && k.enabled == true
                                    ).OrderByDescending(k => k.incomingDate).Skip(pageNumber * pageSize).Take(pageSize).ToList();
                return q;
            }

            public int GetNewNumber()
            {
                var dt1 = DateTime.Today;
                var dt2 = dt1.AddDays(1);

                var q = dal.Get(k => (k.incomingDate > dt1 && k.incomingDate < dt2) && k.enabled == true);

                return q.Count() + 1;
            }

            public PageReturn<IncomingDocumentDto> listByPageable(FilterPageParam<IncomingDocumentDtoRequest> filterPageParam, int userId)
            {
                PageReturn<IncomingDocumentDto>? result = new PageReturn<IncomingDocumentDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? filterUserId = filterPageParam.liste?.userId;
                int? filterSourceId = filterPageParam.liste?.sourceId;
                bool? filterIsOutgoing = filterPageParam.liste?.isOutgoing;
                DateTime? filterGirisTarihi = filterPageParam.liste?.girisTarihi;
                string? filterTitle = filterPageParam.liste?.title;
                int? filterDocumentOrder = filterPageParam.liste?.documentOrder;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                int userRoleId = bllAdminUsers.GetByID(userId)!.roleId;
                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(userRoleId, (int)CommonConstants.MODULES.INCOMING_DOCUMENTS);


                IQueryable<IncomingDocument> query = dal.Get(u => u.enabled &&
                filterGirisTarihi == null ? true : u.documentDate == filterGirisTarihi
                && ((filterSourceId == null || filterSourceId == 0) ? true : u.sourceId == filterSourceId)
                && (filterIsOutgoing == null ? true : u.isOutgoing == filterIsOutgoing)
                && (filterTitle == null || filterTitle == "" ? true : u.title.Contains(filterTitle))
                && (filterDocumentOrder == null || filterDocumentOrder == 0 ? true : u.documentOrder == filterDocumentOrder)
                && (filterUserId == null || filterUserId == 0 ? true : u.userId == filterUserId)
                && (userRoleId == 1 || (roleDetail != null && roleDetail.canSeeLogs) ? u.createdUserId == userId : true)
                );
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new IncomingDocumentDto()
                    {
                        companyName = u.user.company.vkorg ?? "",
                        createdUserId = u.createdUserId,
                        documentDate = u.documentDate,
                        documentNumber = u.documentNumber,
                        documentOrder = u.documentOrder,
                        documentSpecialNumber = u.documentSpecialNumber,
                        id = u.Id,
                        incomingDate = u.incomingDate,
                        isCompleted = u.isCompleted,
                        notes = u.notes,
                        sourceTitle = u.source.title,
                        title = u.title,
                        userName = u.user.name,
                        userTitle = u.user.shortDescription,



                    }).OrderByDescending(u=>u.documentNumber).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public async Task<int> saveMyEdit(int id, string notes, bool isCompleted)
            {
                try
                {
                    IncomingDocument? incomingDocument = GetByID(id);
                    incomingDocument!.notes = notes;
                    incomingDocument.isCompleted = isCompleted;
                    await Update(incomingDocument);
                    return 1;
                }
                catch (Exception)
                {
                    return 2;
                }

            }
        }
    }
}