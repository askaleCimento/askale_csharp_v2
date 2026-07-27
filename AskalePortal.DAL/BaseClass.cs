using System.Linq;
using System.Linq.Expressions;
using AskalePortal.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using AskalePortal.Data.ResponseParams;
using Microsoft.Extensions.Logging;

namespace AskalePortal.DAL
{
    public class BaseClass<T> where T : class
    {

        private IConfiguration _configuration;
        private IWebHostEnvironment _env;
        public DBDataContext dB;


        public BaseClass(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;

            //        if (env.IsProduction())
            //        {
            //            string? connectionstring = _configuration.GetConnectionString("server");
            //            dB = new(

            //              new DbContextOptionsBuilder<DBDataContext>()
            //       //.UseLazyLoadingProxies()
            //       .UseSqlServer(connectionString: connectionstring).Options



            //      );
            //        }
            //        else if (env.IsDevelopment())
            //        {
            //            string? connectionstring = _configuration.GetConnectionString("local");
            //            dB = new(

            //              new DbContextOptionsBuilder<DBDataContext>().UseLoggerFactory(new LoggerFactory(new[] {
            //    new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            //}))
            //       //.UseLazyLoadingProxies()
            //       .UseSqlServer(connectionString: connectionstring).Options



            //      );

            //        }
            //        else if (env.EnvironmentName == "Test")
            //        {
            //            string? connectionstring = _configuration.GetConnectionString("test");
            //            dB = new(

            //              new DbContextOptionsBuilder<DBDataContext>().UseLoggerFactory(new LoggerFactory(new[] {
            //    new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            //}))
            //       //.UseLazyLoadingProxies()
            //       .UseSqlServer(connectionString: connectionstring).Options



            //      );
            //        }


            if (env.IsProduction())
            {
                string? connectionstring = _configuration.GetConnectionString("server");
                dB = new(
                    new DbContextOptionsBuilder<DBDataContext>()
                        //.UseLazyLoadingProxies()
                        .UseSqlServer(
                            connectionstring,
                            options => options.UseCompatibilityLevel(120))
                        .Options
                );
            }
            else if (env.IsDevelopment())
            {
                string? connectionstring = _configuration.GetConnectionString("local");
                dB = new(
                    new DbContextOptionsBuilder<DBDataContext>()
                        .UseLoggerFactory(new LoggerFactory(new[]
                        {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
                        }))
                        //.UseLazyLoadingProxies()
                        .UseSqlServer(
                            connectionstring,
                            options => options.UseCompatibilityLevel(120))
                        .Options
                );
            }
            else if (env.EnvironmentName == "Test")
            {
                string? connectionstring = _configuration.GetConnectionString("test");
                dB = new(
                    new DbContextOptionsBuilder<DBDataContext>()
                        .UseLoggerFactory(new LoggerFactory(new[]
                        {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
                        }))
                        //.UseLazyLoadingProxies()
                        .UseSqlServer(
                            connectionstring,
                            options => options.UseCompatibilityLevel(120))
                        .Options
                );
            }
        }



        protected DBDataContext DB { get => dB; set => dB = value; }
        #region Get

        public IQueryable<T> GetQueryable(Expression<Func<T, bool>> where)
        {

            return DB.Set<T>().Where(where);

        }
        public IQueryable<T> Get(Expression<Func<T, bool>> where)
        {

            IQueryable<T> q = DB.Set<T>().Where(where);

            return q;

        }

        public IQueryable<T> Get(string where, object[] values)
        {


            var q = DB.Set<T>().Where(where, values);

            return q;


        }
        public PageReturn<T> GetPage(Expression<Func<T, bool>> where, int pageNumber = 1, int pageSize = 20)
        {
            PageReturn<T> page = new PageReturn<T>();

            var q = DB.Set<T>().Skip((pageNumber - 1) * pageSize).Take(pageSize).Where(where);
            page.content = q.ToList();
            page.totalElements = q.Count();
            page.totalPages = (int)Math.Ceiling((page.totalElements ?? 0) / (double)pageSize);
            page.size = pageSize;
            page.numberOfElements = page.totalElements;
            return page;


        }




        #endregion Get

        #region Add

        public async Task<T?> AddAsync(T entity)
        {
            try
            {

                DB.Set<T>().Add(entity);
                await DB.SaveChangesAsync();
                return entity;
            }
            catch (Exception)
            {
                throw;
            }


        }

        #endregion Add

        #region AddList

        public async Task AddList(List<T> lst)
        {

            foreach (var entity in lst)
            {
                DB.Set<T>().Add(entity);
            }


            await DB.SaveChangesAsync();


        }

        #endregion AddList

        #region Update

        public async Task<T> Update(T entity)
        {

            try
            {

                DB.Set<T>().Attach(entity);
                DB.Entry(entity).State = EntityState.Modified;
                await DB.SaveChangesAsync();
                return entity;
            }
            catch (Exception)
            {
                throw;
            }


        }

        #endregion Update

        #region UpdateList

        public async Task UpdateList(List<T> lst)
        {


            foreach (var item in lst)
            {
                DB.Set<T>().Attach(item);
                DB.Entry(item).State = EntityState.Modified;
            }

            await DB.SaveChangesAsync();




        }

        #endregion UpdateList

        #region DeletePermanently

        public async Task DeletePermanently(T entity)
        {

            DB.Set<T>().Remove(entity);
            await DB.SaveChangesAsync();


        }

        public async Task DeleteAllPermanently()
        {
            DB.Set<T>().RemoveRange(DB.Set<T>());
            await DB.SaveChangesAsync();
        }

        #endregion DeletePermanently

        #region SaveChanges

        public async Task SaveChanges()
        {

            await DB.SaveChangesAsync();

        }

        #endregion SaveChanges

        private void LogError(Exception ex)
        {

        }




    }
}
