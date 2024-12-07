using Platformer.Core;
using Platformer.Model;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This class exposes the the game model in the inspector, and ticks the
    /// simulation.
    /// </summary> 
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        //This model field is public and can be therefore be modified in the 
        //inspector.
        //The reference actually comes from the InstanceRegister, and is shared
        //through the simulation and events. Unity will deserialize over this
        //shared reference when the scene loads, allowing the model to be
        //conveniently configured inside the inspector.
        public PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        void Update()
        {
            if (Instance == this) Simulation.Tick();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Reinitialize the model references
            model.virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            model.player = FindObjectOfType<PlayerController>();
            GameObject spawnPointObj = GameObject.Find("SpawnPoint");
            if (spawnPointObj != null)
            {
                model.spawnPoint = spawnPointObj.transform;
            }
            else
            {
                UnityEngine.Debug.LogError("SpawnPoint GameObject not found");
            }

        }
    }
}