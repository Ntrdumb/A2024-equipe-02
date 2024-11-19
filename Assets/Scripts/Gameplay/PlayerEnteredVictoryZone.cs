using System.Collections;
using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.Gameplay
{

    /// <summary>
    /// This event is triggered when the player character enters a trigger with a VictoryZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredVictoryZone"></typeparam>
    public class PlayerEnteredVictoryZone : Simulation.Event<PlayerEnteredVictoryZone>
    {
        public VictoryZone victoryZone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            model.player.animator.SetTrigger("victory");
            model.player.controlEnabled = false;
            model.player.StartCoroutine(BackToMenu(5));
        }

        IEnumerator BackToMenu(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene("Main Menu");
        }
    }
}