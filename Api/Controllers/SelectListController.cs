using BusinessLogic.Interfaces;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Data;


namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SelectListController : ControllerBase
    {
        private readonly IDropDownService _dropDownService;
        public SelectListController(IDropDownService dropdownService)
        {
            _dropDownService = dropdownService;
        }
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> GetSelectListAsync(string inputTypes)
        //{
        //    return Ok(await _dropDownService.GetSelectList(inputTypes));
        //}

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSelectListAsync(string inputTypes)
        {





//            var app = ConfidentialClientApplicationBuilder.Create("6b5cb635-b477-4eda-a39b-78f3c80d4f21")
//            .WithClientSecret("e1m8Q~8slNytggZ6UHtO3dijvE6RmEx3T.tYMan_") // 🔴 replace with rotated secret
//            .WithAuthority("https://login.microsoftonline.com/bae04769-f27e-49d4-89f9-2dd2ade385b7")
//            .Build();

//            var result = await app
//                .AcquireTokenForClient(new[] { "https://database.windows.net/.default" })
//                .ExecuteAsync();

//            string accessToken = result.AccessToken;



//            var connectionString =
//            "Server=ercprodws1.sql.azuresynapse.net;" +
//            "Database=ercprodws1pOds;" +
//            "Encrypt=True;" +
//            "TrustServerCertificate=False;" +
//            "Connection Timeout=30;";

//            using var conn = new SqlConnection(connectionString);
//            conn.AccessToken = accessToken;

//            await conn.OpenAsync();


//            using var cmd = conn.CreateCommand();
//            cmd.CommandText = "SELECT * FROM MDStaff.LookupValue";

//            // execute reader
//            using var reader = await cmd.ExecuteReaderAsync();

//            // create datatable
//            var dataTable = new DataTable();

//            // load data
//            dataTable.Load(reader);

//            // now you can use it
//            foreach (DataRow row in dataTable.Rows)
//            {
//                Console.WriteLine(row[0]?.ToString());
//            }



//            // get all tables in MDStaff schema
//            var tablesCmd = conn.CreateCommand();
//            tablesCmd.CommandText = @"
//SELECT TABLE_NAME 
//FROM INFORMATION_SCHEMA.TABLES 
//WHERE TABLE_SCHEMA = 'MDStaff' AND TABLE_TYPE = 'BASE TABLE'";

//            var tables = new List<string>();

//            using (var reader1 = await tablesCmd.ExecuteReaderAsync())
//            {
//                while (await reader1.ReadAsync())
//                {
//                    tables.Add(reader1.GetString(0));
//                }
//            }

//            string filePath =@"C:\Uploads\MDStaff_Schema.txt";

//            using var writer = new StreamWriter(filePath);

//            foreach (var table in tables)
//            {
//                writer.WriteLine($"Table: {table}");
//                writer.WriteLine(new string('-', 50));

//                var cmd1 = conn.CreateCommand();
//                cmd1.CommandText = $"SELECT TOP 1 * FROM MDStaff.{table}";

//                using var reader1 = await cmd1.ExecuteReaderAsync(); // ✅ FIXED

//                var schema = reader1.GetSchemaTable();

//                foreach (DataRow row in schema.Rows)
//                {
//                    string columnName = row["ColumnName"].ToString();
//                    string dataType = row["DataType"].ToString();
//                    string sqlType = row["DataTypeName"]?.ToString();

//                    writer.WriteLine($"  {columnName}  |  {sqlType}  |  {dataType}");
//                }

//                writer.WriteLine();
//            }

//            Console.WriteLine($"Schema saved to: {filePath}");




















            if (string.IsNullOrWhiteSpace(inputTypes))
                return BadRequest("inputTypes is required.");

            // Allow comma-separated input like "VendorType,State,RegistrationStatus"
            var types = inputTypes
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToArray();

            if (types.Length == 0)
                return Ok(new Dictionary<string, List<DropdownItem>>());

            // Backwards-compatible: single type returns the list directly
            if (types.Length == 1)
            {
                var list = await _dropDownService.GetSelectList(types[0]);
                return Ok(list ?? new List<DropdownItem>());
            }

            // Multiple types: fetch concurrently and return a map { type -> items }
            var tasks = types.Select(t => _dropDownService.GetSelectList(t)).ToArray();
            var results = await Task.WhenAll(tasks);

            var map = new Dictionary<string, List<DropdownItem>>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < types.Length; i++)
            {
                map[types[i]] = results[i] ?? new List<DropdownItem>();
            }

            return Ok(map);
        }


    }
}
