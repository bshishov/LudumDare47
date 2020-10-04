namespace Gameplay
{
    [System.Flags]   
    public enum ObjectType
    {
        None = 0,
        Character = 1,
        Wall = 2,
        Projectile = 4,
        Fence = 8
    }
}