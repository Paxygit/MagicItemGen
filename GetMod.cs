using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.IO;

namespace MagicItemGen
{
    public class GetMod
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
        public static string GetItem()
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

             string[] prefix = ReadAffix(rand.Next(0, 19), 0);
             string[] suffix = ReadAffix(rand.Next(0, 19), 1);
            return prefix[0] + tempweap + suffix[0] + "\n\n" + 
                prefix[0] + ": " + prefix[1] + "\n\n" +
                suffix[0] + ": " + suffix[1] + "\n\n";
        }
        /// <summary>
        /// Pass your desired index into n, and 0 for suffix or 1 for prefix into suffixOrPrefix.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="suffixOrPrefix"></param>
        /// <returns></returns>
        private static string[] ReadAffix(int n, int suffixOrPrefix)
        {
            var range = "";
            switch (suffixOrPrefix)
            {
                case 0:
                    range = $"{sheetPrefix}!A2:B21"; //FIRST COLUMN OF PREFIXES
                    break;
                case 1:
                    range = $"{sheetSuffix}!A2:B21"; //FIRST COLUMN OF PREFIXES
                    break;
            }

            var request = service.Spreadsheets.Values.Get(spreadsheetID, range);
            var values = request.Execute().Values; //USE FOR VALUES
            var value = values[n]; // GEN THROUGH HERE
            switch (values.Count)
            {
                case 0:
                    return new[] { "Error Generating Prefix" };

                default:          //Affix at generated line | Description
                    return new[] { value[0].ToString() + " ", value[1].ToString() + " "};
            }

        }
    }
}
