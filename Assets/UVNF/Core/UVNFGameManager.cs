using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UVNF.Core.Entities;

namespace UVNF.Core
{
    public class UVNFGameManager : Singleton<UVNFGameManager>
    {
        [SerializeField]
        private UVNFScript _startingScript;

        [SerializeField]
        private RectTransform _uvnfCanvas;
        
        [SerializeField] 
        private UVNFManager[] _managerPrefabs;
        
        private readonly IUniTaskAsyncEnumerable<UVNFManager> _managersAsync = UniTaskAsyncEnumerable.Create<UVNFManager>(async (writer, token) => { await UniTask.Yield(); });

        public async UniTask<T> GetManager<T>(CancellationToken token) where T : UVNFManager
        {
            UVNFManager manager = null;
            if(await _managersAsync.CountAsync(cancellationToken: token) > 0)
                manager = await _managersAsync.FirstAsync(x => x.GetType() == typeof(T), cancellationToken: token);

            if (manager == null)
            {
                manager = gameObject.AddComponent<T>();
                manager.Init(token).Forget();

                _managersAsync.Append(manager);
            }

            return (T)manager;
        }

        private async void Awake()
        {
            await _startingScript.Lines[0].Execute(this, CancellationToken.None);
            Debug.Log("Done");
        }
    }
}