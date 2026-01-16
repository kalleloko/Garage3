using System.Text.RegularExpressions;

namespace Garage3.Helpers
{
    /// <summary>
    /// Handling of Swedish SSN's ("personnummer").
    /// </summary>
    /// <remarks>
    /// Only persons born during the 20th and 21th century are handled. (There are no persons alive
    /// anymore from earlier centuries.)
    /// </remarks>
    public static partial class SSNHandler
    {
        /// <summary>
        /// Validates an SSN.
        /// </summary>
        /// <remarks>
        /// We allow these five SSN formats: YYYYMMDD-NNNC (13 characters), YYYYMMDDNNNC (12 characters),
        /// YYMMDD-NNNC (11 characters), YYMMDD+NNNC (person is >= 100 years) (11 characters) and
        /// YYMMDDNNNC (10 characters).
        /// </remarks>
        public static bool IsValid(string SSN)
        {
            return SSNRegex().IsMatch(SSN) &&
                IsValidDate(SSN) &&
                CalculateCheckDigitAccordingToLuhnAlgorithm(SSN) == char.GetNumericValue(SSN[^1]) &&
                !(SSN.Length >= 12 && BirthDateInFuture(SSN)) &&
                !(SSN.Contains('+') && CenturyForSSNWithoutCentury(SSN) == "18");
        }

        /// <summary>
        /// Returns the age of the person with the SSN in full/completed years if the SSN is valid, otherwise zero.
        /// </summary>
        public static int AgeOfPerson(string SSN)
        {
            if (!IsValid(SSN))
                return 0;
            string birthDate = DateFromSSN(SSN);
            string today = DateTime.Today.ToString("yyyyMMdd");
            int completedYears = int.Parse(today[..4]) - int.Parse(birthDate[..4]);
            if (string.Compare(birthDate[^4..], today[^4..]) > 0)
                completedYears--;
            return completedYears;
        }

        private static bool BirthDateInFuture(string SSNWithCentury)
        {
            return DateTime.Today <
                DateTime.ParseExact(SSNWithCentury[..8], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static int CalculateCheckDigitAccordingToLuhnAlgorithm(string SSN)
        {
            string digits = SSN.Replace("-", string.Empty).Replace("+", string.Empty);
            if (digits.Length >= 12)
                digits = digits[2..];
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int product = (int)char.GetNumericValue(digits[i]) * (i % 2 == 0 ? 2 : 1);
                sum += product > 9 ? product - 9 : product;
            }
            return (10 - (sum % 10)) % 10;
        }

        private static string CenturyForSSNWithoutCentury(string SSNWithoutCentury)
        {
            var todaysDateWithoutCentury = DateTime.Today.ToString("yyMMdd");
            var dateInSSNWithoutCentury = SSNWithoutCentury[..6];
            return string.Compare(dateInSSNWithoutCentury, todaysDateWithoutCentury) > 0
                ? (SSNWithoutCentury.Contains('+') ? "18" : "19")
                : (SSNWithoutCentury.Contains('+') ? "19" : "20");
        }

        private static string DateFromSSN(string SSN)
        {
            return SSN.Length >= 12
                ? SSN[..8]
                : string.Concat(CenturyForSSNWithoutCentury(SSN), SSN.AsSpan(0, 6));
        }

        private static bool IsValidDate(string SSN)
        {
            return DateTime.TryParseExact(
                DateFromSSN(SSN),
                "yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime _);
        }

        [GeneratedRegex(@"^((19|20)\d{6}\-?\d{4}|\d{6}[-+]?\d{4})$")]
        private static partial Regex SSNRegex();
    }
}
