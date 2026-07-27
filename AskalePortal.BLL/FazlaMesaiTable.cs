using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class FazlaMesaiTable : BaseBLL<AskalePortal.Data.Models.FazlaMesaiTable>
        {
            public FazlaMesaiTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.FazlaMesaiTable? GetByUniteIdAndDate(int uniteId, DateTime date)
            {
                return dal.Get(u => u.uniteId == uniteId && u.createdDate!.Value.Day== date.Day 
                && u.createdDate.Value.Year==date.Year && u.createdDate.Value.Month==date.Month && u.enabled==true).FirstOrDefault();
            }

            public List<AskalePortal.Data.Models.FazlaMesaiTable> GetByCompanyId(int? companyID,DateTime? Tarih, int? uniteId, int activePage, int recordsPerPage)
            {
                return dal.Get(u =>  uniteId.HasValue?u.uniteId == uniteId:true && Tarih.HasValue?
                (u.createdDate!.Value.Year == Tarih.Value.Year && u.createdDate.Value.Month == Tarih.Value.Month && u.createdDate.Value.Day == Tarih.Value.Day):true 
                && u.enabled == true).OrderByDescending(u => u.createdDate).Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
                
            }

            public List<AskalePortal.Data.Models.FazlaMesaiTable> GetAllByPage(DateTime? Tarih, int? uniteId, int? companyId, int activePage, int recordsPerPage)
            {
                return dal.Get(u=>(uniteId.HasValue?u.uniteId==uniteId :true) && (Tarih.HasValue?(u.createdDate!.Value.Year==Tarih.Value.Year 
                    && u.createdDate.Value.Month==Tarih.Value.Month && u.createdDate.Value.Day == Tarih.Value.Day):true) && u.enabled==true)
                    .OrderByDescending(u=>u.createdDate)
                    .Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
            }

            public List<AskalePortal.Data.Models.FazlaMesaiTable> GetAllByCompanyAndDateAndUnit(int companyId, DateTime? dateBas, DateTime? dateBit, int uniteId)
            {
                return dal.Get(u => 
                 ((dateBas.HasValue && dateBit.HasValue)? (u.createdDate>=dateBas && u.createdDate<=dateBit):
                (dateBas.HasValue?u.createdDate==dateBas:true)) &&(uniteId == -1 ? true : u.uniteId == uniteId)).ToList();
            }
        }
    }
}
