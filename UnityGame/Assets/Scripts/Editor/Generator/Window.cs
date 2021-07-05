using System.Collections.Generic;
using Editor.Generator.Csp;
using Gameplay;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor.Generator
{
    public class Window : EditorWindow
    {
        private LevelState _levelState;
        private Appearance _appearance;
        private int _width = 6;
        private int _height = 6;
        
        [MenuItem("Window/Generator")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<Window>();
            window.Show();
        }

        void OnGUI()
        {
            _appearance = EditorGUILayout.ObjectField(_appearance, typeof(Appearance), allowSceneObjects: false) as Appearance;

            if (GUILayout.Button("Generate"))
            {
                _levelState = new LevelState(_width, _height);

                var constraints = new List<IConstraint<LevelState>>
                {
                    new AllShootersAreShootingSomethingUseful(
                        shootThis: new HashSet<TileType>
                        {
                            TileType.CatGirl,
                            TileType.Player,
                            TileType.Bomb
                        }, 
                        dontShootThis: new HashSet<TileType>
                        {
                            TileType.Wall,
                            TileType.Shooter,
                            TileType.Box,
                        }
                    ),
                    //new ThereShouldBeLessThanXOfTiles(TileType.Player, 2),
                    //new ThereShouldBeLessThanXOfTiles(TileType.CatGirl, 2),
                    //new ThereShouldBeLessThanXOfTiles(TileType.Star, 3),
                    // new ThereShouldBeLessThanXOfTiles(TileType.Wall, 3),
                    //new ThereShouldBeExactNumberOfTiles(TileType.Shooter, 1),
                    // new ThereShouldBeLessThanXOfTiles(TileType.InvisibleWall, 3),
                    // new ThereShouldBeLessThanXOfTiles(TileType.Box, 3),
                    // ThereShouldBeAWalkableSpaceAroundPlayer()
                    // A CatGirlShouldBeInDanger
                };

                var actions = new List<IAction<LevelState>>();

                for (var i = 0; i < _levelState.Width * _levelState.Height; i++)
                {    
                    // Position related actions
                    actions.Add(new SetTileAtPosition(TileType.Player, i, Direction.Front, limit: 1));
                    actions.Add(new SetTileAtPosition(TileType.CatGirl, i, Direction.Right, limit: 1));
                    actions.Add(new SetTileAtPosition(TileType.Wall, i, limit: 1));
                    actions.Add(new SetTileAtPosition(TileType.Ground, i));
                    actions.Add(new SetTileAtPosition(TileType.InvisibleWall, i, limit: 3));
                    actions.Add(new SetTileAtPosition(TileType.Box, i, limit: 3));
                    actions.Add(new SetTileAtPosition(TileType.Star, i, limit: 2));
                    actions.Add(new SetTileAtPosition(TileType.Shooter, i, Direction.Front, limit: 1));
                    actions.Add(new SetTileAtPosition(TileType.Shooter, i, Direction.Back, limit: 1));
                    actions.Add(new SetTileAtPosition(TileType.Shooter, i, Direction.Left, limit: 1));
                    actions.Add(new SetTileAtPosition(TileType.Shooter, i, Direction.Right, limit: 1));
                    actions.Add(new SetTileAtPosition(TileType.Bomb, i, limit: 1));
                }
                
                //actions.Add(new SpawnShooterLookingBackAtCatGirl());

                var problem = new Problem<LevelState>(_levelState, constraints);
                if (!problem.TrySolve(actions))
                {
                    Debug.Log("Failed to Solve :(");
                }

                Build();
            }

            _width = EditorGUILayout.IntField("Width", _width);
            _height = EditorGUILayout.IntField("Height", _height);
        }

        private const string RootObjectName = "[GENERATED]";
        
        void Build()
        {
            if(_levelState == null || _appearance == null)
                return;
            
            var root = GameObject.Find(RootObjectName);
            if (root == null)
            {
                root = new GameObject(RootObjectName);
            }
            
            // Destroy children
            for (var i = root.transform.childCount; i > 0; --i)
                DestroyImmediate(root.transform.GetChild(0).gameObject);

            var w = _levelState.Width;
            var h = _levelState.Height;
            
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    TryPlaceObject(root, x, y, _levelState.GetDirectionAt(x, y), _levelState.GetTileAt(x, y));
                }
            }
        }

        private void TryPlaceObject(GameObject root, int x, int y, Direction direction, TileType tile)
        {
            if (tile == TileType.Player)
            {
                PlacePrefab(root, _appearance.PlayerPrefab, x, y, direction);
                PlacePrefab(root, _appearance.Ground, x, y, direction);
            }
            
            if (tile == TileType.CatGirl)
            {
                PlacePrefab(root, _appearance.Kitten, x, y, direction);
                PlacePrefab(root, _appearance.Ground, x, y, direction);
            }
            
            if (tile == TileType.Wall)
            {
                PlacePrefab(root, _appearance.Wall, x, y, direction);
                PlacePrefab(root, _appearance.Ground, x, y, direction);
            }
            
            if (tile == TileType.Box)
            {
                PlacePrefab(root, _appearance.Crate, x, y, direction);
                PlacePrefab(root, _appearance.Ground, x, y, direction);
            }
            
            if (tile == TileType.Ground)
            {
                PlacePrefab(root, _appearance.Ground, x, y, direction);
            }
            
            if (tile == TileType.Shooter)
            {
                PlacePrefab(root, _appearance.Shooter, x, y, direction);
                PlacePrefab(root, _appearance.Ground, x, y, direction);
            }
            
            if (tile == TileType.Star)
            {
                PlacePrefab(root, _appearance.Star, x, y, direction);
                PlacePrefab(root, _appearance.Ground, x, y, direction);
            }
            
            if (tile == TileType.Bomb)
            {
                PlacePrefab(root, _appearance.Bomb, x, y, direction);
                PlacePrefab(root, _appearance.Ground, x, y, direction);
            }
            
            if (tile == TileType.InvisibleWall)
            {
                PlacePrefab(root, _appearance.InvisibleWall, x, y, direction);
            }
        }

        void PlacePrefab(GameObject root, GameObject prefab, int x, int y, Direction direction)
        {
            if (prefab != null)
            {
                var pos = Gameplay.Utils.LevelToWorld(new Vector2Int(x, y));
                var rot = Gameplay.Utils.DirectionToRotation(direction);
                Instantiate(prefab, pos, rot, root.transform);
            }
        }
    }
}
