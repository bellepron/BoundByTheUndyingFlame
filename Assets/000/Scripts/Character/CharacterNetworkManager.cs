using Unity.Netcode;
using UnityEngine;

namespace BBTUF
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        CharacterManager character;

        [Header("Position")]
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.1f;
        public float networkRotationSmoothTime = 0.1f;

        [Header("Animation")]
        public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        // A server RPC is a function called from a client, to the server (in our case the host)
        [ServerRpc]
        public void NotifyTheServerOfActionAnimationServerRPC(ulong clientID, string animationID, bool applyRootMotion)
        {
            // If this character is the Host/Server, then activate the client RPC
            if (IsServer)
            {
                PlayActionAnimationForAllClientsClientRPC(clientID, animationID, applyRootMotion);
            }
        }

        // A client RPC is sent to all clients present, from the server
        [ClientRpc]
        public void PlayActionAnimationForAllClientsClientRPC(ulong clientID, string animationID, bool applyRootMotion)
        {
            // Making sure to not run the function on the character who sent it (So we don't play animation twice)
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }
    }
}