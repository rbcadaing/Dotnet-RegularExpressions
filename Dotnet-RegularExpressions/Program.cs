using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dotnet_RegularExpressions
{
    class Program
    {
        static void Main(string[] args)
        {
            //PatternMaching();
            //StringSplitting();
            //GroupingAndSubstitution();
            //AnchorsAndBoundaries();
            RegularExpressionOptions();
        }

        /// <summary>
        /// <quantifier>
        ///     *  : Matches the previous element zero or more times.
        ///     +  : Matches the previous element one or more times.
        ///     ?  : Matches the previous element zero or one time.
        /// </quantifier>
        /// </summary>
        private static void PatternMaching()
        {

            var patterns = new List<string> { "a*b", "a+b", "a?b" };
            var inputs = new List<string> { "a", "b", "ab", "aab", "abb" };

            patterns.ForEach(pattern =>
            {
                Console.WriteLine("Regular expression: {0}", pattern);
                var regex = new Regex(pattern);
                inputs.ForEach(input =>
                {
                    Console.WriteLine("\tInput pattern: {0}", input);
                    var results = regex.Matches(input);
                    if (results.Count <= 0)
                        Console.WriteLine("\t\tNo Matches Found");
                    foreach (Match result in results)
                        Console.WriteLine("\t\tMatch found at index {0}. Length: {1}", result.Index, result.Length);
                });

            });

            Console.ReadKey();
        }

        /// <summary>
        /// Example Character classes
        ///  \w : Matches any wor character
        ///  \W : Matches any non-word character
        ///  \d : Matches any decimal digit
        ///  
        /// Alternation Construct
        ///  | : Matches any one element separated by vertical bar character (|)
        ///  (?(expression)yes|no) : Matches yes if the regex designated by expression matches.
        ///                          Otherwise, matches the optional no part. the regex engine
        ///                          does not advance the input stream after it evaluates expressions
        ///                          
        /// Anchoring : preventing evaluating morethan the characters anchored
        /// anchor syntax ^ and $
        /// 
        /// </summary>
        private static void StringSplitting()
        {
            var patterns = new List<string>
            {
                @"^\d\d\d-\d\d\d-\d\d\d\d$"
            };

            var inputs = new List<string>
            {
                "5555555555",
                "(555)-555-5555",
                "555-555-5555",
                "555-555-555a",
                "5555-555-5555",
                "555-5555555",
                "000-000-0000",
                "a",
                "5.55.555.5555",
                "...-...-...."
            };

            patterns.ForEach(pattern =>
            {
                Console.WriteLine("Regular expression: {0}", pattern);
                var regex = new Regex(pattern);
                inputs.ForEach(input =>
                {
                    Console.WriteLine("\tInput pattern: {0}", input);
                    var isMatch = regex.IsMatch(input);
                    Console.WriteLine("\t\t{0}", isMatch ? "Accepted" : "Rejected");
                    if (!isMatch)
                        return;
                    var splits = Regex.Split(input, @"-\d\d\d-").ToList();
                    Console.WriteLine("\t\t\tArea code: {0}", splits[0]);
                    Console.WriteLine("\t\t\tLast 4 digits: {0}", splits[1]);
                });
            });
            Console.ReadKey();
        }

        /// <summary>
        /// Example Grouping Conctruct
        /// (subexpression) Captures the matched subexpression and assigns it a one based ordinal number  "(\d)x(\d)F" matches "0x1F"
        /// (?<name>subexpression) Captures the matched subexpression into a named group "(?<One>\d)x(?<two>\d)F" matches "0x1F"
        /// (?:subexpression) Defines a non-capturing group. "(?:\d)x(?:\d)F" matches "0x1F"
        /// 
        /// Exmaple Backreference Constructs
        ///  \number : Backreference matches the value of numbered subexpression "(\d)x\1F" matches "0x0F"
        ///  \k<name> : Named backreference matches the value of named expression. "(?<one>\d)x\k<one>F" matches "0x0F"
        /// </summary>
        private static void GroupingAndSubstitution()
        {
            var patterns = new List<string>
            {
                @"([A-Za-z]+).*\$(\d+.\d+)"
            };

            var inputs = new List<string>
            {
                @"
                    |----------------------|
                    | Receipt from         |
                    | Alexandru's shop     |
                    |                      |
                    | Thanks for shopping! |
                    |---------|------------|
                    |  Item   | Price $USD |
                    |---------|------------|
                    | Shoes   |   $47.99   |
                    | Cabbage |    $2.99   |
                    | Carrots |    $1.23   |
                    | Chicken |    $9.99   |
                    | Beef    |   $12.47   |
                    | Shirt   |    $5.97   |
                    | Salt    |    $2.99   |
                    |---------|------------|"
            };
            patterns.ForEach(pattern =>
            {
                Console.WriteLine("Regular expression: {0}", pattern);
                var regex = new Regex(pattern);
                inputs.ForEach(input =>
                {
                    Console.WriteLine("\tInput pattern: {0}", input);
                    var matches = regex.Matches(input);
                    if (matches.Count <= 0)
                        Console.WriteLine("\t\tNo matches found.");

                    foreach (Match match in matches)
                    {
                        Console.WriteLine("\t\tMatch at index {0} with length {1}", match.Index, match.Length);
                        foreach (Group group in match.Groups)
                        {
                            Console.WriteLine("\t\t\tGroup at index {0} has value {1}", group.Index, group.Value);
                        }
                    }
                    Console.WriteLine("Simple replacement results: {0}",
                        Regex.Replace(input, @"(Chicken)(.*) \$(9.99)", @"$1$2 $$0.00"));

                    var results = Regex.Replace(input, pattern, (match) =>
                    {
                        if (match.Groups[1].Value == "Chicken")
                            return match.Value.Replace(match.Groups[2].Value, "0.00");
                        return match.Value;
                    });
                    Console.WriteLine("Advance replacement results: {0}", results);

                });
            });
            Console.ReadKey();
        }
        /// <summary>
        /// Example Anchors
        /// ^ : By default, the match must start at the beginning of the string in multiline mode it must start at the beginning of the line
        /// $ : By default the match must occur at the end of the string or before \n at the 
        ///      end of the string: in multiline mode it must occur before the end of the line or before 
        ///      \n at the end of the line
        ///      
        /// Example Boundaries
        /// \b : The match must occur on a boundary between a \w (alphanumeric) and a \W
        ///      (nonalphanumeric) character
        /// \B : the match mus not occur on a \b boundary
        /// </summary>
        private static void AnchorsAndBoundaries()
        {
            var patterns = new List<string>
            {
                @"\b",
                @"\B",
                @"^hi",
                @"hi$"
            };

            var inputs = new List<string>
            {
                "a b",
                "a",
                " ",
                "",
                "hi",
                " hi",
                " hi",
                "him",
                " him",
                "him "
            };

            patterns.ForEach(pattern =>
            {
                Console.WriteLine("Regular expression: {0}", pattern);
                var regex = new Regex(pattern);
                inputs.ForEach(input =>
                {
                    Console.WriteLine("\tInput pattern: {0}", input);
                    var results = regex.Matches(input);
                    if (results.Count <= 0)
                        Console.WriteLine("\t\tNo Matches found");
                    foreach (Match result in results)
                        Console.WriteLine("\t\tMatch Found at index {0}. Length: {1}.", result.Index, result.Length);
                });
            });

            Console.ReadKey();
        }
        /// <summary>
        /// Exmaple Regex Options
        /// i : use case-incensitive matching "H(?i)e(?-i)y" matches "Hey", " "HEy" mismatches "hey", "HEY"
        /// m : use multiline mode. ^ and $ match the beginning and end of a line, instead of the beginning and end of a string
        ///     "(?m)^hey$" matches "hey\nhey","hey" mismatches "hey\n\ hey","hey "
        /// x : ignore unescaped white space in the regular expression pattern
        ///     "(?x) \r\n h e y" matches "hey"," hey " mismatches "Hey", "HEy"
        ///     
        /// Example of Miscellaneous Constructs
        /// (?imnsx-imnsx) : sets or disable options such as case incensitivity in the middle of a pattern
        ///                   "H(?i)e(?-i)y" matches "Hey","HEy" mistchaes  mismatches
        ///                   "hey, "HEY"
        /// (?# Comment)   : Inline comment . the comment ens at the first clossing parenthesis
        ///                  "He(?# this is an inline comment....)y" maches "Hey" mismatches "hey, "HEY","HEy"
        ///  # Comment     : X-mode comment. The comment starts at an unescaped # and continues to the end of the line.
        ///                  "(?x)Hey#this is a comment" matches "Hey" mismatches "hey","HEY","HEy"
        /// </summary>
        private static void RegularExpressionOptions()
        {
            var patterns = new List<string>
            {
                "(?x)Hey#this is a comment",
                "He(?# this is an inline comment....)y",
                "H(?i)e(?-i)y",
                @"(?m)^hey$",
                "(he)y",
                "(?n)(he)(?-n)y",
                "(?x) \r\n h e y"
            };

            var inputs = new List<string>
            {
                "hey\nhey",
                " hey\nhey",
                " hey\n hey",
                "Hey",
                "hey",
                "HEy",
                "HEY",
                " hey",
                "hey ",
                " hey "
            };
            patterns.ForEach(pattern =>
            {
                Console.WriteLine("Regular expression: \"{0}\"", pattern);
                var regex = new Regex(pattern);
                inputs.ForEach(input =>
                {
                    Console.WriteLine("\tInput pattern: \"{0}\"", input);
                    var results = regex.Matches(input);
                    if (results.Count <= 0) Console.WriteLine("\t\tNo Matches Found");
                    foreach (Match match in results)
                    {
                        Console.WriteLine("\t\tMatch Found at index {0}. Length: {1}.", match.Index, match.Length);
                        foreach (Group group in match.Groups)
                            Console.WriteLine("\t\t\tGroup at index {0} has value {1}", group.Index, group.Value);
                    }
                });
            });
            Console.ReadKey();
        }
    }
}
