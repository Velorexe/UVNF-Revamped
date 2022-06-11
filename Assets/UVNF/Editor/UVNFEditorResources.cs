using Cysharp.Threading.Tasks;
using System;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class UVNFEditorResources
{
    private static AddressableAssetSettings _settings = AddressableAssetSettingsDefaultObject.Settings;

    public static async UniTask<bool> RemoveResource<T>(string resourceAddress) where T : UnityEngine.Object
    {
        AddressableAssetGroup group = AddressableAssetSettingsDefaultObject.Settings.FindGroup(x => x.Name == "UVNF");
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(await Addressables.LoadAssetAsync<T>(resourceAddress), out string guid, out long localId))
        {
            group.RemoveAssetEntry(group.GetAssetEntry(guid));
            return true;
        }
        return false;
    }

    public static bool AddResource(string assetGuid, string resourceAddress, string label)
    {
        AddressableAssetGroup group = AddressableAssetSettingsDefaultObject.Settings.FindGroup(x => x.Name == "UVNF");
        AddressableAssetEntry entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(assetGuid, group);

        entry.address = resourceAddress;
        entry.labels.Add(label);

        return true;
    }

    public static async UniTask<bool> ResourceExists<T>(string assetGuid) where T : UnityEngine.Object
    {
        try
        {
            if (await Addressables.LoadAssetAsync<T>(assetGuid) != null)
            {
                return true;
            }
            return false;
        }
        catch (Exception) { return false; }
    }
}
