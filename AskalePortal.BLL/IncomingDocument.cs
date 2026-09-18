using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            public IncomingDocuments(
                IConfiguration configuration,
                IWebHostEnvironment env,
                IMapper mapper)
                : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public List<IncomingDocument> GetAllIncoming(
                AdminUser USER,
                DateTime? incomingDate,
                int? sourceID,
                string title,
                int? documentOrder,
                int? userID,
                int pageNumber,
                int pageSize,
                bool newRole)
            {
                BLLActions.AdminUsers bllUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                var idList = bllUsers
                    .GetByID(USER.Id)?
                    .documentUserId
                    .ToUserIDIntList() ?? new List<int>();

                IQueryable<IncomingDocument> query =
                    dal.Get(k =>
                        k.enabled &&
                        !k.isOutgoing &&
                        (
                            USER.roleId == 1 ||
                            newRole ||
                            k.createdUserId == USER.Id ||
                            (
                                k.createdUserId.HasValue &&
                                idList.Contains(k.createdUserId.Value)
                            )
                        )
                    );

                if (incomingDate.HasValue)
                {
                    DateTime date1 = incomingDate.Value.Date;
                    DateTime date2 = date1.AddDays(1);

                    query = query.Where(k =>
                        k.incomingDate >= date1 &&
                        k.incomingDate < date2);
                }

                if (sourceID.HasValue && sourceID.Value != 0)
                {
                    query = query.Where(k =>
                        k.sourceId == sourceID.Value);
                }

                if (!string.IsNullOrWhiteSpace(title))
                {
                    query = query.Where(k =>
                        k.title.Contains(title));
                }

                if (documentOrder.HasValue &&
                    documentOrder.Value != 0)
                {
                    query = query.Where(k =>
                        k.documentOrder == documentOrder.Value);
                }

                if (userID.HasValue &&
                    userID.Value != 0)
                {
                    query = query.Where(k =>
                        k.userId == userID.Value);
                }

                return query
                    .OrderByDescending(k => k.Id)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public List<IncomingDocument> GetAllOutgoing(
                AdminUser USER,
                DateTime? incomingDate,
                int? sourceID,
                string title,
                int? documentOrder,
                int? userID,
                int pageNumber,
                int pageSize,
                bool newRole)
            {
                BLLActions.AdminUsers bllUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                var idList = bllUsers
                    .GetByID(USER.Id)?
                    .documentUserId
                    .ToUserIDIntList() ?? new List<int>();

                IQueryable<IncomingDocument> query =
                    dal.Get(k =>
                        k.enabled &&
                        k.isOutgoing &&
                        (
                            USER.roleId == 1 ||
                            newRole ||
                            k.createdUserId == USER.Id ||
                            (
                                k.createdUserId.HasValue &&
                                idList.Contains(k.createdUserId.Value)
                            )
                        )
                    );

                if (incomingDate.HasValue)
                {
                    DateTime date1 = incomingDate.Value.Date;
                    DateTime date2 = date1.AddDays(1);

                    query = query.Where(k =>
                        k.incomingDate >= date1 &&
                        k.incomingDate < date2);
                }

                if (sourceID.HasValue &&
                    sourceID.Value != 0)
                {
                    query = query.Where(k =>
                        k.sourceId == sourceID.Value);
                }

                if (!string.IsNullOrWhiteSpace(title))
                {
                    query = query.Where(k =>
                        k.title.Contains(title));
                }

                if (documentOrder.HasValue &&
                    documentOrder.Value != 0)
                {
                    query = query.Where(k =>
                        k.documentOrder == documentOrder.Value);
                }

                if (userID.HasValue &&
                    userID.Value != 0)
                {
                    query = query.Where(k =>
                        k.userId == userID.Value);
                }

                return query
                    .OrderByDescending(k => k.incomingDate)
                    .ThenByDescending(k => k.Id)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public IncomingDocumentMyEditDto getMyEdit(
                int id,
                bool isOutgoing)
            {
                IncomingDocumentMyEditDto? incomingDocumentMyEditDto =
                    dal.Get(u =>
                            u.enabled &&
                            u.Id == id &&
                            u.isOutgoing == isOutgoing)
                        .Select(u => new IncomingDocumentMyEditDto()
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

                            sourceTitle =
                                u.source != null
                                    ? u.source.title
                                    : "",

                            title = u.title,

                            typeTitle =
                                u.type != null
                                    ? u.type.title
                                    : "",

                            userCompanyName = "",
                            userName = ""
                        })
                        .FirstOrDefault();

                if (incomingDocumentMyEditDto == null)
                {
                    throw new Exception("Evrak bulunamadı.");
                }

                BLLActions.AttachedFiles bllAttachedFiles =
                    new BLLActions.AttachedFiles(
                        _configuration,
                        _env);

                List<AttachedFile> listFiles =
                    bllAttachedFiles.getByModuleIdAndTargetId(
                        (int)CommonConstants.MODULES.INCOMING_DOCUMENTS,
                        id);

                List<string> listFilesName =
                    new List<string>();

                foreach (AttachedFile item in listFiles)
                {
                    if (!string.IsNullOrWhiteSpace(item.filePath))
                    {
                        listFilesName.Add(item.filePath);
                    }
                }

                incomingDocumentMyEditDto.files =
                    listFilesName;

                return incomingDocumentMyEditDto;
            }

            public List<IncomingDocument> GetMyIncomingDocuments(
                AdminUser USER,
                string title,
                int pageNumber,
                int pageSize)
            {
                BLLActions.AdminUsers bllUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                var idList = bllUsers
                    .GetByID(USER.Id)?
                    .documentUserId
                    .ToUserIDIntList() ?? new List<int>();

                IQueryable<IncomingDocument> query =
                    dal.Get(k =>
                        k.enabled &&
                        !k.isOutgoing &&
                        (
                            k.userId == USER.Id ||
                            (
                                k.userId.HasValue &&
                                idList.Contains(k.userId.Value)
                            ) ||
                            (
                                k.userIds != null &&
                                k.userIds.Contains(
                                    "[" + USER.Id + "]")
                            )
                        )
                    );

                if (!string.IsNullOrWhiteSpace(title))
                {
                    query = query.Where(k =>
                        k.title.Contains(title));
                }

                return query
                    .OrderByDescending(k => k.Id)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public List<IncomingDocument> GetMyOutgoingDocuments(
                AdminUser USER,
                string title,
                int pageNumber,
                int pageSize)
            {
                BLLActions.AdminUsers bllUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                var idList = bllUsers
                    .GetByID(USER.Id)?
                    .documentUserId
                    .ToUserIDIntList() ?? new List<int>();

                IQueryable<IncomingDocument> query =
                    dal.Get(k =>
                        k.enabled &&
                        k.isOutgoing &&
                        (
                            k.userId == USER.Id ||
                            (
                                k.userId.HasValue &&
                                idList.Contains(k.userId.Value)
                            ) ||
                            (
                                k.userIds != null &&
                                k.userIds.Contains(
                                    "[" + USER.Id + "]")
                            )
                        )
                    );

                if (!string.IsNullOrWhiteSpace(title))
                {
                    query = query.Where(k =>
                        k.title.Contains(title));
                }

                return query
                    .OrderByDescending(k => k.incomingDate)
                    .ThenByDescending(k => k.Id)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public int GetNewNumber()
            {
                DateTime dt1 = DateTime.Today;
                DateTime dt2 = dt1.AddDays(1);

                return dal.Get(k =>
                        k.enabled &&
                        k.incomingDate >= dt1 &&
                        k.incomingDate < dt2)
                    .Count() + 1;
            }

            public PageReturn<IncomingDocumentDto> listByPageable(
                FilterPageParam<IncomingDocumentDtoRequest> filterPageParam,
                int userId)
            {
                PageReturn<IncomingDocumentDto> result =
                    new PageReturn<IncomingDocumentDto>();

                int pageSize =
                    filterPageParam.size ?? 20;

                int pageNumber =
                    filterPageParam.page ?? 0;

                if (pageSize <= 0)
                {
                    pageSize = 20;
                }

                if (pageNumber < 0)
                {
                    pageNumber = 0;
                }

                int? filterUserId =
                    filterPageParam.liste?.userId;

                int? filterSourceId =
                    filterPageParam.liste?.sourceId;

                bool? filterIsOutgoing =
                    filterPageParam.liste?.isOutgoing;

                DateTime? filterGirisTarihi =
                    filterPageParam.liste?.girisTarihi;

                string? filterTitle =
                    filterPageParam.liste?.title;

                int? filterDocumentOrder =
                    filterPageParam.liste?.documentOrder;

                BLLActions.AdminUsers bllAdminUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                AdminUser? currentUser =
                    bllAdminUsers.GetByID(userId);

                if (currentUser == null)
                {
                    throw new Exception("Kullanıcı bulunamadı.");
                }

                int userRoleId =
                    currentUser.roleId;

                BLLActions.RoleDetails bllRoleDetails =
                    new BLLActions.RoleDetails(
                        _configuration,
                        _env,
                        _mapper);

                RoleDetail? roleDetail =
                    bllRoleDetails
                        .GetByRoleIDAndModuleID(
                            userRoleId,
                            (int)CommonConstants.MODULES
                                .INCOMING_DOCUMENTS);

                IQueryable<IncomingDocument> query =
                    dal.Get(u => u.enabled);

                if (filterGirisTarihi.HasValue)
                {
                    DateTime date1 =
                        filterGirisTarihi.Value.Date;

                    DateTime date2 =
                        date1.AddDays(1);

                    query = query.Where(u =>
                        u.documentDate.HasValue &&
                        u.documentDate.Value >= date1 &&
                        u.documentDate.Value < date2);
                }

                if (filterSourceId.HasValue &&
                    filterSourceId.Value != 0)
                {
                    query = query.Where(u =>
                        u.sourceId ==
                        filterSourceId.Value);
                }

                if (filterIsOutgoing.HasValue)
                {
                    query = query.Where(u =>
                        u.isOutgoing ==
                        filterIsOutgoing.Value);
                }

                if (!string.IsNullOrWhiteSpace(
                        filterTitle))
                {
                    query = query.Where(u =>
                        u.title.Contains(filterTitle));
                }

                if (filterDocumentOrder.HasValue &&
                    filterDocumentOrder.Value != 0)
                {
                    query = query.Where(u =>
                        u.documentOrder ==
                        filterDocumentOrder.Value);
                }

                if (filterUserId.HasValue &&
                    filterUserId.Value != 0)
                {
                    query = query.Where(u =>
                        u.userId ==
                        filterUserId.Value);
                }

                bool canSeeAll =
                    userRoleId == 1 ||
                    (
                        roleDetail != null &&
                        roleDetail.canSeeLogs
                    );

                if (!canSeeAll)
                {
                    query = query.Where(u =>
                        u.createdUserId == userId);
                }

                result.totalElements =
                    query.Count();

                result.content = query
                    .OrderByDescending(u =>
                        u.documentNumber)
                    .ThenByDescending(u =>
                        u.Id)
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .Select(u => new IncomingDocumentDto()
                    {
                        companyName =
                            u.user != null &&
                            u.user.company != null
                                ? u.user.company.vkorg ?? ""
                                : "",

                        createdUserId =
                            u.createdUserId,

                        documentDate =
                            u.documentDate.HasValue
                                ? u.documentDate.Value
                                    .ToString("dd.MM.yyyy")
                                : "",

                        documentNumber =
                            u.documentNumber,

                        documentOrder =
                            u.documentOrder,

                        documentSpecialNumber =
                            u.documentSpecialNumber,

                        id =
                            u.Id,

                        incomingDate =
                            u.incomingDate
                                .ToString("dd.MM.yyyy"),

                        isCompleted =
                            u.isCompleted,

                        notes =
                            u.notes,

                        sourceTitle =
                            u.source != null
                                ? u.source.title
                                : "",

                        title =
                            u.title,

                        userName =
                            u.user != null
                                ? u.user.name
                                : "",

                        userTitle =
                            u.user != null
                                ? u.user.shortDescription
                                : ""
                    })
                    .ToList();

                result.number =
                    result.content.Count();

                result.size =
                    pageSize;

                return result;
            }

            public async Task<int> saveMyEdit(
                int id,
                string notes,
                bool isCompleted)
            {
                try
                {
                    IncomingDocument? incomingDocument =
                        dal.Get(u =>
                                u.enabled &&
                                u.Id == id)
                            .FirstOrDefault();

                    if (incomingDocument == null)
                    {
                        return 2;
                    }

                    incomingDocument.notes =
                        notes;

                    incomingDocument.isCompleted =
                        isCompleted;

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