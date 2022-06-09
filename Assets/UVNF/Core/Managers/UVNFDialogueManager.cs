using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Threading;
using System;

namespace UVNF.Core
{
    public class UVNFDialogueManager : UVNFManager
    {
        [SerializeField]
        private TextMeshProUGUI _namePrinter;

        [SerializeField]
        private TextMeshProUGUI _dialoguePrinter;

        private UVNFInputManager _inputManager;

        public async override UniTask Init(UVNFGameManager gameManager, CancellationToken token)
        {
            _inputManager = await gameManager.GetManager<UVNFInputManager>();
        }

        public async UniTask Say(string characterName, string dialogue, bool waitForInput)
        {
            if (waitForInput)
            {
                bool confirmedInput = false;

                void InputHandler(InputAction.CallbackContext ctx) => confirmedInput = true;
                _inputManager.OnConfirmDown += InputHandler;

                _namePrinter.SetText(characterName);
                _dialoguePrinter.SetText(dialogue);

                while (waitForInput && !confirmedInput)
                    await UniTask.Yield();

                _inputManager.OnConfirmDown -= InputHandler;
            }
            else
            {
                _namePrinter.SetText(characterName);
                _dialoguePrinter.SetText(dialogue);
            }
            await UniTask.CompletedTask;
        }
    }
}