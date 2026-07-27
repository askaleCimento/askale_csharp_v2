using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class UserGridSettings : BaseBLL<AskalePortal.Data.Models.UserGridSetting>
        {
            public UserGridSettings(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.UserGridSetting? GetSettings(int userID, string pageName, bool isMobile)
            {
                var q = dal.Get(k => k.userId == userID && k.pageName == pageName && k.isMobile == isMobile && k.enabled == true).FirstOrDefault();
                return q;
            }
        }
    }
}