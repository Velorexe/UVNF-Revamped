using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UVNF.Utilities;

using Type = UVNF.Utilities.SerializableSystemType;

public class UVNFResources : Singleton<UVNFResources>
{
    public async UniTask<T> GetResource<T>(string resourceAdres)
    {
        return await Addressables.LoadAssetAsync<T>(resourceAdres);
    }

#if UNITY_EDITOR
    private AddressableAssetSettings _settings = AddressableAssetSettingsDefaultObject.Settings;

    public async UniTask<bool> RemoveResource<T>(string resourceAdres) where T : Object
    {
        if (_settings == null)
            _settings = AddressableAssetSettingsDefaultObject.Settings;

        AddressableAssetGroup group = _settings.FindGroup(x => x.Name == "UVNF");
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(await Addressables.LoadAssetAsync<T>(resourceAdres), out string guid, out long localId))
        {
            group.RemoveAssetEntry(group.GetAssetEntry(guid));
            return true;
        }
        return false;
    }

    public bool AddResource(string assetGuid, string resourceAdres)
    {
        if (_settings == null)
            _settings = AddressableAssetSettingsDefaultObject.Settings;

        AddressableAssetGroup group = _settings.FindGroup(x => x.Name == "UVNF");
        AddressableAssetEntry entry = _settings.CreateOrMoveEntry(assetGuid, group);

        entry.address = resourceAdres;

        return true;
    }
#endif
}
