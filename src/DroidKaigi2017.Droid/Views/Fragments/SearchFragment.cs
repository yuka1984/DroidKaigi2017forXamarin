using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Droid.Views.CustomViews;
using FFImageLoading;
using FFImageLoading.Work;
using Nyanto;
using Reactive.Bindings.Extensions;
using TwoWayView.Core;
using TwoWayView.Layout;
using SearchView = Android.Support.V7.Widget.SearchView;

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class SearchFragment : FragmentBase<SearchViewModel>
	{
		private const string ELLIPSIZE_TEXT = "...";

		private const int ELLIPSIZE_LIMIT_COUNT = 30;

		private SearchResultsAdapter adapter;
		public override int ViewResourceId => Resource.Layout.fragment_search;
		protected override void Bind(View view)
		{
			var holder = new fragment_search_holder(view);
			HasOptionsMenu = true;

			adapter = new SearchResultsAdapter(Context, new List<SearchResultViewModel>());
			var layoutManager = new LinearLayoutManager(Context);
			holder.recycler_view.SetAdapter(adapter);			
			holder.recycler_view.AddItemDecoration(new Android.Support.V7.Widget.DividerItemDecoration(Context, layoutManager.Orientation));
			holder.recycler_view.SetLayoutManager(layoutManager);

			ViewModel.SearchSessions
				.ObserveOnUIDispatcher()
				.Subscribe(x =>
				{
					adapter.Reset(x);
				})
				.AddTo(CompositeDisposable);
		}

		public class SearchResultsAdapter : ArrayRecyclerAdapter<SearchResultViewModel>
		{
			public SearchResultsAdapter(Context context) : base(context)
			{
			}

			public SearchResultsAdapter(Context context, List<SearchResultViewModel> list) : base(context, list)
			{
			}

			public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
			{
				var accessor = new view_search_result_holder(holder.ItemView);
				var vm = Get(position);

				accessor.txt_type.Text = vm.SessionTitle;

				var context = accessor.txt_type.Context;
				var icon = ContextCompat.GetDrawable(context, SearchResultViewModel.GetTypeResourceid(vm.ResultType));
				Drawable checkMark = ContextCompat.GetDrawable(context, Resource.Drawable.ic_check_circle_24_vector);
				int size = context.Resources.GetDimensionPixelSize(Resource.Dimension.text_drawable_12dp);
				checkMark.SetBounds(0, 0, size, size);
				icon.SetBounds(0, 0, size, size);
				accessor.txt_type.SetCompoundDrawables(icon, null, vm.IsMySession ? checkMark : null, null);

				if (string.IsNullOrEmpty(vm.SpeakerImageUrl))
				{
					accessor.img_speaker.SetImageDrawable(
						ContextCompat.GetDrawable(Context, Resource.Drawable.ic_speaker_placeholder));
				}
				else
				{
					size = (int) Math.Round(accessor.img_speaker.Resources.GetDimension(Resource.Dimension.icon_36dp));
					ImageService.Instance.LoadUrl(vm.SpeakerImageUrl, TimeSpan.FromDays(1))
						.Retry(3, 200)
						.DownSample(size, size)
						.LoadingPlaceholder(nameof(Resource.Drawable.ic_speaker_placeholder), ImageSource.CompiledResource)
						.ErrorPlaceholder(nameof(Resource.Drawable.ic_speaker_placeholder), ImageSource.CompiledResource)
						.Into(accessor.img_speaker);
				}

				accessor.root.SetOnClickListener(new AnonymousOnClickListner
				{
					OnClickAction = view =>
					{
						vm.GoDetailCommand.CheckExecute(null);
					}
				});

				accessor.txt_search_result.TextFormatted = GetSpannableStringBuilder(vm);
			}

			public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
			{
				var view = LayoutInflater.From(Context).Inflate(Resource.Layout.view_search_result, parent, false);
				return new CompositDiposableViewHolder(view);
			}

			private SpannableStringBuilder GetSpannableStringBuilder(SearchResultViewModel viewModel)
			{
				var builder = new SpannableStringBuilder();

				if (string.IsNullOrEmpty(viewModel.Text))
				{
					return builder;
				}

				var text = viewModel.Text.Replace("\n", "  ");
				var searchText = viewModel.SearchWord;

				if (string.IsNullOrEmpty(searchText))
				{
					builder.Append(text);
					return builder;
				}
				else
				{
					int idx = text.ToLower().IndexOf(searchText.ToLower(), StringComparison.CurrentCultureIgnoreCase);
					if (idx >= 0)
					{
						builder.Append(text);
						builder.SetSpan(
							new TextAppearanceSpan(Context, Resource.Style.SearchResultAppearance),
							idx,
							idx + searchText.Length,
							SpanTypes.ExclusiveExclusive);


						if (idx > ELLIPSIZE_LIMIT_COUNT && viewModel.IsEllipsis)
						{
							builder.Delete(0, idx - ELLIPSIZE_LIMIT_COUNT);
							builder.Insert(0, ELLIPSIZE_TEXT);
						}

						return builder;
					}
					else
					{
						builder.Append(text);
						return builder;
					}
				}
			}
		}

		public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
		{
			base.OnCreateOptionsMenu(menu, inflater);
			inflater.Inflate(Resource.Menu.menu_search, menu);

			var menuItem = menu.FindItem(Resource.Id.action_search);

			MenuItemCompat.SetOnActionExpandListener(menuItem, new OnActionExpandListenerd
			{
				OnMenuItemActionExpandAction = item => true,
				OnMenuItemActionCollapseAction = item =>
				{
					Activity.OnBackPressed();
					return false;
				}
			});

			var searchView = (SearchView) menuItem.ActionView;
			searchView.QueryHint = GetString(Resource.String.search_hint);
			searchView.SetOnQueryTextListener(new OnQueryTextListener
			{
				OnQueryTextSubmitAction = s =>
				{
					ViewModel.SearchCommand.CheckExecute(s);
					return true;
				},
				OnQueryTextChangeAction = s =>
				{
					ViewModel.SearchCommand.CheckExecute(s);
					return true;
				}
			});
		}

		public override void OnPrepareOptionsMenu(IMenu menu)
		{
			base.OnPrepareOptionsMenu(menu);
			MenuItemCompat.ExpandActionView(menu.FindItem(Resource.Id.action_search));
		}

		private class OnQueryTextListener : Java.Lang.Object, SearchView.IOnQueryTextListener
		{
			public Func<string, bool> OnQueryTextChangeAction { get; set; }
			public bool OnQueryTextChange(string newText)
			{
				return OnQueryTextChangeAction?.Invoke(newText) ?? false;
			}

			public Func<string, bool> OnQueryTextSubmitAction { get; set; }
			public bool OnQueryTextSubmit(string query)
			{
				return OnQueryTextSubmitAction?.Invoke(query) ?? false;
			}
		}

		private class OnActionExpandListenerd : Java.Lang.Object, MenuItemCompat.IOnActionExpandListener
		{
			public Func<IMenuItem, bool> OnMenuItemActionCollapseAction { get; set; }
			public bool OnMenuItemActionCollapse(IMenuItem item)
			{
				return OnMenuItemActionCollapseAction?.Invoke(item) ?? false;
			}

			public Func<IMenuItem, bool> OnMenuItemActionExpandAction { get; set; }
			public bool OnMenuItemActionExpand(IMenuItem item)
			{
				return OnMenuItemActionExpandAction?.Invoke(item) ?? false;
			}
		}
	}
}

