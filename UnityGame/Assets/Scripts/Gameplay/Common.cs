using System.Collections.Generic;

namespace Gameplay
{
    public enum ObjectType
    {
        Default,
        Player,
        Character,
        Wall,  // Or Train also?
        Projectile,
        Fence,
        Box,
        Spark,
        // Any type you want goes here
        Collectable,
        Rails,
        // The more generic the type is - the better
    }

    public static class CollisionConfig
    {
        // Who can collide with whom?
        // Collision between A and B means that two object cant stay on the same cell
        public static readonly HashSet<CPair> CollideMap = new HashSet<CPair>
        {
            new CPair(ObjectType.Character, ObjectType.Character),
            new CPair(ObjectType.Character, ObjectType.Wall),
            new CPair(ObjectType.Character, ObjectType.Player),
            new CPair(ObjectType.Player, ObjectType.Character),
            new CPair(ObjectType.Player, ObjectType.Player),
            new CPair(ObjectType.Projectile, ObjectType.Wall),
            new CPair(ObjectType.Player, ObjectType.Wall),
            new CPair(ObjectType.Player, ObjectType.Projectile),
            new CPair(ObjectType.Projectile, ObjectType.Player),
            new CPair(ObjectType.Projectile, ObjectType.Projectile),
            new CPair(ObjectType.Player, ObjectType.Box),
            new CPair(ObjectType.Box, ObjectType.Player),
            new CPair(ObjectType.Projectile, ObjectType.Box),
            new CPair(ObjectType.Box, ObjectType.Projectile),
            new CPair(ObjectType.Box, ObjectType.Wall),
            new CPair(ObjectType.Wall, ObjectType.Box),
            new CPair(ObjectType.Box, ObjectType.Character),
            new CPair(ObjectType.Character, ObjectType.Box),
            new CPair(ObjectType.Box, ObjectType.Fence),
            new CPair(ObjectType.Fence, ObjectType.Box),
            new CPair(ObjectType.Box, ObjectType.Box),
            new CPair(ObjectType.Fence, ObjectType.Wall),
            new CPair(ObjectType.Fence, ObjectType.Player),
            new CPair(ObjectType.Fence, ObjectType.Character),            
            new CPair(ObjectType.Wall, ObjectType.Wall),
            new CPair(ObjectType.Projectile, ObjectType.Character),

        };
        
        // Who can hit whom?
        // Hit is done separately from collisions
        public static readonly HashSet<CPair> HitsMap = new HashSet<CPair>
        {
            new CPair(ObjectType.Character, ObjectType.Character),
            new CPair(ObjectType.Character, ObjectType.Player),
            new CPair(ObjectType.Player, ObjectType.Player),
            new CPair(ObjectType.Projectile, ObjectType.Character),
            new CPair(ObjectType.Projectile, ObjectType.Player),
            new CPair(ObjectType.Projectile, ObjectType.Wall),
            new CPair(ObjectType.Projectile, ObjectType.Box),
            new CPair(ObjectType.Wall, ObjectType.Character),
            new CPair(ObjectType.Wall, ObjectType.Player),
            new CPair(ObjectType.Box, ObjectType.Projectile),
            new CPair(ObjectType.Character, ObjectType.Wall),
            
            // Spark
            new CPair(ObjectType.Spark, ObjectType.Character),
            new CPair(ObjectType.Spark, ObjectType.Player),

            new CPair(ObjectType.Collectable, ObjectType.Player),
            new CPair(ObjectType.Collectable, ObjectType.Box),
            //new CPair(ObjectType.Collectable, ObjectType.Projectile),
            new CPair(ObjectType.Collectable, ObjectType.Wall),
            new CPair(ObjectType.Collectable, ObjectType.Character),


        };
        
        // Who can push whom?
        public static readonly HashSet<CPair> PushMap = new HashSet<CPair>
        {           
            new CPair(ObjectType.Character, ObjectType.Player),
            new CPair(ObjectType.Player, ObjectType.Character),
            new CPair(ObjectType.Player, ObjectType.Player),
            new CPair(ObjectType.Player, ObjectType.Projectile),
            new CPair(ObjectType.Player, ObjectType.Box),
        };
        
        public static bool ObjectsCollide(ObjectType a, ObjectType b)
        {
            return CollideMap.Contains(new CPair(a, b)) || 
                   CollideMap.Contains(new CPair(b, a));
        }
        
        public static bool ObjectsHit(ObjectType a, ObjectType b)
        {
            return HitsMap.Contains(new CPair(a, b)) || 
                   HitsMap.Contains(new CPair(b, a));
        }

        /// <summary>
        /// Function that checks whether an object can be pushed by another object.
        /// NOTE! "Push" can happen only when objects collide
        /// </summary>
        /// <param name="pusher">Object that pushes</param>
        /// <param name="pushee">Object that is being pushed</param>
        /// <returns></returns>
        public static bool CanPush(ObjectType pusher, ObjectType pushee)
        {
            return PushMap.Contains(new CPair(pusher, pushee));
        }
    }

    public struct CPair
    {
        private readonly ObjectType _a;
        private readonly ObjectType _b;
        
        public CPair(ObjectType a, ObjectType b)
        {
            _a = a;
            _b = b;
        }
        
        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + _a.GetHashCode();
            hash = (hash * 7) + _b.GetHashCode();
            return hash;
        }
    }
}