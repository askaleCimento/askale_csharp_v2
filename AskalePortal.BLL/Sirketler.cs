//using Microsoft.AspNetCore.Hosting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace AskalePortal.BLL
//{
//    public partial class BLLActions
//    {
//        public class Sirketler : BaseBLL<AskalePortal.Data.Models.Sirketler>
//        {
           
//            #region GetAll

//            public override List<AskalePortal.Data.Models.Sirketler> GetAll()
//            {
//                var q = dal.Get(k=> k.VKORG != "").OrderBy(k => k.VKORG);
//                return q.ToList();
//            }

//            public List<AskalePortal.Data.Models.Sirketler> GetAllFromSAP()
//            {
//                List<AskalePortal.Data.Models.Sirketler> lstSirketler = new List<AskalePortal.Data.Models.Sirketler>();
//                try
//                {
//                    SAPConnection con = new SAPConnection(new BLLActions.Configs().GetByID(1));
//                    IRfcFunction function = con.Repostory.CreateFunction("ZWEBI006");
//                    IRfcTable table = function.GetTable("ET_COMPANY");
//                    function.Invoke(con.Destination);

//                    for (int cuIndex = 0; cuIndex < table.RowCount; cuIndex++)
//                    {
//                        table.CurrentIndex = cuIndex;
                        
//                        Models.Sirketler c = new Models.Sirketler();
//                        c.MANDT = table.Getstring("MANDT");
//                        c.SPRAS = table.Getstring("SPRAS");
//                        c.VKORG = table.Getstring("VKORG");
//                        c.VTEXT = table.Getstring("VTEXT");

//                        lstSirketler.Add(c);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    LogError(ex);
//                }

//                return lstSirketler;
//            }

//            #endregion GetAll
//        }
//    }
//}
