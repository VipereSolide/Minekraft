using com.rose.content.world.generation;
using TMPro;
using UnityEngine;

namespace com.rose.debugging.world.generation
{
    public class WorldGenerationDebugger : MonoBehaviour
    {
        public static WorldGenerationDebugger Instance { get; private set; }

        private static bool isStarted = false;
        public static bool showChunkBorders = false;

        public TMP_Text routinesText;
        public long totalChunkLoadingTime = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            var gen = WorldGenerationEngine.Instance;
            routinesText.text = $"Chunk initialisation: {(gen.chunkInitializingRoutine.IsFree() ? "FREE" : $"{gen.chunkInitializingRoutine.waitingList.Count} left")}\nChunk cache: {(gen.updateRoutine.IsFree() ? "FREE" : $"{gen.updateRoutine.waitingList.Count} left")}";

            if (Input.GetKey(KeyCode.Quote))
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    foreach (var chunk in gen.chunks)
                    {
                        if (!gen.ShouldChunkBeRendered(chunk))
                            continue;

                        chunk.shouldUpdate = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.C))
                    showChunkBorders = !showChunkBorders;

                if (Input.GetKeyDown(KeyCode.L))
                {
                    var routine = gen.updateRoutine;
                    Debug.Log($"Routine list size: {routine.waitingList.Count}");
                    Debug.Log($"Routine updating chunks state:");

                    for (int i = 0; i < routine.updatingChunks.Length; i++)
                    {
                        Chunk chunk = routine.updatingChunks[i];
                        Debug.Log($"Is thread {i} free? {chunk == null}");
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