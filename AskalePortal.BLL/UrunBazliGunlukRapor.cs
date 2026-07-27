//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using CMS.General;
//using SAP.Middleware.Connector;
//using AskalePortal.Data.Models;
//using System.Web;

//namespace AskalePortal.BLL
//{
//    public partial class BLLActions
//    {
//        public class UrunBazliGunlukRapor : BaseBLL<AskalePortal.Data.Models.UrunBazliGunlukRapor>
//        {
//            #region GetAllWithParameters

//            public List<AskalePortal.Data.Models.UrunBazliGunlukRapor> GetAllFromSAP(DateTime? TARIH)
//            {
//                List<AskalePortal.Data.Models.UrunBazliGunlukRapor> lstData = new List<AskalePortal.Data.Models.UrunBazliGunlukRapor>();
//                try
//                {
//                    SAPConnection con = new SAPConnection(GetConfig());
//                    IRfcFunction function = con.Repostory.CreateFunction("ZWEBI005");

//                    if (TARIH.HasValue)
//                        function.SetValue("IV_TARIH", TARIH.Value.ToString("dd.MM.yyyy"));
//                    IRfcTable table = function.GetTable("OUTPUT");
//                    function.Invoke(con.Destination);

//                    AdminUser user = (AdminUser)HttpContext.Current.Session["Admin"];
//                    string companies = user.Role.companies;

//                    for (int cuIndex = 0; cuIndex < table.RowCount; cuIndex++)
//                    {
//                        table.CurrentIndex = cuIndex;

//                        Models.UrunBazliGunlukRapor c = new Models.UrunBazliGunlukRapor();

//                        c.MANDT = table.Getstring("MANDT");
//                        c.TARIH = DataReader.GetDateTime(table.Getstring("TARIH"));
//                        c.SATORG = table.Getstring("SATORG");
//                        c.MALZEME = table.Getstring("MALZEME");
//                        c.MALZEMETNM = table.Getstring("MALZEMETNM");
//                        c.DAGITIMKANALI = table.Getstring("DAGITIMKANALI");
//                        c.DAGITIMKANALITNM = table.Getstring("DAGITIMKANALITNM");
//                        c.BOLGE = table.Getstring("BOLGE");
//                        c.BOLGETNM = table.Getstring("BOLGETNM");
//                        c.ODEMEKOSULU = table.Getstring("ODEMEKOSULU");
//                        c.TESLIMATMIKTARI = table.GetDecimal("TESLIMATMIKTARI");
//                        c.MIKTARBIRIM = table.Getstring("MIKTARBIRIM");
//                        c.KOSULTUTARI = table.GetDecimal("KOSULTUTARI");
//                        c.NAKLIYELIFIYAT = table.GetDecimal("NAKLIYELIFIYAT");
//                        c.PARABIRIM = table.Getstring("PARABIRIM");
//                        c.BRUTFIYAT = table.GetDecimal("BRUTFIYAT");
//                        c.INDIRIMTOPLAMI = table.GetDecimal("INDIRIMTOPLAMI");
//                        c.INDIRIMSONRASI = table.GetDecimal("INDIRIMSONRASI");
//                        c.NAKLIYESIGORTA = table.GetDecimal("NAKLIYESIGORTA");
//                        c.NETDEGER = table.GetDecimal("NETDEGER");
//                        c.KDV = table.GetDecimal("KDV");
//                        c.NIHAITUTAR = table.GetDecimal("NIHAITUTAR");

//                        if (companies.Contains(string.Format("[{0}]", c.SATORG)))
//                            lstData.Add(c);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    LogError(ex);
//                    //"Hata : " + ex.Message;
//                }

//                    return lstData;
              
//            }

//            #endregion GetAll
//        }
//    }
//}
