using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Diep3D.Manager
{
    public class GameManager : NetworkBehaviour
    {
        static public GameManager s_Instance;

        //this is static so tank can be added even without the scene loaded (from lobby)
        static public List<TankManager> m_Tanks = new List<TankManager>();

        // The delay between the start of RoundStarting and RoundPlaying phases.
        public float m_StartDelay = 2f;
        public float m_EndDelay = 3f;
        public GameObject m_TankPrefab;
        public Transform[] m_SpawnPoints;

        private WaitForSeconds m_StartWait;
        private WaitForSeconds m_EndWait;

        void Awake()
        {
            s_Instance = this;
        }

        [ServerCallback]
        private void Start()
        {
            // Create the delays so they only have to be made once.
            m_StartWait = new WaitForSeconds(m_StartDelay);

            // Once the tanks have been created and the camera is using them as targets, start the game.
            StartCoroutine(GameLoop());
        }

        /// <summary>
        /// Add a tank from the lobby hook
        /// </summary>
        /// <param name="prefabPlayer">The actual GameObject instantiated by the lobby</param>
        /// <param name="num">The position of the player in the lobby</param>
        /// <param name="name">The name of the Player, choosen in the lobby</param>
        /// <param name="color">The color of the player, choosen in the lobby</param>
        static public void AddTank(GameObject prefabPlayer, int num, string name, Color color)
        {
            TankManager tank = new TankManager();
            tank.m_Instance = prefabPlayer;
            tank.m_PlayerNumber = num;
            tank.m_PlayerName = name;
            tank.m_PlayerColor = color;

            tank.Setup();

            m_Tanks.Add(tank);
        }

        /// <summary>
        /// Remove a tank
        /// </summary>
        /// <param name="tank">Tank GameObject</param>
        public void RemoveTank(GameObject tank)
        {
            // Search tank "to remove"
            TankManager toRemove = null;
            foreach (var tmp in m_Tanks)
            {
                if (tmp.m_Instance == tank)
                {
                    toRemove = tmp;
                    break;
                }
            }

            if (toRemove != null)
                m_Tanks.Remove(toRemove);
        }

        /// <summary>
        /// Game general loop: This is called from start and will run each phase of the game one after another
        /// Runned ONLY on server
        /// </summary>
        private IEnumerator GameLoop()
        {
            while (m_Tanks.Count < 1)
            {
                yield return null;
            }

            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(GameStarting());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(GamePlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine.
            yield return StartCoroutine(GameEnding());
        }

        private IEnumerator GameStarting()
        {
            //we notify all clients that the round is starting
            RpcGameStarting();

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_StartWait;
        }

        [ClientRpc]
        void RpcGameStarting()
        {
            // As soon as the round starts reset the tanks and make sure they can't move.
            ResetAllTanks();
            DisableTankControl();

            StartCoroutine(ClientGameStartingFade());
        }

        private IEnumerator ClientGameStartingFade()
        {
            float elapsedTime = 0.0f;
            float wait = m_StartDelay - 0.5f;

            yield return null;

            while (elapsedTime < wait)
            {
                elapsedTime += Time.deltaTime;

                //sometime, synchronization lag behind because of packet drop, so we make sure our tank are reseted
                if (elapsedTime / wait < 0.5f)
                {
                    ResetAllTanks();
                }

                yield return null;
            }
        }

        private IEnumerator GamePlaying()
        {
            //notify clients that the round is now started, they should allow player to move.
            RpcGamePlaying();

            while (!StopGame())
            {
                yield return null;
            }
        }

        [ClientRpc]
        void RpcGamePlaying()
        {
            // As soon as the round begins playing let the players control the tanks.
            EnableTankControl();
        }

        private IEnumerator GameEnding()
        {
            //notify client they should disable tank control
            RpcRoundEnding();

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_EndWait;
        }

        [ClientRpc]
        private void RpcRoundEnding()
        {
            DisableTankControl();
            StartCoroutine(ClientGameEndingFade());
        }

        private IEnumerator ClientGameEndingFade()
        {
            float elapsedTime = 0.0f;
            float wait = m_EndDelay;
            while (elapsedTime < wait)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool StopGame()
        {
            return false;
        }

        // This function is used to turn all the tanks back on and reset their positions and properties.
        private void ResetAllTanks()
        {
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                m_Tanks[i].m_SpawnPoint = m_SpawnPoints[Random.Range(0, m_SpawnPoints.Length)];
                m_Tanks[i].Reset();
            }
        }

        /// <summary>
        /// Enable players control
        /// </summary>
        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }

        /// <summary>
        /// Disable players control
        /// </summary>
        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }
    }
}
