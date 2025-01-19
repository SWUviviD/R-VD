using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapChangeManager : MonoBehaviour
{
    [SerializeField] private Transform targetPlane; // 충돌 대상 Plane
    [SerializeField] private string nextSceneName;  // 로드할 다음 스테이지 이름

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 플레이어인지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어가 Plane과 충돌했는지 확인
            if (IsPlayerOnTargetPlane(other))
            {
                LoadNextStage();
            }
        }
    }

    /// <summary>
    /// 플레이어가 Plane 위에 있는지 확인
    /// </summary>
    private bool IsPlayerOnTargetPlane(Collider playerCollider)
    {
        if (targetPlane == null)
        {
            LogManager.LogWarning("Target Plane 오류");
            return false;
        }

        // Plane 영역 체크
        Collider planeCollider = targetPlane.GetComponent<Collider>();
        if (planeCollider == null)
        {
            LogManager.LogWarning("Collider 오류");
            return false;
        }

        return planeCollider.bounds.Intersects(playerCollider.bounds);
    }

    /// <summary>
    /// 다음 스테이지 로드
    /// </summary>
    private void LoadNextStage()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            LogManager.LogWarning("스테이지 이름이 설정되지 않았습니다.");
        }
    }
}
