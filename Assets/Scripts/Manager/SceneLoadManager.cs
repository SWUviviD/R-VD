using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
    [SerializeField] private GameObject Panel;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    private bool isSceneLoading = false;

    private UnityEvent<Scene, LoadSceneMode> onSceneLoaded;
    private UnityEvent<Scene, LoadSceneMode> onSceneLoaded_permanent;
    private UnityEvent<Scene, Scene> onActiveSceneChanged;
    private UnityEvent<Scene, Scene> onActiveSceneChanged_permanent;
    private UnityEvent<Scene> onSceneUnloaded;
    private UnityEvent<Scene> onSceneUnloaded_permanent;

    protected override void Init()
    {
        base.Init();

        Panel.SetActive(false);
        // 새 씬이 "로드되어 활성화"된 직후
        SceneManager.sceneLoaded += OnSceneLoaded;
        onSceneLoaded = new UnityEvent<Scene, LoadSceneMode>();
        onSceneLoaded_permanent = new UnityEvent<Scene, LoadSceneMode>();

        // 활성 씬이 바뀔 때 (로딩과 별개로 ActiveScene 전환 시점)
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        onActiveSceneChanged = new UnityEvent<Scene, Scene>();
        onActiveSceneChanged_permanent = new UnityEvent<Scene, Scene>();

        // 씬이 언로드된 직후
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        onSceneUnloaded = new UnityEvent<Scene>();
        onSceneUnloaded_permanent = new UnityEvent<Scene>();
    }

    public SceneDefines.Scene GetActiveScene()
    {
        return (SceneDefines.Scene) SceneManager.GetActiveScene().buildIndex;
    }

    public StageID GetActiveStage()
    {
        SceneDefines.Scene scene = GetActiveScene();
        switch (scene)
        {
            case SceneDefines.Scene.Stage1: return StageID.Stage1;
            default: return StageID.MAX;
        }
    }

    public SceneDefines.Scene GetAccordingSceneID(StageID stageID)
    {
        switch (stageID)
        {
            case StageID.Stage1: return SceneDefines.Scene.Stage1;
            default: return SceneDefines.Scene.Title;
        }
    }

    public void LoadScene(StageID stageID,
        bool loadingPageShown = false,
        UnityAction<Scene, LoadSceneMode> onSceneLoaded = null,
        UnityAction<Scene, Scene> onActiveSceneChanged = null,
        UnityAction<Scene> onSceneUnloaded = null)
    {
        LoadScene(GetAccordingSceneID(stageID),
            loadingPageShown, onSceneLoaded, 
            onActiveSceneChanged, onSceneUnloaded);
    }

    public void LoadScene(SceneDefines.Scene scene, 
        bool loadingPageShown = false,
        UnityAction<Scene, LoadSceneMode> onSceneLoaded = null,
        UnityAction<Scene, Scene> onActiveSceneChanged = null,
        UnityAction<Scene> onSceneUnloaded = null)
    {
        if (isSceneLoading == true)
            return;

        isSceneLoading = true;

        if (onSceneLoaded != null)
            this.onSceneLoaded.AddListener(onSceneLoaded);
        if (onActiveSceneChanged != null)
            this.onActiveSceneChanged.AddListener(onActiveSceneChanged);
        if (onSceneUnloaded != null)
            this.onSceneUnloaded.AddListener(onSceneUnloaded);

        if (loadingPageShown == false)
        {
            SceneManager.LoadScene((int)scene);
            return;
        }

        StartCoroutine(LoadSceneCoroutine((int)scene));
    }

    private IEnumerator LoadSceneCoroutine(int sceneNumber)
    {
        Panel.SetActive(true);
        SetProgress(0f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNumber);

        // 진행률 표시 가능 (0~0.9f 까지만 올라감)
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            SetProgress(progress);

            yield return null;
        }
    }

    private void  SetProgress(float progress)
    {
        progressText.text = $"{progress * 100f}%";
        progressBar.fillAmount = progress;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        onSceneLoaded_permanent?.Invoke(scene, mode);
        onSceneLoaded?.Invoke(scene, mode);
        onSceneLoaded?.RemoveAllListeners();
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        onActiveSceneChanged_permanent?.Invoke(oldScene, newScene);
        onActiveSceneChanged?.Invoke(oldScene, newScene);
        onActiveSceneChanged?.RemoveAllListeners();

        Panel.SetActive(false);

        isSceneLoading = false;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        onSceneUnloaded_permanent?.Invoke(scene);
        onSceneUnloaded?.Invoke(scene);
        onSceneUnloaded?.RemoveAllListeners();
    }

    public void PermanentOnSceneLoadedAction(UnityAction<Scene, LoadSceneMode> action)
    {
        onSceneLoaded_permanent.RemoveListener(action);
        onSceneLoaded_permanent.AddListener(action);
    }

    public void PermanentOnActiveSceneChanged(UnityAction<Scene, Scene> action)
    {
        onActiveSceneChanged_permanent.RemoveListener(action);
        onActiveSceneChanged_permanent.AddListener(action);
    }

    public void PermanentOnSceneUnoadedAction(UnityAction<Scene> action)
    {
        onSceneUnloaded_permanent.RemoveListener(action);
        onSceneUnloaded_permanent.AddListener(action);
    }
}
