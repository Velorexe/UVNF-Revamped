using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UVNF.Core.Entities.Factories;
using System.Text;
using System.Globalization;

namespace UVNF.Core.Entities
{
    /// <summary>
    /// A collection of Lines and Chapter pointers
    /// </summary>
    public class UVNFScript : ScriptableObject
    {
        public string Name = "Default Title";

        /// <summary>
        /// The Compiled UVNFScriptLines from the raw .uvnf file
        /// </summary>
        public List<UVNFScriptLine> Lines => _lines;
        [SerializeReference]
        private List<UVNFScriptLine> _lines = new List<UVNFScriptLine>();

        /// <summary>
        /// The path of the original .uvnf file
        /// </summary>
        public string AssetPath = string.Empty;

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
        /// Replaces all UVNFScriptLines with the given List
        /// </summary>
        /// <param name="lines">The List from which the lines should be copied</param>
        public void ReplaceLines(List<UVNFScriptLine> lines)
        {
            _lines = new List<UVNFScriptLine>(lines);
        }

        /// <summary>
        /// Removes a UVNFScriptLine at the given location
        /// </summary>
        /// <param name="location">The Index at which the line should be removed</param>
        public void RemoveAt(int location)
        {
            _lines.RemoveAt(location);
        }
    }
}