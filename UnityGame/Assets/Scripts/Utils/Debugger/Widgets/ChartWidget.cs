using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Utils.Debugger.Widgets
{
    public class ChartWidget : IWidget
    {
        struct TickPosition
        {
            public float Value;
            public float AxisPosition;
        }
        
        public Color SeriesColor = Color.yellow;
        public Color AxisColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Color BackgroundColor = new Color(0, 0, 0.2f, 0f);
        public bool FixedScaleRatio = true;
        private float TickPadding = 0.05f;
        
        private readonly Vector2Int _size;
        private readonly RenderTexture _target;
        private readonly TrackSeries _series;
        private readonly Material _material;
        private bool _isDirty;
        private readonly List<TickPosition> _xTicksPositions = new List<TickPosition>(50);
        private readonly List<TickPosition> _yTicksPositions = new List<TickPosition>(50);
        
        public ChartWidget(uint capacity, Vector2Int size)
        {
            _size = size;
            _target = new RenderTexture(_size.x,_size.y, 
                0, RenderTextureFormat.ARGB32);
            _series = new TrackSeries(capacity, "Default", SeriesColor);
            _material = new Material(Shader.Find("Sprites/Default"));
        }

        public ChartWidget() : this(300, new Vector2Int(512, 256))
        {
        }
        
        public Vector2 GetSize(Style style)
        {
            return _size;
        }

        public void Draw(Rect rect, Style style)
        {
            if (_isDirty)
            {
                Render();
                _isDirty = false;
            }


            var labelSize = style.LabelStyle.CalcSize(new GUIContent("12312312"));
            for (var i = 0; i < _xTicksPositions.Count; i++)
            {
                var tickPosition = _xTicksPositions[i];
                var pos = new Vector2(tickPosition.AxisPosition, 0);
                
                GUI.Label(
                    new Rect(rect.min + pos, labelSize), 
                    tickPosition.Value.ToString(CultureInfo.InvariantCulture), 
                    style.LabelStyle);
            }
            
            for (var i = 0; i < _yTicksPositions.Count; i++)
            {
                var tickPosition = _yTicksPositions[i];
                var pos = new Vector2(0, rect.height - tickPosition.AxisPosition - labelSize.y * 0.5f);
                
                GUI.Label(
                    new Rect(rect.min + pos, labelSize), 
                    tickPosition.Value.ToString(CultureInfo.InvariantCulture), 
                    style.LabelStyle);
            }
            
            GUI.DrawTexture(rect, _target);
        }

        private void Render()
        {
            Graphics.SetRenderTarget(_target);
            GL.Clear(false, true, BackgroundColor);
            _material.SetPass(0);

            var bounds = _series.GetBounds();
            var xMin = bounds.xMin;
            var xMax = bounds.xMax;
            var yMin = bounds.yMin;
            var yMax = bounds.yMax;
            var xRange = xMax - xMin;
            var yRange = yMax - yMin;
            
            if (FixedScaleRatio)
            {
                // Rectangular
                var centerX = (xMin + xMax)*0.5f;
                var centerY = (yMin + yMax)*0.5f;
                var halfRange = Mathf.Max(xRange, yRange)*0.5f;

                xMin = centerX - halfRange;
                xMax = centerX + halfRange;
                yMin = centerY - halfRange;
                yMax = centerY + halfRange;

                yRange = 2 * halfRange;
                xRange = 2 * halfRange;
            }
            
            // Padding
            xMin -= xRange * TickPadding;
            xMax += xRange * TickPadding;
            yMin -= yRange * TickPadding;
            yMax += yRange * TickPadding;
            xRange = xMax - xMin;
            yRange = yMax - yMin;
            
            GL.LoadPixelMatrix(xMin, xMax, yMin, yMax);
            
            
            
            // HORIZONTAL TICKS (Y)
            var ylogRange = Mathf.Log10(yRange);
            var sy1 = Mathf.Floor(ylogRange);
            var sy2 = Mathf.Ceil(ylogRange);

            var ay2 = (sy2 - ylogRange);
            //var ay1 = (ylogRange - sy1);
            

            var dy1 = Mathf.Pow(10f, sy1 - 1); // Small ticks
            var dy2 = Mathf.Pow(10f, sy2 - 1); // Big ticks
            var y1 = (Mathf.Floor(yMin / dy1) + 1) * dy1;
            var y2 = (Mathf.Floor(yMin / dy2) + 1) * dy2;
            
            // VERTICAL TICKS (X)
            var xlogRange = Mathf.Log10(xRange);
            var sx1 = Mathf.Floor(xlogRange);
            var sx2 = Mathf.Ceil(xlogRange);

            var ax2 = (sx2 - xlogRange);
            //var ax1 = (xlogRange - sx1);


            var dx1 = Mathf.Pow(10f, sx1 - 1); // Small ticks
            var dx2 = Mathf.Pow(10f, sx2 - 1); // Big ticks
            var x1 = (Mathf.Floor(xMin / dx1) + 1) * dx1;
            var x2 = (Mathf.Floor(xMin / dx2) + 1) * dx2;
            
            
            // Y BIG TICKS
            GL.Begin(GL.LINES);
            GL.Color(AxisColor);
            _yTicksPositions.Clear();
            while (y2 < yMax)
            {
                GL.Vertex3(xMin, y2, 0);
                GL.Vertex3(xMax, y2, 0);
                
                _yTicksPositions.Add(new TickPosition {
                    AxisPosition = _size.y * (y2 - yMin) / yRange,
                    Value = y2
                });
                y2 += dy2;
            }

            // Y SMALL TICKS
            GL.Color(AxisColor * ay2 * ay2);
            while (y1 <=yMax)
            {
                GL.Vertex3(xMin, y1, 0);
                GL.Vertex3(xMax, y1, 0);
                y1 += dy1;
            }

            // X BIG TICKS
            _xTicksPositions.Clear();
            GL.Color(AxisColor);
            while (x2 < xMax)
            {
                GL.Vertex3(x2, yMin, 0);
                GL.Vertex3(x2, yMax, 0);
                
                _xTicksPositions.Add(new TickPosition {
                    AxisPosition = _size.x * (x2 - xMin) / xRange,
                    Value = x2
                });
                x2 += dx2;
            }

            // X SMALL TICKS
            GL.Color(AxisColor * ax2 * ax2);
            while (x1 < xMax)
            {
                GL.Vertex3(x1, yMin, 0);
                GL.Vertex3(x1, yMax, 0);
                x1 += dx1;
            }
            GL.End();
            
            _series.GlDraw();
            Graphics.SetRenderTarget(null);
        }

        public void Track(float value)
        {
            _isDirty = true;
            _series.AddPoint(_x, value);
            _x += 1f;
        }

        private float _x;
    }
}