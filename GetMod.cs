using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
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
        static readonly string sheetDropRate = "DROPRATE TO CR";
        static readonly string fifthEditionWeaponCPDM = "https://roll20.net/compendium/dnd5e/Weapons#content";
        static private Random rand = new Random();
        #endregion
        public static string GetItem(int cr)
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

            #region Rarity Stuff
            var range = $"{sheetDropRate}!A{cr}:B{cr}";
            var request = service.Spreadsheets.Values.Get(spreadsheetID, range);
            var values = request.Execute().Values; //USE FOR VALUES
            var value = values[0]; // GEN THROUGH HERE
            #endregion
            string[] weaponType = GetWeaponType();
            int[] rarity = dropRarity(value);
            switch (rarity[0])
            {
                case 1: // MAGIC
                    return GenerateMagic(rarity[1], Convert.ToInt32(value[0]), weaponType);                               

                case 2: // RARE
                    return GenerateRare(rarity[1], Convert.ToInt32(value[1]), weaponType);

                case 0: // RARE
                    Console.WriteLine("Normal");
                    return "Normal " + weaponType;
            }
            return "ERROR GENERATING ITEM RARITY";
        }

        /// <summary>
        /// First is Switch index (0 for normal, 1 for magic, 2 for rare), second is the generated random number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int[] dropRarity(IList<object> value)
        {
            int localRand = rand.Next(1, 100);

            if ((localRand <= Convert.ToInt32(value[0]))) {return new[] { 1, localRand };}//MAGIC

            else if (localRand <= Convert.ToInt32(value[1])){return new[] { 2, localRand };}//RARE

            else {return new[] { 0, localRand };}//NORMAL
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
        /// 1-100 roll, upperBound is the limit at which the magic item roll threshold is, weaponType is the generated weaponType
        /// </summary>
        /// <param name="roll"></param>
        /// <param name="upperBound"></param>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        private static string GenerateMagic(int roll, int upperBound, string[] weaponType)
        {
            string[] prefixOne = ReadAffix(rand.Next(0, 19), 0);
            string[] suffixOne = ReadAffix(rand.Next(0, 19), 1);

            return prefixOne[0] + weaponType[0] + suffixOne[0] + "\n\n" +    // Prefix Weapon of Suffixing
              prefixOne[0] + ": " + prefixOne[1] + "\n\n" +                 //Prefix: Description
              suffixOne[0] + ": " + suffixOne[1] + "\n\n" +                //Suffix: Description
              weaponType[1];                                              //Link to Roll20 Weapon Compendium
        }
        /// <summary>
        /// 1-100 roll, upperBound is the limit at which the rare item roll threshold is, weaponType is the generated weaponType
        /// </summary>
        /// <param name="roll"></param>
        /// <param name="upperBound"></param>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        private static string GenerateRare(int roll, int upperBound, string[] weaponType)
        {
            string[] prefixOne = ReadAffix(rand.Next(0, 19), 0);
            string[] suffixOne = ReadAffix(rand.Next(0, 19), 1);
            string[] prefixTwo = new[] { "" };
            string[] suffixTwo = new[] { "" };
            int belowBound = upperBound - roll;
            if (belowBound > 0)
            {
                belowBound = belowBound / 2;
                if (belowBound <= 10) { prefixTwo = ReadAffix(rand.Next(0, 19), 0); }; //Moderate Roll? Extra Prefix
                if (belowBound <= 5) { suffixTwo = ReadAffix(rand.Next(0, 19), 1); }; //High Roll? Extra Suffix

                
            }
            string rareName = "Temp Name, " + weaponType[0] + "\n\n";
            rareName = rareName + prefixOne[0] + ": " + prefixOne[1] + "\n\n";
            if (prefixTwo[0] != "") { rareName = rareName + prefixTwo[0] + ": " + prefixTwo[1] + "\n\n"; }
            rareName = rareName + suffixOne[0] + ": " + suffixOne[1] + "\n\n";
            if (suffixTwo[0] != "") { rareName = rareName + suffixTwo[0] + ": " + suffixTwo[1] + "\n\n"; }

            return rareName + weaponType[1];                                // + Link to Roll20 Weapon Compendium
        }

        /// <summary>
        /// Pass your desired index into n, and 1 for suffix or 0 for prefix into suffixOrPrefix.
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
                    return new[] { value[0].ToString() + " ", value[1].ToString() + " " };
            }

        }

    }
}
