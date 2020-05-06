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
        static SheetsService service;
        static readonly string appName = "Fixes";
        static readonly string spreadsheetID = "1g7hTUbNkulh-Z63PMEo1-lR4B0Kw-rKX9uC54iGSVFw";
        static readonly string sheetPrefix = "Prefix List";
        static readonly string sheetSuffix = "Suffix List";
        static readonly string sheetWeapType = "Weapon List";
        static readonly string fifthEditionWeaponCPDM = "https://roll20.net/compendium/dnd5e/Weapons#content";
        static private Random rand = new Random();
        #endregion
        public static string GetItem()
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

             string[] prefix = ReadAffix(rand.Next(0, 19), 0);
             string[] suffix = ReadAffix(rand.Next(0, 19), 1); 
             string[] weaponType = GetWeaponType();

            return prefix[0] + weaponType[0] + suffix[0] + "\n\n" + 
                prefix[0] + ": " + prefix[1] + "\n\n" +
                suffix[0] + ": " + suffix[1] + "\n\n" +
                weaponType[1];
        }

        private static string[] GetWeaponType()
        {
            int weapArchetype = rand.Next(1, 4);
            var range = "";
            int n = 0;
            switch (weapArchetype)
            {
                case 1: //SIMPLE MELEE
                    range = $"{sheetWeapType}!A2:A11";
                    n = 10;
                    break;
                case 2: //SIMPLE RANGED
                    range = $"{sheetWeapType}!B2:B5";
                    n = 4;
                    break;
                case 3: //MARTIAL MELEE
                    range = $"{sheetWeapType}!C2:C19";
                    n = 18;
                    break;
                case 4: //MARTIAL RANGED
                    range = $"{sheetWeapType}!D2:D6";
                    n = 5;
                    break;
            }

            var request = service.Spreadsheets.Values.Get(spreadsheetID, range);
            var values = request.Execute().Values; //USE FOR VALUES

            switch (values.Count)
            {
                case 0:
                    return new[] { "ERROR, UNABLE TO ACCESS ITEM TYPES" };

                default:
                    var value = values[rand.Next(0, n)]; // GEN THROUGH HERE
                    return new[] { " " + value[0].ToString() + " ", fifthEditionWeaponCPDM };
            }
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
