using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableAssetsManager : MonoSingleton<AddressableAssetsManager>
{
    private class AddressablePack
    {
        public AddressablePack(string _key)
        {
            Key = _key;
            IsComplete = false;
            Assets = null;
        }

        public string Key { get; private set; }
        public object Assets { get; private set; }
        public bool IsComplete { get; private set; }

        public void CallBack(object _value)
        {
            Assets = _value;
            IsComplete = true;
        }

        public void SetAssets(object _value)
        {
            Assets = _value;
            IsComplete = true;
        }
    }

    private List<AddressablePack> packList = new List<AddressablePack>();

    public object SyncLoadObject(string _path, string _key)
    {
        // 이미 로드되어 있는지 확인
        var pack = packList.Find(_ => _.Key == _key);
        if(pack != null)
        {
            if(pack.IsComplete == true)
            {
                return pack.Assets;
            }

            packList.Remove(pack);
        }

        AddressablePack newPack = new AddressablePack(_key);

        var task = Addressables.LoadAssetAsync<object>(_path);
        newPack.SetAssets(task.WaitForCompletion());
        Addressables.Release(task);

        return newPack.Assets;
    }

    public void LoadAsyncAssets(string _path, string _key, Action<object> _callback)
    {
        var pack = packList.Find(_ => _.Key == _key);
        if (pack != null)
        {
            if (pack.IsComplete == true)
            {
                _callback?.Invoke(pack.Assets);
                return;
            }

            packList.Remove(pack);
        }

        AddressablePack newPack = new AddressablePack(_key);

        Addressables.LoadAssetAsync<object>(_path).Completed += (AsyncOperationHandle<object> _obj) =>
        {
            if(_obj.Status == AsyncOperationStatus.Succeeded)
            {
                newPack.CallBack(_obj.Result);
                _callback?.Invoke(_obj.Result);
            }
            else
            {
                Addressables.Release(_obj);
            }
        };
    }

    public string GetPrefabPath(string _folder, string _assetName)
    {
        return $"Assets/Prefabs/{_folder}/{_assetName}";
    }
}
