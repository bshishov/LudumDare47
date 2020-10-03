using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Utils
{
    public class CoroutineUtils
    {
        public static IEnumerator SlowUpdate([NotNull] Action<float> slowUpdate, float delay)
        {
            if(slowUpdate == null)
                throw new ArgumentNullException(nameof(slowUpdate));
            
            var delayWait = new WaitForSeconds(delay);
            while (true)
            {
                slowUpdate(delay);
                yield return delayWait;
            }
        }
    }
}