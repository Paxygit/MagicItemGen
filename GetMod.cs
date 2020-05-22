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
        static readonly string sheetPrefixWeapon = "Prefix List Weapon";
        static readonly string sheetSuffixWeapon = "Suffix List Weapon";
        static readonly string sheetPrefixArmor = "Prefix List Armor";
        static readonly string sheetSuffixArmor = "Suffix List Armor";
        static readonly string sheetPrefixJewelry = "Prefix List Jewelry";
        static readonly string sheetSuffixJewelry = "Suffix List Jewelry";
        static readonly string sheetPrefixPotion = "Prefix List Potion";
        static readonly string sheetSuffixPotion = "Suffix List Potion";
        static string itemType = string.Empty;
        static readonly string sheetWeapType = "Weapon List";
        static readonly string sheetArmorType = "Armor List";
        static readonly string sheetJewelryType = "Jewelry List"; //INCLUDES BELTS
        static readonly string sheetDropRate = "DROPRATE TO CR";
        static readonly string fifthEditionWeaponCPDM = "https://roll20.net/compendium/dnd5e/Weapons#content"; 
        static readonly string fifthEditionArmorCPDM = "https://roll20.net/compendium/dnd5e/Armor#content";
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

            #region Rarity Declarations
            cr++; //+1 DUE TO TITLE IN GOOGLE SHEET TABLE
            if (cr <= 1)  { return "Error: CR entered was below 1!";} //bad input handle

            var range = $"{sheetDropRate}!A{cr}:B{cr}";
            var request = service.Spreadsheets.Values.Get(spreadsheetID, range);
            var values = request.Execute().Values; //USE FOR VALUES
            var value = values[0]; // GEN THROUGH HERE
            #endregion

            string[] itemType = GetItemType();
            int[] rarity = dropRarity(value);
            switch (rarity[0])
            {
                case 1: // MAGIC
                    return GenerateMagic(rarity[1], Convert.ToInt32(value[0]), itemType) + "\nMAGIC";                               

                case 2: // RARE
                    return GenerateRare(rarity[1], Convert.ToInt32(value[1]), itemType) + "\nRARE";

                case 0: //NO DROP
                    return "\n\n\n\n" + "NONE"; //ADD LINES FOR FORMATTING
            } //read dropRarity output
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

            else { return new[] { 0, localRand }; }//NO DROP LOL
        }

        private static string[] GetItemType()
        {
            int itemRoll = rand.Next(1, 7); 
            var range = string.Empty;
            int n = 0;
            string localLink = string.Empty;
            switch (itemRoll)
            {
                case 1: //SIMPLE MELEE OR RANGED
                    itemRoll = rand.Next(1, 2);
                    if (itemRoll == 1) { range = $"{sheetWeapType}!A2:A11"; n = 10; }
                    else { range = $"{sheetWeapType}!B2:B5"; n = 4; }
                    itemType = "Weapon";
                    localLink = fifthEditionWeaponCPDM;
                    break;

                case 2: //MARTIAL MELEE
                    range = $"{sheetWeapType}!C2:C19";
                    itemType = "Weapon";
                    n = 18;
                    localLink = fifthEditionWeaponCPDM;
                    break;

                case 3: //MARTIAL RANGED
                    range = $"{sheetWeapType}!D2:D10";
                    itemType = "Weapon";
                    n = 9;
                    localLink = fifthEditionWeaponCPDM;
                    break;

                case 4: //ARMORS 
                    range = $"{sheetArmorType}!A2:A14";
                    itemType = "Armor";
                    n = 13;
                    localLink = fifthEditionArmorCPDM;
                    break;

                case 5: //ARMORS (DUPLICATE FOR BETTER DROPRATE)
                    range = $"{sheetArmorType}!A2:A14";
                    itemType = "Armor";
                    n = 13;
                    localLink = fifthEditionArmorCPDM;
                    break;

                case 6: //JEWELRY
                    range = $"{sheetJewelryType}!A2:A5";
                    itemType = "Jewelry";
                    n = 4;
                    break;

                case 7: //POTION
                    itemType = "Potion";
                    return new[] { " " + itemType + " ", localLink };
            }
            var request = service.Spreadsheets.Values.Get(spreadsheetID, range);
             var values = request.Execute().Values; //USE FOR VALUES

            switch (values.Count)
            {
                case 0:
                    return new[] { "ERROR, UNABLE TO ACCESS ITEM TYPES" };

                default:
                    var value = values[rand.Next(0, n)]; // GEN THROUGH HERE
                    return new[] { " " + value[0].ToString() + " ", localLink };
            } //GENERATES WHAT TYPE OF ITEM IT IS
        }

        /// <summary>
        /// 1-100 roll, upperBound is the limit at which the magic item roll threshold is, item is the generated item
        /// </summary>
        /// <param name="roll"></param>
        /// <param name="upperBound"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string GenerateMagic(int roll, int upperBound, string[] item)
        {
            string[] prefixOne = ReadAffix(rand.Next(0, 19), 0); //EXPAND FOR ANY PREFIX OR SUFFIX ADDITION UPDATES
            string[] suffixOne = ReadAffix(rand.Next(0, 19), 1);

            return prefixOne[0] + item[0] + suffixOne[0] + "\n\n" +    // Prefix Weapon of Suffixing
              prefixOne[0] + ": " + prefixOne[1] + "\n\n" +                 //Prefix: Description
              suffixOne[0] + ": " + suffixOne[1] + "\n\n" +                //Suffix: Description
              item[1];                                              //Link to Roll20 Weapon Compendium
        }
        /// <summary>
        /// 1-100 roll, upperBound is the limit at which the rare item roll threshold is, item is the generated item
        /// </summary>
        /// <param name="roll"></param>
        /// <param name="upperBound"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string GenerateRare(int roll, int upperBound, string[] item)
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
            string rareNameAffixes = GenerateRareName();
            string rareName = rareNameAffixes + item[0] + "\n\n";
            rareName = rareName + prefixOne[0] + ": " + prefixOne[1] + "\n\n";
            if (prefixTwo[0] != "") { rareName = rareName + prefixTwo[0] + ": " + prefixTwo[1] + "\n\n"; }
            rareName = rareName + suffixOne[0] + ": " + suffixOne[1] + "\n\n";
            if (suffixTwo[0] != "") { rareName = rareName + suffixTwo[0] + ": " + suffixTwo[1] + "\n\n"; }

            return rareName + item[1];                                // + Link to Roll20 Weapon Compendium
        }
