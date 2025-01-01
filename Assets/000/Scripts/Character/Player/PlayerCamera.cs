using UnityEngine;

namespace BBTUF
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] Transform cameraPivotTransform;

        [Header("Camera Settings")]
        private float cameraSmoothSpeed = 1;
        [SerializeField] float leftAndRightRotationSpeed = 220;
        [SerializeField] float upAndDownRotationSpeed = 220;
        [SerializeField] float pivotMinimum = -30;
        [SerializeField] float pivotMaximum = 60;
        [SerializeField] float radiusCameraCollision = 0.2f;
        [SerializeField] float speedCameraCollision = 0.2f;
        [SerializeField] LayerMask collideLayers = 0;

        [Header("Camera Values")]
        Vector3 _cameraVelocity;
        Vector3 _positionCameraObject;
        [SerializeField] private float leftAndRightLookAngle;
        [SerializeField] private float upAndDownLookAngle;
        private float currentCameraPosizitonZ;
        private float targetCameraPositionZ;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            currentCameraPosizitonZ = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraActions()
        {
            if (player != null)
            {
                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref _cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        private void HandleRotations()
        {
            leftAndRightLookAngle += (PlayerInputManager.instance.inputCameraHorizontal * leftAndRightRotationSpeed) * Time.deltaTime;
            upAndDownLookAngle -= (PlayerInputManager.instance.inputCameraVertival * upAndDownRotationSpeed) * Time.deltaTime;
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, pivotMinimum, pivotMaximum);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCollisions()
        {
            targetCameraPositionZ = currentCameraPosizitonZ;

            RaycastHit hit;
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, radiusCameraCollision, direction, out hit, Mathf.Abs(targetCameraPositionZ), collideLayers))
            {
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetCameraPositionZ = -(distanceFromHitObject - radiusCameraCollision);
            }

            if (Mathf.Abs(targetCameraPositionZ) < radiusCameraCollision)
            {
                targetCameraPositionZ = -radiusCameraCollision;
            }

            _positionCameraObject.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraPositionZ, speedCameraCollision);
            cameraObject.transform.localPosition = _positionCameraObject;
        }
    }
}