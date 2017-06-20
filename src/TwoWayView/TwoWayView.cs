#region

using System;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using Java.Lang;
using TwoWayView.Core;
using Exception = System.Exception;

#endregion

namespace TwoWayView.Layout
{
	public class TwoWayView : RecyclerView
	{
		public TwoWayView(Context context) : this(context, null)
		{
			;
		}

		public TwoWayView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
			;
		}

		public TwoWayView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			var a =
				context.ObtainStyledAttributes(attrs, Resource.Styleable.twowayview_TwoWayView, defStyle, 0);

			var name = a.GetString(Resource.Styleable.twowayview_TwoWayView_twowayview_layoutManager);
			if (!string.IsNullOrEmpty(name))
				LoadLayoutManagerFromName(context, attrs, name);

			a.Recycle();
		}

		public static string Logtag { get; } = "TwoWayView";

		private void LoadLayoutManagerFromName(Context context, IAttributeSet attrs, string name)
		{
			try
			{
				var dotIndex = name.IndexOf('.');
				if (dotIndex == -1)
				{
					name = "org.lucasr.twowayview.widget." + name;
				}
				else if (dotIndex == 0)
				{
					var packageName = context.PackageName;
					name = packageName + "." + name;
				}

				var type = Type.GetType(name);
				SetLayoutManager((LayoutManager) Activator.CreateInstance(type, context, attrs));
			}
			catch (Exception e)
			{
				throw new IllegalStateException("Could not load TwoWayLayoutManager from " +
				                                "class: " + name);
			}
		}

		public override void SetLayoutManager(LayoutManager layout)
		{
			if (!(layout is TwoWayLayoutManager))
				throw new IllegalArgumentException("TwoWayView can only use TwoWayLayoutManager " +
				                                   "subclasses as its layout manager");

			base.SetLayoutManager(layout);
		}

		public Orientation GetOrientation()
		{
			var layout = (TwoWayLayoutManager) GetLayoutManager();
			return layout.getOrientation();
		}

		public void setOrientation(Orientation orientation)
		{
			var layout = (TwoWayLayoutManager) GetLayoutManager();
			layout.setOrientation(orientation);
		}

		public int getFirstVisiblePosition()
		{
			var layout = (TwoWayLayoutManager) GetLayoutManager();
			return layout.getFirstVisiblePosition();
		}


		public int getLastVisiblePosition()
		{
			;
			var layout = (TwoWayLayoutManager) GetLayoutManager();
			return layout.getLastVisiblePosition();
		}
	}
}