namespace Humanizer.Localisation.NumberToWords
{
    using System;
    using System.Collections.Generic;
    
    internal class CatalanNumberToWordsConverter : GenderedNumberToWordsConverter
    {
        private static readonly string[] UnitsMap = { "zero", "un", "dos", "tres", "quatre", "cinc", "sis", "set", "vuit", "nou", "deu", "onze", "dotze",
                                                        "tretze", "catorze", "quinze", "setze", "disset", "divuit", "dinou", "vint", "vint-i-un",
                                                        "vint-i-dos", "vint-i-tres", "vint-i-quatre", "vint-i-cinc", "vint-i-sis", "vint-i-set", "vint-i-vuit", "vint-i-nou"};
        private const string Feminine1 = "una";
        private const string Feminine21 = "vint-i-una";
        private static readonly string[] TensMap = { "zero", "deu", "vint", "trenta", "quaranta", "cinquanta", "seixanta", "setanta", "vuitanta", "noranta" };
        private static readonly string[] HundredsMap = { "zero", "cent", "dos-cents", "tres-cents", "quatre-cents", "cinc-cents", "sis-cents", "set-cents", "vuit-cents", "nou-cents" };
        private static readonly string[] FeminineHundredsMap = { "zero", "cent", "dos-cents", "tres-cents", "quatre-cents", "cinc-cents", "sis-cents", "set-cents", "vuit-cents", "nou-cents" };
        private static readonly string[] TensMapOrdinal = { "", "desè", "vintè", "trentè", "quarantè", "ciquantè", "seixantè", "setantè", "vuitantè", "noratè" };
        private static readonly string[] HundredsMapOrdinal = { "", "centè", "dos-centè", "tres-cents", "quatre-cents", "cinc-cents", "sis-cents", "set-cents", "vuit-cents", "nou-cents" };
        private static readonly string[] ThousandsMapOrdinal = { "", "milè", "mil dos-centè", "mil tres-cents", "mil quatre-cents", "mil cinc-cents", "mil sis-cents", "mil set-cents", "mil vuit-cents", "mil nou-cents" };
        private static readonly Dictionary<int, string> Ordinals = new Dictionary<int, string>
        {
            {1, "primer"},
            {2, "segon"},
            {3, "tercer"},
            {4, "quart"},
            {5, "cinquè"},
            {6, "sisè"},
            {7, "setè"},
            {8, "vuitè"},
            {9, "novè"},
        };

        public override string Convert(long input, GrammaticalGender gender, bool addAnd = true)
        {
            return Convert(input, WordForm.Normal, gender, addAnd);
        }

        public override string Convert(long number, WordForm wordForm, GrammaticalGender gender, bool addAnd = true)
        {
            if (number == 0)
            {
                return "zero";
            }

            if (number < 0)
            {
                return string.Format("menys {0}", Convert(Math.Abs(number)));
            }

            var parts = new List<string>();

            if ((number / 1000000000) > 0)
            {
                parts.Add(number / 1000000000 == 1
                    ? "mil milions"
                    : string.Format("{0} mil milions", Convert(number / 1000000000)));

                number %= 1000000000;
            }

            if ((number / 1000000) > 0)
            {
                parts.Add(number / 1000000 == 1
                    ? "un milió"
                    : string.Format("{0} milions", Convert(number / 1000000)));

                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                parts.Add(number / 1000 == 1
                    ? "mil"
                    : string.Format("{0} mil", Convert(number / 1000, gender)));

                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                parts.Add(number == 100
                    ? "cent"
                    : gender == GrammaticalGender.Feminine
                        ? FeminineHundredsMap[(number / 100)]
                        : HundredsMap[(number / 100)]);
                number %= 100;
            }

            if (number > 0)
            {
                if (number < 30)
                {
                    if (gender == GrammaticalGender.Feminine && (number == 1 || number == 21))
                    {
                        parts.Add(number == 1 ? Feminine1 : Feminine21);
                    }
                    else
                    {
                        parts.Add(UnitsMap[number]);
                    }
                }
                else
                {
                    var lastPart = TensMap[number / 10];
                    var units = number % 10;
                    if (units == 1 && gender == GrammaticalGender.Feminine)
                    {
                        lastPart += " una";
                    }
                    else if (units > 0)
                    {
                        lastPart += string.Format("-{0}", UnitsMap[number % 10]);
                    }

                    parts.Add(lastPart);
                }
            }

            return string.Join(" ", parts.ToArray());
        }

        public override string ConvertToOrdinal(int number, GrammaticalGender gender)
        {
            var parts = new List<string>();

            if (number > 9999) // @mihemihe: Implemented only up to 9999 - Dec-2017
            {
                return Convert(number, gender);
            }

            if (number < 0)
            {
                return string.Format("menys {0}", Convert(Math.Abs(number)));
            }

            if (number == 0)
            {
                return string.Format("zero");
            }

            if ((number / 1000) > 0)
            {
                var thousandsPart = ThousandsMapOrdinal[(number / 1000)];
                if (gender == GrammaticalGender.Feminine)
                {
                    thousandsPart = thousandsPart.TrimEnd('o') + "a";
                }

                parts.Add(thousandsPart);
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                var hundredsPart = HundredsMapOrdinal[(number / 100)];
                if (gender == GrammaticalGender.Feminine)
                {
                    hundredsPart = hundredsPart.TrimEnd('o') + "a";
                }

                parts.Add(hundredsPart);
                number %= 100;
            }

            if ((number / 10) > 0)
            {
                var tensPart = TensMapOrdinal[(number / 10)];
                if (gender == GrammaticalGender.Feminine)
                {
                    tensPart = tensPart.TrimEnd('o') + "a";
                }

                parts.Add(tensPart);
                number %= 10;
            }

            if (Ordinals.TryGetValue(number, out var towords))
            {
                if (gender == GrammaticalGender.Feminine)
                {
                    towords = towords.TrimEnd('o') + "a";
                }

                parts.Add(towords);
            }

            return string.Join(" ", parts.ToArray());
        }
    }
}
