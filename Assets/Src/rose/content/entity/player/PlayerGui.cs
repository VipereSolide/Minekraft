using com.rose.content.world.entity.player.ui;
using com.rose.fundation.extensions;
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
        public RectTransform hotbarRect;
        public float hotbarLifetime = 6;
        public float hotbarLerpSpeed = 4;
        public float hotbarOutPosition;
        public float hotbarInPosition;
        public bool isHotbarVisible;
        public float currentHotbarLifetime;

        private void Awake()
        {
            hotbar.onSelectSlot += OnSelectSlot;
        }

        private void OnSelectSlot(int previouslySelectedSlot)
        {
            currentHotbarLifetime = hotbarLifetime;
        }

        private void Update()
        {
            currentHotbarLifetime -= Time.deltaTime;
            isHotbarVisible = currentHotbarLifetime > 0 || hotbar.GetSelectedSlot().content != null;

            hotbarRect.anchoredPosition = Vector2.MoveTowards(
                hotbarRect.anchoredPosition,
                hotbarRect.anchoredPosition.WithY(isHotbarVisible ? hotbarOutPosition : hotbarInPosition),
                Time.deltaTime * hotbarLerpSpeed);

            crosshairCanvasGroup.alpha = Mathf.Lerp(crosshairCanvasGroup.alpha, player.interactivity.isHitting ? 1 : 0, Time.deltaTime * crosshairVisibilityLerpTime);
        }
    }
}