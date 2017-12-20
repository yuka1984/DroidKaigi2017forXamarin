using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Autofac;
using CoreGraphics;
using DroidKaigi2017.Interface.Services;
using Foundation;
using ObjCRuntime;
using Reactive.Bindings;
using UIKit;

namespace DroidKaigi2017.iOS
{
    public partial class FirstViewController : UIViewController,IUICollectionViewDelegateFlowLayout, IUICollectionViewDataSource
    {
        void HandleAction()
        {

        }

        private readonly ISessionService _sessionService;
        private readonly IMySessionService _mySessionService;

        protected FirstViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            _sessionService = AppDelegate.Container.Resolve<ISessionService>();
            _mySessionService = AppDelegate.Container.Resolve<IMySessionService>();
        }

        private ReadOnlyReactiveProperty<List<Tuple<DateTimeOffset, Session[]>>> sessions;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            var layout = new MyLayout();

            var collectionView = new UICollectionView(this.View.Frame, layout);
            collectionView.BackgroundColor = UIColor.Gray;

            collectionView.Delegate = this;
            collectionView.DataSource = this;
            collectionView.RegisterClassForCell(typeof(MyCell), "cell");
            collectionView.RegisterClassForCell(typeof(EmptyCell), "empty");
            var longtap = new UILongPressGestureRecognizer(async r =>
            {
                var lp = (UILongPressGestureRecognizer)r;
                if (lp.State != UIGestureRecognizerState.Began)
                    return;

                var point = r.LocationInView(collectionView);
                var indexPath = collectionView.IndexPathForItemAtPoint(point);
                var eqTimes = sessions.Value[indexPath.Section];
                var room = _sessionService.Rooms.Value[indexPath.Row];
                var session = default(Session);
                if (eqTimes.Item2.Any(x => x.SessionModel.RoomId == room.Id))
                {
                    session = eqTimes.Item2.First(x => x.SessionModel.RoomId == room.Id);
                }
                else if (eqTimes.Item2.Any(x => x.SessionModel.RoomId == 0) && indexPath.Row == 0)
                {
                    session = eqTimes.Item2.First(x => x.SessionModel.RoomId == 0);
                }
                else
                {
                    return;
                }

                if (_mySessionService.MySessions.Any(y => y == session?.SessionModel?.Id))
                {
                    await _mySessionService.RemoveAsync(session.SessionModel.Id);
                }
                else
                {
                    await _mySessionService.AddAsync(session.SessionModel.Id);
                }
            });
            longtap.DelaysTouchesBegan = true;
            collectionView.AddGestureRecognizer(longtap);

            this.View = (collectionView);

            sessions = _sessionService
                .Sessions
                .Select(x =>
                {
                    var list = new List<Tuple<DateTimeOffset, Session[]>>();
                    if (x == null || x.Count() <= 0)
                        return list;
                
                    DateTimeOffset nowTime = x.First().SessionModel.StartTime;
                    var sessionList = new List<Session>();
                    foreach (var session in x)
                    {
                        if (nowTime != session.SessionModel.StartTime)
                        {
                            list.Add(Tuple.Create(nowTime, sessionList.ToArray()));
                            sessionList.Clear();
                            nowTime = session.SessionModel.StartTime;
                        }
                        sessionList.Add(session);
                    }
                    return list;
                })
                .ToReadOnlyReactiveProperty()
                ;

            sessions.Subscribe(x=>{
                collectionView.ReloadData();
            });

