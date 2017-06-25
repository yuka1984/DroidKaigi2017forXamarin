#region

using System;
using DroidKaigi2017.Interface.Models;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public class TopicColor
	{
		private static readonly Lazy<TopicColor[]> _listLazy = new Lazy<TopicColor[]>(() => new[]
		{
			None, ProductivityAndTooling, ArchitectureAndDevelopmentProcessMethodology, Hardware, UiAndDesign,
			QualityAndSustainability, Platform, Other
		});

		private TopicColor(int paleColorResId, int middleColorResId, int vividColorResId, int themeId)
		{
			PaleColorResId = paleColorResId;
			MiddleColorResId = middleColorResId;
			VividColorResId = vividColorResId;
			ThemeId = themeId;
		}

		public static TopicColor[] Values => _listLazy.Value;


		public static TopicColor None => new TopicColor(Resource.Color.purple_alpha_15, Resource.Color.purple_alpha_50,
			Resource.Color.purple, Resource.Style.AppTheme_NoActionBar_Purple);

		public static TopicColor ProductivityAndTooling => new TopicColor(Resource.Color.light_green_alpha_15,
			Resource.Color.light_green_alpha_50,
			Resource.Color.light_green, Resource.Style.AppTheme_NoActionBar_LightGreen);

		public static TopicColor ArchitectureAndDevelopmentProcessMethodology => new TopicColor(
			Resource.Color.yellow_alpha_15, Resource.Color.yellow_alpha_50,
			Resource.Color.yellow, Resource.Style.AppTheme_NoActionBar_Yellow);

		public static TopicColor Hardware => new TopicColor(Resource.Color.red_alpha_15, Resource.Color.red_alpha_50,
			Resource.Color.red, Resource.Style.AppTheme_NoActionBar_Red);

		public static TopicColor UiAndDesign => new TopicColor(Resource.Color.blue_alpha_15, Resource.Color.blue_alpha_50,
			Resource.Color.blue, Resource.Style.AppTheme_NoActionBar_Blue);

		public static TopicColor QualityAndSustainability => new TopicColor(Resource.Color.light_blue_alpha_15,
			Resource.Color.light_blue_alpha_50,
			Resource.Color.light_blue, Resource.Style.AppTheme_NoActionBar_LightBlue);

		public static TopicColor Platform => new TopicColor(Resource.Color.pink_alpha_15, Resource.Color.pink_alpha_50,
			Resource.Color.pink, Resource.Style.AppTheme_NoActionBar_Pink);

		public static TopicColor Other => new TopicColor(Resource.Color.purple_alpha_15, Resource.Color.purple_alpha_50,
			Resource.Color.purple, Resource.Style.AppTheme_NoActionBar_Purple);


		public int PaleColorResId { get; }

		public int MiddleColorResId { get; }

		public int VividColorResId { get; }

		public int ThemeId { get; }

		public static TopicColor GettopiColor(TopicModel topicModel)
		{
			if (topicModel == null)
				return None;
			if (topicModel.Id < 0)
				return None;
			if (topicModel.Id >= Values.Length)
				return None;
			return Values[topicModel.Id];
		}
	}
}