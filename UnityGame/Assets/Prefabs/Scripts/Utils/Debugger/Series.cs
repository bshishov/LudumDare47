using UnityEngine;

namespace Utils.Debugger
{
    public interface ISeries
    {
        string Name { get; }
        Color Color { get; }
        Rect GetBounds();
        void GlDraw();
    }

    public class TrackSeries : ISeries
    {
        public Color Color { get; }
        public string Name { get; }
        public readonly Vector2[] Values;
        
        private uint _counter;
        private Rect _bounds;

        public TrackSeries(uint capacity, string name, Color color)
        {
            Values = new Vector2[capacity];
            Color = color;
            Name = name;
        }

        public void AddPoint(float x, float y)
        {
            AddPoint(new Vector2(x, y));
        }
        
        public void AddPoint(Vector2 v)
        {
            Values[_counter % Values.Length] = v;
            _counter++;
            CalcBounds();
        }

        private void CalcBounds()
        {
            var v0 = Values[0];
            float minX = v0.x, minY = v0.y, maxX = v0.x, maxY = v0.y;

            // TODO: MAKE IT O(1)
            for (var i = 1; i < Values.Length && i < _counter; i++)
            {
                var v = Values[i];
                if (v.x > maxX)
                    maxX = v.x;

                if (v.x < minX)
                    minX = v.x;

                if (v.y > maxY)
                    maxY = v.y;

                if (v.y < minY)
                    minY = v.y;
            }

            _bounds.xMax = maxX;
            _bounds.xMin = minX;
            _bounds.yMax = maxY;
            _bounds.yMin = minY;
        }

        public Rect GetBounds()
        {
            return _bounds;
        }

        public void GlDraw()
        {
            GL.Begin(GL.LINES);
            GL.Color(Color);
            for (var i = 1; i < Values.Length && i < _counter; i++)
            {
                GL.Vertex(Values[(_counter - i - 1) % Values.Length]);
                GL.Vertex(Values[(_counter - i) % Values.Length]);
            }
            GL.End();
        }
    }

    public class TrackSeries2D : ISeries
    {
        public string Name { get; set; } = "TrackSeries2D";
        public Color Color { get; set; } = Color.yellow;

        private readonly CyclePool<Vector2> _buffer;
        
        public TrackSeries2D(int maxPoints)
        {
            _buffer = new CyclePool<Vector2>(maxPoints);
        }

        public void AddPoint(Vector2 point)
        {
            _buffer.Add(point);
        }

        public Rect GetBounds()
        {
            return new Rect();
        }

        public void GlDraw()
        {
            GL.Begin(GL.LINES);
            GL.Color(Color);
            for (var i = 0; i < _buffer.Length; i++)
            {
                GL.Vertex(_buffer.GetByIndexStartFromOldest(i - 1));
                GL.Vertex(_buffer.GetByIndexStartFromOldest(i));
            }
            GL.End();
        }
    }
}