using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Diep3D.Tank
{
    public class TankSetupSync : NetworkBehaviour
    {
        [Header("UI")]
        public Text m_NameText;
        public GameObject m_Crown;

        [Header("Network")]
        [HideInInspector] [SyncVar] public Color m_Color;
        [HideInInspector] [SyncVar] public string m_PlayerName;
        [HideInInspector] [SyncVar] public int m_PlayerNumber;

        //This allow to know if the crown must be displayed or not
        protected bool m_isLeader = false;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!isServer)
            {
                Manager.GameManager.AddTank(gameObject, m_PlayerNumber, m_PlayerName, m_Color);
            }

            gameObject.tag = "Player";

            GameObject m_TankBody = transform.Find("TankBody").gameObject;

            // Colorize tank
            Renderer[] renderers = m_TankBody.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = m_Color;
            }

            if (m_TankBody)
            {
                m_TankBody.SetActive(false);
            }

            m_NameText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(m_Color) + ">" + m_PlayerName + "</color>";
            m_Crown.SetActive(false);
        }

        public void SetLeader(bool leader)
        {
            RpcSetLeader(leader);
        }

        [ClientRpc]
        public void RpcSetLeader(bool leader)
        {
            m_isLeader = leader;
        }

        public void ActivateCrown(bool active)
        {
            m_Crown.SetActive(active ? m_isLeader : false);
            m_NameText.gameObject.SetActive(active);
        }

        public override void OnNetworkDestroy()
        {
            Manager.GameManager.s_Instance.RemoveTank(gameObject);
        }
    }
}