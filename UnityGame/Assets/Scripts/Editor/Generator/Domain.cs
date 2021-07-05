using System;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using Utils;

namespace Editor.Generator.Csp
{
    public enum TileType
    {
        Unset = 0,
        Ground,
        Player,
        CatGirl,
        Wall,
        Shooter,
        InvisibleWall,
        Box,
        Star,
        Bomb
    }
    
    public class LevelState: ISolvable
    {
        public readonly int Width;
        public readonly int Height;
        
        private readonly TileType[] _tiles;
        private readonly Direction[] _directions;
        private readonly List<int> _catLocations = new List<int>(2);
        private readonly List<int> _shooterLocations = new List<int>(10);
        private readonly Dictionary<TileType, int> _tileCounts = new Dictionary<TileType, int>();

        public LevelState(int width, int height)
        {
            Width = width;
            Height = height;

            _tiles = new TileType[width * height];
            _directions = new Direction[width * height];

            var range = new List<int>();
            for (var i = 0; i < _tiles.Length; i++)
            {
                _tiles[i] = TileType.Unset;
                _directions[i] = Direction.Front;
                range.Add(i);
            }

            range.Shuffle();

            foreach (TileType tile in Enum.GetValues(typeof(TileType)))
                _tileCounts.Add(tile, 0);

            _tileCounts[TileType.Unset] = _tiles.Length;
        }

        public TileType GetTileAt(int x, int y)
        {
            return _tiles[y * Width + x];
        }
        
        public TileType GetTileByIndex(int ix)
        {
            return _tiles[ix];
        }

        public int GetIndexFromPosition(int x, int y)
        {
            return y * Width + x;
        }

        public void SetTileAt(int index, TileType tile, Direction direction)
        {
            switch (tile)
            {
                case TileType.CatGirl:
                    _catLocations.Add(index);
                    break;
                case TileType.Shooter:
                    _shooterLocations.Add(index);
                    break;
                default:
                    break;
            }

            _tileCounts[TileType.Unset] -= 1;
            _tileCounts[tile] += 1;
            
            _tiles[index] = tile;
            _directions[index] = direction;
        }
        
        public void UnsetTileAt(int index)
        {
            var oldTile = _tiles[index];
            switch (oldTile)
            {
                case TileType.CatGirl:
                    _catLocations.RemoveAt(_catLocations.Count - 1);
                    break;
                case TileType.Shooter:
                    _shooterLocations.RemoveAt(_shooterLocations.Count - 1);
                    break;
                default:
                    break;
            }

            _tileCounts[oldTile] -= 1;
            _tileCounts[TileType.Unset] += 1;
            
            _tiles[index] = TileType.Unset;
            _directions[index] = Direction.Front;            
        }

        public Vector2Int GetPositionFromIndex(int index)
        {
            return new Vector2Int(index % Width, index / Width);
        }

        public int CountTiles(TileType tile)
        {
            return _tileCounts[tile];
        }

        public int SearchTile(TileType tile)
        {
            for (var i = 0; i < _tiles.Length; i++)
            {
                if (_tiles[i] == tile)
                    return i;
            }

            return -1;
        }

        public bool IsUnset(int index)
        {
            return _tiles[index] == TileType.Unset;
        }

        public bool IsSolved()
        {
            return _tileCounts[TileType.Unset] == 0;
        }

        public Direction GetDirectionAt(int x, int y)
        {
            return _directions[x + y * Width];
        }
        
        public Direction GetDirectionAt(int index)
        {
            return _directions[index];
        }

        public int GetCatGirlIndex()
        {
            if(_catLocations.Count > 0)
                return _catLocations[_catLocations.Count - 1];
            return -1;
        }

        public IEnumerable<int> GetShooterLocations()
        {
            return _shooterLocations;
        }
    }

    public class TileSetEffect : IActionEffect<LevelState>
    {
        private readonly int _index;
        private readonly TileType _tile;
        private readonly IEnumerable<IConstraint<LevelState>> _constraints;
        private readonly Direction _direction;

        public TileSetEffect(
            int index, 
            TileType tile, 
            Direction direction,
            IEnumerable<IConstraint<LevelState>> constraints)
        {
            _index = index;
            _tile = tile;
            _direction = direction;
            _constraints = constraints;
        }
        
        public void Apply(LevelState state)
        {
            state.SetTileAt(_index, _tile, _direction);
        }

        public void Rollback(LevelState state)
        {
            state.UnsetTileAt(_index);
        }

        public IEnumerable<IConstraint<LevelState>> EnforcedConstraints()
        {
            return _constraints;
        }
    }

    public class SetTileAtPosition : IAction<LevelState>
    {
        private readonly TileType _tile;
        private readonly IConstraint<LevelState>[] _constraints;
        private readonly int _index;
        private readonly Direction _direction;
        private readonly int _limit;

        public SetTileAtPosition(TileType tile, int index, Direction direction = Direction.Front, int limit = 1000)
        {
            _tile = tile;
            _index = index;
            _constraints = new IConstraint<LevelState>[0];
            _direction = direction;
            _limit = limit;
        }

