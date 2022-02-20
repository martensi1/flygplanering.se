using System.Text.RegularExpressions;


namespace FPSE.Core.Common
{
    public class SimpleRegex
    {
        public static string FindPattern(string pattern, string input)
        {
            Match match = RegexMatch(pattern, input);

            bool success = match.Success;
            GroupCollection groups = match.Groups;

            if (success && groups.Count > 0)
            {
                return groups[0].Value;
            }

            return string.Empty;
        }

        public static bool PatternExists(string pattern, string input)
        {
            Match match = RegexMatch(pattern, input);
            return match.Success;
        }


        private static Match RegexMatch(string pattern, string input)
        {
            Regex regex = new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.Singleline);
            return regex.Match(input);
        }
    }
}
