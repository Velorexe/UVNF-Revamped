using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace UVNF.Core
{
    public abstract class UVNFManager : MonoBehaviour
    {
        public virtual async UniTask Init(UVNFGameManager gameManager, CancellationToken token) => await UniTask.Yield();
    }
}