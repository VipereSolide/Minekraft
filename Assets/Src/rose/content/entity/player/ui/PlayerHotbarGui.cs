using com.rose.fundation.ui;
using UnityEngine;
using UnityEngine.UI;

namespace com.rose.content.world.entity.player.ui
{
    public class PlayerHotbarGui : MonoBehaviour
    {
        public Slot[] slots = new Slot[9];
        public int selectedSlot = 0;

        [Header("Resources")]

        public RectTransform hotbarSlotIndicator;
        public float slotSize;

        public RawImage[] slotImages;

        private void Start()
        {
            UpdateSlotIcons();
            UpdateUI();
        }

        private void UpdateSlotIcons()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].content == null)
                {
                    slotImages[i].texture = null;
                    slotImages[i].color = Color.clear;
                    continue;
                }

                slotImages[i].color = Color.white;
                slotImages[i].texture = slots[i].content.main.mainTexture;
            }
        }

        private void Update()
        {
            float scrollwheel = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scrollwheel < 0)
            {
                NextSlot();
            }
            else if (scrollwheel > 0)
            {
                PreviousSlot();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SelectSlot(0);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                SelectSlot(1);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                SelectSlot(2);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                SelectSlot(3);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                SelectSlot(4);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                SelectSlot(5);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                SelectSlot(6);
            if (Input.GetKeyDown(KeyCode.Alpha8))
                SelectSlot(7);
            if (Input.GetKeyDown(KeyCode.Alpha9))
                SelectSlot(8);
        }

        public void NextSlot()
        {
            selectedSlot++;
            if (selectedSlot >= slots.Length)
                selectedSlot = 0;

            UpdateUI();
        }

        public void PreviousSlot()
        {
            selectedSlot--;
            if (selectedSlot < 0)
                selectedSlot = slots.Length - 1;

            UpdateUI();
        }

        public void SelectSlot(int slotIndex)
        {
            selectedSlot = slotIndex;
            UpdateUI();
        }

        private void UpdateUI()
        {
            hotbarSlotIndicator.anchoredPosition = new Vector2(selectedSlot * slotSize, 0);
        }

        public Slot GetSelectedSlot()
        {
            return slots[selectedSlot];
        }
    }
}