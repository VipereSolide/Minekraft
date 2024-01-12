using com.rose.content.world.generation;
using UnityEngine;

namespace com.rose.debugging.world.generation
{
    public class WorldGenerationDebugger : MonoBehaviour
    {
        public static WorldGenerationDebugger Instance { get; private set; }

        private static bool isStarted = false;
        public long totalChunkLoadingTime = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Quote))
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    foreach (var chunk in WorldGenerationEngine.Instance.chunks)
                    {
                        chunk.shouldUpdate = true;
                    }
                }
            }
        }

        public static void WorldGenerationEngineAwakens()
        {
            isStarted = true;
        }

        public static void AddChunkLoadingResult(Chunk chunk, long elapsedTime)
        {
            if (isStarted)
            {
                Instance.totalChunkLoadingTime += elapsedTime;
            }
        }
    }
}