using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVNF.Core.Entities.ScriptLines
{
    [ScriptLineAlias("comment"), ColorTint(0.5f, 0.5f, 0.5f)]
    public class CommentScriptLine : UVNFScriptLine
    {
        public override char Literal => '!';

        [TextParameter("info", false)]
        public string Info = string.Empty;
    }
}
