using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace UVNF.Editor.Extensions
{
    public static class StringExtensions
    {
        //https://stackoverflow.com/questions/272633/add-spaces-before-capital-letters
        public static string AddSpaceToString(this string text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}
