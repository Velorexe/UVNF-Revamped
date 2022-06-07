using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UVNF.Core.Canvas;

namespace UVNF.Core.Entities.ScriptLines
{
    [ScriptLineAlias("char"), ColorTint(1f, 0f, 0f)]
    public class CharacterScriptLine : UVNFScriptLine
    {
        public override char Literal => '@';

        [TextParameter("name", false)]
        [SerializeField]
        private string _characterName = string.Empty;

        [TextParameter("pose")]
        [SerializeField]
        private string _characterPose = string.Empty;

        [Vector2Parameter("pos")]
        [SerializeField]
        private Vector2 _characterPosition = Vector2.zero;

        [Vector2Parameter("scale", 1f, 1f)]
        [SerializeField]
        private Vector2 _characterScale = Vector2.one;

        public async UniTask Execute(UVNFCharacterManager characterManager)
        {
            await characterManager.AddCharacter(_characterName, _characterPose, _characterPosition, _characterScale);
        }
    }

    public enum CharacterEmotion
    {
        Happy,
        Sad
    }
}
