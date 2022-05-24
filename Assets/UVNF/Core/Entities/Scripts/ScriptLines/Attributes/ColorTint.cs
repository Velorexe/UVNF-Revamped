using System.Collections;
using System;
using UnityEngine;

namespace UVNF.Core.Entities.ScriptLines
{
    /// <summary>
    /// Creates a faint Tint of the provided color on the node and line
    /// </summary>
    [AttributeUsage(AttributeTargets.Class), Serializable]
    public class ColorTint : Attribute
    {
        /// <summary>
        /// Parsed RGB float value
        /// </summary>
        public readonly Color Tint;

        /// <summary>
        /// Lower the Alpha to faint the ColorTint more
        /// </summary>
        public const float Alpha = 0.3f;

        /// <summary>
        /// Sets the RGB of the Tint. Max value of 1f
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public ColorTint(float r, float g, float b)
        {
            Tint = new Color(r, g, b, Alpha);
        }
    }
}