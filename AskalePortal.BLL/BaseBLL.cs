using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AskalePortal.DAL;
using Models = AskalePortal.Data.Models;


using System.Globalization;
using AskalePortal.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using AutoMapper;

namespace AskalePortal.BLL
{
    public class BaseBLL<T> where T : class
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public BaseClass<T> dal;
      
        public BaseBLL(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration; 
            _env = env;
            dal= new BaseClass<T>(_configuration, env);
        }

        public virtual T? GetByID(int ID)
        {
            var q = dal.Get("ID = @0 && enabled = @1", new object[] { ID, true });
            return q.FirstOrDefault();
        }

        public virtual T? GetByIDAll(int ID)
        {
            var q = dal.Get("ID = @0", new object[] { ID });
            return q.FirstOrDefault();
        }

     

        public virtual async Task<T?> Add(T entity)
        {
            PropertyInfo? enabled = entity.GetType().GetProperty("enabled");
            enabled?.SetValue(entity, true, null);

           return await dal.AddAsync(entity);
        }

        public virtual async Task<T> Update(T entity)
        {
            return await dal.Update(entity);
        }

        public async virtual void Delete(int ID)
        {
            var q = GetByID(ID);
            if (q != null)
            {
                PropertyInfo enabled = q.GetType().GetProperty("enabled");
                enabled.SetValue(q, false, null);
               await Update(q);
            }
        }
        public virtual List<T> GetAll()
        {
            var q = dal.Get("enabled = @0", new object[] { true });
            return q.ToList();
        }
        public async virtual void DeletePermanently(int ID)
        {
            var q = GetByID(ID);
            if (q != null)
            {
                await dal.DeletePermanently(q);
            }
        }

		public async virtual void DeletePermanentlyAll()
		{

            await dal.DeleteAllPermanently();
			
		}
		protected Models.Config GetConfig()
        {
            BLLActions.Configs bllConfig = new(_configuration,_env);
            return bllConfig.GetByID(1);
        }

        protected void LogError(Exception ex)
        {

        }
    }

    public static class stringExtensions
    {
        public static bool ContainsUserID(this string source, int userID)
        {
            return source != null && source.IndexOf("[" + userID + "]") >= 0;
        }

        public static int[] ToUserIDIntArray(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return new int[] { };
            else
                return source.Split(',').Select(n => DataReader.GetInt32(n.TrimStart('[').TrimEnd(']'))).ToArray();
        }
		public static string ToUpperIgnoreNull(this string value)
		{
			if (value != null)
			{
				value = value.ToUpper(CultureInfo.InvariantCulture);
			}
			return "";
		}

		public static List<int> ToUserIDIntList(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return new List<int> { };
            else
                return source.Split(',').Select(n => DataReader.GetInt32(n.TrimStart('[').TrimEnd(']'))).ToList();
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string ToUserIDstring(this int[] source)
        {
            if (source.Any())
            {
                string idList = string.Empty;
                source.ToList().ForEach(item => { if (item > 0) { idList += "[" + item + "],"; } });
                return idList.TrimEnd(',');
            }
            else
                return "";
        }
        public static string ToUserIDListToUserName(this string source, IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            BLLActions.AdminUsers bllAdmin = new BLLActions.AdminUsers(configuration,env,mapper);
            if (source.Any())
            {
                string idList = string.Empty;
                
                source.Split(',').Select(n => DataReader.GetInt32(n.TrimStart('[').TrimEnd(']'))).ToList().ForEach(item => { if (item > 0) { idList += bllAdmin.GetByID(item)==null?"": bllAdmin.GetByID(item)?.name+","; } });
                return idList.TrimEnd(',');
            }
            else
                return "";
        }
    }
}
