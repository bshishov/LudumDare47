namespace Utils.Debugger
{
    public class DrawingContext
    {
        public bool ActionRequested = false;
        public bool CollapseRequested = false;
        public int CursorIndex = 0;
        public int Depth = 0;
        public int Index = 0;
        public Style Style = new Style();
        public float Y = 0f;
        public bool MouseCursorIsOverUI;
    }
}