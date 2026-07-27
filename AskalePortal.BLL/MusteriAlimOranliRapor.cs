

//using AskalePortal.Data.Models;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;


//namespace AskalePortal.BLL
//{
	
//	public partial class BLLActions
//	{
//		public class MusteriAlimOranliRapor : BaseBLL<AskalePortal.Data.Models.MusteriAlimOranliRapor>
//		{

//            public MusteriAlimOranliRapor(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
//            {
//            }
//            public List<AskalePortal.Data.Models.MusteriAlimOranliRapor> GetAllFromSAP(List<SelectOptions> sirket,DateTime? TARIH, List<SelectOptions> malzeme,List<SelectOptions> musteri,string yurticidisi)
//			{
//				List<AskalePortal.Data.Models.MusteriAlimOranliRapor> lstData = new List<AskalePortal.Data.Models.MusteriAlimOranliRapor>();
//				try
//				{
//					SAPConnection con = new SAPConnection(GetConfig());
//					IRfcFunction function = con.Repostory.CreateFunction("ZWEBI016");
			
//					IRfcTable t_sirket = function.GetTable("IV_SIRKET");
//					IRfcTable t_malzeme = function.GetTable("IV_MALZEME");
//					IRfcTable t_musteri = function.GetTable("IV_MUSTERI");
				

//					if (sirket != null)
//					{
//						foreach (var item in sirket)
//						{
//							t_sirket.Append();
//							t_sirket.SetValue(0, item.SIGN);
//							t_sirket.SetValue(1, item.OPTION);
//							t_sirket.SetValue(2, stringExtensions.ToUpperIgnoreNull(item.LOW));
//							t_sirket.SetValue(3, stringExtensions.ToUpperIgnoreNull(item.HIGH));
//						}
					
//					}

//					if (musteri != null)
//					{
//						foreach (var item in musteri)
//						{
//							t_musteri.Append();
//							t_musteri.SetValue(0, item.SIGN);
//							t_musteri.SetValue(1, item.OPTION);
//							t_musteri.SetValue(2, stringExtensions.ToUpperIgnoreNull(item.LOW));
//							t_musteri.SetValue(3, stringExtensions.ToUpperIgnoreNull(item.HIGH));
//						}

//					}


//					malzeme = (from x in malzeme
//							   select x).Distinct().ToList();

//					if (malzeme != null)
//					{
//						foreach (var item in malzeme)
//						{
//							t_malzeme.Append();
//							t_malzeme.SetValue(0, item.SIGN);
//							t_malzeme.SetValue(1, item.OPTION);
//							t_malzeme.SetValue(2, stringExtensions.ToUpperIgnoreNull(item.LOW));
//							t_malzeme.SetValue(3, stringExtensions.ToUpperIgnoreNull(item.HIGH));
//						}
//					}

					
					
//					if (TARIH.HasValue)
//						function.SetValue("IV_TARIH", TARIH.Value.ToString("dd.MM.yyyy"));
//					function.SetValue("IV_SIRKET", t_sirket);
//					function.SetValue("IV_MALZEME", t_malzeme);
//					function.SetValue("IV_MUSTERI", t_musteri);
//					function.SetValue("IV_YURICIDISI", yurticidisi);
//					IRfcTable table = function.GetTable("OUTPUT");
//					function.Invoke(con.Destination);

//					//Models.AdminUser user = (Models.AdminUser)HttpContext.Current.Session["Admin"];
//					//string companies = user.Role.companies;

//					for (int cuIndex = 0; cuIndex < table.RowCount; cuIndex++)
//					{
//						table.CurrentIndex = cuIndex;

//						Models.MusteriAlimOranliRapor c = new Models.MusteriAlimOranliRapor();
					
//						c.MANDT					= table.Getstring("MANDT");
//						c.VKORG					= table.Getstring("VKORG");
//						c.MUSTERI				= table.Getstring("MUSTERI");
//						c.MUSTERIADI			= table.Getstring("MUSTERIADI");
//						c.MALZEME				= table.Getstring("MALZEME");
//						c.MALZEMEADI			= table.Getstring("MALZEMEADI");
//						c.ORTALAMADEGER			= table.GetDouble("ORTALAMADEGER");
//						c.BUAYORT				= table.GetDouble("BUAYORT");
//						c.BUORTALAMADEGER		= table.GetDouble("BUORTALAMADEGER");
//						c.GUN1					= table.GetDouble("GUN1");
//						c.GUN2					= table.GetDouble("GUN2");
//						c.GUN3					= table.GetDouble("GUN3");
//						c.GUN4					= table.GetDouble("GUN4");
//						c.GUN5					= table.GetDouble("GUN5");
//						c.GUN6					= table.GetDouble("GUN6");
//						c.GUN7					= table.GetDouble("GUN7");
//						c.GUN8					= table.GetDouble("GUN8");
//						c.GUN9					= table.GetDouble("GUN9");
//						c.GUN10					= table.GetDouble("GUN10");
//						c.GUN11					= table.GetDouble("GUN11");
//						c.GUN12					= table.GetDouble("GUN12");
//						c.GUN13					= table.GetDouble("GUN13");
//						c.GUN14					= table.GetDouble("GUN14");
//						c.GUN15					= table.GetDouble("GUN15");
//						c.GUN16					= table.GetDouble("GUN16");
//						c.GUN17					= table.GetDouble("GUN17");
//						c.GUN18					= table.GetDouble("GUN18");
//						c.GUN19					= table.GetDouble("GUN19");
//						c.GUN20					= table.GetDouble("GUN20");
//						c.GUN21					= table.GetDouble("GUN21");
//						c.GUN22					= table.GetDouble("GUN22");
//						c.GUN23					= table.GetDouble("GUN23");
//						c.GUN24					= table.GetDouble("GUN24");
//						c.GUN25					= table.GetDouble("GUN25");
//						c.GUN26					= table.GetDouble("GUN26");
//						c.GUN27					= table.GetDouble("GUN27");
//						c.GUN28					= table.GetDouble("GUN28");
//						c.GUN29					= table.GetDouble("GUN29");
//						c.GUN30					= table.GetDouble("GUN30");
//						c.GUN31					= table.GetDouble("GUN31");
//						c.MIKTARBIRIM			= table.Getstring("MIKTARBIRIM");


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