            _mySessionService.MySessions.ToCollectionChanged().Subscribe(x=> collectionView.ReloadData());
        }

        public override void LoadView()
        {
            base.LoadView();
            AppDelegate.Container.Resolve<ISessionService>().LoadAsync();
            AppDelegate.Container.Resolve<IMySessionService>().LoadAsync();
            AppDelegate.Container.Resolve<IFeedBackService>().LoadAsync();

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var eqTimes = sessions.Value[indexPath.Section];
            var room = _sessionService.Rooms.Value[indexPath.Row];
            var session = default(Session);
            if (eqTimes.Item2.Any(x => x.SessionModel.RoomId == room.Id))
            {
                session = eqTimes.Item2.First(x => x.SessionModel.RoomId == room.Id);
            }
            else if(eqTimes.Item2.Any(x => x.SessionModel.RoomId == 0) && indexPath.Row == 0)
            {
                session = eqTimes.Item2.First(x => x.SessionModel.RoomId == 0);
            }
            else
            {
                return;
            }

            var storyboard = this.Storyboard;
            var nextView = new DetailViewController(session.SessionModel.Id);
            var nav = this.NavigationController;
            nav.PushViewController(nextView, true);
        }

        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            var eqTimes = sessions.Value[(int)section];
            if (eqTimes.Item2.Any(x => x.SessionModel.RoomId == 0))
            {
                return 1;
            }

            return _sessionService.Rooms.Value?.Length ?? 0;
        }

        [Export("numberOfSectionsInCollectionView:"), Introduced(PlatformName.iOS, 6, 0, PlatformArchitecture.None, null), CompilerGenerated]
        public nint NumberOfSections(UICollectionView collectionView)
        {
            return sessions.Value.Count;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var eqTimes = sessions.Value[indexPath.Section];
            var room = _sessionService.Rooms.Value[indexPath.Row];


            if (eqTimes.Item2.Any(x => x.SessionModel.RoomId == room.Id))
            {
                var item = (MyCell)collectionView.DequeueReusableCell("cell", indexPath);
                var row = eqTimes.Item2.First(x=> x.SessionModel.RoomId == room.Id);
                item.TimeText = $"{row.SessionModel.StartTime.ToString("hh:MM")}({(row.SessionModel.StartTime - row.SessionModel.EndTime).Minutes}min)";
                item.Title = row.SessionModel.Title;
                item.Name = row.SpeakerModel?.Name;
                item.MySession = _mySessionService.MySessions.Any(x => x == row.SessionModel.Id);
                item.TopicId = row.TopicModel.Id;
                return item;
            }
            else if(eqTimes.Item2.Any(x=> x.SessionModel.RoomId == 0) && indexPath.Row == 0)
            {
                var item = (MyCell)collectionView.DequeueReusableCell("cell", indexPath);
                var row = eqTimes.Item2.First(x => x.SessionModel.RoomId == 0);
                item.TimeText = $"{row.SessionModel.StartTime.ToString("hh:MM")}({(row.SessionModel.StartTime - row.SessionModel.EndTime).Minutes}min)";
                item.Title = row.SessionModel.Title;
                item.Name = row.SpeakerModel?.Name;
                item.MySession = false;
                item.TopicId = 0;
                return item;
            }
            else
            {
                var a = (EmptyCell)collectionView.DequeueReusableCell("empty", indexPath);
                return a;
            }


        }
        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var eqTimes = sessions.Value[indexPath.Section];
            var room = _sessionService.Rooms.Value[indexPath.Row];
            if (eqTimes.Item2.Any(x => x.SessionModel.RoomId == 0) && indexPath.Row == 0)
            {
                //return new CGSize(width: 330, height: 110);
            }


            return new CGSize(width: 110, height: 110);
        }
    }
    public class EmptyCell : UICollectionViewCell
    {
        public EmptyCell(IntPtr handle) : base(handle)
        {
            BackgroundColor = UIColor.Gray;
        }
    }
    public class MyCell : UICollectionViewCell
    {
        private UILabel timeLabel;
        public string TimeText
        {
            get { return timeLabel.Text; }
            set { timeLabel.Text = value; }
        }

        public UILabel titleLabel;
        public string Title
        {
            get => titleLabel.Text;
            set => titleLabel.Text = value;
        }

        public UILabel nameLabel;
        public string Name
        {
            get => nameLabel.Text;
            set => nameLabel.Text = value;
        }

        public UILabel checlLabel;
        public bool MySession
        {
            get => !string.IsNullOrEmpty(checlLabel.Text);
            set => checlLabel.Text = value ? "✅" : "";
        }

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
        
        public MyCell(IntPtr handle) : base(handle)
        {
            var container = new UIView(new CGRect(0, 0, this.Frame.Width-1, this.Frame.Height-1));
            container.BackgroundColor = UIColor.White;
            AddSubview(container);

            timeLabel = new UILabel();
            timeLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            timeLabel.Font = UIFont.SystemFontOfSize(9);
            timeLabel.TextColor = UIColor.Red;

            titleLabel = new UILabel();
            titleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            titleLabel.Font = UIFont.SystemFontOfSize(10);
            titleLabel.Lines = 0;

            nameLabel = new UILabel();
            nameLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            nameLabel.Font = UIFont.SystemFontOfSize(9);
            nameLabel.TextColor = UIColor.LightGray;

            checlLabel = new UILabel();
            checlLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            nameLabel.Font = UIFont.SystemFontOfSize(9);

            TopicColor = new UIView();
            TopicColor.TranslatesAutoresizingMaskIntoConstraints = false;

    
            container.AddSubview(timeLabel);
            container.AddSubview(titleLabel);
            container.AddSubview(nameLabel);
            container.AddSubview(checlLabel);
            container.AddSubview(TopicColor);

            timeLabel.TopAnchor.ConstraintEqualTo(this.TopAnchor, 2).Active = true;
            timeLabel.WidthAnchor.ConstraintEqualTo(this.WidthAnchor).Active = true;
            timeLabel.LeftAnchor.ConstraintEqualTo(this.LeftAnchor).Active = true;;

            titleLabel.TopAnchor.ConstraintEqualTo(timeLabel.BottomAnchor, 4).Active = true;
            titleLabel.WidthAnchor.ConstraintEqualTo(this.WidthAnchor).Active = true;
            titleLabel.HeightAnchor.ConstraintEqualTo(this.HeightAnchor, 0.4f).Active = true;
            titleLabel.LeftAnchor.ConstraintEqualTo(this.LeftAnchor).Active = true; ;

            nameLabel.TopAnchor.ConstraintEqualTo(titleLabel.BottomAnchor, 7).Active = true;
            nameLabel.WidthAnchor.ConstraintEqualTo(this.WidthAnchor).Active = true;
            nameLabel.LeftAnchor.ConstraintEqualTo(this.LeftAnchor).Active = true;

            checlLabel.BottomAnchor.ConstraintEqualTo(this.BottomAnchor, -5).Active = true;
            checlLabel.RightAnchor.ConstraintEqualTo(this.RightAnchor, -5).Active = true;

            TopicColor.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            TopicColor.HeightAnchor.ConstraintEqualTo(1).Active = true;
            TopicColor.WidthAnchor.ConstraintEqualTo(WidthAnchor).Active = true;



            BackgroundColor = UIColor.LightGray;
        }


    }
    public class MyLayout : UICollectionViewFlowLayout
    {
        private Dictionary<NSIndexPath, UICollectionViewLayoutAttributes> layoutInfo = new Dictionary<NSIndexPath, UICollectionViewLayoutAttributes>();
        private nfloat maxRowsWidth = 0;
        private nfloat maxColumnHeight = 0;
        private IUICollectionViewDelegateFlowLayout _delegate;

        private void CalcMaxRowsWidth()
        {
            var collection = this.CollectionView;
            var delegat = this._delegate;
            if (collection == null || delegat == null)
                return;

            nfloat maxRowWidth = 0;
            for (int section = 0; section < collection.NumberOfSections(); section++)
            {
                nfloat maxWidth = 0;
                for (int item = 0; item < CollectionView.NumberOfItemsInSection(section); item++)
                {
                    var indexPath = NSIndexPath.FromItemSection(item, section);
                    var itemSize = delegat.GetSizeForItem(CollectionView, this, indexPath);
                    maxWidth += itemSize.Width;
                }
                maxRowWidth = maxWidth > maxRowWidth ? maxWidth : maxRowWidth;
            }
            this.maxRowsWidth = maxRowWidth;
        }

        private void CalcMaxColumnHeight()
        {
            var collection = this.CollectionView;
            var delegat = this._delegate;
            if (collection == null || delegat == null)
                return;

            nfloat maxHeight = 0;
            for (int section = 0; section < collection.NumberOfSections(); section++)
            {
                nfloat maxRowHeight = 0;
                for (int item = 0; item < CollectionView.NumberOfItemsInSection(section); item++)
                {
                    var indexPath = NSIndexPath.FromItemSection(item, section);
                    var itemSize = delegat.GetSizeForItem(CollectionView, this, indexPath);
                    maxRowHeight = itemSize.Height > maxRowHeight ? itemSize.Height : maxRowHeight;
                }
                maxHeight += maxRowHeight;
            }
            this.maxColumnHeight = maxHeight;
        }

        private void CalcCellLayoutInfo()
        {
            var collection = this.CollectionView;
            var delegat = this._delegate;
            if (collection == null || delegat == null)
                return;

            var cellLayoutInfo = new Dictionary<NSIndexPath, UICollectionViewLayoutAttributes>();
            nfloat originY = 0;
            for (int section = 0; section < collection.NumberOfSections(); section++)
            {
                nfloat height = 0;
                nfloat originX = 0;

                for (int item = 0; item < CollectionView.NumberOfItemsInSection(section); item++)
                {
                    var indexPath = NSIndexPath.FromItemSection(item, section);
                    var itemAttribute = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
                    var itemSize = delegat.GetSizeForItem(CollectionView, this, indexPath);
                    itemAttribute.Frame = new CoreGraphics.CGRect(originX, originY, itemSize.Width, itemSize.Height);
                    cellLayoutInfo.Add(indexPath, itemAttribute);
                    originX += itemSize.Width;
                    height = height > itemSize.Height ? height : itemSize.Height;
                }
                originY += height;
            }

            this.layoutInfo = cellLayoutInfo;
        }



        public override void PrepareLayout()
        {
            base.PrepareLayout();

            this._delegate = this.CollectionView?.Delegate as IUICollectionViewDelegateFlowLayout;


            this.CalcMaxRowsWidth();
            this.CalcMaxColumnHeight();
            this.CalcCellLayoutInfo();

        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CoreGraphics.CGRect rect)
        {
            var allAttributes = new List<UICollectionViewLayoutAttributes>();
            foreach (var attributes in this.layoutInfo.Values)
            {
                if(rect.IntersectsWith(attributes.Frame))
                {
                    allAttributes.Add(attributes);
                }
            }

            return allAttributes.ToArray();
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
        {
            return layoutInfo[indexPath];
        }

        public override CoreGraphics.CGSize CollectionViewContentSize
        {
            get
            {
                return new CoreGraphics.CGSize(maxRowsWidth, maxColumnHeight);
            }
        }


    }

}
