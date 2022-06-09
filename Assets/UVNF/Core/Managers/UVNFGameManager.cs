using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UVNF.Core.Entities;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace UVNF.Core
{
    public class UVNFGameManager : MonoBehaviour
    {
        [SerializeField]
        private UVNFScript _currentScript;

        public RectTransform Canvas { get { return _uvnfCanvas; } }
        [SerializeField]
        private RectTransform _uvnfCanvas;

        [SerializeField]
        private UVNFManager[] _managerPrefabs;

        private int _scriptLineIndex = 0;

        public UVNFResources Resources;

        private List<UVNFManager> _managers = new List<UVNFManager>();

        private Dictionary<Type, UVNFManager[]> _cachedInjection = new Dictionary<Type, UVNFManager[]>();
        private Type[] _cachedTypes = new Type[0];

        public async UniTask<T> GetManager<T>() where T : UVNFManager
        {
            Type managerType = typeof(T);
            UVNFManager manager = manager = _managers.FirstOrDefault(x => x.GetType() == managerType);

            if (manager == null)
            {
                manager = _managerPrefabs.FirstOrDefault(x => x.GetType() == managerType);

                GameObject managerObject = Instantiate(manager.gameObject, _uvnfCanvas);
                manager = (UVNFManager)managerObject.GetComponent(managerType);

                await manager.Init(this, CancellationToken.None);

                _managers.Add(manager);
            }

            return (T)manager;
        }

        private async void Awake()
        {
            await SyncInjection();

            while (_scriptLineIndex < _currentScript.Lines.Count)
            {
                MethodInfo invoke = _cachedTypes[_scriptLineIndex].GetMethod("Execute");
                if (invoke != null)
                {
                    await (UniTask)invoke.Invoke(_currentScript.Lines[_scriptLineIndex], _cachedInjection[_cachedTypes[_scriptLineIndex]]);
                }
                _scriptLineIndex++;
            }
        }

        private async UniTask SyncInjection()
        {
            _cachedInjection.Clear();
            _cachedTypes = new Type[_currentScript.Lines.Count];

            for (int i = 0; i < _currentScript.Lines.Count; i++)
            {
                Type lineType = _currentScript.Lines[i].GetType();
                _cachedTypes[i] = lineType;

                if (!_cachedInjection.ContainsKey(lineType))
                {
                    MethodInfo invoke = lineType.GetMethod("Execute");

                    if (invoke != null)
                    {
                        ParameterInfo[] parameters = invoke.GetParameters();
                        UVNFManager[] managerTypes = new UVNFManager[parameters.Length];

                        for (int j = 0; j < parameters.Length; j++)
                        {
                            if (parameters[j].ParameterType.IsSubclassOf(typeof(UVNFManager)))
                                managerTypes[j] = await CreateCachedManager(parameters[j].ParameterType);
                            else
                            {
                                Debug.LogWarning($"UVNF can't handle ScriptLines that need a different injection type than a 'UVNFManager' subclass.\n" +
                                    $"Error using Type '{parameters[j].ParameterType.Name}' in class '{lineType.Name}'.");

                                _cachedInjection.Clear();
                                return;
                            }
                        }

                        _cachedInjection.Add(lineType, managerTypes);
                    }

                }

                await UniTask.Yield();
            }
        }

        private async UniTask<UVNFManager> CreateCachedManager(Type managerType)
        {
            UVNFManager manager = manager = _managers.FirstOrDefault(x => x.GetType() == managerType);

            if (manager == null)
            {
                manager = _managerPrefabs.FirstOrDefault(x => x.GetType() == managerType);

                GameObject managerObject = Instantiate(manager.gameObject, _uvnfCanvas);
                manager = (UVNFManager)managerObject.GetComponent(managerType);

                await manager.Init(this, CancellationToken.None);

                _managers.Add(manager);
            }

            return manager;
        }
    }
}