using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using UVNF.Assembly;
using UVNF.Core.Entities;
using UVNF.Core.Entities.Factories;
using UVNF.Core.Entities.ScriptLines;

namespace UVNF.Editor.Importer.Parser
{
    /// <summary>
    /// The default UVNF Script Parser
    /// </summary>
    public class UVNFScriptParser : IUVNFScriptParser
    {
        private UVNFScriptLine[] _scriptLineTypes = new UVNFScriptLine[0];

        public UVNFScriptParser()
        {
            _scriptLineTypes = AssemblyHelpers.GetEnumerableOfType<UVNFScriptLine>();
        }

        public UVNFScript CompileLines(UVNFScript script, string[] lines)
        {
            UVNFScriptLineFactory factory = new UVNFScriptLineFactory();

            foreach (string line in lines)
            {
                string[] disectedLine = line.Split(' ');
                Type lineType = GetTypeFromTag(disectedLine[0]);

                if (lineType != null)
                {
                    disectedLine = string.Join(" ", disectedLine, 1, disectedLine.Length - 1).Split(':');
                    string defaultParameterValue = string.Empty;

                    // Has a default (non-optional) parameter
                    int lastSpaceIndex = disectedLine[0].LastIndexOf(' ');
                    if (lastSpaceIndex > -1)
                    {
                        defaultParameterValue = disectedLine[0].Substring(0, lastSpaceIndex);
                        disectedLine[0] = disectedLine[0].Substring(lastSpaceIndex, disectedLine[0].Length - lastSpaceIndex);
                    }

                    // Item1 = Label, Item2 = Value
                    (string, string)[] valueCombo = new (string, string)[disectedLine.Length - 1];

                    if (valueCombo.Length > 0)
                    {
                        for (int i = 0; i < disectedLine.Length; i++)
                        {
                            int splitIndex = disectedLine[i].LastIndexOf(' ');

                            if (i == 0)
                                valueCombo[i].Item1 = disectedLine[i].Substring(1, disectedLine[i].Length - 1);
                            else if (i == disectedLine.Length - 1)
                                valueCombo[i - 1].Item2 = disectedLine[i];
                            else
                            {
                                valueCombo[i - 1].Item2 = disectedLine[i].Substring(0, splitIndex);
                                valueCombo[i].Item1 = disectedLine[i].Substring(splitIndex, disectedLine[i].Length - splitIndex);
                            }
                        }
                    }

                    script.AddLine(factory.CreateScriptLine(lineType, valueCombo, defaultParameterValue));
                }
            }

            return script;
        }

        public string ExportLines(UVNFScript script)
        {
            CultureInfo ci = CultureInfo.CurrentCulture;

            CultureInfo.CurrentCulture = (CultureInfo)ci.Clone();
            CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

            StringBuilder builder = new StringBuilder();

            foreach (UVNFScriptLine t in script.Lines)
                builder.AppendLine(t.ToCommandString());

            CultureInfo.CurrentCulture = ci;

            return builder.ToString();
        }

        private Type GetTypeFromTag(string tagLine)
        {
            char literal = tagLine[0];
            string title = tagLine.Substring(1, tagLine.Length - 1);

            for (int i = 0; i < _scriptLineTypes.Length; i++)
            {
                if (_scriptLineTypes[i].Literal == literal)
                {
                    string lineAlias = string.Empty;
                    if (_scriptLineTypes[i].GetType().GetCustomAttribute<ScriptLineAliasAttribute>() is ScriptLineAliasAttribute aliasAttribute)
                        lineAlias = aliasAttribute.GetAlias();
                    else
                        lineAlias = _scriptLineTypes[i].GetType().Name.ToLower().Replace("ScriptLine", "");

                    if (title.Equals(lineAlias))
                        return _scriptLineTypes[i].GetType();
                }
            }

            return null;
        }
    }
}
