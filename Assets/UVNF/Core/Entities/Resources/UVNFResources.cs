using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UVNF.Core;

public class UVNFResources : UVNFManager
{
    public async UniTask<T> GetResource<T>(string resourceAdres)
    {
        return await Addressables.LoadAssetAsync<T>(resourceAdres);
    }

    public async UniTask<List<T>> GetResourcesWithLabel<T>(string label)
    {
        List<T> addressables = new List<T>();
        addressables = (List<T>)await Addressables.LoadAssetsAsync<T>(label, null);

        return addressables;
    }
}
