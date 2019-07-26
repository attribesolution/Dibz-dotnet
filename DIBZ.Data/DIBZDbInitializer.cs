using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Data
{
    //public class DIBZDbInitializer : DropCreateDatabaseIfModelChanges<DIBZDbContext>
    public class DIBZDbInitializer : DropCreateDatabaseAlways<DIBZDbContext>
    {
        protected override void Seed(DIBZDbContext context)
        {
            //var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DBScripts");
            //var sqlFiles = Directory.GetFiles(basePath, "*.sql").OrderBy(x => x);

            //foreach (var file in sqlFiles)
            //{
            //    var commands = File.ReadAllLines(file, Encoding.UTF8);
            //    //context.Database.ExecuteSqlCommand(script);
            //    using (var conn = new Npgsql.NpgsqlConnection(ConfigurationManager.ConnectionStrings["DIBZDbContext"].ConnectionString))
            //    {
            //        conn.Open();
            //        var tran = conn.BeginTransaction();
            //        foreach (var c in commands)
            //        {
            //            var cmd = conn.CreateCommand();
            //            cmd.CommandText = c;
            //            cmd.ExecuteNonQuery();
            //        }
            //        tran.Commit();
            //    }
            //}
        }
    }
}
