using DB.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Api.Controllers
{
    //Intial Creation
    //dotnet ef migrations add InitialCreate
    //dotnet ef database update


    //to add new migration
    //dotnet ef migrations add AddCategoryProductRelation

    //To update to database
    //dotnet ef database update


    //If EF Core doesn't detect the new table, force a migration:
    //dotnet ef migrations add AddProductsTable --verbose

    //If EF Core doesn't detect the new table, force a migration:
    //dotnet ef migrations remove
    //dotnet ef migrations add RefreshSchema
    //dotnet ef database update



   //Final Change

    //If EF Core doesn't detect the new table, force a migration:
    //dotnet ef migrations add AddProductsTable --verbose
    //dotnet ef database update



    //MIGRATION="2024R4/08.ID-6459/" 
    //FROM MIGRATION = "20241129005441_ID-6412-Organisation Type"
    //TO MIGRATION = "20241209051209 

    //dotnet ef migrations script -i -o../../ReleaseScripts/$MIGRATION/release.sql $FROM MIGRATION $TO_MIGRATION
    //dotnet ef migrations script -i -o../../ReleaseScripts/SMIGRATION/rollback.sql $TO_MIGRATION $FROM_MIGRATION

    //MIGRATION' should be the next increment in the latest 'OneCard/ReleaseScripts/2024R1 directory.
    //(Or newer release if that is what is currently being prepared.) FROM MIGRATION' and 'TO MIGRATION' 
    //should be the last two migrations from dotnet ef migrations list.
    //The TO MIGRATION' should be your new

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}