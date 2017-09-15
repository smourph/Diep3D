using UnityEngine;
using UnityEngine.Networking;

namespace Diep3D.Lobby
{
    public class TankLobbyHook : Prototype.NetworkLobby.LobbyHook
    {
        /// <summary>
        /// Hook called on the server by lobby when a player is loaded
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="lobbyPlayer">The player object from lobby</param>
        /// <param name="gamePlayer">The prefab use to instantiate player in game</param>
        public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
        {
            if (lobbyPlayer == null)
            {
                return;
            }

            Prototype.NetworkLobby.LobbyPlayer lp = lobbyPlayer.GetComponent<Prototype.NetworkLobby.LobbyPlayer>();

            if (lp != null)
            {
                Manager.GameManager.AddTank(gamePlayer, lp.slot, lp.nameInput.text, lp.playerColor);
            }
        }
    }
}