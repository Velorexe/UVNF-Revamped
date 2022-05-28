using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVNF.Core.Entities;

namespace UVNF.Core.Entities.ScriptLines
{
    [ScriptLineAlias("chapter")]
    public class ChapterScriptLine : UVNFScriptLine
    {
        public override char Literal => '#';

        [TextParameter("title", false)]
        public string _chapterTitle = string.Empty;
    }
}