        public bool CanPerform(LevelState state)
        {
            return state.IsUnset(_index) && state.CountTiles(_tile) < _limit;
        }

        public IActionEffect<LevelState> Perform(LevelState state)
        {
            return new TileSetEffect(_index, _tile, _direction, _constraints);
        }
    }

    public class ThereShouldBeLessThanXOfTiles : IConstraint<LevelState>
    {
        private readonly int _amount;
        private readonly TileType _tile;

        public ThereShouldBeLessThanXOfTiles(TileType tile, int amount)
        {
            _amount = amount;
            _tile = tile;
        }
        
        public bool IsViolated(LevelState state)
        {
            return state.CountTiles(_tile) >= _amount;
        }
    }
    
    public class ThereShouldBeExactNumberOfTiles : IConstraint<LevelState>
    {
        private readonly int _amount;
        private readonly TileType _tile;

        public ThereShouldBeExactNumberOfTiles(TileType tile, int amount)
        {
            _amount = amount;
            _tile = tile;
        }
        
        public bool IsViolated(LevelState state)
        {
            return state.IsSolved() && state.CountTiles(_tile) != _amount;
        }
    }

    public class ThereShouldBeAtLeastOneTileInIndices : IConstraint<LevelState>
    {
        private readonly IEnumerable<int> _lookupIndices;
        private readonly TileType _tile;

        public ThereShouldBeAtLeastOneTileInIndices(TileType tile, IEnumerable<int> lookupIndices)
        {
            _tile = tile;
            _lookupIndices = lookupIndices;
        }
        
        public bool IsViolated(LevelState state)
        {
            foreach (var index in _lookupIndices)
            {
                var tile = state.GetTileByIndex(index);
                if (tile == TileType.Unset || tile == _tile)
                {
                    // There is or might potentially be a queried tile
                    return false;
                }
            }
            return true;
        }
    }

    public class SpawnShooterLookingBackAtCatGirl : IAction<LevelState>
    {
        public bool CanPerform(LevelState state)
        {
            var catGirlIndex = state.GetCatGirlIndex();
            if (catGirlIndex < 0)  
                return false;

            var catPos = state.GetPositionFromIndex(catGirlIndex);
            var hasUnsetCell = false;
            for (var y = catPos.y + 1; y < state.Height; y++)
            {
                if (state.GetTileAt(catPos.x, y) == TileType.Unset)
                {
                    hasUnsetCell = true;
                    break;
                }
            }

            return hasUnsetCell;
        }

        public IActionEffect<LevelState> Perform(LevelState state)
        {
            var catIndex = state.GetCatGirlIndex();
            var catPos = state.GetPositionFromIndex(catIndex);
            var unsetPositions = new List<int>();
            for (var y = catPos.y + 1; y < state.Height; y++)
            {
                if (state.GetTileAt(catPos.x, y) == TileType.Unset)
                {
                    unsetPositions.Add(state.GetIndexFromPosition(catPos.x, y));
                }
            }

            var unsetTileIndex = RandomUtils.Choice(unsetPositions);
            var pos = state.GetPositionFromIndex(unsetTileIndex);
            
            var targetIndices = new List<int>();
            for (var y = pos.y - 1; y >= catPos.y; y--)
                targetIndices.Add(pos.x + y * state.Width);

            var constraints = new List<IConstraint<LevelState>>
            {
                new ThereShouldBeAtLeastOneTileInIndices(TileType.CatGirl, targetIndices)
            };

            return new TileSetEffect(unsetTileIndex, TileType.Shooter, Direction.Back, constraints);
        }
    }

    public class AllShootersAreShootingSomethingUseful : IConstraint<LevelState>
    {
        private readonly HashSet<TileType> _shootThis;
        private readonly HashSet<TileType> _dontShootThis;

        public AllShootersAreShootingSomethingUseful(HashSet<TileType> shootThis, HashSet<TileType> dontShootThis)
        {
            _shootThis = shootThis;
            _dontShootThis = shootThis;
        }
        
        public bool IsViolated(LevelState state)
        {
            foreach (var shooterLocation in state.GetShooterLocations())
            {
                var pos = state.GetPositionFromIndex(shooterLocation);
                var delta = Gameplay.Utils.MoveDelta(state.GetDirectionAt(shooterLocation));

                while (true)
                {
                    pos += delta;  // Following bullet trajectory
                    
                    if (pos.x < 0 || pos.x >= state.Width || pos.y < 0 || pos.y >= state.Height)
                        return true;  // Shooting walls
                    
                    var tileAtPos = state.GetTileAt(pos.x, pos.y);

                    if (tileAtPos == TileType.Unset)
                    {
                        // Special case. There potentialy could be something, not violated yet
                        break; 
                    }

                    if (_shootThis.Contains(tileAtPos))
                    {
                        // Current guy shoots something he needs to shoot
                        // Non-violating the constraint
                        break;
                    }

                    if (_dontShootThis.Contains(tileAtPos))
                    {
                        // Violating the rules
                        return true;
                    }
                }
            }

            return false; 
        }
    }
}