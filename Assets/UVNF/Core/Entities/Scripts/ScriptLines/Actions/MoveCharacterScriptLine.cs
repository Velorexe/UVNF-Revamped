using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVNF.Core.Entities.ScriptLines
{
    [ScriptLineAlias("move"), ColorTint(0f, 0f, 1f)]
    public class MoveCharacterScriptLine : UVNFScriptLine
    {
        public override char Literal => '@';

        [TextParameter("name"), SerializeField]
        private string _characterName = string.Empty;

        [IntParameter("position"), SerializeField]
        private int _position = 0;
    }
}