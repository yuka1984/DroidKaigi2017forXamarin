#region

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;

#endregion

namespace DroidKaigi2017.Droid.Views.CustomViews
{
	public class ParticlesAnimationView : View
	{
		private const int MAX_HEXAGONS = 40;

		private readonly List<Line> _lines = new List<Line>();

		private readonly Paint _paint = new Paint();

		private readonly List<Particle> _particles = new List<Particle>();

		public ParticlesAnimationView(Context context) : this(context, null)
		{
			;
		}

		public ParticlesAnimationView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
			;
		}

		public ParticlesAnimationView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs,
			defStyleAttr)
		{
			_paint.Color = Color.White;
			SetGradientBackground();
		}

		public override void OnWindowFocusChanged(bool hasWindowFocus)
		{
			base.OnWindowFocusChanged(hasWindowFocus);
			if (hasWindowFocus)
			{
				_particles.Clear();
				_particles.AddRange(createParticles(MAX_HEXAGONS));

				_lines.Clear();
				for (var i = 0; i < _particles.Count - 1; i++)
				{
					var particle = _particles[i];
					// So there are exactly C(particles.size(), 2) (Mathematical Combination) number of lines, which makes more sense.
					for (var j = i + 1; j < _particles.Count; j++)
						_lines.Add(new Line(particle, _particles[j]));
				}
			}
		}

		protected override void OnDetachedFromWindow()
		{
			base.OnDetachedFromWindow();
			_lines.Clear();
			_particles.Clear();
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
			for (int i = 0, size = _particles.Count; i < size; i++)
				_particles[i].Draw(canvas, _paint);

			for (int i = 0, size = _lines.Count; i < size; i++)
				_lines[i].draw(canvas, _paint);
		}

		private void SetGradientBackground()
		{
			var shaderFactory = new MyShaderFactory
			{
				ResizeAction = (width, height) => new LinearGradient(0, height, width, 0,
					new[]
					{
						ContextCompat.GetColor(Context, Resource.Color.dark_light_green),
						(ContextCompat.GetColor(Context, Resource.Color.dark_light_green)
						 + ContextCompat.GetColor(Context, Resource.Color.dark_purple)) / 2,
						ContextCompat.GetColor(Context, Resource.Color.dark_purple),
						(ContextCompat.GetColor(Context, Resource.Color.dark_purple)
						 + ContextCompat.GetColor(Context, Resource.Color.dark_pink)) / 2,
						ContextCompat.GetColor(Context, Resource.Color.dark_pink)
					},
					new[]
					{
						0.0f, 0.15f, 0.5f, 0.85f, 1.0f
					},
					Shader.TileMode.Clamp)
			};
			var backgroundPaint = new PaintDrawable();
			backgroundPaint.Shape = new RectShape();
			backgroundPaint.SetShaderFactory(shaderFactory);

			SetBackgroundDrawable(backgroundPaint);
		}

		private List<Particle> createParticles(int count)
		{
			var particles = new List<Particle>();
			for (var i = 0; i < count; i++)
				particles.Add(new Particle(Width, Height, this));
			return particles;
		}

		public class MyShaderFactory : ShapeDrawable.ShaderFactory
		{
			public Func<int, int, Shader> ResizeAction { get; set; }

			public override Shader Resize(int width, int height)
			{
				return ResizeAction(width, height);
			}
		}

		public class Line : Tuple<Particle, Particle>
		{
			public const int LINK_HEXAGON_DISTANCE = 600;

			private const int MAX_ALPHA = 172;

			/**
		     * Constructor for a Pair.
		     *
		     * @param first  the first object in the Pair
		     * @param Item2 the Item2 object in the pair
		     */
			public Line(Particle first, Particle second) : base(first, second)
			{
			}

			public void draw(Canvas canvas, Paint paint)
			{
				if (!Item1.ShouldBeLinked(LINK_HEXAGON_DISTANCE, Item2))
					return;

				var distance = Math.Sqrt(
					Math.Pow(Item2.center.X - Item1.center.X, 2) + Math.Pow(Item2.center.Y - Item1.center.Y, 2)
				);
				var alpha = MAX_ALPHA - (int) Math.Floor(distance * MAX_ALPHA / LINK_HEXAGON_DISTANCE);
				paint.Alpha = alpha;
				canvas.DrawLine(Item1.center.X, Item1.center.Y, Item2.center.X, Item2.center.X, paint);
			}
		}

		public class Particle
		{
			private const int MAX_ALPHA = 128;
			private const float BASE_RADIUS = 100f;

			private readonly Path path = new Path();
			private int alpha;
			public Point center;
			private float flashSpeed;
			private float moveSpeed;

			private float scale;
			public Point vector;

			public Particle(int maxWidth, int maxHeight, View view) : this(maxWidth, maxHeight, view.Width, view.Height)
			{
			}

			public Particle(int maxWidth, int maxHeight, int hostWidth, int hostHeight)
			{
				var random = new Random();
				center = new Point();
				vector = new Point();
				Reset(maxWidth, maxHeight);
				center.X = (int) (hostWidth - hostWidth * random.NextDouble());
				center.Y = (int) (hostHeight - hostHeight * random.NextDouble());
			}

			public bool ShouldBeLinked(int linkedDistance, Particle particle)
			{
				if (Equals(particle))
					return false;
				// Math.pow(x, 2) and x * x stuff, for your information and my curiosity.
				// ref: http://hg.openjdk.java.net/jdk8u/jdk8u/hotspot/file/5755b2aee8e8/src/share/vm/opto/library_call.cpp#l1799
				var distance = Math.Sqrt(
					Math.Pow(particle.center.X - center.X, 2) + Math.Pow(particle.center.Y - center.Y, 2)
				);
				return distance < linkedDistance;
			}

			public void Draw(Canvas canvas, Paint paint)
			{
				Move(canvas.Width, canvas.Height);
				paint.Alpha = alpha;
				CreateHexagonPathOrUpdate(center.X, center.Y, scale);
				canvas.DrawPath(path, paint);
			}

			private void Move(int maxWidth, int maxHeight)
			{
				alpha += (int) flashSpeed;
				if (alpha < 0)
				{
					alpha = 0;
					flashSpeed = Math.Abs(flashSpeed);
				}
				else if (MAX_ALPHA < alpha)
				{
					alpha = MAX_ALPHA;
					flashSpeed = -Math.Abs(flashSpeed);
				}

				center.X += vector.X * (int) moveSpeed;
				center.Y += vector.Y * (int) moveSpeed;
				var radius = (int) (BASE_RADIUS * scale);
				if (center.X < -radius || maxWidth + radius < center.X || center.Y < -radius || maxHeight + radius < center.Y)
					Reset(maxWidth, maxHeight);
			}
			private static readonly Random random = new Random();

			private void Reset(int maxWidth, int maxHeight)
			{
				
				scale = (float) (random.NextDouble() + random.NextDouble());
				alpha = random.Next(MAX_ALPHA + 1);
				moveSpeed = (float) random.NextDouble() + (float) random.NextDouble() + 0.5f;
				flashSpeed = random.Next(8) + 1f;

				// point on the edge of the screen
				var radius = (int) (BASE_RADIUS * scale);
				if (random.Next() % 2 == 0 % 2)
				{
					center.X = (int) (maxWidth - maxWidth * random.NextDouble());
					center.Y = random.Next() % 2 == 0 ? -radius : maxHeight + radius;
				}
				else
				{
					center.X = random.Next() % 2 == 0 ? -radius : maxWidth + radius;
					center.Y = (int) (maxHeight - maxHeight * random.NextDouble());
				}

				// move direction
				vector.X = (random.Next(5) + 1) * (random.Next() % 2 == 0 ? 1 : -1);
				vector.Y = (random.Next(5) + 1) * (random.Next() % 2 == 0 ? 1 : -1);
				if (center.X == 0)
					vector.X = Math.Abs(vector.X);
				else if (center.X == maxWidth)
					vector.X = -Math.Abs(vector.X);
				if (center.Y == 0)
					vector.Y = Math.Abs(vector.Y);
				else if (center.Y == maxHeight)
					vector.Y = -Math.Abs(vector.Y);
			}

			private void CreateHexagonPathOrUpdate(float centerX, float centerY, float scale)
			{
				path.Reset();
				var radius = (int) (BASE_RADIUS * scale);
				for (var i = 0; i < 6; i++)
				{
					var x = (float) (centerX + radius * Math.Cos(2.0 * i * Math.PI / 6.0 + Math.PI));
					var y = (float) (centerY - radius * Math.Sin(2.0 * i * Math.PI / 6.0 + Math.PI));
					if (i == 0)
						path.MoveTo(x, y);
					else
						path.LineTo(x, y);
				}
				path.Close();
			}
		}
	}
}