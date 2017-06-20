#region

using System;
using System.Reactive.Linq;
using Android.Content;
using Android.Support.V4.Content;
using Android.Util;
using Android.Widget;

#endregion

namespace DroidKaigi2017.Droid.Views.CustomViews
{
	public class SettingSwitchRowView : BindableRelativeLayout
	{
		private new static string Tag = typeof(SettingSwitchRowView).FullName;
		private readonly TextView _descriptionTextView;

		private readonly CompoundButton _switch;
		private readonly TextView _titleTextView;

		private bool f;

		public SettingSwitchRowView(Context context) : this(context, null)
		{
		}

		public SettingSwitchRowView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
		}

		public SettingSwitchRowView(Context context, IAttributeSet attrs, int defStyleAttr)
			: base(context, attrs, defStyleAttr)
		{
			Inflate(context, Resource.Layout.view_setting_switch_row, this);
			if (IsInEditMode)
				return;

			_titleTextView = FindViewById<TextView>(Resource.Id.setting_title);

			_descriptionTextView = FindViewById<TextView>(Resource.Id.setting_description);

			_switch = FindViewById<CompoundButton>(Resource.Id.setting_switch);

			_switch.Checked = false;

			var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.SettingSwitchRow);
			var title = a.GetString(Resource.Styleable.SettingSwitchRow_settingTitle);
			Title = title;
			var description = a.GetString(Resource.Styleable.SettingSwitchRow_settingDescription);
			Description = description;


			Observable
				.FromEventPattern<CompoundButton.CheckedChangeEventArgs>(h => _switch.CheckedChange += h,
					h => _switch.CheckedChange -= h)
				.Select(x => x.EventArgs.IsChecked)
				.Subscribe(x => { OnPropertyChanged(nameof(IsChecked)); });

			a.Recycle();
		}


		public string Title
		{
			get => _titleTextView.Text;
			set => SetProperty(_titleTextView, textview => textview.Text, value);
		}

		public string Description
		{
			get => _descriptionTextView.Text;
			set => SetProperty(_descriptionTextView, textview => textview.Text, value);
		}

		public bool IsChecked
		{
			get => _switch.Checked;
			set => _switch.Checked = value;
		}

		public override bool Enabled
		{
			get => base.Enabled;
			set
			{
				base.Enabled = value;
				if (value)
				{
					_titleTextView.SetTextColor(ContextCompat.GetColorStateList(Context, Resource.Color.black));
					_descriptionTextView.SetTextColor(ContextCompat.GetColorStateList(Context, Resource.Color.grey600));
				}
				else
				{
					var disabledTextColor = ContextCompat.GetColorStateList(Context, Resource.Color.black_alpha_30);
					_titleTextView.SetTextColor(disabledTextColor);
					_descriptionTextView.SetTextColor(disabledTextColor);
				}
			}
		}
	}
}