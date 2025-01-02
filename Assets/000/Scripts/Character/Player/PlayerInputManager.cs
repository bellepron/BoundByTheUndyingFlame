using UnityEngine.SceneManagement;
using UnityEngine;

namespace BBTUF
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;

        public PlayerManager player;

        PlayerControls playerControls;

        [Header("Input Camera Movement")]
        [SerializeField] Vector2 inputCamera;
        public float inputCameraVertival;
        public float inputCameraHorizontal;

        [Header("Input Player Movement")]
        [SerializeField] Vector2 inputMovement;
        public float inputVertical;
        public float inputHorizontal;
        public float moveAmount;

        [Header("Input Player Action")]
        [SerializeField] bool inputDodge = false;

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

        // If minimize or lower the window, stop adjusting inputs
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
                playerControls.PlayerActions.Dodge.performed += i => inputDodge = true;
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
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
        }

        // Movement

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

            if (player == null)
                return;

            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
        }

        private void HandleCameraMovementInput()
        {
            inputCameraVertival = inputCamera.y;
            inputCameraHorizontal = inputCamera.x;
        }

        // Action

        private void HandleDodgeInput()
        {
            if (inputDodge)
            {
                inputDodge = false;

                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }
    }
}