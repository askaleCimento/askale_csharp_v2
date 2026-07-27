using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AskalePortal.BLL.BLLActions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Models = AskalePortal.Data.Models;


namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class HRExpenseTripTable : BaseBLL<AskalePortal.Data.Models.HRExpenseTripTable>
        {
            private IConfiguration _configuration; private IWebHostEnvironment _env; private readonly IMapper _mapper;
            public HRExpenseTripTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }


            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetUnapproved(int userId)
            {
                var q = dal.Get(u => (u.currentUserId == userId) && u.enabled == true && u.lastApproved == null && u.approval != false).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFinished(int userId)
            {
                var q = dal.Get(u => u.userId == userId && u.lastApproved == true && u.enabled == true).ToList();
                return q;
            }



            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetActive(int userId)
            {
                return dal.Get(u => (u.createdUserId == userId || u.userId == userId) && u.enabled == true && u.approval == null && u.lastApproved == null).ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetApprovedByUserId(int userId)
            {
                return dal.Get(u => u.approval == null && u.lastApproved == true && (u.userId == userId || u.createdUserId == userId) && u.enabled == true).ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetUnapprovedForSuperUser()
            {
                var q = dal.Get(u => u.enabled == true && u.lastApproved == null && u.approval != false).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFinishedForSuperUser()
            {
                var q = dal.Get(u => u.lastApproved == true && u.enabled == true).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetUnapprovedByTripId(int userId, int tripId)
            {
                var q = dal.Get(u => u.userId == userId && u.enabled == true && u.lastApproved == null).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFullFinished(int userId)
            {
                var q = dal.Get(u => u.userId == userId && u.enabled == true && u.lastApproved == true && u.approval == true).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFullFinished()
            {
                var q = dal.Get(u => u.enabled == true && u.lastApproved == true && u.approval == true).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFinishedForSuperUser(int activePage, int pageSize)
            {
                var q = dal.Get(u => u.lastApproved == true && u.enabled == true).OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFinished(int userId, int activePage, int pageSize)
            {
                var q = dal.Get(u => u.userId == userId && u.enabled == true && u.lastApproved == true && u.approval == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFinished(int userId, string name, string username, int activePage, int pageSize)
            {
                var q = dal.Get(u => u.userId == userId && u.user.name.Contains(name) && u.user.username.Contains(username) && u.enabled == true && u.currentStateId != 1).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFinishedForSuperUser(string name, string username, int activePage, int pageSize)
            {
                var q = dal.Get(u => u.user.name.Contains(name) && u.user.username.Contains(username) && u.currentStateId != 1 && u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFullFinished(string name, string username, int activePage, int pageSize)
            {
                var q = dal.Get(u => u.user.name.Contains(name) && u.user.username.Contains(username) && u.enabled == true && u.lastApproved == true && u.approval == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFullFinished(int userId, string name, string username, int activePage, int pageSize)
            {
                var q = dal.Get(u => u.userId == userId && u.user.name.Contains(name) && u.user.username.Contains(username) && u.enabled == true && u.lastApproved == true && u.approval == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetAllActiveByUser(int currentUserId, int userId)
            {
                return dal.Get(u => u.currentUserId == currentUserId && u.userId == userId && u.enabled == true && u.currentStateId == 1).ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFullFinished(string name, int? destinationLocationGidis,
                int? destinationLocationDonus, string gidisTarihi, string donusTarihi, string aciklama, int? expenseId, int activePage, int pageSize)
            {


                DateTime? gidisTarihiDate = null;
                if (!string.IsNullOrWhiteSpace(gidisTarihi))
                {
                    gidisTarihiDate = DateTime.ParseExact(gidisTarihi, "dd.MM.yyyy", null);
                }

                DateTime? donusTarihiDate = null;
                if (!string.IsNullOrWhiteSpace(donusTarihi))
                {
                    donusTarihiDate = DateTime.ParseExact(donusTarihi, "dd.MM.yyyy", null);
                }

                var q = dal.Get(u => ((u.HRExpenseTable.Count != 0)) && u.enabled == true && u.lastApproved == true && u.approval == true && (string.IsNullOrEmpty(name) ? true : u.user.name.ToLower().Contains(name.ToLower()))
                         && (destinationLocationDonus.HasValue ? u.destinationLocationId == destinationLocationDonus.Value : true)
                         && (destinationLocationGidis.HasValue ? u.hereLocationId == destinationLocationGidis.Value : true)
                         && (gidisTarihiDate.HasValue ? u.gidisTarihi == gidisTarihiDate.Value : true)
                         && (donusTarihiDate.HasValue ? u.donusTarihi == donusTarihiDate.Value : true)
                         && (string.IsNullOrEmpty(aciklama) ? true : u.tripDescription.ToLower().Contains(aciklama.ToLower())
                         ));


                if (expenseId.HasValue)
                {
                    var l = q.Where(u => u.HRExpenseTable.Any(y => expenseId.HasValue ? y.expenseTypeId == expenseId : true)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return l;
                }
                else
                {
                    var l = q.OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return l;
                }



            }
            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFullFinished(int userId, string name, int? destinationLocationGidis,
               int? destinationLocationDonus, string gidisTarihi, string donusTarihi, string aciklama, int? expenseId, int activePage, int pageSize)
            {
                DateTime? gidisTarihiDate = null;
                if (!string.IsNullOrWhiteSpace(gidisTarihi))
                {
                    gidisTarihiDate = DateTime.ParseExact(gidisTarihi, "dd.MM.yyyy", null);
                }

                DateTime? donusTarihiDate = null;
                if (!string.IsNullOrWhiteSpace(donusTarihi))
                {
                    donusTarihiDate = DateTime.ParseExact(donusTarihi, "dd.MM.yyyy", null);
                }

                var q = dal.Get(u => ((u.HRExpenseTable.Count != 0)) && (u.userId == userId)
                && (string.IsNullOrEmpty(name) ? true : u.user.name.Contains(name))
                && (destinationLocationDonus.HasValue ? u.hereLocationId == destinationLocationDonus.Value : true)
                && (destinationLocationGidis.HasValue ? u.destinationLocationId == destinationLocationGidis.Value : true)
                && (gidisTarihiDate.HasValue ? u.gidisTarihi == gidisTarihiDate.Value : true)
                && (donusTarihiDate.HasValue ? u.donusTarihi == donusTarihiDate.Value : true)
                && (string.IsNullOrEmpty(aciklama) ? true : u.tripDescription.ToLower().Contains(aciklama.ToLower()))
                && (u.enabled == true) && (u.lastApproved == true) && (u.approval == true));
                if (expenseId.HasValue)
                {
                    var l = q.Where(u => u.HRExpenseTable.Any(y => y.expenseType.HRExpenseAmount.Any(k => k.createdDate > y.spendingTime) && expenseId.HasValue ? y.expenseTypeId == expenseId : true)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return l;
                }
                else
                {
                    var l = q.OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return l;
                }

            }

            //public Models.HRExpenseTripTable GetByIDActive(int tripId)
            //{
            //    throw new NotImplementedException();
            //}

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFullActive(string name, int? destinationLocationGidis, int? destinationLocationDonus, string gidisTarihi, string donusTarihi, string aciklama, int? expenseId, int activePage, int pageSize)
            {
                DateTime? gidisTarihiDate = null;
                if (!string.IsNullOrWhiteSpace(gidisTarihi))
                {
                    gidisTarihiDate = DateTime.ParseExact(gidisTarihi, "dd.MM.yyyy", null);
                }

                DateTime? donusTarihiDate = null;
                if (!string.IsNullOrWhiteSpace(donusTarihi))
                {
                    donusTarihiDate = DateTime.ParseExact(donusTarihi, "dd.MM.yyyy", null);
                }

                var q = dal.Get(u => (u.HRExpenseTable.Where(y => y.currentStateId == 1 && y.enabled == true).Count() != 0) && (u.HRExpenseDetail.Where(y => y.enabled == true).Count() != 0) && (u.enabled == true) && (u.approval == null) && (u.lastApproved == true) && (string.IsNullOrEmpty(name) ? true : u.user.name.ToLower().Contains(name.ToLower()))
                         && (destinationLocationDonus.HasValue ? u.destinationLocationId == destinationLocationDonus.Value : true)
                         && (destinationLocationGidis.HasValue ? u.hereLocationId == destinationLocationGidis.Value : true)
                         && (gidisTarihiDate.HasValue ? u.gidisTarihi == gidisTarihiDate.Value : true)
                         && (donusTarihiDate.HasValue ? u.donusTarihi == donusTarihiDate.Value : true)


                         && (string.IsNullOrEmpty(aciklama) ? true : u.tripDescription.ToLower().Contains(aciklama.ToLower())
                         ));


                if (expenseId.HasValue)
                {
                    var l = q.Where(u => u.HRExpenseTable.Any(y => expenseId.HasValue ? y.expenseTypeId == expenseId : true)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return l;
                }
                else
                {
                    var l = q.OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return l;
                }


            }

            public List<AskalePortal.Data.Models.HRExpenseTripTable> GetFullActive(int userId, string name, int? destinationLocationGidis, int? destinationLocationDonus, string gidisTarihi, string donusTarihi, string aciklama, int? expenseId, int activePage, int pageSize)
            {
                DateTime? gidisTarihiDate = null;
                if (!string.IsNullOrWhiteSpace(gidisTarihi))
                {
                    gidisTarihiDate = DateTime.ParseExact(gidisTarihi, "dd.MM.yyyy", null);
                }

                DateTime? donusTarihiDate = null;
                if (!string.IsNullOrWhiteSpace(donusTarihi))
                {
                    donusTarihiDate = DateTime.ParseExact(donusTarihi, "dd.MM.yyyy", null);
                }

                var q = dal.Get(u => (u.HRExpenseTable.Where(y => y.currentStateId == 1 && y.enabled == true).Count() != 0) && (u.HRExpenseDetail.Where(y => y.enabled == true).Count() != 0) && (u.userId == userId) && (u.enabled == true) && (u.approval == null) && (u.lastApproved == true)
                && (string.IsNullOrEmpty(name) ? true : u.user.name.Contains(name))
                && (destinationLocationDonus.HasValue ? u.hereLocationId == destinationLocationDonus.Value : true)
                && (destinationLocationGidis.HasValue ? u.destinationLocationId == destinationLocationGidis.Value : true)
                && (gidisTarihiDate.HasValue ? u.gidisTarihi == gidisTarihiDate.Value : true)
                && (donusTarihiDate.HasValue ? u.donusTarihi == donusTarihiDate.Value : true)

                && (string.IsNullOrEmpty(aciklama) ? true : u.tripDescription.ToLower().Contains(aciklama.ToLower()))
                && (u.enabled == true) && (u.approval == null && u.lastApproved == true));
                if (expenseId.HasValue)
                {
                    var l = q.Where(u => u.HRExpenseTable.Any(y => y.expenseType.HRExpenseAmount.Any(k => k.createdDate > y.spendingTime) && expenseId.HasValue ? y.expenseTypeId == expenseId : true)).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return l;
                }
                else
                {
                    var l = q.OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
                    return l;
                }
            }

            public int approvalCount(int userId)
            {
                int deger = dal.Get(k => k.enabled == true && k.currentUserId == userId && k.currentStateId == 1 && k.lastApproved == null).Count();
                return deger;
            }

            public List<Models.HRExpenseTripTable> findAllByCurrentUserIdAndCurrentStateIdAndEnabledAndUserId(
               int? hremployer1, int? currentStateId, bool enabled, int? id)
            {
                List<Models.HRExpenseTripTable> liste = dal.Get(u => u.currentUserId == hremployer1 && u.currentStateId == currentStateId && u.enabled == enabled && u.userId == id).ToList();
                return liste;
            }

            public async Task<Models.HRExpenseTripTable?> save(HRExpenseTripTableSaveDto entity, int userId)
            {
                try
                {

                    if (entity.id == 0)
                    {
                        entity.createdUserId = (userId);
                        entity.enabled = (true);
                        entity.createdDate = (DateTime.Now.ToString());
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        Data.Models.AdminUser? user = bllAdminUsers.GetByID(entity.userId ?? 0);

                        Data.Models.HRExpenseTripTable? expenseTripTable = await Add(_mapper.Map<Data.Models.HRExpenseTripTable>(entity));
                        if (!entity.currentUserId.Equals(entity.userId))
                        {
                            BLLActions.HRExpenseTripDetail bllHRExpenseTripDetail = new BLLActions.HRExpenseTripDetail(_configuration, _env);
                            Data.Models.HRExpenseTripDetail hrExpenseTripDetailNext = new Data.Models.HRExpenseTripDetail();
                            hrExpenseTripDetailNext.approved = (null);
                            hrExpenseTripDetailNext.userId = (user?.manager1 ?? 0);
                            hrExpenseTripDetailNext.createdDate = (DateTime.Now);
                            hrExpenseTripDetailNext.guid = Guid.NewGuid();
                            hrExpenseTripDetailNext.isReplied = (false);
                            hrExpenseTripDetailNext.replyDate = (null);
                            hrExpenseTripDetailNext.tripId = (expenseTripTable!.Id);
                            hrExpenseTripDetailNext.enabled = (true);
                            await bllHRExpenseTripDetail.Add(hrExpenseTripDetailNext);
                            Data.Models.AdminUser? manager1 = bllAdminUsers.GetByID(user?.manager1 ?? 0);

                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            EmailMessage emailMessage = new EmailMessage();
                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager1?.name +
                             "Bekleyen Seyahat Onayı hk.",
                                       expenseTripTable.Id.ToString() + " ID kodlu Seyahat onayınızı beklemektedir.");
                            emailMessage.subject = ("Bekleyen Seyahat Onayı hk.");
                            emailMessage.toAddress = (manager1?.email);
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = (DateTime.Now);
                            await bllEmailMessages.Add(emailMessage);

                        }

                        return expenseTripTable;
                        //			}

                    }
                    else
                    {
                        entity.updatedUserId = (userId);
                        entity.updateDate = (DateTime.Now.ToString());

                        return await Update(_mapper.Map<Data.Models.HRExpenseTripTable>(entity));
                    }
                }
                catch (Exception e)
                {
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    UserByNameEMailDto byNameEMailDto = bllAdminUsers.getUserByNameAndEmail(userId);
                    Console.WriteLine(byNameEMailDto.name + "," + " Seyahat oluşturamıyor. Hata: " + e.Message);
                    return new Data.Models.HRExpenseTripTable();
                }
            }

            public PageReturn<HRExpenseTripTableSaveDto> listByUserIdActive(FilterPageParam<HRExpenseTripTableActiveListDtoParameter> filterPageParam)
            {

                PageReturn<HRExpenseTripTableSaveDto>? result = new PageReturn<HRExpenseTripTableSaveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                DateTime? gidisTarihi = filterPageParam.liste?.filterGidisTarihi;
                DateTime? donusTarihi = filterPageParam.liste?.filterDonusTarihi;
                int? userId = filterPageParam.liste?.filterUser;
                int? filterDestination = filterPageParam.liste?.filterDestination;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                Models.AdminUser? user = bllAdminUsers.GetByID(userId ?? 0);


                IQueryable<Models.HRExpenseTripTable> query = dal.Get(u =>
                (u.enabled && u.approval == null && u.lastApproved == null && u.currentStateId == 1) &&
                (user!.roleId  == 1 ? true : (u.createdUserId == userId || u.userId == userId)) &&
                (filterDestination == null || filterDestination == 0 ? true : u.destinationLocationId == filterDestination) &&
                (gidisTarihi == null ? true : u.gidisTarihi == gidisTarihi) &&
                (donusTarihi == null ? true : u.donusTarihi == donusTarihi)
                ).OrderByDescending(u => u.Id);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new Data.ResponseModels.HRExpenseTripTableSaveDto()
                    {
                        disaprovecondition = u.disaprovecondition,
                        gidisTarihi = u.gidisTarihi.ToString(),
                        destinationLocationId = u.destinationLocationId,
                        hereLocationId = u.hereLocationId,
                        approval = u.approval,
                        avans = u.avans,
                        createdDate = u.createdDate.ToString(),
                        createdUserId = u.createdUserId,
                        currentStateId = u.currentStateId,
                        currentUserId = u.currentUserId,
                        digerDestination = u.digerDestination,
                        donusTarihi = u.donusTarihi.ToString(),
                        enabled = u.enabled,
                        id = u.Id,
                        lastApproved = u.lastApproved,
                        onaySirasi = u.onaySirasi,
                        tripDescription = u.tripDescription,
                        tripDescriptionId = u.tripDescriptionId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,
                        userId = u.userId,
                        vekaletId = u.vekaletId,

                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;


            }

            public PageReturn<HRExpenseTripTableSaveDto> listByUserIdMyList(FilterPageParam<HRExpenseTripTableMyListDtoParameter> filterPageParam)
            {
                PageReturn<HRExpenseTripTableSaveDto>? result = new PageReturn<HRExpenseTripTableSaveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                DateTime? gidisTarihi = filterPageParam.liste?.filterGidisTarihi;
                DateTime? donusTarihi = filterPageParam.liste?.filterDonusTarihi;
                int? userId = filterPageParam.liste?.filterUser;
                int? filterDestination = filterPageParam.liste?.filterDestination;
                int? filterWhereYouAre = filterPageParam.liste?.filterWhereYouAre;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                Models.AdminUser? user = bllAdminUsers.GetByID(userId ?? 0);


                IQueryable<Models.HRExpenseTripTable> query = dal.Get(u =>
                (u.enabled && u.currentUserId == userId && u.approval == null && u.lastApproved == null && u.currentStateId == 1) &&

                (filterWhereYouAre == null || filterWhereYouAre == 0 ? true : u.hereLocationId == filterWhereYouAre) &&
                (filterDestination == null || filterDestination == 0 ? true : u.destinationLocationId == filterDestination) &&
                (gidisTarihi == null ? true : u.gidisTarihi == gidisTarihi) &&
                (donusTarihi == null ? true : u.donusTarihi == donusTarihi)
                ).OrderByDescending(u => u.Id);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new Data.ResponseModels.HRExpenseTripTableSaveDto()
                    {
                        disaprovecondition = u.disaprovecondition,
                        gidisTarihi = u.gidisTarihi.ToString(),
                        destinationLocationId = u.destinationLocationId,
                        hereLocationId = u.hereLocationId,
                        approval = u.approval,
                        avans = u.avans,
                        createdDate = u.createdDate.ToString(),
                        createdUserId = u.createdUserId,
                        currentStateId = u.currentStateId,
                        currentUserId = u.currentUserId,
                        digerDestination = u.digerDestination,
                        donusTarihi = u.donusTarihi.ToString(),
                        enabled = u.enabled,
                        id = u.Id,
                        lastApproved = u.lastApproved,
                        onaySirasi = u.onaySirasi,
                        tripDescription = u.tripDescription,
                        tripDescriptionId = u.tripDescriptionId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,
                        userId = u.userId,
                        vekaletId = u.vekaletId,

                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;

            }

            public async Task<int> approve(int userId, int tripId, bool approved)
            {
                try
                {

                    Data.Models.HRExpenseTripTable? hrExpenseTripTable = GetByID(tripId);

                    BLLActions.HRExpenseTripDetail bllHRExpenseTripDetail = new BLLActions.HRExpenseTripDetail(_configuration, _env);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser user = bllAdminUsers.GetByID(hrExpenseTripTable!.userId)!;
                    AdminUser manager1 = bllAdminUsers.GetByID(user.manager1 ?? 0)!;
                    if (hrExpenseTripTable.currentUserId.Equals(hrExpenseTripTable.userId))
                    {
                        if (approved)
                        {

                            hrExpenseTripTable.onaySirasi = (1);
                            hrExpenseTripTable.currentUserId = (user.manager1 ?? 0);
                            await Update(hrExpenseTripTable);

                            Models.HRExpenseTripDetail hrExpenseTripDetailNext = new Models.HRExpenseTripDetail();
                            hrExpenseTripDetailNext.approved = (null);
                            hrExpenseTripDetailNext.userId = (user.manager1 ?? 0);
                            hrExpenseTripDetailNext.createdDate = (DateTime.Now);
                            hrExpenseTripDetailNext.guid = Guid.NewGuid();
                            hrExpenseTripDetailNext.isReplied = (true);
                            hrExpenseTripDetailNext.replyDate = (null);
                            hrExpenseTripDetailNext.tripId = (tripId);
                            hrExpenseTripDetailNext.enabled = (true);
                            await bllHRExpenseTripDetail.Add(hrExpenseTripDetailNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Seyahat Onayı hk.");
                            emailMessage.toAddress = (manager1.email);


                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager1.name +
                            "Bekleyen Seyahat Onayı hk.",
                                     tripId.ToString() + " ID kodlu Seyahat onayınızı beklemektedir.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = (DateTime.Now);
                            await bllEmailMessages.Add(emailMessage);
                            return 1;

                        }
                        else if (!approved)
                        {

                            hrExpenseTripTable.onaySirasi = (10);
                            hrExpenseTripTable.lastApproved = (false);
                            hrExpenseTripTable.currentStateId = (2);
                            await Update(hrExpenseTripTable);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Seyahat Onayı hk.");
                            emailMessage.toAddress = (user.email);

                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user.name +
                           "RED Seyahat Onayı hk.",
                                    tripId.ToString() + " ID kodlu Seyahatiniz red edilmiştir.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = (DateTime.Now);
                            await bllEmailMessages.Add(emailMessage);
                            return 1;

                        }
                    }
                    else if (hrExpenseTripTable.currentUserId.Equals(user.manager1))
                    {

                        if (approved)
                        {
                            if (hrExpenseTripTable.avans.CompareTo(0) > 0)
                            {
                                AdminUser? employer1 = bllAdminUsers.GetByID(user.hremployer1 ?? 0);
                                if (!Equals(user.manager1, user.hremployer1))
                                {
                                    Data.Models.HRExpenseTripDetail hrExpenseTripDetail = bllHRExpenseTripDetail.getByActive(tripId,
                                            userId);
                                    hrExpenseTripDetail.approved = (approved);
                                    hrExpenseTripDetail.replyDate = (DateTime.Now);
                                    hrExpenseTripDetail.isReplied = (true);
                                    await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);

                                    hrExpenseTripTable.onaySirasi = (1);
                                    hrExpenseTripTable.currentUserId = (user.hremployer1 ?? 0);
                                    await Update(hrExpenseTripTable);

                                    Models.HRExpenseTripDetail hrExpenseTripDetailNext = new Models.HRExpenseTripDetail();
                                    hrExpenseTripDetailNext.approved = (null);
                                    hrExpenseTripDetailNext.userId = (user.hremployer1 ?? 0);
                                    hrExpenseTripDetailNext.createdDate = (DateTime.Now);
                                    hrExpenseTripDetailNext.guid = Guid.NewGuid();
                                    hrExpenseTripDetailNext.isReplied = (true);
                                    hrExpenseTripDetailNext.replyDate = (null);
                                    hrExpenseTripDetailNext.tripId = (tripId);
                                    hrExpenseTripDetailNext.enabled = (true);
                                    await bllHRExpenseTripDetail.Add(hrExpenseTripDetailNext);

                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject = ("Bekleyen Seyahat Onayı hk.");
                                    emailMessage.toAddress = (employer1?.email);


                                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + employer1?.name +
                                   "Seyahat Onayı hk.",
                                            tripId.ToString() + " ID kodlu Seyahat onayınızı beklemektedir.");
                                    emailMessage.emailText = (mailMessage);
                                    emailMessage.mailTuru = (3);
                                    emailMessage.enabled = (true);
                                    emailMessage.isSent = (false);
                                    emailMessage.plannedDate = (DateTime.Now);
                                    await bllEmailMessages.Add(emailMessage);
                                    return 1;
                                }
                                else
                                {
                                    Models.HRExpenseTripDetail hrExpenseTripDetail = bllHRExpenseTripDetail.getByActive(tripId,
                                            userId);
                                    hrExpenseTripDetail.approved = (approved);
                                    hrExpenseTripDetail.replyDate = (DateTime.Now);
                                    hrExpenseTripDetail.isReplied = (true);
                                    await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);

                                    hrExpenseTripTable.onaySirasi = (10);
                                    hrExpenseTripTable.lastApproved = (true);
                                    hrExpenseTripTable.currentStateId = (4);

                                    await Update(hrExpenseTripTable);

                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject = ("Bekleyen Seyahat Onayı hk.");
                                    emailMessage.toAddress = (user.email);

                                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user.name +
                                   "Seyahat Onayı hk.",
                                            tripId.ToString() + " ID kodlu Seyahatiniz onaylanmıştır.");
                                    emailMessage.emailText = (mailMessage);
                                    emailMessage.mailTuru = (3);
                                    emailMessage.enabled = (true);
                                    emailMessage.isSent = (false);
                                    emailMessage.plannedDate = (DateTime.Now);
                                    await bllEmailMessages.Add(emailMessage);

                                    return 1;
                                }

                            }
                            else
                            {
                                Models.HRExpenseTripDetail hrExpenseTripDetail = bllHRExpenseTripDetail.getByActive(tripId,
                                        userId);
                                hrExpenseTripDetail.approved = (approved);
                                hrExpenseTripDetail.replyDate = (DateTime.Now);
                                hrExpenseTripDetail.isReplied = (true);
                                await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);

                                hrExpenseTripTable.onaySirasi = (10);
                                hrExpenseTripTable.lastApproved = (true);
                                hrExpenseTripTable.currentStateId = (4);

                                await Update(hrExpenseTripTable);

                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject = ("Bekleyen Seyahat Onayı hk.");
                                emailMessage.toAddress = (user.email);



                                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user.name +
                                   "Seyahat Onayı hk.",
                                            tripId.ToString() + " ID kodlu Seyahatiniz onaylanmıştır.");
                                emailMessage.emailText = (mailMessage);
                                emailMessage.mailTuru = (3);
                                emailMessage.enabled = (true);
                                emailMessage.isSent = (false);
                                emailMessage.plannedDate = (DateTime.Now);
                                await bllEmailMessages.Add(emailMessage);

                                return 1;
                            }
                        }
                        else if (!approved)
                        {

                            Models.HRExpenseTripDetail hrExpenseTripDetail = bllHRExpenseTripDetail.getByActive(tripId, userId);
                            hrExpenseTripDetail.approved = (approved);
                            hrExpenseTripDetail.replyDate = (DateTime.Now);
                            hrExpenseTripDetail.isReplied = (true);
                            await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);

                            hrExpenseTripTable.onaySirasi = (10);
                            hrExpenseTripTable.lastApproved = (false);
                            hrExpenseTripTable.currentStateId = (2);
                            await Update(hrExpenseTripTable);

                            Models.EmailMessage emailMessage = new Models.EmailMessage();
                            emailMessage.subject = ("Bekleyen Seyahat Onayı hk.");
                            emailMessage.toAddress = (user.email);



                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user.name +
                               "RED Seyahat Onayı hk.",
                                        tripId.ToString() + " ID kodlu Seyahatiniz red edilmiştir.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = (DateTime.Now);
                            await bllEmailMessages.Add(emailMessage);
                            return 1;
                        }
                        return 1;

                    }
                    else if (hrExpenseTripTable.currentUserId.Equals(user.hremployer1))
                    {

                        if (approved)
                        {
                            Models.HRExpenseTripDetail hrExpenseTripDetail = bllHRExpenseTripDetail.getByActive(tripId, userId);
                            hrExpenseTripDetail.approved = (approved);
                            hrExpenseTripDetail.replyDate = (DateTime.Now);
                            hrExpenseTripDetail.isReplied = (true);
                            await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);

                            //hrExpenseTripTable.onaySirasi=(1);
                            //hrExpenseTripTable.currentUserId=(user.manager1??0);
                            //await Update(hrExpenseTripTable);

                            hrExpenseTripTable.onaySirasi = (10);
                            hrExpenseTripTable.lastApproved = (true);
                            hrExpenseTripTable.currentStateId = (4);

                            await Update(hrExpenseTripTable);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Seyahat Onayı hk.");
                            emailMessage.toAddress = (user.email);



                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user.name +
                               "Seyahat Onayı hk.",
                                        tripId.ToString() + " ID kodlu Seyahatiniz onaylanmıştır.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = (DateTime.Now);
                            await bllEmailMessages.Add(emailMessage);

                            if (hrExpenseTripTable.avans.CompareTo(0) > 0)
                            {
                                BLLActions.FinansUserTable bllFinansUserTable = new BLLActions.FinansUserTable(_configuration, _env);
                                List<Models.FinansUserTable> listFinansUsers = bllFinansUserTable
                                        .GetByCompanyId(user.companyId);
                                foreach (Models.FinansUserTable finansUserTable in listFinansUsers)
                                {
                                    UserByNameEMailDto userdto = bllAdminUsers
                                            .getUserByNameAndEmail(finansUserTable.userId);

                                    EmailMessage emailMessageAvans = new EmailMessage();
                                    emailMessageAvans.subject = ("Onaylanan Avans Onayı hk.");
                                    emailMessageAvans.toAddress = (userdto.email);



                                    string mailMessageAvans = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user.name +
                              "Avans Onayı", tripId.ToString() + " ID kodlu Seyahatin, " + hrExpenseTripTable.avans.ToString() + " avans tutarı onaylanmıştır.");
                                    emailMessageAvans.emailText = (mailMessageAvans);
                                    emailMessageAvans.mailTuru = (3);
                                    emailMessageAvans.enabled = (true);
                                    emailMessageAvans.isSent = (false);
                                    emailMessageAvans.plannedDate = (DateTime.Now);
                                    await bllEmailMessages.Add(emailMessageAvans);
                                }
                            }

                            return 1;

                        }
                        else if (!approved)
                        {
                            Models.HRExpenseTripDetail hrExpenseTripDetail = bllHRExpenseTripDetail.getByActive(tripId, userId);
                            hrExpenseTripDetail.approved = (approved);
                            hrExpenseTripDetail.replyDate = (DateTime.Now);
                            hrExpenseTripDetail.isReplied = (true);
                            await bllHRExpenseTripDetail.Update(hrExpenseTripDetail);

                            hrExpenseTripTable.onaySirasi = (10);
                            hrExpenseTripTable.lastApproved = (false);
                            hrExpenseTripTable.currentStateId = (2);
                            await Update(hrExpenseTripTable);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Seyahat Onayı hk.");
                            emailMessage.toAddress = (user.email);

                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user.name +
                              "RED Seyahat Onayı hk.", tripId.ToString() + " ID kodlu Seyahatiniz red edilmiştir.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = (DateTime.Now);
                            await bllEmailMessages.Add(emailMessage);
                            return 1;

                        }
                        return 1;

                    }
                    return -1;

                }
                catch (Exception )
                {



                    return -1;
                }
            }

            public PageReturn<HRExpenseTripTableSaveDto> listCompleted(FilterPageParam<HRExpenseTripTableCompletedListDtoParameter> filterPageParam)
            {
                PageReturn<HRExpenseTripTableSaveDto>? result = new PageReturn<HRExpenseTripTableSaveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? filterName = filterPageParam.liste?.filterName;
                string? filterUserName = filterPageParam.liste?.filterUserName;
                int? userId = filterPageParam.liste?.filterUser;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                Models.AdminUser? currentUser = bllAdminUsers.GetByID(userId ?? 0);

                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                Models.RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(currentUser!.roleId, (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);
                IQueryable<Models.HRExpenseTripTable> query;
                if (currentUser.roleId == 1)
                {
                    query = dal.Get(u =>
               (u.enabled && u.currentStateId != 1) &&
               filterName != null ? u.user.name.Contains(filterName) : true &&
               filterUserName != null ? u.user.username.Contains(filterUserName) : true
               ).OrderByDescending(u => u.Id);
                }
                else if (roleDetail != null && roleDetail.canSeeLogs)
                {
                    BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);
                    Models.Role role = bllRoles.GetByID(currentUser.roleId)!;
                    string[] listCompanyIds = role.companies.Replace("[", "").Replace("]", "").Split(",");
                    List<int> listCompanyIdsint = new List<int>();
                    foreach (string id in listCompanyIds)
                    {
                        BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                        Company company = bllCompanies.getByVkorgCompany(id);
                        listCompanyIdsint.Add(company.Id);
                    }

                    query = dal.Get(u =>
               (u.enabled && u.currentStateId != 1) &&
               filterName != null ? u.user.name.Contains(filterName) : true &&
               filterUserName != null ? u.user.username.Contains(filterUserName) : true &&
               listCompanyIdsint.Contains(u.user.companyId)

               ).OrderByDescending(u => u.Id);

                }
                else
                {
                    query = dal.Get(u =>
              (u.enabled && u.currentStateId != 1) &&
              (u.createdUserId == userId || u.userId == userId) &&
              filterName != null ? u.user.name.Contains(filterName) : true &&
              filterUserName != null ? u.user.username.Contains(filterUserName) : true
              ).OrderByDescending(u => u.Id);
                }

                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new Data.ResponseModels.HRExpenseTripTableSaveDto()
                    {
                        disaprovecondition = u.disaprovecondition,
                        gidisTarihi = u.gidisTarihi.ToString(),
                        destinationLocationId = u.destinationLocationId,
                        hereLocationId = u.hereLocationId,
                        approval = u.approval,
                        avans = u.avans,
                        createdDate = u.createdDate.ToString(),
                        createdUserId = u.createdUserId,
                        currentStateId = u.currentStateId,
                        currentUserId = u.currentUserId,
                        digerDestination = u.digerDestination,
                        donusTarihi = u.donusTarihi.ToString(),
                        enabled = u.enabled,
                        id = u.Id,
                        lastApproved = u.lastApproved,
                        onaySirasi = u.onaySirasi,
                        tripDescription = u.tripDescription,
                        tripDescriptionId = u.tripDescriptionId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,
                        userId = u.userId,
                        vekaletId = u.vekaletId,

                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;

            }

            public List<Models.HRExpenseTripTable> getFinishedForExpense(int userId)
            {
                List<Models.HRExpenseTripTable>? liste = dal.Get(u => u.userId == userId && u.approval == null && u.lastApproved == true && u.enabled).ToList();
                return liste ?? [];
            }

            public PageReturn<HRExpenseTripDto> myActiveExpense(FilterPageParam<HRExpenseTripTableMyListDtoParameter> filterPageParam)
            {
                PageReturn<HRExpenseTripDto>? result = new PageReturn<HRExpenseTripDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? userId = filterPageParam.liste?.userId;

                var query =
    (from trip in dal.dB.HRExpenseTripTable
     join expense in dal.dB.HRExpenseTable
         on trip.Id equals expense.tripId
     where trip.enabled
           && trip.approval == null
           //&& trip.lastApproved == null
           && trip.currentStateId == 4
           && expense.currentStateId == 1
           && expense.currentUserId == userId && expense.enabled
     select new HRExpenseTripDto
     {
         id = trip.Id,
         userId = trip.userId,
         description = trip.tripDescription,
         destination = trip.hereLocation.destinationLocation,
         donusTarihi = (trip.donusTarihi ?? DateTime.Now).ToString("dd.MM.yyyy"),
         gidisTarihi = (trip.gidisTarihi ?? DateTime.Now).ToString("dd.MM.yyyy"),
         kisi = trip.user.name,
         whereareyou = trip.destinationLocation.destinationLocation
     })
    .Distinct()
    .OrderByDescending(t => t.id);
                result.totalElements = query.Count();

                result.content = query
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .ToList();

                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public PageReturn<HRExpenseTripTableSaveDto> listCompletedExpense(FilterPageParam<HRExpenseTripTableCompletedListDtoParameter> filterPageParam)
            {
                PageReturn<HRExpenseTripTableSaveDto>? result = new PageReturn<HRExpenseTripTableSaveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int userId = filterPageParam.liste?.filterUser ?? 0;
                string? filtername = filterPageParam.liste?.filterName;
                string? filterusername = filterPageParam.liste?.filterUserName;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? currentUser = bllAdminUsers.GetByID(userId);
               
                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(currentUser!.roleId, (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);
                if (currentUser.roleId == 1)
                {
                  var  query = dal.Get(u => (u.enabled && u.currentStateId != 1 && u.approval == true)
                     && (string.IsNullOrEmpty(filtername) ? true : u.user.name.Contains(filtername))
                      && (string.IsNullOrEmpty(filterusername) ? true : u.user.username.Contains(filterusername))
                    ).ToList().OrderByDescending(u=>u.Id).Select(u=> new HRExpenseTripTableSaveDto()
                    {
                        userId=u.userId,
                        createdUserId=u.createdUserId,
                        approval=u.approval,
                        avans=u.avans,
                        createdDate=u.createdDate.ToString(),
                        currentStateId=u.currentStateId,
                        currentUserId=u.currentUserId,
                        destinationLocationId=u.destinationLocationId,
                        digerDestination=u.digerDestination,
                        disaprovecondition = u.disaprovecondition,
                        donusTarihi = u.donusTarihi.ToString(),
                        enabled = u.enabled,
                        gidisTarihi = u.gidisTarihi.ToString(),
                        hereLocationId=u.hereLocationId,
                        id=u.Id,
                        lastApproved = u.lastApproved,
                        onaySirasi=u.onaySirasi,
                        tripDescription = u.tripDescription,
                        tripDescriptionId = u.tripDescriptionId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,
                        vekaletId = u.vekaletId
                        
                    });

                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = result.content.Count();
                    result.size = pageSize;

                    return result;
                }
                else if (roleDetail != null && roleDetail.canSeeLogs)
                {
                    BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);
                    Role? role = bllRoles.GetByID(currentUser.roleId);
                    string[] listCompanyIds = role?.companies.Replace("[", "").Replace("]", "").Split(",") ?? [];
                    List<int> listCompanyIdsint = new List<int>();
                    foreach (string item in listCompanyIds)
                    {
                        BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                        Company company = bllCompanies.getByVkorgCompany(item);
                        listCompanyIdsint.Add(company.Id);
                    }
                  var  query = dal.Get(u => (u.enabled && u.currentStateId != 1 && u.approval == true)
                    && (string.IsNullOrEmpty(filtername) ? true : u.user.name.Contains(filtername))
                      && (string.IsNullOrEmpty(filterusername) ? true : u.user.username.Contains(filterusername))
                      && listCompanyIdsint.Contains(u.user.companyId)
                    ).ToList().OrderByDescending(u => u.Id).Select(u => new HRExpenseTripTableSaveDto()
                    {
                        userId = u.userId,
                        createdUserId = u.createdUserId,
                        approval = u.approval,
                        avans = u.avans,
                        createdDate = u.createdDate.ToString(),
                        currentStateId = u.currentStateId,
                        currentUserId = u.currentUserId,
                        destinationLocationId = u.destinationLocationId,
                        digerDestination = u.digerDestination,
                        disaprovecondition = u.disaprovecondition,
                        donusTarihi = u.donusTarihi.ToString(),
                        enabled = u.enabled,
                        gidisTarihi = u.gidisTarihi.ToString(),
                        hereLocationId = u.hereLocationId,
                        id = u.Id,
                        lastApproved = u.lastApproved,
                        onaySirasi = u.onaySirasi,
                        tripDescription = u.tripDescription,
                        tripDescriptionId = u.tripDescriptionId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,
                        vekaletId = u.vekaletId

                    }); ;

                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = result.content.Count();
                    result.size = pageSize;

                    return result;

                }
                else
                {
                  var  query =dal.Get(u => (u.enabled && u.currentStateId != 1 && u.approval == true)
                    && (u.createdUserId == userId || u.userId == userId)
                     && (string.IsNullOrEmpty(filtername) ? true : u.user.name.Contains(filtername))
                      && (string.IsNullOrEmpty(filterusername) ? true : u.user.username.Contains(filterusername))
                    ).ToList().OrderByDescending(u => u.Id).Select(u => new HRExpenseTripTableSaveDto()
                    {
                        userId = u.userId,
                        createdUserId = u.createdUserId,
                        approval = u.approval,
                        avans = u.avans,
                        createdDate = u.createdDate.ToString(),
                        currentStateId = u.currentStateId,
                        currentUserId = u.currentUserId,
                        destinationLocationId = u.destinationLocationId,
                        digerDestination = u.digerDestination,
                        disaprovecondition = u.disaprovecondition,
                        donusTarihi = u.donusTarihi.ToString(),
                        enabled = u.enabled,
                        gidisTarihi = u.gidisTarihi.ToString(),
                        hereLocationId = u.hereLocationId,
                        id = u.Id,
                        lastApproved = u.lastApproved,
                        onaySirasi = u.onaySirasi,
                        tripDescription = u.tripDescription,
                        tripDescriptionId = u.tripDescriptionId,
                        updateDate = u.updatedDate.ToString(),
                        updatedUserId = u.updatedUserId,
                        vekaletId = u.vekaletId

                    }); ;

                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = result.content.Count();
                    result.size = pageSize;

                    return result;
                }

            }

            public List<HRExpenseTripTableSaveDto> listAllByEnabled(bool enabled)
            {
                List<HRExpenseTripTableSaveDto> liste = dal.Get(u => u.enabled == enabled).Select(u => new HRExpenseTripTableSaveDto
                {
                    enabled = u.enabled,
                    updatedUserId = u.updatedUserId,
                    approval = u.approval,
                    avans=u.avans,
                    createdDate=u.createdDate.ToString(),
                    createdUserId=u.createdUserId,
                    currentStateId=u.currentStateId,
                    currentUserId=u.currentUserId,
                    destinationLocationId=u.destinationLocationId,
                    digerDestination=u.digerDestination,
                    disaprovecondition=u.disaprovecondition,
                    donusTarihi = u.donusTarihi.ToString(),
                    gidisTarihi = u.gidisTarihi.ToString(),
                    hereLocationId = u.hereLocationId,
                    id = u.Id,
                    lastApproved = u.lastApproved,
                    onaySirasi = u.onaySirasi,
                    tripDescription = u.tripDescription,
                    tripDescriptionId = u.tripDescriptionId,
                    updateDate = u.updatedDate.ToString(),
                    userId = u.userId,
                    vekaletId = u.vekaletId
                }).ToList();
                return liste;
            }

            public PageReturn<HRExpenseTripDto> mylistAprovalStatus(FilterPageParam<HRExpenseTableApprovalStatusDtoParameter> filterPageParam)
            {
                PageReturn<HRExpenseTripDto>? result = new PageReturn<HRExpenseTripDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                DateTime? gidisTarihi = filterPageParam.liste?.filterGidisTarihi;
                DateTime? donusTarihi = filterPageParam.liste?.filterDonusTarihi;
                int? userId = filterPageParam.liste?.userId;
                int? gidisYeriId = filterPageParam.liste?.filterGidisYeriId;
                int? donusYeriId = filterPageParam.liste?.filterDonusYeriId;
                var query = dal.Get(u =>
                       u.enabled &&
                       u.userId == userId &&
                       (gidisYeriId == null || u.hereLocationId == gidisYeriId) &&
                       (donusYeriId == null || u.destinationLocationId == donusYeriId) &&
                       (gidisTarihi == null || u.gidisTarihi == gidisTarihi) &&
                       (donusTarihi == null || u.donusTarihi == donusTarihi)
                    )
                    .Include(u => u.user)
                    .Include(u => u.hereLocation)
                    .Include(u => u.destinationLocation)
                    .OrderByDescending(u => u.Id)
                    .Select(u => new HRExpenseTripDto()
                    {
                        gidisTarihi = u.gidisTarihi.HasValue
                            ? u.gidisTarihi.Value.ToString("dd.MM.yyyy")
                            : "",

                        donusTarihi = u.donusTarihi.HasValue
                            ? u.donusTarihi.Value.ToString("dd.MM.yyyy")
                            : "",

                        userId = u.userId,
                        description = u.tripDescription,

                        destination = u.destinationLocation != null
                            ? u.destinationLocation.destinationLocation
                            : "",

                        kisi = u.user != null
                            ? u.user.name
                            : "",

                        whereareyou = u.hereLocation != null
                            ? u.hereLocation.destinationLocation
                            : "",

                        id = u.Id
                    })
                    .ToList();

                result.totalElements = query.Count();

                result.content = query
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .ToList();

                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }
        }
    }

}
