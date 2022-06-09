using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

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

        public async UniTask Say(string characterName, string dialogue, bool waitForInput)
        {
            bool confirmedInput = false;
            _onConfirm += () => confirmedInput = true;

            _namePrinter.SetText(characterName);
            _dialoguePrinter.SetText(dialogue);

            while (waitForInput && !confirmedInput)
                await UniTask.Yield();

            Debug.Log("Confirmed");

            _onConfirm = null;
            await UniTask.CompletedTask;
        }
    }
}