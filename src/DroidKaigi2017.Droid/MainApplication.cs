#region

using System;
using Android.App;
using Android.Runtime;
using Autofac;
using DroidKaigi2017.Droid.Mocks;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Interface.MySession;
using DroidKaigi2017.Interface.Room;
using DroidKaigi2017.Interface.Session;
using DroidKaigi2017.Interface.Speaker;
using DroidKaigi2017.Interface.Topic;
using Nyanto;

#endregion

namespace DroidKaigi2017.Droid
{
	[Application]
	public class MainApplication : ApplicationBase
	{
		public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
		}

		protected override void ContainerSetting(ContainerBuilder builder)
		{
			builder.RegisterInstance(Context);


			// Application Singleton
			builder.Register(c => { return new AppSettings(Context); }).As<IAppSettings>().SingleInstance();
			builder.RegisterType<LocaleUtil>().SingleInstance();

			builder.RegisterType<MockRoomService>().As<IRoomService>().SingleInstance();
			builder.RegisterType<MockSessionService>().As<ISessionService>().SingleInstance();
			builder.RegisterType<MockSpeakerService>().As<ISpeakerService>().SingleInstance();
			builder.RegisterType<MockTopicService>().As<ITopicService>().SingleInstance();
			builder.RegisterType<MockMySessionService>().As<IMySessionService>().SingleInstance();

			// Activity LyfeTime

			builder.RegisterType<SettingViewModel>()
				.InstancePerLifetimeScope();

			builder.RegisterType<SessionsViewModel>()
				.InstancePerLifetimeScope();

			builder.RegisterType<SessionDetailViewModel>()
				.InstancePerLifetimeScope();

			builder.Register(c => new DateUtil(Context)).As<IDateUtil>();
			builder.Register(c => new ViewUtil());
		}
	}
}