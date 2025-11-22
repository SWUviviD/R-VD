using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateStage2Platforms : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject cornerPrefab;   // 모서리
    public GameObject edgePrefab;     // 테두리
    public GameObject facePrefab;     // 안쪽 평평한 큐브 (Vertical에서는 이걸로만 사용)

    [Header("Center (Checkpoint)")]
    public bool addCenterCheckpoint = false;
    public GameObject centerPrefab;   // 가운데 큐브 (Horizontal 중앙 체크포인트용)

    [Header("Layout Settings")]
    [Min(1)] public int width = 3;    // ★ Vertical에서는 1도 가능
    [Min(1)] public int height = 3;   // ★ Vertical에서는 1도 가능

    public LayoutPlane plane = LayoutPlane.XZ;
    public LayoutOrientation layoutOrientation = LayoutOrientation.Horizontal;

    public enum LayoutPlane
    {
        XZ, // 바닥 (x, z 기준 그리드)
        XY  // 정면 (x, y 기준 그리드)
    }

    public enum LayoutOrientation
    {
        Horizontal, // 평면 그대로 (corner/edge/face 사용)
        Vertical    // width x height 행렬을 facePrefab으로만 채우고 parent를 세움
    }

    [Header("Base Rotations (deg)")]
    public Vector3 edgeBaseRotation = Vector3.zero;
    public Vector3 cornerBaseRotation = Vector3.zero;

    [System.NonSerialized]
    private GameObject lastBuiltRoot;

    [ContextMenu("Build Rectangle Border + Fill")]
    public void Build()
    {
        // 기존 자식 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }
        lastBuiltRoot = null;

        // Horizontal일 때만 최소 2x2 강제
        if (layoutOrientation == LayoutOrientation.Horizontal &&
            (width < 2 || height < 2))
        {
            Debug.LogWarning("Horizontal 모드에서는 width, height는 최소 2 이상이어야 합니다.");
            return;
        }

        // 한 칸 실제 크기 계산
        Vector3 cellSize = GetCellSize();
        if (cellSize == Vector3.zero)
        {
            Debug.LogWarning("사용 가능한 프리팹에 Renderer가 없습니다. (corner/edge/face/center)");
            return;
        }

        // parent 생성 + 이름
        string orientationName = layoutOrientation.ToString(); // Horizontal / Vertical
        string parentName = $"{orientationName} {width}X{height}";
        if (addCenterCheckpoint && centerPrefab != null && layoutOrientation == LayoutOrientation.Horizontal)
            parentName += " with checkpoint";

        GameObject parentRoot = new GameObject(parentName);
        parentRoot.transform.SetParent(transform, false);
        parentRoot.transform.localPosition = Vector3.zero;
        parentRoot.transform.localRotation = Quaternion.identity;
        parentRoot.transform.localScale = Vector3.one;

        // Orientation에 따라 다른 빌드 로직
        if (layoutOrientation == LayoutOrientation.Vertical)
        {
            BuildVerticalGrid(parentRoot.transform, cellSize);
        }
        else // Horizontal
        {
            BuildHorizontalGrid(parentRoot.transform, cellSize);
        }

        lastBuiltRoot = parentRoot;
    }

#if UNITY_EDITOR
    [ContextMenu("Build + Save As Prefab...")]
    private void BuildAndSaveAsPrefab()
    {
        Build();

        if (lastBuiltRoot == null)
        {
            Debug.LogWarning("Build 실패: parentRoot가 없습니다.");
            return;
        }

        string defaultName = lastBuiltRoot.name;
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Layout Prefab",
            defaultName + ".prefab",
            "prefab",
            "레이아웃 프리팹을 저장할 위치를 선택하세요."
        );

        if (string.IsNullOrEmpty(path))
            return;

        PrefabUtility.SaveAsPrefabAssetAndConnect(
            lastBuiltRoot,
            path,
            InteractionMode.UserAction
        );

        Debug.Log($"Rectangle border prefab saved at: {path}");
    }
