using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class UVNFEditorResources
{
    private static AddressableAssetSettings _settings = AddressableAssetSettingsDefaultObject.Settings;

    public static async UniTask<bool> RemoveResource<T>(string resourceAdres) where T : Object
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

    public static bool AddResource(string assetGuid, string resourceAdres)
    {
        if (_settings == null)
            _settings = AddressableAssetSettingsDefaultObject.Settings;

        AddressableAssetGroup group = _settings.FindGroup(x => x.Name == "UVNF");
        AddressableAssetEntry entry = _settings.CreateOrMoveEntry(assetGuid, group);

        entry.address = resourceAdres;

        return true;
    }
}
