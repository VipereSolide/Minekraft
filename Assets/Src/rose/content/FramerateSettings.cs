using UnityEngine;

namespace com.rose.content
{
    [CreateAssetMenu(fileName = "New Framerate Settings", menuName = "world/new framerate settings")]
    public class FramerateSettings : ScriptableObject
    {
        [Tooltip("Maximum framerate possible when teh game is focused.")]
        [Range(10, 600)]
        public int ingameTargetFramerate = 60;

        [Tooltip("Maximum framerate possible when the game is not focused.")]
        [Range(1, 120)]
        public int unfocusedTargetFramerate = 3;
    }
}