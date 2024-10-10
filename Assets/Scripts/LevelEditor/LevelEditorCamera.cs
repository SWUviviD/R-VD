#if UNITY_EDITOR

using System.Linq;
using UnityEngine;

namespace LevelEditor
{
    public class LevelEditorCamera : MonoBehaviour
    {
        /// <summary> 카메라 회전 속도 </summary>
        [SerializeField] private float rotateSpeed = 360f;
        /// <summary> 카메라 이동 속도 </summary>
        [SerializeField] private float moveSpeed = 20f;
        /// <summary> 카메라 가속 이동 속도 </summary>
        [SerializeField] private float sprintSpeed = 40f;

        private Vector3 moveDirection;
        private Vector3 movement;
        private float rotateX, rotateY;
        private float speed;

        private Vector3 lookForward;
        private Vector3 lookRight;
        private Vector3 lookDir;
        private Vector3 lookUp;

        private void Awake()
        {
            rotateX = transform.eulerAngles.y;
            rotateY = transform.eulerAngles.x;
        }

        private void Update()
        {
            // 우클릭을 누르고 있을 시 카메라 제어 가능
            if (Input.GetMouseButton(1))
            {
                // 커서 사라짐
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                // 마우스 이동 : 카메라 회전 값
                rotateX += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
                rotateY -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

                // 카메라 회전
                transform.rotation = Quaternion.Euler(rotateY, rotateX, 0);

                // 방향키(WASD) : 카메라 이동
                moveDirection.x = Input.GetAxis("Horizontal");
                moveDirection.y = Input.GetAxis("Depth");
                moveDirection.z = Input.GetAxis("Vertical");
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                moveDirection = Vector3.zero;
            }

            //// 마우스 휠 : 카매라 확대 축소
            //transform.position += Input.GetAxis("Mouse ScrollWheel") * moveSpeed * transform.forward;
        }

        private void FixedUpdate()
        {
            if (Input.GetMouseButton(1))
            {
                // 카메라 정면 회전 값 계산
                lookForward = transform.forward.normalized;
                lookRight = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
                lookDir = lookForward * moveDirection.z + lookRight * moveDirection.x;

                // 이동
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.position += sprintSpeed * Time.fixedDeltaTime * lookDir;
                    transform.position += moveDirection.y * sprintSpeed * Time.fixedDeltaTime * transform.up;
                }
                else
                {
                    transform.position += moveSpeed * Time.fixedDeltaTime * lookDir;
                    transform.position += moveDirection.y * moveSpeed * Time.fixedDeltaTime * transform.up;
                }
            }
        }
    }
}

#endif
