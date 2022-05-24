using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading;
using UVNF.Core.Canvas;

namespace UVNF.Core.Entities.ScriptLines
{
    [ScriptLineAlias("char", "character"), ColorTint(1f, 0f, 0f)]
    public class CharacterScriptLine : UVNFScriptLine
    {
        public override char Literal => '@';

        [TextParameter("name"), SerializeField]
        private string _characterName = string.Empty;

        [EnumParameter("emotion", typeof(CharacterEmotion)), SerializeField]
        private CharacterEmotion _characterEmotion;

        [FloatParameter("x_position"), SerializeField]
        private float _positionX = 0f;
        [FloatParameter("y_position"), SerializeField]
        private float _positionY = 0f;
        
        [FloatParameter("x_scale"), SerializeField]
        private float _scaleX = 1f;
        [FloatParameter("y_scale"), SerializeField]
        private float _scaleY = 1f;

        public override async UniTask Execute(UVNFGameManager callback, CancellationToken token)
        {
            var manager = await callback.GetManager<CharacterManager>(token);
            manager.AddCharacter(_characterName, new Vector2(_positionX, _positionY), new Vector2(_scaleX, _scaleY)).Forget();
        }
    }

    public enum CharacterEmotion
    {
        Happy,
        Sad
    }
}
