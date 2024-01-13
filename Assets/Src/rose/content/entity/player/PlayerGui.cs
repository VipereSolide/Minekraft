using com.rose.content.world.entity.player.ui;
using UnityEngine;

namespace com.rose.content.world.entity.player
{
    public class PlayerGui : MonoBehaviour
    {
        [Header("Resources")]
        public Player player;

        [Header("Crosshair")]
        public CanvasGroup crosshairCanvasGroup;
        public float crosshairVisibilityLerpTime;

        [Header("Hotbar")]
        public PlayerHotbarGui hotbar;

        private void Update()
        {
            crosshairCanvasGroup.alpha = Mathf.Lerp(crosshairCanvasGroup.alpha, player.interactivity.isHitting ? 1 : 0, Time.deltaTime * crosshairVisibilityLerpTime);
        }
    }
}