using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// Paxydev
// Frank Kuenneke
// Feel free to fork and do your own stuff with it :)
// Currently not in working order... relatable I guess
// Licensed under GNU GPL 3.0
//UPDATE 5/5/20 : Now generates properly locally. Next Pipeline is roll20API
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
        static readonly string tempweap = "TEMPWEAPON";
        #endregion
        static void Main(string[] args)
        {
        Random rand = new Random();
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
            ReadPrefix(rand.Next(0, 19));
            Console.WriteLine(tempweap);
            ReadSuffix(rand.Next(0, 19));
        }
        private static void ReadPrefix(int n)
        {
            var range = $"{sheetPrefix}!A2:B21"; //FIRST COLUMN OF PREFIXES
            var rangeDesc = $"{sheetPrefix}!B2:B21"; //FIRST COLUMN OF PREFIXES DESCRIPTIONS
            var request = service.Spreadsheets.Values.Get(spreadsheetID, range);
            var response = request.Execute();
            var values = response.Values; //USE FOR VALUES
            var value = values[n]; // GEN THROUGH HERE

            if (values != null && values.Count > 0)
            {
                    Console.WriteLine("{0} | DESCRIPTION: {1}", value[0].ToString(), value[1].ToString()); //GEN RANDOM HERE
            }
            else
            {
                Console.WriteLine("No Data");
            }
        }
        private static void ReadSuffix(int n)
        {
            var range = $"{sheetSuffix}!A2:B21"; //FIRST COLUMN OF SUFFIXES
            var rangeDesc = $"{sheetSuffix}!B2:B21"; //FIRST COLUMN OF SUFFIXES DESCRIPTIONS
            var request = service.Spreadsheets.Values.Get(spreadsheetID, range);
            var response = request.Execute();
            var values = response.Values; //USE FOR VALUES
            var value = values[n];

            if (values != null && values.Count > 0)
            {
                
                    Console.WriteLine("{0} | DESCRIPTION: {1}", value[0].ToString(), value[1].ToString()); //GEN RANDOM HERE
            }
            else
            {
                Console.WriteLine("No Data");
            }
        }

        
    }
}
