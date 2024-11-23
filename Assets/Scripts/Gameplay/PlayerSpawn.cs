using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player is spawned after dying.
    /// </summary>
    public class PlayerSpawn : Simulation.Event<PlayerSpawn>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;

            // Clear drawn shapes upon respawn
            if (GameManager.Instance.drawShapes != null)
            {
                GameManager.Instance.drawShapes.ClearDrawnShapes();
            }

            // Enable the player's collider and disable control momentarily
            player.collider2d.enabled = true;
            player.controlEnabled = false;

            // Play respawn sound if available
            if (player.audioSource && player.respawnAudio)
                player.audioSource.PlayOneShot(player.respawnAudio);

            // Reset health or other stats as needed
            player.health.Increment();

            // Set the spawn position to the checkpoint location
            var checkpointPosition = GameManager.Instance.GetCheckpointPosition();
            player.Teleport(checkpointPosition);

            // Reset jump state and animations
            player.jumpState = PlayerController.JumpState.Grounded;
            player.animator.SetBool("dead", false);

            // Adjust the camera to focus on the player
            model.virtualCamera.m_Follow = player.transform;
            model.virtualCamera.m_LookAt = player.transform;

            // Re-enable player input after a delay
            Simulation.Schedule<EnablePlayerInput>(2f);
        }
    }
}
