using Unity.Netcode;
using UnityEngine;

namespace BBTUF
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager character;

        float vertical;
        float horizontal;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
        {
            character.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
            character.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        }

        public virtual void PlayTargetActionAnimation(
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;

            // Tell the Server/Host we played an animation, and to play that animation for everybody else present
            character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRPC(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
    }
}