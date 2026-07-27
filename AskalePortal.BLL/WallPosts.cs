//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace AskalePortal.BLL
//{
//    public partial class BLLActions
//    {
//        public class WallPosts : BaseBLL<AskalePortal.Data.Models.WallPost>
//        {
//            #region GetAll

//            public List<AskalePortal.Data.Models.WallPost> GetByUserID(int userID)
//            {
//                var q = dal.Get(k => k.toUserID==userID && k.status == true).OrderByDescending(k => k.createdDate);
//                return q.ToList();
//            }
            
//            #endregion GetAll
//        }
//    }
//}
