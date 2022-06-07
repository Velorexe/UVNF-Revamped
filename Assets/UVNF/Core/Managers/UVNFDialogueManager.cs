using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

namespace UVNF.Core
{
    public class UVNFDialogueManager : UVNFManager
    {
        [SerializeField]
        private TextMeshProUGUI _namePrinter;

        [SerializeField]
        private TextMeshProUGUI _dialoguePrinter;

        private event _confirm _onConfirm;
        private delegate void _confirm();

        public void OnConfirm() => _onConfirm?.Invoke();

        public async UniTask Say(string characterName, string dialogue, bool waitForInput)
        {
            bool confirmedInput = false;
            _onConfirm += () => confirmedInput = true;

            _namePrinter.SetText(characterName);
            _dialoguePrinter.SetText(dialogue);

            while (waitForInput && !confirmedInput)
                await UniTask.Yield();
        }
    }
}