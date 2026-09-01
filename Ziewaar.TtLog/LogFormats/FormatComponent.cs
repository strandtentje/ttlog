using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ziewaar.TtLog.LogFormats
{
    public class FormatComponent
    {
        public readonly int PlaceholderIndex = -1;
        public readonly string Literal;
        public bool IsPlaceholder
        {
            get
            {
                return PlaceholderIndex != -1;
            }
        }
        public FormatComponent(int placeholderIndex, string literal)
        {
            this.PlaceholderIndex = placeholderIndex;
            this.Literal = literal;
        }
        private static readonly char[] BreakingCharacters = new char[] {
            ' ', '-', ':', '_',
        };
        public static FormatComponent From(
            string text, ref int placeholderCount, ref int cursor)
        {
            if (text[cursor] == '{')
            {
                var closerPosition = text.IndexOf('}', cursor);
                if (closerPosition > cursor)
                {
                    cursor++;
                    var literal = text.Substring(
                        cursor, closerPosition - cursor);
                    cursor = closerPosition + 1;
                    int positionLiteral;
                    if (int.TryParse(literal, out positionLiteral))
                    {
                        return new FormatComponent(
                            positionLiteral, literal);
                    }
                    else
                    {
                        return new FormatComponent(
                            placeholderCount++, literal);
                    }
                }
                else
                {
                    var literal = text.Substring(cursor);
                    cursor = text.Length;
                    return new FormatComponent(
                        -1, literal);                        
                }
            }
            else if (cursor < text.Length)
            {
                if (BreakingCharacters.Contains(text[cursor]))
                {
                    // we're currently at a character that can take
                    // a word wrap, and isn't a placeholder, 
                    // so we include that character as a distinct 
                    // format component
                    return new FormatComponent(-1, text[cursor++].ToString());
                }
                else
                {
                    // we have hit unbreakable text, so we must now 
                    // either look for the next wordwrap character,
                    // or a placeholder opener
                    var endOfUnbreakable = text.IndexOfAny(
                        BreakingCharacters, cursor);
                    var placeholderPosition = text.IndexOf(
                        '{', cursor);
                    // whichever one comes first, marks the end of our
                    // unbreakable literal text, but we can't outright 
                    // math.min that, because if not both types of wrap 
                    // conditions are ahead, we get a -1, which is always
                    // the min-est.
                    var trueEnd = -1;
                    // so if either is -1 and the other one is legal, we
                    // pick it, only then we can math.min if both were found,
                    // and if none were found, the remainder of the string
                    // goes in a literal component.
                    if (endOfUnbreakable == -1 && placeholderPosition > cursor)
                    {
                        trueEnd = placeholderPosition;
                    }
                    else if (endOfUnbreakable > cursor && placeholderPosition == -1)
                    {
                        trueEnd = endOfUnbreakable;
                    }
                    else if (endOfUnbreakable > cursor && placeholderPosition > cursor)
                    {
                        trueEnd = Math.Min(endOfUnbreakable, placeholderPosition);
                    }
                    else
                    {
                        trueEnd = -1;
                    }
                    // we can express this more tersely, and the reader
                    // is invited to do this if it makes things _less_ 
                    // confusing.
                    if (trueEnd > cursor)
                    {
                        // if that position is after our cursor, 
                        // it means we found something
                        var unbreakableLength = trueEnd - cursor;
                        var literal = text.Substring(cursor, unbreakableLength);
                        cursor = trueEnd;
                        return new FormatComponent(-1, literal);
                    }
                    else
                    {
                        var literal = text.Substring(cursor);
                        cursor = text.Length;
                        return new FormatComponent(-1, literal);
                    }
                }
            }
            else
            {
                return new FormatComponent(-1, "");
            }
        }
    }
}
