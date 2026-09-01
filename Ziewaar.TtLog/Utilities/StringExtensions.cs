using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Ziewaar.TtLog.Utilities
{
    public static class StringExtensions
    {

        /// <summary>
        /// i don't know why this works
        /// i stole it from the internet
        /// </summary>
        /// <param name="stIn"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc =
                    CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);

                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        public static IEnumerable<string> GetWordComponents(
            this string nameString, int startAt)
        {
            StringBuilder component = new StringBuilder();
            for (;
                startAt < nameString.Length &&
                char.IsLetterOrDigit(nameString[startAt]);
                startAt++)
                component.Append(nameString[startAt]);

            if (component.Length > 0)
                yield return component.ToString();

            for (;
                startAt < nameString.Length &&
                !char.IsLetterOrDigit(nameString[startAt]);
                startAt++) ;

            if (startAt >= nameString.Length)
                yield break;

            foreach (var after in nameString.GetWordComponents(startAt))
                yield return after;
        }

        public static IEnumerable<string> Enshortify(
            this IEnumerable<string> manyWords)
        {
            var allWords = manyWords.ToArray();
            var longWords = allWords.Where(x => x.Length > 3).ToArray();
            if (allWords.Length < 3 || longWords.Length < 3)
            {
                return allWords;
            }
            else
            {
                return longWords;
            }
        }

        public static IEnumerable<string> MakeWrappable(this string toWrap)
        {
            int cursor = 0;
            char[] lastSegment = new char[] { };
            while (true)
            {
                lastSegment = toWrap.ConsumeRegressingSegment(cursor).ToArray();
                if (lastSegment.Length > 0)
                {
                    cursor += lastSegment.Length;
                    yield return new string(lastSegment);
                }
                else
                {
                    yield break;
                }
            }
        }

        public static IEnumerable<char> ConsumeRegressingSegment(
            this string toWrap, int cursor)
        {
            UnicodeCategory lastSeen = UnicodeCategory.UppercaseLetter;
            UnicodeCategory ucAtCursor = UnicodeCategory.UppercaseLetter;
            char charAtCursor = '\0';
            while (cursor < toWrap.Length)
            {
                charAtCursor = toWrap[cursor];
                ucAtCursor = CharUnicodeInfo.GetUnicodeCategory(charAtCursor);
                if (ucAtCursor >= lastSeen)
                {
                    lastSeen = ucAtCursor;
                    yield return toWrap[cursor++];
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
