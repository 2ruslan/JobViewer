using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace JobViewer.Model
{
    public class JobDbContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobStep> JobSteps { get; set; }
        public DbSet<JobHistory> JobHistory { get; set; }
        public DbSet<TextLine> TextLines { get; set; }
        public DbSet<DataBase> DataBases { get; set; }

        protected override void OnConfiguring(
           DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = $"data source={Properties.Settings.Default.DataSource};initial catalog = msdb; Integrated Security = SSPI;";

            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder
                .LogTo(OnDbError)
                .EnableDetailedErrors(); 
        }

        private void OnDbError(string arg)
        {
            System.Diagnostics.Debug.WriteLine(arg);
        }

        public string SpHelpText(string dbName, string objName)
        {
            string result = string.Empty;
            try
            {
                var dbRes = TextLines
                    .FromSqlRaw($"use {dbName}; EXEC sp_helptext '{objName}';")
                    .ToList();

                result = dbRes.Select(x => x.Text).Aggregate((a, b) => $"{a}{b}");
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            
            return result;
        }

        
    }
}
