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
        public class DocumentArchives : BaseBLL<AskalePortal.Data.Models.DocumentArchive>
        {

            public DocumentArchives(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.DocumentArchive> GetAll(string title)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrWhiteSpace(title)) && k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.DocumentArchive> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderByDescending(k => k.Id);

                return q.ToList();
            }

            public AskalePortal.Data.Models.DocumentArchive GetDefaultTemplate()
            {
                var q = dal.Get(k => k.enabled == true && k.isTemplate);

                return q.FirstOrDefault() ?? new AskalePortal.Data.Models.DocumentArchive();
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.DocumentArchive> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            public List<DocumentArchive>? GetAllFilter(FilterParam<HelpDeskStatusListDtoParameter> filterParam)
            {
                string? title = filterParam.liste?.title;
                List<DocumentArchive> liste = dal.Get(u => u.enabled && (string.IsNullOrEmpty(title) ? true : u.title.ToLower().Contains(title))).ToList();
                return liste;
            }

            #endregion GetAllWithPage
        }
    }

    
}
