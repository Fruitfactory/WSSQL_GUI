
using System;
using System.Drawing;
using System.Windows.Forms;
using WSSQLGUI.Services;

namespace WSSQLGUI.Controls.TextComplete
{
	/// <summary>
	/// Description of TestSearchComplete.
	/// </summary>
	internal class TextSearchComplete : TextBox
	{
		#region fields
		
		private ListBox _list = new ListBox();
		
		#endregion
		
		#region events
		
		public event EventHandler<EventArgs<string>> TextChanging;
		
		#endregion
		
		
		public TextSearchComplete()
		{
			_list.Visible = false;
		}
		
		#region protected
		
		
		protected override void InitLayout()
		{
			base.InitLayout();
			
			_list.KeyDown += OnKeyListDown;
			_list.MouseClick += OnMouseListClick;
			
			Point pt = new Point(){X = this.Location.X, Y = this.Location.Y + this.Height};
			_list.Location = pt;
			_list.Width = this.Width;
			this.Parent.Controls.Add(_list);
		}


		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
            EventHandler<EventArgs<string>> temp = TextChanging;
            if (temp != null)
                temp(this, new EventArgs<string>(this.Text));

			switch(e.KeyCode)
			{
				case Keys.Down:
					_list.Focus();
					break;
				case Keys.Escape:
					_list.Visible = false;
					Focus();
					break;
				default:
					return;
			}
			
		}
		
		#endregion
	
		#region private
		
		private void ShowSuggest()
		{
            UpdateLocationList();
			_list.Visible = true;
		    _list.SelectedIndex = -1;
		}
		
		private void HideSuggest()
		{
			_list.Visible = false;
		}
		
		private void OnKeyListDown(object sender, KeyEventArgs e)
		{
			
			switch(e.KeyCode)
			{
				case Keys.Escape:
					_list.Visible = false;
					Focus();
					break;
				case Keys.Return:
					if(_list.SelectedIndex > -1)
					{
						Text = _list.Items[_list.SelectedIndex].ToString();
						_list.Visible = false;
						Focus();
					}
					break;
			}
		}
		
		private void OnMouseListClick(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left && 
			   _list.SelectedIndex > -1)
			{
				Text = _list.Items[_list.SelectedIndex].ToString();
				_list.Visible = false;
				Focus();
			}
		}
		
        private void UpdateLocationList()
        {
            Point pt = new Point() { X = this.Location.X, Y = this.Location.Y + this.Height };
            _list.Location = pt;
            _list.Width = this.Width;
            _list.BackColor = this.BackColor;
            _list.BorderStyle = this.BorderStyle;
            _list.Height = _list.Parent.Height - (Location.X + Height);

        }
		#endregion
		
		#region properties
		
		public object DataSource
		{
			get { return _list.DataSource; }
			set
			{
				_list.DataSource = value;
				if(value == null) 	HideSuggest(); else
									ShowSuggest();
			}
		}
		
		#endregion
		
		#region public
		
		
		
		#endregion
		
		
	}
	
}
