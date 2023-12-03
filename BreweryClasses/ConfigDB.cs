﻿using System;

using Microsoft.Extensions.Configuration;

/*
 * dotnet tool install --global dotnet-ef
 * Use nuget package manager to install efcore, efcore.analyzers, design, tools and mysql.data.efcore
 * -- Install-Package Microsoft.EntityFrameworkCore.Tools could do from the console too
 * Get-Help about_EntityFrameworkCore from the console
 *        
 *      Scaffold-DbContext "server=127.0.0.1;uid=root;pwd=YOURPASSWORDGOESHERE;database=MMABooks" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -context MMABooksContext -project MMABooksEFClasses -startupproject MMABooksEFClasses -force
 */
namespace BreweryClasses
{
    public class ConfigDB
    {
        public static string GetMySqlConnectionString()
        {
            string folder = System.AppContext.BaseDirectory;
            var builder = new ConfigurationBuilder()
                    .SetBasePath(folder)
                    .AddJsonFile("mySqlSettings.json", optional: true, reloadOnChange: true);

            string connectionString = builder.Build().GetConnectionString("mySql");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MySql connection string is null or empty.");
            }
            return connectionString;           
        }
    }
}
