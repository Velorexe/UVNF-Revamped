using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using UVNF.Core.Canvas;

namespace UVNF.Core.Entities.ScriptLines
{
    [ScriptLineAlias("char", "character"), ColorTint(1f, 0f, 0f)]
    public class CharacterScriptLine : UVNFScriptLine
    {
        public override char Literal => '@';

        [TextParameter("name", false)]
        [SerializeField]
        private string _characterName = string.Empty;

        [TextParameter("pose")]
        [SerializeField]
        private string _characterPose = string.Empty;

        [EnumParameter("emotion", typeof(CharacterEmotion))]
        [SerializeField]
        private CharacterEmotion characterEmotion = CharacterEmotion.Happy;

        [Vector2Parameter("pos")]
        [SerializeField]
        private Vector2 _characterPosition = Vector2.zero;

        [Vector2Parameter("scale")]
        [SerializeField]
        private Vector2 _characterScale = Vector2.zero;

        public override async UniTask Execute(UVNFGameManager callback, CancellationToken token)
        {
            var manager = await callback.GetManager<CharacterManager>(token);
            manager.AddCharacter(_characterName, _characterPose, _characterPosition, _characterScale).Forget();
        }
    }

    public enum CharacterEmotion
    {
        Happy,
        Sad
    }
}
