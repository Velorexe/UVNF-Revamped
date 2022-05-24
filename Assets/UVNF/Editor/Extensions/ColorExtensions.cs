using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVNF.Editor.Extensions
{
    public static class ColorExtensions
    {
        //Credit to Pavel Vladov of Stackoverflow https://stackoverflow.com/questions/801406/c-create-a-lighter-darker-color-based-on-a-system-color

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(this Color color, float correctionFactor)
        {
            if (correctionFactor < 0)
            {
                correctionFactor = 1f + correctionFactor;
                color.r *= correctionFactor;
                color.g *= correctionFactor;
                color.b *= correctionFactor;
            }
            else
            {
                color.r = (1f - color.r) * correctionFactor + color.r;
                color.g = (1f - color.g) * correctionFactor + color.g;
                color.b = (1f - color.b) * correctionFactor + color.b;
            }

            return color;
        }
    }
}