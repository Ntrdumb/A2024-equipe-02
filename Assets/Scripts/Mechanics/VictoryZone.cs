using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Marks a trigger as a VictoryZone, usually used to end the current game level.
    /// </summary>
    public class VictoryZone : MonoBehaviour
    {
        [SerializeField] private Canvas victoryCanvas; // Référence au Canvas à afficher

        void Start()
        {
            if (victoryCanvas != null)
            {
                // Assurez-vous que le Canvas est désactivé au départ
                victoryCanvas.gameObject.SetActive(false);
            }
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.gameObject.GetComponent<PlayerController>();
            if (p != null)
            {
                var ev = Schedule<PlayerEnteredVictoryZone>();
                ev.victoryZone = this;

                // Affiche le Canvas de victoire
                if (victoryCanvas != null)
                {
                    victoryCanvas.gameObject.SetActive(true);
                }
            }
        }
    }
}