#endif

    // ─────────────────────────────
    // Horizontal 모드: 기존 테두리 + 내부 face
    // ─────────────────────────────
    private void BuildHorizontalGrid(Transform parentRoot, Vector3 cellSize)
    {
        float stepX = cellSize.x;
        float stepYorZ = (plane == LayoutPlane.XZ) ? cellSize.z : cellSize.y;

        float totalWidth = (width - 1) * stepX;
        float totalHeight = (height - 1) * stepYorZ;

        Vector3 originLocal;
        if (plane == LayoutPlane.XZ)
            originLocal = new Vector3(-totalWidth * 0.5f, 0f, -totalHeight * 0.5f);
        else
            originLocal = new Vector3(-totalWidth * 0.5f, -totalHeight * 0.5f, 0f);

        // 테두리 + 내부 face 배치
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isBorder = (x == 0 || x == width - 1 || y == 0 || y == height - 1);
                bool isCorner = ((x == 0 || x == width - 1) &&
                                 (y == 0 || y == height - 1));

                GameObject prefab = null;
                Quaternion rot = Quaternion.identity;

                if (isBorder)
                {
                    if (isCorner && cornerPrefab != null)
                    {
                        prefab = cornerPrefab;
                        rot = GetCornerRotation(x, y);
                    }
                    else
                    {
                        prefab = edgePrefab != null ? edgePrefab : facePrefab;
                        rot = GetEdgeRotation(x, y);
                    }
                }
                else
                {
                    prefab = facePrefab;
                    rot = Quaternion.identity;
                }

                if (prefab == null)
                    continue;

                Vector3 localPos;
                if (plane == LayoutPlane.XZ)
                    localPos = originLocal + new Vector3(x * stepX, 0f, y * stepYorZ);
                else
                    localPos = originLocal + new Vector3(x * stepX, y * stepYorZ, 0f);

                GameObject inst = Object.Instantiate(prefab, parentRoot);
                inst.transform.localPosition = localPos;
                inst.transform.localRotation = rot;
            }
        }

        // 중심 체크포인트 (Horizontal에서만)
        if (addCenterCheckpoint && centerPrefab != null)
        {
            GameObject cp = Object.Instantiate(centerPrefab, parentRoot);
            cp.transform.localPosition = Vector3.zero;
            cp.transform.localRotation = Quaternion.identity;
        }
    }

    // ─────────────────────────────
    // Vertical 모드: width x height 행렬을 facePrefab으로만 채우기
    // ─────────────────────────────
    private void BuildVerticalGrid(Transform parentRoot, Vector3 cellSize)
    {
        if (facePrefab == null)
        {
            Debug.LogWarning("Vertical 모드에서는 facePrefab이 필요합니다.");
            return;
        }

        float stepX = cellSize.x;
        float stepYorZ = (plane == LayoutPlane.XZ) ? cellSize.z : cellSize.y;

        float totalWidth = (width - 1) * stepX;
        float totalHeight = (height - 1) * stepYorZ;

        // 먼저 수평 평면 상에 행렬을 만든 후, 나중에 parent를 회전해서 세움
        Vector3 originLocal;
        if (plane == LayoutPlane.XZ)
            originLocal = new Vector3(-totalWidth * 0.5f, 0f, -totalHeight * 0.5f);
        else
            originLocal = new Vector3(-totalWidth * 0.5f, -totalHeight * 0.5f, 0f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 localPos;
                if (plane == LayoutPlane.XZ)
                    localPos = originLocal + new Vector3(x * stepX, 0f, y * stepYorZ);
                else
                    localPos = originLocal + new Vector3(x * stepX, y * stepYorZ, 0f);

                GameObject inst = Object.Instantiate(facePrefab, parentRoot);
                inst.transform.localPosition = localPos;
                inst.transform.localRotation = Quaternion.identity;
            }
        }

        // parent를 세워주기
        if (plane == LayoutPlane.XZ)
        {
            // 바닥(XZ)에 만든 것을 X축으로 90도 돌려서 세우기
            parentRoot.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else // XY
        {
            // 정면(XY)에 만든 것을 Z축으로 90도 돌려 가로/세로 전환
            parentRoot.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }
    }

    // ─────────────────────────────
    //  회전 로직: base + 90도 단위 (Horizontal에서만 사용)
    // ─────────────────────────────
    private Quaternion GetEdgeRotation(int x, int y)
    {
        bool isTop = (y == height - 1);
        bool isBottom = (y == 0);
        bool isLeft = (x == 0);
        bool isRight = (x == width - 1);

        Vector3 euler = edgeBaseRotation;

        if (plane == LayoutPlane.XZ)
        {
            float addY = 0f;

            if (isTop && !isLeft && !isRight)
                addY = 0f;       // 위
            else if (isBottom && !isLeft && !isRight)
                addY = 180f;     // 아래
            else if (isRight && !isTop && !isBottom)
                addY = 90f;      // 오른쪽
            else if (isLeft && !isTop && !isBottom)
                addY = -90f;     // 왼쪽

            euler.y += addY;
        }
        else // XY
        {
            float addZ = 0f;

            if (isTop && !isLeft && !isRight)
                addZ = 0f;       // 위
            else if (isBottom && !isLeft && !isRight)
                addZ = 180f;     // 아래
            else if (isRight && !isTop && !isBottom)
                addZ = -90f;     // 오른쪽
            else if (isLeft && !isTop && !isBottom)
                addZ = 90f;      // 왼쪽

            euler.z += addZ;
        }

        return Quaternion.Euler(euler);
    }

    private Quaternion GetCornerRotation(int x, int y)
    {
        bool isTop = (y == height - 1);
        bool isBottom = (y == 0);
        bool isLeft = (x == 0);
        bool isRight = (x == width - 1);

        Vector3 euler = cornerBaseRotation;

        if (plane == LayoutPlane.XZ)
        {
            float addY = 0f;

            if (isTop && isRight)          // Top-Right (기준)
                addY = 0f;
            else if (isTop && isLeft)      // Top-Left
                addY = -90f;
            else if (isBottom && isLeft)   // Bottom-Left
                addY = 180f;
            else if (isBottom && isRight)  // Bottom-Right
                addY = 90f;

            euler.y += addY;
        }
        else // XY
        {
            float addZ = 0f;

            if (isTop && isRight)          // Top-Right (기준)
                addZ = 0f;
            else if (isTop && isLeft)      // Top-Left
                addZ = 90f;
            else if (isBottom && isLeft)   // Bottom-Left
                addZ = 180f;
            else if (isBottom && isRight)  // Bottom-Right
                addZ = -90f;

            euler.z += addZ;
        }

        return Quaternion.Euler(euler);
    }

    // ─────────────────────────────
    //  셀 크기 계산
    //  Vertical이면 facePrefab 기준, 아니면 기존 우선순위
    // ─────────────────────────────
    private Vector3 GetCellSize()
    {
        GameObject reference = null;

        if (layoutOrientation == LayoutOrientation.Vertical && facePrefab != null)
        {
            // ★ Vertical: facePrefab 기준
            reference = facePrefab;
        }
        else
        {
            // Horizontal 또는 Vertical이지만 facePrefab이 없는 경우
            reference =
                cornerPrefab != null ? cornerPrefab :
                edgePrefab != null ? edgePrefab :
                facePrefab != null ? facePrefab :
                centerPrefab; // fallback
        }

        if (reference == null)
            return Vector3.zero;

        Renderer[] renderers = reference.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0)
            return Vector3.zero;

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);

        return b.size;
    }
}
