using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace UVNF.Core.Entities.ScriptLines
{
    [ScriptLineAlias("say"), ColorTint(1f, 1f, 0f)]
    public class DialogueScriptLine : UVNFScriptLine
    {
        public override char Literal => '@';

        [TextParameter("dialogue", optional: false)]
        [SerializeField]
        private string _dialogue = string.Empty;

        [TextParameter("name")]
        [SerializeField]
        private string _characterName = string.Empty;

        [BooleanParameter("waitInput", defaultValue: true)]
        [SerializeField]
        private bool _waitForInput = true;

        public async UniTask Execute(UVNFDialogueManager dialogueManager)
        {
            await dialogueManager.Say(_characterName, _dialogue, _waitForInput);
        }
    }
}
