using UnityEngine;

namespace Editor.Generator
{
    [CreateAssetMenu(fileName = "Appearance", menuName = "Generator/Appearance", order = 0)]
    public class Appearance : ScriptableObject
    {
        public GameObject PlayerPrefab;
        public GameObject Kitten;
        public GameObject Shooter;
        public GameObject Bomb;
        public GameObject Wall;
        public GameObject Ground;
        public GameObject Crate;
        public GameObject InvisibleWall;
        public GameObject Star;
    }
}