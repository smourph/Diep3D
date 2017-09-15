using System;
using UnityEngine;

namespace Diep3D.Manager
{
    [Serializable]
    public class TankManager
    {
        [HideInInspector] public GameObject m_Instance;

        [HideInInspector] public int m_PlayerNumber;
        [HideInInspector] public Color m_PlayerColor;
        [HideInInspector] public string m_PlayerName;
        [HideInInspector] public Transform m_SpawnPoint;
        [HideInInspector] public GameObject m_TankBody;

        [HideInInspector] public Tank.TankMovement m_Movement;
        [HideInInspector] public Tank.TankShooting m_Shooting;
        [HideInInspector] public Tank.TankHealth m_Health;
        [HideInInspector] public Tank.TankSetupSync m_Setup;

        /// <summary>
        /// Build a tank and all its dependencies
        /// </summary>
        public void Setup()
        {
            // Get references to the local tank constructor
            m_Setup = m_Instance.GetComponent<Tank.TankSetupSync>();

            // Get references to Services.
            m_Movement = m_Instance.GetComponent<Tank.TankMovement>();
            m_Shooting = m_Instance.GetComponent<Tank.TankShooting>();
            m_Health = m_Instance.GetComponent<Tank.TankHealth>();

            // Get references to tank body
            m_TankBody = m_Health.m_TankBody;

            // Set self reference
            m_Health.m_Manager = this;

            // Set player infos
            m_Movement.m_PlayerNumber = m_PlayerNumber;
            m_Shooting.m_PlayerNumber = m_PlayerNumber;
            m_Shooting.m_PlayerColor = m_PlayerColor;

            //setup is use for diverse Network Related sync
            m_Setup.m_Color = m_PlayerColor;
            m_Setup.m_PlayerName = m_PlayerName;
            m_Setup.m_PlayerNumber = m_PlayerNumber;
        }

        /// <summary>
        /// Disable player control
        /// </summary>
        public void DisableControl()
        {
            m_Movement.enabled = false;
            m_Shooting.enabled = false;
        }

        /// <summary>
        /// Enable player control
        /// </summary>
        public void EnableControl()
        {
            m_Movement.enabled = true;
            m_Shooting.enabled = true;
        }

        /// <summary>
        /// Return player name
        /// </summary>
        public string GetName()
        {
            return m_Setup.m_PlayerName;
        }

        /// <summary>
        /// Return player color
        /// </summary>
        public Color GetColor()
        {
            return m_Setup.m_Color;
        }

        /// <summary>
        /// Define if player is the leader
        /// </summary>
        /// <param name="leader">Toggle leader statut</param>
        public void SetLeader(bool leader)
        {
            m_Setup.SetLeader(leader);
        }

        /// <summary>
        /// Used after each respawn to put tank into it's default state
        /// </summary>
        public void Reset()
        {
            m_Movement.SetDefaults();
            m_Shooting.SetDefaults();
            m_Health.SetDefaults();

            if (m_Movement.hasAuthority)
            {
                m_Movement.m_Rigidbody.position = m_SpawnPoint.position;
                m_Movement.m_Rigidbody.rotation = m_SpawnPoint.rotation;
            }
        }
    }
}