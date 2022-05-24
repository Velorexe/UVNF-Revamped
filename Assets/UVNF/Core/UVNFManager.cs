using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace UVNF.Core
{
    public abstract class UVNFManager : MonoBehaviour
    {
        public abstract UniTaskVoid Init(CancellationToken token);
    }
}