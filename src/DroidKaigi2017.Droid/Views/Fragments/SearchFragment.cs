﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Droid.ViewModels;
using Nyanto;

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class SearchFragment : FragmentBase<SearchViewModel>
	{
		public override int ViewResourceId => Resource.Layout.fragment_search;
		protected override void Bind(View view)
		{
			
		}
	}
}