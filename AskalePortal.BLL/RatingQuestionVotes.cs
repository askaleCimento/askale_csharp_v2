using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
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
        public class RatingQuestionVotes : BaseBLL<AskalePortal.Data.Models.RatingQuestionVote>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;   
            public RatingQuestionVotes(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public List<AskalePortal.Data.Models.RatingQuestionVote> GetByUserID(int userID)
            {
                var q = dal.Get(k => k.userId == userID && k.ratingValue > 0 && k.enabled == true).OrderBy(k => k.questionId);

                return q.ToList();
            }

            public AskalePortal.Data.Models.RatingQuestionVote GetByQuestionID(int questionID, int userID)
            {
                var q = dal.Get(k => k.questionId == questionID && k.ratingValue > 0 && k.userId == userID && k.enabled == true).OrderBy(k => k.createdDate);

                return q.FirstOrDefault() ?? new AskalePortal.Data.Models.RatingQuestionVote();
            }

            public List<AskalePortal.Data.Models.RatingQuestionVote> GetByQuestionID(int questionID)
            {
                var q = dal.Get(k => k.questionId == questionID && k.ratingValue > 0 && k.enabled == true).OrderBy(k => k.createdDate);

                return q.ToList();
            }

            public int GetTotalCount(int ratingID)
            {
                var q = dal.Get(k => k.ratingId == ratingID && k.ratingValue>0 && k.enabled == true).GroupBy(g=> g.userId).Count();
                return q;
            }
            public int GetTotalCount(int ratingID,int companyId)
            {
                var q = dal.Get(k => k.ratingId == ratingID && k.ratingValue>0 && k.user.companyId == companyId  && k.enabled == true).GroupBy(g => g.userId).Count();
                return q;
            }

            public decimal GetTotalVoteValueCount(int questionID)
            {
                try
                {
                    var q = dal.Get(k => k.questionId == questionID && k.ratingValue>0 && k.enabled == true).Sum(s => s.ratingValue);
                    var q2 = dal.Get(k => k.questionId == questionID && k.ratingValue>0 && k.enabled == true).GroupBy(g => g.userId).Count();
                    return (decimal)q / (decimal)q2;
                }
                catch
                {
                    return 0m;
                }
            }
            public decimal GetTotalVoteValueCount(int questionID,int companyId)
            {
                try
                {
                    var q = dal.Get(k => k.questionId == questionID && k.ratingValue>0 && k.user.companyId==companyId && k.enabled == true).Sum(s => s.ratingValue);
                    var q2 = dal.Get(k => k.questionId == questionID && k.ratingValue>0 && k.user.companyId == companyId && k.enabled == true).GroupBy(g => g.userId).Count();
                    return (decimal)q / (decimal)q2;
                }
                catch
                {
                    return 0m;
                }
            }

            public List<AskalePortal.Data.Models.RatingQuestionVote> GetByRatingId(int userID, int ratingID)
            {
                return dal.Get(k => k.ratingId == ratingID && k.userId==userID && k.enabled == true).ToList();
            }

            public List<DoubleAndStringDto>? getChartData(int ratingId, int filterCompanyId)
            {
                BLLActions.RatingQuestions bllRatingQuestions = new BLLActions.RatingQuestions(_configuration, _env);
                List<RatingQuestion> listRatingQuestion = bllRatingQuestions.GetByRatingID(ratingId);
                List<DoubleAndStringDto> donenListe = new List<DoubleAndStringDto>();

                foreach (RatingQuestion ratingQuestion in listRatingQuestion)
                {
                    DoubleAndStringDto integerAndStringDto = new DoubleAndStringDto();

                    if (filterCompanyId == 0)
                    {

                        int? deger = ortHesapla(ratingId, ratingQuestion.Id);
                        HashSet<int> userIds = getUserIds(ratingId, ratingQuestion.Id);

                        integerAndStringDto.stringType = ratingQuestion.title;
                        if (deger == null || userIds.First() == 0)
                        {
                            integerAndStringDto.doubleType=(0.0);
                        }
                        else
                        {
                            double ort = (double)deger.Value / userIds.First();
                            integerAndStringDto.doubleType=(ort);
                        }

                        donenListe.Add(integerAndStringDto);
                    }
                    else
                    {

                        int? deger = degerWithCompany(ratingId, ratingQuestion.Id,
                                filterCompanyId);
                        HashSet<int> userIds = userIdsWithCompany(ratingId, ratingQuestion.Id,
                                filterCompanyId);
                        integerAndStringDto.stringType=ratingQuestion.title;
                        if (deger == null || userIds.First() == 0)
                        {
                            integerAndStringDto.doubleType=0.0;
                        }
                        else
                        {
                            double ort = (double)deger.Value / userIds.First();
                            integerAndStringDto.doubleType = (ort);
                        }
                        donenListe.Add(integerAndStringDto);
                    }

                }

                return donenListe;
            }

            public int ortHesapla(int ratingId, int questionId)
            {
                int? deger = dal.Get(u => u.ratingId == ratingId && u.enabled == true && u.questionId == questionId).Sum(u => u.ratingValue);
                return deger ?? 0;
            }

            public HashSet<int> getUserIds(int ratingId, int questionId)
            {
                HashSet<int> list =new HashSet <int> (dal.Get(u => u.ratingId == ratingId && u.enabled == true && u.questionId == questionId).Select(u => u.userId).ToList());
                return list;
            }

            public int degerWithCompany(int ratingId, int questionId,int companyId)
            {
                int deger = dal.Get(u=>u.ratingId==ratingId && u.enabled == true && u.questionId==questionId&&u.user.companyId==companyId).Sum(u => u.ratingValue);
                return deger;
            }

            public HashSet<int> userIdsWithCompany(int ratingId, int questionId,int companyId)
            {
                HashSet<int> list = new HashSet<int>(dal.Get(u => u.ratingId == ratingId && u.enabled == true && u.questionId == questionId && u.user.companyId==companyId).Select(u => u.userId).ToList());
                return list;
            }

            public List<ResultDetailDto> listAnswers(int ratingId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                var listUser = bllAdminUsers.listAllUserDto();

                BLLActions.RatingQuestions bllRatingQuestions = new BLLActions.RatingQuestions(_configuration, _env);
                var listRatingQuestion = bllRatingQuestions.findByEnabledAndRatingId(true, ratingId);

                var allVotes = dal.Get(x => x.enabled && x.ratingId == ratingId).ToList();

                var voteDictionary = allVotes.ToDictionary(
                    x => (x.userId, x.questionId),
                    x => x
                );

                var result = new List<ResultDetailDto>(listUser.Count);

                foreach (var user in listUser)
                {
                    var dto = new ResultDetailDto
                    {
                        name = user?.username ?? "",
                        companyName = user?.vtext ?? "",
                        listAnswers = new List<RatingAnswers>(listRatingQuestion.Count)
                    };

                    foreach (var question in listRatingQuestion)
                    {
                        if (voteDictionary.TryGetValue(((user?.userId ??0), question.Id ), out var vote))
                        {
                            dto.listAnswers.Add(new RatingAnswers
                            {
                                gorus = vote.comment,
                                puan = vote.ratingValue
                            });
                        }
                        else
                        {
                            dto.listAnswers.Add(new RatingAnswers
                            {
                                gorus = null,
                                puan = null
                            });
                        }
                    }

                    result.Add(dto);
                }

                return result;
            }
        }
    }    
}