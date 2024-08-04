using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

/// <summary>
/// UI 관리자
/// UI는 여러번 재사용될 수 있기 때문에 한번 사용하고 닫는다고 해서 삭제되는게 아니라 따로 저장된다.
/// 저장된 UI는 cachedUIList에 저장된다.
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    /// <summary> UI의 최상위 부모 </summary>
    [SerializeField] private RectTransform commonUIRoot;
    /// <summary> 저장된 UI의 최상위 부모 </summary>
    [SerializeField] private RectTransform cachedUIRoot;
    
    /// <summary> 화면에 표시되고 있는 UI 리스트 </summary>
    private List<UIBase> uiList;
    /// <summary> 캐싱한 UI 리스트 </summary>
    private List<UIBase> cachedUIList;
    
    /// <summary> 저장된 UI를 경로와 UI로 매칭하여 가지고 있는 테이블 </summary>
    private Dictionary<string, UIBase> cachedUITable;

    private UIBase lastUI => uiList[uiList.Count - 1];
    
    protected override void Init()
    {
        uiList = new List<UIBase>();
        cachedUIList = new List<UIBase>();
        cachedUITable = new Dictionary<string, UIBase>();
        
        // 임시 UI. 가장 기본이 될 UI를 띄운다.
        // TODO : 최상위 UI는 닫히면 안되니까, 닫지 못하도록 하는 처리가 필요할듯.
        Show(Defines.UIDefines.UISampleFull, (_) =>
        {
            (_ as UISampleFull).Set(false);
        });
    }

    /// <summary>
    /// UI를 띄운다. Addressable로 로드하기 때문에 즉시 로드되지 않으므로 콜백을 받는다.
    /// </summary>
    public void Show(string uiAddress, System.Action<UIBase> cbOpen)
    {
        if (cachedUITable.TryGetValue(uiAddress, out var ui) && cachedUIList.Contains(ui))
        {
            cachedUIList.Remove(ui);
            ui.gameObject.SetActive(true);
            OnLoadUI(ui);
            cbOpen?.Invoke(ui);
        }
        else
        {
            AddressableAssetsManager.Instance.LoadAsyncAssets(uiAddress, uiAddress, (obj) =>
            {
                var instance = Instantiate((GameObject)obj);
                var uiBase = instance.GetComponent<UIBase>();
                cachedUITable.TryAdd(uiAddress, uiBase);
                OnLoadUI(uiBase);
                cbOpen?.Invoke(uiBase);
            });
        }
    }

    private void OnLoadUI(UIBase ui)
    {
        ui.transform.SetParent(commonUIRoot, false);
        ui.OnLoad();

        if (ui.UIType == UIDefines.UIType.FullScreen)
        {
            // 화면을 덮는 ui가 로드된 경우, 기존에 보이던 ui를 닫는다.
            uiList.ForEach(_ => _.gameObject.SetActive(false));
        }
        
        uiList.Add(ui);
    }

    /// <summary>
    /// UI를 닫는다. 다만 해당 UI가 최상에 있어야만 닫을 수 있다.
    /// </summary>
    /// <param name="ui"></param>
    public void Close(UIBase ui)
    {
        if (lastUI != ui)
        {
            Debug.LogError($"최상위 UI가 아닙니다. ( {ui.name} )");
            return;
        }
        
        cachedUIList.Add(ui);
        uiList.RemoveAt(uiList.Count - 1);
        ui.gameObject.SetActive(false);
        ui.transform.SetParent(cachedUIRoot);
        
        if (ui.UIType == UIDefines.UIType.FullScreen)
        {
            // 화면을 덮는 UI가 보여질때까지 uiList의 뒷부분부터 하나씩 띄운다.
            for (int i = uiList.Count - 1; i >= 0; --i)
            {
                uiList[i].gameObject.SetActive(true);
                if (uiList[i].UIType == UIDefines.UIType.FullScreen) break;
            }
        }
        
        // 자식이 닫혔음을 알림.
        lastUI.OnChildPopupClose();
    }
}
