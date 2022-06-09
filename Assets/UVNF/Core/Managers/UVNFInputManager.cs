using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UVNF.Core;

namespace UVNF.Core
{
    public class UVNFInputManager : UVNFManager
    {
        public event OnInput OnConfirmDown;
        public event OnInput OnConfirmUp;

        public delegate void OnInput(InputAction.CallbackContext ctx);

        private UVNFInputActions _input;

        public async override UniTask Init(UVNFGameManager gameManager, CancellationToken token)
        {
            _input = new UVNFInputActions();

            this.AddBindings();
            this.Enable();

            await UniTask.CompletedTask;
        }

        public void Enable()
        {
            _input.Enable();
        }

        public void Disable()
        {
            _input.Disable();
        }

        private void AddBindings()
        {
            _input.UI.Click.performed += (ctx) => OnConfirmDown?.Invoke(ctx);
            _input.UI.Click.canceled += (ctx) => OnConfirmUp?.Invoke(ctx);
        }

        private void RemoveBindings()
        {
            _input.UI.Click.performed -= (ctx) => OnConfirmDown?.Invoke(ctx);
            _input.UI.Click.canceled -= (ctx) => OnConfirmUp?.Invoke(ctx);
        }
    }
}