using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UVNF.Core.Entities.Factories;
using System.Text;

namespace UVNF.Core.Entities
{
    /// <summary>
    /// A collection of Lines and Chapter pointers
    /// </summary>
    public class UVNFScript : ScriptableObject
    {
        /// <summary>
        /// The Compiled UVNFScriptLines from the raw .uvnf file
        /// </summary>
        public List<UVNFScriptLine> Lines => _lines;
        [SerializeReference]
        private List<UVNFScriptLine> _lines = new List<UVNFScriptLine>();

        /// <summary>
        /// Indexes and titles for Chapters
        /// </summary>
        public Dictionary<int, string> Chapters { get; } = new Dictionary<int, string>();

        /// <summary>
        /// The path of the original .uvnf file
        /// </summary>
        public string AssetPath = string.Empty;

#if UNITY_EDITOR
        /// <summary>
        /// [EDITOR ONLY] Creates a UVNFScript compiled from the given lines
        /// </summary>
        /// <param name="rawLines">The lines straight from the .uvnf file</param>
        /// <param name="assetPath">The path of the .uvnf file</param>
        public void CompileRawLines(string[] rawLines, string assetPath)
        {
            AssetPath = assetPath;
            CompileLines(rawLines);
        }
#endif

        /// <summary>
        /// Creates a UVNFScript compiled from the given lines
        /// </summary>
        /// <param name="rawLines"></param>
        public void CompileRawLines(string[] rawLines)
        {
            CompileLines(rawLines);
        }

        /// <summary>
        /// Compiles the given lines into UVNFScriptLines
        /// </summary>
        /// <param name="lines">The line, starting with the Literal</param>
        private void CompileLines(string[] lines)
        {
            UVNFScriptLineFactory factory = new UVNFScriptLineFactory();

            for (int i = 0; i < lines.Length; i++)
            {
                UVNFScriptLine scriptLine = factory.FromTag(lines[i]);

                if (scriptLine != null)
                {
                    _lines.Add(scriptLine);
                }
                else
                {
                    Debug.Log("Found a strange, non-UVNF compatible character at line " + (i + 1));
                }
            }
        }

        /// <summary>
        /// Inserts a new UVNFScriptLine at the given location
        /// </summary>
        /// <param name="line"></param>
        /// <param name="location">The Index at which the line should be inserted</param>
        public void InsertLine(UVNFScriptLine line, int location)
        {
            _lines.Insert(location - 1, line);
        }

        /// <summary>
        /// Adds a line at the end of the Line stack
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(UVNFScriptLine line)
        {
            _lines.Add(line);
        }

        /// <summary>
        /// Removes a UVNFScriptLine at the given location
        /// </summary>
        /// <param name="location">The Index at which the line should be removed</param>
        public void RemoveAt(int location)
        {
            _lines.RemoveAt(location);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Compiles all the UVNFScriptLines into a neat document
        /// </summary>
        /// <param name="includeComments">Default to true</param>
        /// <returns>A string compiled from the UVNFScriptLines</returns>
        public string ToDocument(bool includeComments = true)
        {
            StringBuilder builder = new StringBuilder();

            foreach (UVNFScriptLine t in _lines)
                builder.AppendLine(t.ToCommandString());

            return builder.ToString();
        }
#endif
    }
}