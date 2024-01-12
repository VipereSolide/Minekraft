using System;

namespace com.rose.content.events
{
    public static class EventManager
    {
        /// <summary>
        /// Called whenever the world generation engine finished creating values for the heightmap.
        /// </summary>
        public static Action onFinishedPopulatingHeightmap;
    }
}