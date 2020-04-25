using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.IO;
// Paxydev
// Frank Kuenneke
// Feel free to fork and do your own stuff with it :)
// Currently not in working order... relatable I guess
// Licensed under GNU GPL 3.0
namespace MagicItemGen
{
    class Program
    {
        #region GAPI Static Declares
        static readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string appName = "Fixes";
        static readonly string spreadsheetID = "1g7hTUbNkulh-Z63PMEo1-lR4B0Kw-rKX9uC54iGSVFw";
        static readonly string sheetPrefix = "Prefix List";
        static readonly string sheetSuffix = "Suffix List";
        static SheetsService service;
        #endregion
        static void Main(string[] args)
        {
            GoogleCredential credential; // credential gen from json

            using (var stream = new FileStream("ItemGen-c89894405329.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(scopes); //find and narrow scope to sheet api
            }

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName,
            }); //run api service
            Read();
        }
        static void Read() //TEMP READ FUNCTION, GEN LATER
        {
            var range = $"{sheetPrefix}!A2:A100"; //FIRST COLUMN OF PREFIXES
            var request = service.Spreadsheets.Values.Get(spreadsheetID, range);
            var response = request.Execute();
            var values = response.Values; //USE FOR VALUES

            if (values != null && values.Count >0)
            {
                foreach ( var row in values)
                {
                    Console.WriteLine("{0} | {1} | {2}", row[0], row[1], row[2]); //GEN RANDOM HERE
                }
            }
            else
            {
                Console.WriteLine("No Data");
            }
        }
    }
}
