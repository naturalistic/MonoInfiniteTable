using System;
using UIKit;
using Foundation;
using System.Collections.Generic;

namespace MonoInfiniteTable
{
	public partial class InfiniteListVC : UIViewController
	{
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.LightGray;
			// Perform any additional setup after loading the view, typically from a nib.
			var table = new UITableView(new CoreGraphics.CGRect() { 
				X = View.Frame.X + 20, 
				Y = View.Frame.Y + 40, 
				Width = View.Frame.Width-40, 
				Height = View.Frame.Height-60 
			});
			table.Source = new InfiniteTableSource ();
			table.ShowsVerticalScrollIndicator = false;
			this.View.Add (table);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}

	public class InfiniteTableSource : UITableViewSource {

		DateTime[] tableItems;
		string CellIdentifier = "TableCell";
		const int chunkSize = 30;

		public InfiniteTableSource ()
		{
			var items = new List<DateTime> ();
			for (int i = 0; i < chunkSize*2; i++) {
				items.Add (DateTime.Now.AddDays (i));
			}
			tableItems = items.ToArray ();
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);
			DateTime item = tableItems[indexPath.Row];

			//---- if there are no cells to reuse, create a new one
			if (cell == null)
			{ cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier); }

			cell.TextLabel.Text = item.ToString ("D");

			return cell;
		}

		public override void Scrolled (UIScrollView scrollView)
		{
			if (scrollView.ContentOffset.Y >= scrollView.ContentSize.Height - scrollView.Frame.Height) 
			{
				DateTime lastItem = tableItems [chunkSize * 2 - 1];
				for (int i = 0; i < chunkSize; i++) {	// Move items up by chunk size, calculating the new ones from last in list
					tableItems [i] = tableItems [i + chunkSize];
					tableItems [i + chunkSize] = lastItem.AddDays (i + 1);
				}
				var contentOffset = scrollView.ContentOffset;
				contentOffset.Y = (scrollView.ContentSize.Height) / 2 - scrollView.Frame.Height;
				scrollView.SetContentOffset (contentOffset, false);
			} 
			else if (scrollView.ContentOffset.Y < 1) 
			{
				DateTime firstItem = tableItems [0];
				for (int i = 0; i < chunkSize; i++) {	// Move items down by chunk size, calculating the new ones from first in list
					tableItems [i + chunkSize] = tableItems [i];
					tableItems [chunkSize-1-i] = firstItem.AddDays (-i -1);
				}
				var contentOffset = scrollView.ContentOffset;
				contentOffset.Y = (scrollView.ContentSize.Height) / 2;
				scrollView.SetContentOffset (contentOffset, false);	
			}
		}
	}
}

