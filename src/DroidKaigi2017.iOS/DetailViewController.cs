using System;
using System.Linq;
using System.Reactive.Linq;
using Autofac;
using CoreGraphics;
using DroidKaigi2017.Interface.Services;
using FFImageLoading;
using Foundation;
using Reactive.Bindings;
using UIKit;

namespace DroidKaigi2017.iOS
{
    public partial class DetailViewController : UIViewController
    {
        private readonly ISessionService _sessionService;
        private readonly IMySessionService _mySessionService;

        private readonly ReactiveProperty<int> SessionId;
        private readonly ReadOnlyReactiveProperty<Session> Session;

        public DetailViewController(int sessionId) : base("DetailViewController", null)
        {
            _sessionService = AppDelegate.Container.Resolve<ISessionService>();
            _mySessionService = AppDelegate.Container.Resolve<IMySessionService>();

            this.SessionId = new ReactiveProperty<int>(sessionId);
            this.Session = this.SessionId.Select(x =>
            {
                return _sessionService.Sessions.Value?.FirstOrDefault(y => y.SessionModel.Id == x);
            }).ToReadOnlyReactiveProperty();

        }
        private UILabel title;
        private UILabel date;
        private UILabel Tags;
        private UILabel Category;
        private UIImageView Icon;
        private UILabel User;
        private UILabel Descript;
        private UIButton MySession;
        public UIView TopicColor;
        public int TopicId
        {
            set
            {
                switch (value)
                {
                    case 1:
                        TopicColor.BackgroundColor = UIColor.Green;
                        break;
                    case 2:
                        TopicColor.BackgroundColor = UIColor.Yellow;
                        break;
                    case 3:
                        TopicColor.BackgroundColor = UIColor.Red;
                        break;
                    case 4:
                        TopicColor.BackgroundColor = UIColor.Blue;
                        break;
                    case 5:
                        TopicColor.BackgroundColor = UIColor.Orange;
                        break;
                    case 6:
                        TopicColor.BackgroundColor = UIColor.Purple;
                        break;
                    default:
                        TopicColor.BackgroundColor = UIColor.Clear;
                        break;
                }

            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            var scroll = new UIScrollView();
            scroll.ContentSize = new CGSize(View.Frame.Width, 1500);
            scroll.TranslatesAutoresizingMaskIntoConstraints = false;
            // Perform any additional setup after loading the view, typically from a nib.

            var container = new UIView();
            container.TranslatesAutoresizingMaskIntoConstraints = false;
            scroll.AddSubview(container);
            container.TopAnchor.ConstraintEqualTo(scroll.TopAnchor);
            container.WidthAnchor.ConstraintEqualTo(scroll.WidthAnchor).Active = true;
            container.CenterXAnchor.ConstraintEqualTo(scroll.CenterXAnchor).Active = true;
            container.BottomAnchor.ConstraintEqualTo(scroll.BottomAnchor).Active = true;



            title = new UILabel();
            title.Lines = 0;
            title.TextAlignment = UITextAlignment.Natural;
            title.TranslatesAutoresizingMaskIntoConstraints = false;
            title.TextColor = UIColor.Gray;

            container.AddSubview(title);


            title.CenterXAnchor.ConstraintEqualTo(container.CenterXAnchor).Active = true;
            title.TopAnchor.ConstraintEqualTo(scroll.TopAnchor, 10).Active = true;
            title.WidthAnchor.ConstraintEqualTo(container.WidthAnchor, 0.9f).Active = true;

            date = new UILabel();
            date.TranslatesAutoresizingMaskIntoConstraints = false;
            date.Font = UIFont.SystemFontOfSize(11);
            date.TextColor = UIColor.LightGray;
            container.AddSubview(date);
            date.TopAnchor.ConstraintEqualTo(title.BottomAnchor).Active = true;
            date.WidthAnchor.ConstraintEqualTo(container.WidthAnchor, 0.9f).Active = true;
            date.CenterXAnchor.ConstraintEqualTo(container.CenterXAnchor).Active = true;
            date.HeightAnchor.ConstraintEqualTo(container.HeightAnchor, 0.06f).Active = true;

            Tags = new UILabel();
            Tags.TranslatesAutoresizingMaskIntoConstraints = false;
            Tags.TextColor = UIColor.Gray;
            Tags.Font = UIFont.SystemFontOfSize(11);
            container.AddSubview(Tags);
            Tags.TopAnchor.ConstraintEqualTo(date.BottomAnchor).Active = true;
            Tags.WidthAnchor.ConstraintEqualTo(container.WidthAnchor, 0.9f).Active = true;
            Tags.CenterXAnchor.ConstraintEqualTo(container.CenterXAnchor).Active = true;
            Tags.HeightAnchor.ConstraintEqualTo(container.HeightAnchor, 0.05f).Active = true;

            Category = new UILabel();
            Category.TranslatesAutoresizingMaskIntoConstraints = false;
            Category.TextColor = UIColor.LightGray;
            Category.Lines = 0;
            Category.LineBreakMode = UILineBreakMode.WordWrap;
            Category.Font = UIFont.SystemFontOfSize(10);
            container.AddSubview(Category);
            Category.TopAnchor.ConstraintEqualTo(Tags.BottomAnchor).Active = true;
            Category.WidthAnchor.ConstraintEqualTo(container.WidthAnchor,0.8f).Active = true;
            Category.CenterXAnchor.ConstraintEqualTo(container.CenterXAnchor).Active = true;
            Category.HeightAnchor.ConstraintEqualTo(container.HeightAnchor, 0.05f).Active = true;

            Icon = new UIImageView();
            Icon.ContentMode = UIViewContentMode.ScaleAspectFit;
            Icon.TranslatesAutoresizingMaskIntoConstraints = false;
            container.AddSubview(Icon);

            Icon.TopAnchor.ConstraintEqualTo(Category.BottomAnchor, 10).Active = true;
            Icon.LeftAnchor.ConstraintEqualTo(container.LeftAnchor, 15).Active = true;
            Icon.HeightAnchor.ConstraintEqualTo(container.HeightAnchor, 0.1f).Active = true;

            User = new UILabel();
            User.TranslatesAutoresizingMaskIntoConstraints = false;
            container.AddSubview(User);

            User.LeftAnchor.ConstraintEqualTo(Icon.RightAnchor, 10).Active = true;
            User.TopAnchor.ConstraintEqualTo(Icon.TopAnchor).Active = true;
            User.BottomAnchor.ConstraintEqualTo(Icon.BottomAnchor).Active = true;

            Descript = new UILabel();
            Descript.Lines = 0;
            Descript.AutosizesSubviews = true;
            Descript.TranslatesAutoresizingMaskIntoConstraints = false;
            container.AddSubview(Descript);

            Descript.TopAnchor.ConstraintEqualTo(Icon.BottomAnchor, 10).Active = true;
            Descript.WidthAnchor.ConstraintEqualTo(container.WidthAnchor, 0.9f).Active = true;
            Descript.CenterXAnchor.ConstraintEqualTo(container.CenterXAnchor).Active = true;
            //Descript.HeightAnchor.ConstraintEqualTo(container.HeightAnchor).Active = true;

            MySession = new UIButton();
            MySession.TranslatesAutoresizingMaskIntoConstraints = false;
            scroll.AddSubview(MySession);

            MySession.TopAnchor.ConstraintEqualTo(Descript.BottomAnchor, 50).Active = true;
            MySession.WidthAnchor.ConstraintEqualTo(50).Active = true;
            MySession.HeightAnchor.ConstraintEqualTo(50).Active = true;
            MySession.CenterXAnchor.ConstraintEqualTo(container.CenterXAnchor).Active = true;
            MySession.BackgroundColor = UIColor.Red;

            this.View.AddSubview(scroll);
            scroll.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            scroll.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            scroll.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            scroll.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

            TopicColor = new UIView();
            TopicColor.TranslatesAutoresizingMaskIntoConstraints = false;
            container.AddSubview(TopicColor);
            TopicColor.TopAnchor.ConstraintEqualTo(scroll.TopAnchor).Active = true;
            TopicColor.HeightAnchor.ConstraintEqualTo(3).Active = true;
            TopicColor.WidthAnchor.ConstraintEqualTo(container.WidthAnchor).Active = true;

            this.Session.Subscribe(x=>{
                title.Text = x?.SessionModel.Title;
                date.Text = x?.SessionModel.StartTime.DateTime.ToString() + $" ({(x.SessionModel.EndTime - x.SessionModel.StartTime).TotalMinutes}min)";
                Tags.Text = $"{x?.RoomModel?.Name} {x?.SessionModel.Lang}";
                Category.Text = x?.TopicModel?.Name;
                ImageService.Instance.LoadUrl(x?.SpeakerModel?.ImageUrl).Into(Icon);
                User.Text = x?.SpeakerModel?.Name;
                Descript.Text = x?.SessionModel.Description;
                _mySessionService.MySessions.ToCollectionChanged().Subscribe(z =>
                {
                    var title = _mySessionService.MySessions.Any(y => y == SessionId.Value) ? "-" : "+";
                    MySession.SetTitle(title, UIControlState.Normal);
                });
                TopicId = x?.TopicModel?.Id ?? 0;
            });


            MySession.SetTitle(_mySessionService.MySessions.Any(y => y == SessionId.Value) ? "-" : "+", UIControlState.Normal);


            MySession.TouchUpInside +=  async (object sender, EventArgs e) => {
                if (_mySessionService.MySessions.Any(y => y == SessionId.Value))
                {
                    await _mySessionService.RemoveAsync(SessionId.Value);
                }
                else
                {
                    await _mySessionService.AddAsync(SessionId.Value);
                }
            };;
            scroll.DelaysContentTouches = false;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

