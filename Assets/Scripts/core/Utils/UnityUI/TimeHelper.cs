using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevilMind.Utils
{
    public static class TimeHelper
    {
        public static IEnumerator RunAfterFrames(int frames, System.Action action)
        {
            if (action == null)
            {
                yield break;
            }

            int frameNumber = 0;
            while (frameNumber < frames)
            {
                yield return new WaitForEndOfFrame();
                frameNumber++;
            }

            action();
        }

        public static IEnumerator RunAfterSeconds(float seconds, System.Action action)
        {
            if (action == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(seconds);
            action();
        }
    }
}