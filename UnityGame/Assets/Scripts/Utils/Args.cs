using System.Collections.Generic;

namespace Utils
{
    public static class Args
    {
        public static bool Parse(this IList<string> args, out string x)
        {
            if (args == null || args.Count < 1)
            {
                x = null;
                return false;
            }

            x = args[0];
            return true;
        }
        
        public static bool Parse(this IList<string> args, out float x)
        {
            if (args == null || args.Count < 1)
            {
                x = 0f;
                return false;
            }

            return float.TryParse(args[0], out x);
        }
        
        public static bool Parse(this IList<string> args, out string x, out float y)
        {
            if (args == null || args.Count < 2)
            {
                x = null;
                y = 0f;
                return false;
            }

            x = args[0];
            return float.TryParse(args[1], out y);
        }
        
        public static bool Parse(this IList<string> args, out int x)
        {
            if (args == null || args.Count < 1)
            {
                x = 0;
                return false;
            }

            return int.TryParse(args[0], out x);
        }
        
        public static bool Parse(this IList<string> args, out string x, out int y)
        {
            if (args == null || args.Count < 2)
            {
                x = null;
                y = 0;
                return false;
            }

            x = args[0];
            return int.TryParse(args[1], out y);
        }
    }
}