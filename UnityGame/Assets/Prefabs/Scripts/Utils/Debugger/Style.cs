using UnityEngine;

namespace Utils.Debugger
{
    public class Style
    {
        public Color ContentColor = new Color(0.1f, 0.1f, 0.1f, .9f);
        public string DefaultFontName = "Consolas";
        public Color HeaderColor = new Color(0.1f, 0.1f, 0.1f, .9f);
        public float HeaderColumn = 200f;
        public float LineHeight = 20f;
        public float Padding = 10f;
        public Color PropertyHeaderColor = new Color(0.1f, 0.1f, 0.3f, .9f);
        public Color SelectedHeaderColor = new Color(0.4f, 0.0f, 0.0f, .9f);
        public Color LogLine1Bgr = new Color(0.1f, 0.1f, 0.1f, .9f);
        public Color LogLine2Bgr = new Color(0.15f, 0.15f, 0.15f, .9f);

        public bool IsInitialized { get; private set; }
        public Font DefaultFont { get; private set; }
        public GUIStyle HeaderStyle { get; private set; }
        public GUIStyle PropertyHeaderStyle { get; private set; }
        public GUIStyle SelectedHeaderStyle { get; private set; }
        public GUIStyle ContentStyle { get; private set; }
        public GUIStyle LabelStyle { get; private set; }
        public GUIStyle LogLineStyle { get; private set; }
        public GUIStyle LogLineStyleAlt { get; private set; }

        public void Initialize()
        {
            DefaultFont = Font.CreateDynamicFontFromOSFont(DefaultFontName, 14);
            HeaderStyle = CreateLabelStyle(DefaultFont, HeaderColor);
            PropertyHeaderStyle = CreateLabelStyle(DefaultFont, PropertyHeaderColor);
            SelectedHeaderStyle = CreateLabelStyle(DefaultFont, SelectedHeaderColor);
            ContentStyle = CreateLabelStyle(DefaultFont, ContentColor);
            LabelStyle = CreateLabelStyle(DefaultFont);

            LogLineStyle = CreateLabelStyle(DefaultFont, LogLine1Bgr);
            LogLineStyleAlt = CreateLabelStyle(DefaultFont, LogLine2Bgr);
            
            LogLineStyle.padding = new RectOffset(5, 5, 5, 8);
            LogLineStyle.wordWrap = true;
            LogLineStyleAlt.padding = new RectOffset(5, 5, 5, 8);
            LogLineStyleAlt.wordWrap = true;
            
            IsInitialized = true;
        }

        private static GUIStyle CreateLabelStyle(Font font, Color background)
        {
            return new GUIStyle(GUI.skin.label)
            {
                normal =
                {
                    background = MakeTex(2, 2, background)
                },
                contentOffset = new Vector2(5f, 0f),
                font = font
            };
        }
        
        private static GUIStyle CreateLabelStyle(Font font)
        {
            return new GUIStyle(GUI.skin.label)
            {
                contentOffset = new Vector2(5f, 0f),
                font = font
            };
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; ++i) pix[i] = col;

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}