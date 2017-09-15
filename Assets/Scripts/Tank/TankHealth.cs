using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Diep3D.Tank
{
    public class TankHealth : NetworkBehaviour
    {
        public float m_StartingHealth = 100f;

        // The slider to represent how much health the tank currently has.
        public Slider m_Slider;
        // The image component of the slider.
        public Image m_FillImage;
        public Color m_FullHealthColor = Color.green;
        public Color m_ZeroHealthColor = Color.red;

        public ParticleSystem m_ExplosionParticles;
        // References to all the gameobjects that need to be disabled when the tank is dead.
        public GameObject m_TankBody;
        public GameObject m_HealthCanvas;

        // Associated manager, to disable control when dying.
        public Manager.TankManager m_Manager;

        // How much health the tank currently has
        [SyncVar(hook = "OnCurrentHealthChanged")] private float m_CurrentHealth;
        // Has the tank been reduced beyond zero health yet?
        [SyncVar] private bool m_ZeroHealthHappened;
        // Used so that the tank doesn't collide with anything when it's dead.
        private BoxCollider[] m_Colliders;

        private void Awake()
        {
            m_Colliders = GetComponentsInChildren<BoxCollider>();
        }

        /// <summary>
        /// Apply damages to tank
        /// </summary>
        /// <param name="amount">Amount of damage</param>
        public void Damage(float amount)
        {
            // Reduce current health by the amount of damage done.
            m_CurrentHealth -= amount;

            // If the current health is at or below zero and it has not yet been registered, call OnZeroHealth.
            if (m_CurrentHealth <= 0f && !m_ZeroHealthHappened)
            {
                OnZeroHealth();
            }
        }

        /// <summary>
        /// Redraw healthUI
        /// </summary>
        private void SetHealthUI()
        {
            // Set the slider's value appropriately.
            m_Slider.value = m_CurrentHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }

        /// <summary>
        /// Hooked on m_CurrentHealth to update UI
        /// </summary>
        /// <param name="value">Current health value</param>
        void OnCurrentHealthChanged(float value)
        {
            m_CurrentHealth = value;
            // Change the UI elements appropriately.
            SetHealthUI();

        }

        /// <summary>
        /// Called once when tank has zero health
        /// </summary>
        private void OnZeroHealth()
        {
            // Set the flag so that this function is only called once.
            m_ZeroHealthHappened = true;

            RpcOnZeroHealth();
        }

        /// <summary>
        /// Internal actions when player die
        /// </summary>
        private void InternalOnZeroHealth()
        {
            // Disable the collider and all the appropriate child gameobjects so the tank doesn't interact or show up when it's dead.
            SetTankActive(false);
        }

        /// <summary>
        /// RPC clients actions when player die
        /// </summary>
        [ClientRpc]
        private void RpcOnZeroHealth()
        {
            // Play the particle system of the tank exploding.
            m_ExplosionParticles.Play();

            InternalOnZeroHealth();
        }

        /// <summary>
        /// Set tank statut
        /// </summary>
        /// <param name="active">Toggle active statut</param>
        private void SetTankActive(bool active)
        {
            foreach(Collider elementCollider in m_Colliders)
            {
                elementCollider.enabled = active;
            }

            m_TankBody.SetActive(active);
            m_HealthCanvas.SetActive(active);

            if (active)
            {
                m_Manager.EnableControl();
            }
            else
            {
                m_Manager.DisableControl();
            }
        }

        /// <summary>
        /// This function is called to make sure each tank is set up correctly.
        /// </summary>
        public void SetDefaults()
        {
            m_CurrentHealth = m_StartingHealth;
            m_ZeroHealthHappened = false;
            SetTankActive(true);
        }
    }
}