#region

using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using Autofac;
using DroidKaigi2017.Droid.Mocks;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Services;
using DroidKaigi2017.Interface.Tools;
using DroidKaigi2017.Service;
using DroidKaigi2017.Services;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Nyanto;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Droid
{
	[Application]
	public class MainApplication : ApplicationBase
	{
		public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
			ReactivePropertyScheduler.SetDefault(TaskPoolScheduler.Default);
			MobileCenter.Start("7415fae6-0dd6-463d-b347-2ce8d24a100f",　typeof(Crashes));
		}

		protected override void ContainerSetting(ContainerBuilder builder)
		{
			builder.RegisterInstance(Context);


			// Application Singleton
			builder.Register(c => new AppSettings(Context)).As<IAppSettings>().SingleInstance();
			builder.RegisterType<LocaleUtil>().SingleInstance();
			try
			{
				var client = new MobileServiceClient("https://droidxamarin.azurewebsites.net/");
				builder.RegisterInstance(client);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			
			builder.RegisterType<AzureEasyTableSessionRepository>().As<ISessionRepository>().SingleInstance();
			builder.RegisterType<AzureEasyTableRoomRepository>().As<IRoomRepository>().SingleInstance();
			builder.RegisterType<AzureEasyTableTopicRepository>().As<ITopicRepository>().SingleInstance();
			builder.RegisterType<AzureEasyTableSpeakerRepository>().As<ISpeakerRepository>().SingleInstance();
			builder.RegisterType<MySessionRepository>().As<IMySessionRepository>().SingleInstance();
			builder.RegisterType<AzureEasyTableFeedbackRepository>().As<IFeedbackRepository>().SingleInstance();

			builder.RegisterType<SessionService>().As<ISessionService>().SingleInstance();
			builder.RegisterType<MySessionService>().As<IMySessionService>().SingleInstance();
			builder.RegisterType<FeedBackService>().As<IFeedBackService>().SingleInstance();

			// Activity LyfeTime

			builder.RegisterType<SettingViewModel>()
				.InstancePerLifetimeScope();

			builder.RegisterType<SessionsViewModel>()
				.InstancePerLifetimeScope();

			builder.RegisterType<SessionDetailViewModel>()
				.InstancePerLifetimeScope();

			builder.RegisterType<SessionFeedbackViewModel>()
				.InstancePerLifetimeScope();

			builder.RegisterType<SearchViewModel>()
				.InstancePerLifetimeScope();

			builder.RegisterType<MySessionsViewModel>().InstancePerDependency();

			builder.Register(c => new DateUtil(Context)).As<IDateUtil>();
			builder.Register(c => new ViewUtil());
			builder.RegisterType<KeyValueStore>().As<IKeyValueStore>();
		}
	}
}