using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Autofac;
using DroidKaigi2017.Interface.Services;
using Foundation;
using Reactive.Bindings;
using UIKit;

namespace DroidKaigi2017.iOS
{
    public partial class SecondViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource
    {
        private readonly IMySessionService _mySessionService;
        private readonly ISessionService _sessionService;

        protected SecondViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            _mySessionService = AppDelegate.Container.Resolve<IMySessionService>();
            _sessionService = AppDelegate.Container.Resolve<ISessionService>();
        }

        private UITableView _tableView;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            _tableView = new UITableView(View.Frame);
            _tableView.Delegate = this;
            _tableView.DataSource = this;
            _tableView.RegisterClassForCellReuse(typeof(MyTableCell), "cell");
            View.AddSubview(_tableView);

            _mySessionService.MySessions.ToCollectionChanged().Subscribe(x =>
            {
                _tableView.ReloadData();
            });
        }

        public override void DidReceiveMemoryWarning()
        {
            
            // Release any cached data, images, etc that aren't in use.
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return _mySessionService.MySessions.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var myTableCell = (MyTableCell)tableView.DequeueReusableCell("cell", indexPath);
            var id = _mySessionService.MySessions[indexPath.Row];
            var session = _sessionService.Sessions.Value.FirstOrDefault(x => x.SessionModel.Id == id);
            if(session == null)
            {
                myTableCell.Title = "";
                myTableCell.Time = "";
            }
            else
            {
                myTableCell.Title = session?.SessionModel.Title;
                myTableCell.Time = $"{session.SessionModel.StartTime.DateTime.ToString()}({(session.SessionModel.EndTime - session.SessionModel.StartTime).TotalMinutes}min)";
            }

            return myTableCell;
        }

        [Export("tableView:didSelectRowAtIndexPath:"), CompilerGenerated]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var id = _mySessionService.MySessions[indexPath.Row];
            var session = _sessionService.Sessions.Value.FirstOrDefault(x => x.SessionModel.Id == id);
            if (session != null)
            {
                var storyboard = this.Storyboard;
                var nextView = new DetailViewController(session.SessionModel.Id);
                var nav = this.NavigationController;
                nav.PushViewController(nextView, true);
            }
        }
    }

    public class MyTableCell : UITableViewCell
    {
        private UILabel _titleLabel = new UILabel();
        public string Title
        {
            get => _titleLabel.Text;
            set => _titleLabel.Text = value;
        }

        private UILabel _timeLabel = new UILabel();
        public string Time
        {
            get => _timeLabel.Text;
            set => _timeLabel.Text = value;
        }

        public MyTableCell(IntPtr handle) : base(handle)
        {
            _titleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            this.AddSubview(_titleLabel);
            _timeLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            this.AddSubview(_timeLabel);

            _titleLabel.TextColor = UIColor.DarkGray;
            _titleLabel.Font = UIFont.SystemFontOfSize(11);
            _timeLabel.TextColor = UIColor.LightGray;
            _timeLabel.Font = UIFont.SystemFontOfSize(9);

            _titleLabel.TopAnchor.ConstraintEqualTo(TopAnchor, 3).Active = true;
            _titleLabel.LeftAnchor.ConstraintEqualTo(LeftAnchor, 3).Active = true;

            _timeLabel.TopAnchor.ConstraintEqualTo(_titleLabel.BottomAnchor).Active = true;
            _timeLabel.LeftAnchor.ConstraintEqualTo(LeftAnchor, 7).Active = true;;
        }
    }
}
