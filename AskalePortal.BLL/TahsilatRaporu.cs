//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AskalePortal.BLL
//{
//	public partial class BLLActions
//	{
//		public class TahsilatRaporu:BaseBLL<AskalePortal.Data.Models.TahsilatRaporuTable>
//		{
//            public TahsilatRaporu(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
//            {
//            }
//            public void WriteDataBaseFromSAP()
//			{
//				SAPConnection con = new SAPConnection(new BLLActions.Configs().GetByID(1));
//				IRfcFunction function = con.Repostory.CreateFunction("ZWEBI020");

//				IRfcTable t_sirket = function.GetTable("LV_BUKRS");
//				IRfcTable t_date = function.GetTable("LV_DATE");



//				t_sirket.Append();
//				t_sirket.SetValue(0, "I");
//				t_sirket.SetValue(1, "EQ");
//				t_sirket.SetValue(2, "AC10");
//				t_sirket.SetValue(3, "");



//				t_date.Append();
//				t_date.SetValue(0, "I");
//				t_date.SetValue(1, "BT");
//				t_date.SetValue(2,"20151101");
//				t_date.SetValue(3, "20151130");

//				function.SetValue("LV_BUKRS", t_sirket);
//				function.SetValue("LV_DATE", t_date);
//				function.SetValue("LV_DATE", t_date);
//				function.SetValue("LV_GJAHR", "2015");
//				IRfcTable table = function.GetTable("LT_BSIS");
//				function.Invoke(con.Destination);
			
//				for (int cuIndex = 0; cuIndex < table.RowCount; cuIndex++)
//				{
//					table.CurrentIndex = cuIndex;
//                    AskalePortal.Data.Models.TahsilatRaporuTable tahsilatRaporuTable = new AskalePortal.Data.Models.TahsilatRaporuTable()
//					{
//						BELNR = table.Getstring("BELNR"),
//						BLART = table.Getstring("BLART"),
//						BUDAT = table.Getstring("BUDAT"),
//						BUKRS = table.Getstring("BUKRS"),
//						BUZEI = table.Getstring("BUZEI"),
//						KUNNR = table.Getstring("KUNNR"),
//						GJAHR = table.Getstring("GJAHR"),
//						HWAE2 = table.Getstring("HWAE2"),
//						DMBE2 = table.GetDecimal("DMBE2"),
//						DMBE3 = table.GetDecimal("DMBE3"),
//						DMBTR = table.GetDecimal("DMBTR"),
//						HKONT = table.Getstring("HKONT"),
//						HWAE3 = table.Getstring("HWAE3"),
//						HWAER = table.Getstring("HWAER"),
//						LIFNR = table.Getstring("LIFNR"),
//						NAME1 = table.Getstring("NAME1"),
//						SGTXT = table.Getstring("SGTXT"),
//						ZUONR = table.Getstring("ZUONR"),
//						SHKZG = table.Getstring("SHKZG"),
//						WAERS = table.Getstring("WAERS"),
//						WRBTR = table.GetDecimal("WRBTR")
//					};

//					Add(tahsilatRaporuTable);
					
//				}

				
//			}
//			public List<AskalePortal.Data.Models.TahsilatRaporuTable> GetFromSAP(DateTime time)
//			{
//				SAPConnection con = new SAPConnection(new BLLActions.Configs().GetByID(1));
//				IRfcFunction function = con.Repostory.CreateFunction("ZWEBI020");

//				IRfcTable t_sirket = function.GetTable("LV_BUKRS");
//				IRfcTable t_date = function.GetTable("LV_DATE");
//				DateTime time2 = time.AddDays(-1);

//				t_sirket.Append();
//				t_sirket.SetValue(0, "I");
//				t_sirket.SetValue(1, "BT");
//				t_sirket.SetValue(2, "AC10");
//				t_sirket.SetValue(3, "AC99");

				

//				t_date.Append();
//				t_date.SetValue(0, "I");
//				t_date.SetValue(1, "BT");
//				t_date.SetValue(2, time2.ToString("dd.MM.yyyy"));
//				t_date.SetValue(3, time.ToString("dd.MM.yyyy"));

//				function.SetValue("LV_BUKRS", t_sirket);
//				function.SetValue("LV_DATE", t_date);
//				if (time.Year == time2.Year)
//				{
//					function.SetValue("LV_GJAHR", time.Year.ToString());
//				}
			
//				IRfcTable table = function.GetTable("LT_BSIS");
//				function.Invoke(con.Destination);
//				List<AskalePortal.Data.Models.TahsilatRaporuTable> liste = new List<AskalePortal.Data.Models.TahsilatRaporuTable>();
//				for (int cuIndex = 0; cuIndex < table.RowCount; cuIndex++)
//				{
//					table.CurrentIndex = cuIndex;
//                    AskalePortal.Data.Models.TahsilatRaporuTable tahsilatRaporuTable = new AskalePortal.Data.Models.TahsilatRaporuTable()
//					{
//						BELNR = table.Getstring("BELNR"),
//						BLART = table.Getstring("BLART"),
//						BUDAT = table.Getstring("BUDAT"),
//						BUKRS = table.Getstring("BUKRS"),
//						BUZEI = table.Getstring("BUZEI"),
//						KUNNR = table.Getstring("KUNNR"),
//						GJAHR = table.Getstring("GJAHR"),
//						HWAE2 = table.Getstring("HWAE2"),
//						DMBE2 = table.GetDecimal("DMBE2"),
//						DMBE3 = table.GetDecimal("DMBE3"),
//						DMBTR = table.GetDecimal("DMBTR"),
//						HKONT = table.Getstring("HKONT"),
//						HWAE3 = table.Getstring("HWAE3"),
//						HWAER = table.Getstring("HWAER"),
//						LIFNR = table.Getstring("LIFNR"),
//						NAME1 = table.Getstring("NAME1"),
//						SGTXT = table.Getstring("SGTXT"),
//						ZUONR = table.Getstring("ZUONR"),
//						SHKZG = table.Getstring("SHKZG"),
//						WAERS = table.Getstring("WAERS"),
//						WRBTR = table.GetDecimal("WRBTR")
//					};

//					liste.Add(tahsilatRaporuTable);

//				}
//				return liste;

//			}
//			public void DeleteDatabase()
//			{
//				DateTime time = DateTime.Now;
//				string today = "2015-11-11";
//				if(dal.Get(u => u.budat == today).Count() != 0)
//				{

				
//				List<AskalePortal.Data.Models.TahsilatRaporuTable> liste = dal.Get(u => u.budat == today).ToList();
//				foreach (var item in liste)
//				{
//					DeletePermanently(item.Id);
//				}
//				}

//			}
//		}
//	}
//}
