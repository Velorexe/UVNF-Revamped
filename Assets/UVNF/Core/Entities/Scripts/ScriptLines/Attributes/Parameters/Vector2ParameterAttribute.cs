using System.Globalization;
using UnityEngine;

namespace UVNF.Core.Entities.ScriptLines
{
    public class Vector2ParameterAttribute : ScriptLineParameterAttribute
    {
        public Vector2ParameterAttribute(string label, float defaultX = 0f, float defaultY = 0f, bool optional = true) : base(label, new Vector2(defaultX, defaultY), optional) { }

        public override object ParseParameterValue(string parameter)
        {
            parameter = parameter.Substring(1, parameter.Length - 2).Replace(" ", "");

            string[] splitValues = parameter.Split(',');
            float[] floats = new float[splitValues.Length];

            for (int i = 0; i < splitValues.Length; i++)
            {
                if (float.TryParse(splitValues[i], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float value))
                    floats[i] = value;
                else
                {
                    Debug.LogWarning($"Tried parsing float from '{splitValues[i]}', but failed.");
                    floats[i] = 0f;
                }
            }

            if (floats.Length == 2)
                return new Vector2(floats[0], floats[1]);
            else if (floats.Length == 1)
                return new Vector2(floats[0], 0f);

            return new Vector2();
        }

        public override string ValueToString(object value)
        {
            Vector2 vector2 = (Vector2)value;
            return "(" + vector2.x + ", " + vector2.y + ")";
        }
    }
}
