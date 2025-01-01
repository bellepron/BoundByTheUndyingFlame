using UnityEngine.SceneManagement;
using UnityEngine;

namespace BBTUF
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;

        PlayerControls playerControls;

        [Header("Input Player Movement")]
        [SerializeField] Vector2 inputMovement;
        public float inputVertical;
        public float inputHorizontal;
        public float moveAmount;

        [Header("Input Camera Movement")]
        [SerializeField] Vector2 inputCamera;
        public float inputCameraVertival;
        public float inputCameraHorizontal;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            // When the scene changes, run this logic.
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // If we are loading into our world scene, enable our players controls.
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            // Otherwise we must be at the MainMenu, disable our palyers controls.
            // This is so our player cant move around if we enter things like a character creation menu etc.
            else
            {
                instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => inputMovement = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => inputCamera = i.ReadValue<Vector2>();
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
        }

        private void HandlePlayerMovementInput()
        {
            inputVertical = inputMovement.y;
            inputHorizontal = inputMovement.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(inputVertical) + Mathf.Abs(inputHorizontal));

            if (moveAmount <= 0.5f && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5f && moveAmount <= 1)
            {
                moveAmount = 1;
            }
        }

        private void HandleCameraMovementInput()
        {
            inputCameraVertival = inputCamera.y;
            inputCameraHorizontal = inputCamera.x;
        }
    }
}