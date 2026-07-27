//using SAP.Middleware.Connector;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AskalePortal.BLL
//{
//	public partial class BLLActions
//	{
//		public class OdemeRaporu:BaseBLL<AskalePortal.Data.Models.OdemeRaporu>
//		{
//			public List<AskalePortal.Data.Models.OdemeRaporu> GetAllFromSAP(DateTime Tarih)
//			{
//				List<AskalePortal.Data.Models.OdemeRaporu> lstData = new List<AskalePortal.Data.Models.OdemeRaporu>();
//				try
//				{
//					SAPConnection con = new SAPConnection(GetConfig());
//					IRfcFunction function = con.Repostory.CreateFunction("ZWEBI026");

//					string ay = Tarih.Month < 10 ? "0" + Tarih.Month.ToString() : Tarih.Month.ToString();
//					string tarihAra = Tarih.Year.ToString() + ay;
//					function.SetValue("LV_YIL_AY", tarihAra);
//					IRfcTable table = function.GetTable("OUTPUT");
//					function.Invoke(con.Destination);

//					//Models.AdminUser user = (Models.AdminUser)HttpContext.Current.Session["Admin"];
//					//string companies = user.Role.companies;

//					for (int cuIndex = 0; cuIndex < table.RowCount; cuIndex++)
//					{
//						table.CurrentIndex = cuIndex;

//						Models.OdemeRaporu c = new Models.OdemeRaporu();

//						c.MANDT = table.Getstring("MANDT");
//						c.NAME1 = table.Getstring("NAME1");
//						c.KUNNR = table.Getstring("KUNNR")== ""? table.Getstring("LIFNR") : table.Getstring("KUNNR");
//						c.BUKRS = table.Getstring("BUKRS");
//						c.SIRA = table.Getstring("SAYI");
//						c.GUN1 = table.GetDouble("GUN1");
//						c.GUN2 = table.GetDouble("GUN2");
//						c.GUN3 = table.GetDouble("GUN3");
//						c.GUN4 = table.GetDouble("GUN4");
//						c.GUN5 = table.GetDouble("GUN5");
//						c.GUN6 = table.GetDouble("GUN6");
//						c.GUN7 = table.GetDouble("GUN7");
//						c.GUN8 = table.GetDouble("GUN8");
//						c.GUN9 = table.GetDouble("GUN9");
//						c.GUN10 = table.GetDouble("GUN10");
//						c.GUN11 = table.GetDouble("GUN11");
//						c.GUN12 = table.GetDouble("GUN12");
//						c.GUN13 = table.GetDouble("GUN13");
//						c.GUN14 = table.GetDouble("GUN14");
//						c.GUN15 = table.GetDouble("GUN15");
//						c.GUN16 = table.GetDouble("GUN16");
//						c.GUN17 = table.GetDouble("GUN17");
//						c.GUN18 = table.GetDouble("GUN18");
//						c.GUN19 = table.GetDouble("GUN19");
//						c.GUN20 = table.GetDouble("GUN20");
//						c.GUN21 = table.GetDouble("GUN21");
//						c.GUN22 = table.GetDouble("GUN22");
//						c.GUN23 = table.GetDouble("GUN23");
//						c.GUN24 = table.GetDouble("GUN24");
//						c.GUN25 = table.GetDouble("GUN25");
//						c.GUN26 = table.GetDouble("GUN26");
//						c.GUN27 = table.GetDouble("GUN27");
//						c.GUN28 = table.GetDouble("GUN28");
//						c.GUN29 = table.GetDouble("GUN29");
//						c.GUN30 = table.GetDouble("GUN30");
//						c.GUN31 = table.GetDouble("GUN31");



//						lstData.Add(c);
//					}
//				}
//				catch (Exception ex)
//				{
//					LogError(ex);
//				}

//				return lstData;

//			}

//		}
//	}
//}
