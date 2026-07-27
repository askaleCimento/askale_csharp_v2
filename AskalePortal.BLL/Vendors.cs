//using CMS.Data.SAP.Models;
//using SAP.Middleware.Connector;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;

//namespace AskalePortal.BLL
//{
//	public partial class BLLActions
//	{
//		public class Vendors : BaseBLL<CMS.Data.SAP.Models.Vendors>
//		{

//			public List<CMS.Data.SAP.Models.Vendors> GetAllFromSAP(string bukrs="")
//			{

//				List<SAP.Models.Vendors> lstvendors = new List<SAP.Models.Vendors>();
//				//return lstCustomers.ToPagedList(activePage, recordsPerPage);

//					try
//					{
//						SAPConnection con = new SAPConnection(new BLLActions.Configs().GetByID(1));
//						IRfcFunction function = con.Repostory.CreateFunction("ZWEBI018");
//						function.SetValue("lv_bukrs", bukrs);
//						IRfcTable table = function.GetTable("OUTPUT");
//						function.Invoke(con.Destination);

//						for (int cuIndex = 0; cuIndex < table.RowCount; cuIndex++)
//						{
//							table.CurrentIndex = cuIndex;

//							SAP.Models.Vendors c = new SAP.Models.Vendors();

//							c.LIFNR = table.Getstring("LIFNR");
//							c.LAND1 = table.Getstring("LAND1");
//							c.NAME1 = table.Getstring("NAME1");
//							c.NAME2 = table.Getstring("NAME2");
//							c.NAME3 = table.Getstring("NAME3");
//							c.NAME4 = table.Getstring("NAME4");
//							c.TELF1 = table.Getstring("TELF1");
//							lstvendors.Add(c);
//						}

					
//					}
//					catch (Exception ex)
//					{
//						LogError(ex);
//					}
				

//				return lstvendors;
//			}

//			public List<SAP.Models.Vendors> GetAllFromSAPByDate(DateTime datetime, string bukrs = "")
//			{
//				string ay = datetime.Month < 10 ? "0" + datetime.Month.ToString() : datetime.Month.ToString();
//				string gun = datetime.Day < 10 ? "0" + datetime.Day.ToString() : datetime.Day.ToString();
//				string date =datetime.Year.ToString()+ay+gun;
//				string time = datetime.ToLongTimestring();
//				List<SAP.Models.Vendors> lstvendors = new List<SAP.Models.Vendors>();
//				//return lstCustomers.ToPagedList(activePage, recordsPerPage);

//				try
//				{
//					SAPConnection con = new SAPConnection(new BLLActions.Configs().GetByID(1));
//					IRfcFunction function = con.Repostory.CreateFunction("ZWEBI030");
//					function.SetValue("lv_bukrs", bukrs);
//					function.SetValue("lv_tarih", date);
//					function.SetValue("lv_saat", time);
//					IRfcTable table = function.GetTable("OUTPUT");
//					function.Invoke(con.Destination);

//					for (int cuIndex = 0; cuIndex < table.RowCount; cuIndex++)
//					{
//						table.CurrentIndex = cuIndex;

//						SAP.Models.Vendors c = new SAP.Models.Vendors();

//						c.LIFNR = table.Getstring("LIFNR");
//						c.LAND1 = table.Getstring("LAND1");
//						c.NAME1 = table.Getstring("NAME1");
//						c.NAME2 = table.Getstring("NAME2");
//						c.NAME3 = table.Getstring("NAME3");
//						c.NAME4 = table.Getstring("NAME4");
//						c.TELF1 = table.Getstring("TELF1");
//						lstvendors.Add(c);
//					}


//				}
//				catch (Exception ex)
//				{
//					LogError(ex);
//				}


//				return lstvendors;
//			}
//		}
	
//	}
//}
