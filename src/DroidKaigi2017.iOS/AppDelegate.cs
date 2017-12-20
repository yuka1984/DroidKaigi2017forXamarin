using System;
using System.Threading.Tasks;
using Autofac;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Services;
using DroidKaigi2017.Interface.Tools;
using DroidKaigi2017.iOS.Utils;
using DroidKaigi2017.Service;
using DroidKaigi2017.Services;
using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;

namespace DroidKaigi2017.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public static IContainer Container { get => _container.Value; }

        private static Lazy<IContainer> _container = new Lazy<IContainer>(() => { 
            var builder = new ContainerBuilder();
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            //builder.Register(c => new AppSettings(Context)).As<IAppSettings>().SingleInstance();
            //builder.RegisterType<LocaleUtil>().SingleInstance();
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

            builder.RegisterType<KeyValueStore>().As<IKeyValueStore>();
            return builder.Build();
        });

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            
            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}

