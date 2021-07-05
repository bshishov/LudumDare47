using System;
using System.Collections.Generic;

namespace Editor.Generator
{
    public static class ListExt
    {
        private static readonly Random _rng = new Random();  
        
        public static void Shuffle<T>(this IList<T> list)  
        {  
            var n = list.Count;  
            while (n > 1) {  
                n--;  
                var k = _rng.Next(n + 1);  
                var value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }    
    }
}