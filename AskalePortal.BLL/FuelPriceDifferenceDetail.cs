using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
   

    public partial class BLLActions
    {
        public class FuelPriceDifferenceDetail : BaseBLL<AskalePortal.Data.Models.FuelPriceDifferenceDetail>
        {
            public FuelPriceDifferenceDetail(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {

            }

            public List<Data.Models.FuelPriceDifferenceDetail> listByFuelId(int fuelId)
            {
                List<Data.Models.FuelPriceDifferenceDetail> liste = dal.Get(u => u.enabled && u.fuelId == fuelId).ToList();
                return liste;
            }

            public List<Data.Models.FuelPriceDifferenceDetail> getByActiveFuelId(int id)
            {
                List<Data.Models.FuelPriceDifferenceDetail> liste = dal.Get(u => u.enabled && u.fuelId == id).ToList();
                return liste;
            }

            public Data.Models.FuelPriceDifferenceDetail getByActive(int fuelId, int userId)
            {
                Data.Models.FuelPriceDifferenceDetail fuelPriceDifferenceDetail = dal.Get(u => u.enabled && u.fuelId == fuelId && u.userId == userId).First();
                return fuelPriceDifferenceDetail;
            }
        }
    }
}