/*
        private static string GeneratePotion(int roll, int upperBound, string[] item)
        {
            string[] prefixOne = ReadAffix(rand.Next(0, 12), 0);
            string[] suffixOne = ReadAffix(rand.Next(0, 12), 1);                                        


            return prefixOne[0] + item[0] + suffixOne[0] + "\n\n" +    // Prefix Weapon of Suffixing
              prefixOne[0] + ": " + prefixOne[1] + "\n\n" +                 //Prefix: Description
              suffixOne[0] + ": " + suffixOne[1] + "\n\n" +                //Suffix: Description
              item[1];                                              //Link to Roll20 Weapon Compendium
        } */ //DEPRECATED GENERATE POTION FUNCTION, POTIONS NOW ON NORMAL DROP TABLES

        private static string GenerateRareName()
        {
            var rangePrefix = $"{sheetPrefixWeapon}!C2:C41";//P
            var rangeSuffix = $"{sheetSuffixWeapon}!C2:C41";//S
            var requestPrefix = service.Spreadsheets.Values.Get(spreadsheetID, rangePrefix);//P
            var requestSuffix = service.Spreadsheets.Values.Get(spreadsheetID, rangeSuffix);//S
            var valuesPrefix = requestPrefix.Execute().Values; //P
            var valuesSuffix = requestSuffix.Execute().Values; //S

            //                          SEND GENERATED PREFIX                 SEND GENERATED SUFFIX
            return  valuesPrefix[rand.Next(1,40)][0].ToString() + " " + valuesSuffix[rand.Next(1,40)][0].ToString();
        }

        /// <summary>
        /// Pass your desired index into n, and 1 for suffix or 0 for prefix into suffixOrPrefix.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="suffixOrPrefix"></param>
        /// <returns></returns>
        private static string[] ReadAffix(int n, int AffixType)
        {
            var range = "";
            switch (itemType)
            {
                case "Weapon":
                    range = ReadType(AffixType, new[] { sheetPrefixWeapon, sheetSuffixWeapon}); 
                    break;

                case "Armor":
                    range = ReadType(AffixType, new[] { sheetPrefixArmor, sheetSuffixArmor });
                    break;

                case "Jewelry":
                    range = ReadType(AffixType, new[] { sheetPrefixJewelry, sheetSuffixJewelry }); 
                    break;

                case "Potion":
                    range = ReadType(AffixType, new[] { sheetPrefixPotion, sheetSuffixPotion });
                    n = rand.Next(0, 12);
                    break;
            }
            
            var request = service.Spreadsheets.Values.Get(spreadsheetID, range);
            var values = request.Execute().Values; //USE FOR VALUES
            var value = values[n]; // GEN THROUGH HERE
            switch (values.Count)
            {
                case 0:
                    return new[] { "Error Generating Affix" };

                default:          //Affix at generated line | Description
                    return new[] { value[0].ToString() + " ", value[1].ToString() + " " };
            }

        }
        /// <summary>
        /// Pass in AffixType, range to return, prefix at index 0 and suffix at index 1 to cast into range.
        /// </summary>
        /// <param name="AffixType"></param>
        /// <param name="range"></param>
        /// <param name="prefixSuffix"></param>
        /// <returns></returns>
        private static string ReadType(int AffixType, string[] prefixSuffix)
        {
            string cellRange = string.Empty;
            if (prefixSuffix[0] == sheetPrefixPotion) { cellRange = "A2:B13"; } //LESS AFFIXES FOR POTIONS
            else { cellRange = "A2:B21"; } //IF NOT A POTION

                switch (AffixType)
            {
                case 0:
                    return $"{prefixSuffix[0]}!{cellRange}"; //FIRST COLUMN OF PREFIXES
                case 1:
                    return $"{prefixSuffix[1]}!{cellRange}"; //FIRST COLUMN OF SUFFIXES
            }
            return "ERROR";
        }
    }
}
