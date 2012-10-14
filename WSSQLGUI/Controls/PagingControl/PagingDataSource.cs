using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WSSQLGUI.Controls.PagingControl
{
    public partial class PagingDataSource<T> : UserControl, IPaggingDataSource<T> where T : class
    {
        #region fields

        private const int CountItemsInPage = 20;
        private const int MaxLinks = 5;

        private List<T> _listDataSource = new List<T>();
        private List<T> _currentDataSource = new List<T>(); 
        private List<PageEntity> _pages = new List<PageEntity>();
        private int _currentPageNumber = -1;
        //private PageEntity _currentPage;
        private int _pageCount;
        private List<LinkLabel> _listLinkLabel  = new List<LinkLabel>();
        private T _currentSelected;

        #endregion

        #region classes

        class PageEntity
        {
            private int _number = 0;
            private bool _isVisited = false;
            private bool _isVisible = false;

            
            public int Number
            {
                get { return _number; }
                set { _number = value; }
            }

            public string Name
            {
                get { return (_number + 1).ToString(); }
            }

            public bool IsVisited
            {
                get { return _isVisited; }
                set { _isVisited = value; }
            }

            public bool IsVisible
            {
                get { return _isVisible; }
                set { _isVisible = value; }
            }

        }

        #endregion

        #region events

        public event EventHandler SelectedChanged;
        #endregion

        public PagingDataSource()
        {
            InitializeComponent();
            _listLinkLabel.Add(link1);
            _listLinkLabel.Add(link2);
            _listLinkLabel.Add(link3);
            _listLinkLabel.Add(link4);
            _listLinkLabel.Add(link5);
            _listLinkLabel.ForEach(l => l.Visible = false);
            dataGridView.AutoGenerateColumns = false;
            dataGridView.SelectionChanged += (o, e) =>
                                                 {
                                                     if(IsLoading || dataGridView.SelectedRows.Count == 0)
                                                         return;
                                                     var row = dataGridView.SelectedRows[0];
                                                     if(row.Index >= _currentDataSource.Count)
                                                         _currentSelected = null;
                                                     _currentSelected = _currentDataSource[row.Index];
                                                     EventHandler temp = SelectedChanged;
                                                     if(temp != null)
                                                         temp(this,new EventArgs());
                                                 };
        }


        #region IPaggingDataSource

        public bool IsLoading { get; private set; }

        public void StartLoading()
        {
            IsLoading = true;
            //Todo: 
            foreach(var ctrl in Controls.OfType<Control>())
            {
                ctrl.Enabled = false;
            }
        }

        public void CompleteLoading()
        {

            //Todo:
            foreach (var ctrl in Controls.OfType<Control>())
            {
                ctrl.Enabled = true;
            }
            UpdateData();
            IsLoading = false;
            if (_pages.Count > 0)
            {
                tableLinks.Visible = true;
            }
            else
                tableLinks.Visible = false;
        }

        public void AddData(T data)
        {
            if(_listDataSource == null)
                return;
            _listDataSource.Add(data);
        }

        public void AddData(List<T> data)
        {
            if (_listDataSource == null)
                return;
            _listDataSource.Clear();
            _listDataSource = data;
            UpdateData();
        }

        public T Selected
        {
            get { return _currentSelected; }
        }

        public List<T> VisibleItems
        {
            get { return _currentDataSource; }
        }

        public List<T> AllItems
        {
            get { return _listDataSource; }
        }

        public int CurrentPage
        {
            get { return _currentPageNumber; }
        }

        public void MoveLeft()
        {
            MoveToLeft();
        }

        public void MoveRight()
        {
            MoveToRight();
        }

        public void AddColumn(DataGridViewColumn col)
        {
            dataGridView.Columns.Add(col);
        }

        public int ColumnCount
        {
            get { return dataGridView.ColumnCount; }
        }

        public void DeleteColumn(int index)
        {
            if (index >= dataGridView.ColumnCount ||
                index < 0)
                return;
            dataGridView.Columns.RemoveAt(index);
        }

        public void DeleteColumnAll()
        {
            dataGridView.Columns.Clear();
        }

        public void ClearRows()
        {
            dataGridView.DataSource = null;
            dataGridView.Rows.Clear();
        }



        public void MoveToFirst()
        {
            throw new NotImplementedException();
        }

        public void MoveToLast()
        {
            throw new NotImplementedException();
        }


        #endregion

        #region private

        private void SetCurrentPageSource()
        {
            if (_currentPageNumber >= _pageCount)
                return;
            int begin = _currentPageNumber * CountItemsInPage;
            int count = (begin + CountItemsInPage) < _listDataSource.Count ? CountItemsInPage : _listDataSource.Count - begin;
            _currentDataSource.Clear();
            _currentDataSource.AddRange(_listDataSource.GetRange(begin,count));
            dataGridView.DataSource = null;
            BindingSource bs = new BindingSource();
            bs.DataSource = _currentDataSource;
            dataGridView.DataSource = bs.DataSource;
            UpdatedStatics();
        }

        private void UpdateLinks()
        {
            if (_currentPageNumber >= _pageCount)
                return;
            // TODO add updating links
            for (int i = 0; i < MaxLinks; i++)
            {
                if (i >= _pages.Count)
                    break;
                var link = _listLinkLabel[i];
                var page = _pages[i];
                link.Text = page.Name;
                link.LinkVisited = page.IsVisited;
                link.Visible = page.IsVisible;
                link.Tag = page;
            }
        }

        private void MoveToRight()
        {
            if(link5.Tag == null)
                return;
            var curRight = link5.Tag as PageEntity;
            if (curRight.Number == _pages.Count - 1)
                return;
            int start = curRight.Number + 1;
            for (int i = MaxLinks - 1; i >= 0; i--)
            {
                var page = _pages[start];
                var link = _listLinkLabel[i];
                link.Text = page.Name;
                link.LinkVisited = page.IsVisited;
                link.Visible = page.IsVisible;
                link.Tag = page;
                start--;
            }
            UpdatedStatics();
        }

        private void MoveToLeft()
        {
            if(link1.Tag == null)
                return;
            var curLeft = link1.Tag as PageEntity;
            if(curLeft.Number == 0)
                return;
            int start = curLeft.Number - 1;
            for (int i = 0; i < MaxLinks; i++)
            {
                var page = _pages[start];
                var link = _listLinkLabel[i];
                link.Text = page.Name;
                link.LinkVisited = page.IsVisited;
                link.Visible = page.IsVisible;
                link.Tag = page;
                start++;
            }
            UpdatedStatics();
        }

        private void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = sender as LinkLabel;
            if(link.Tag == null)
                return;
            var page = link.Tag as PageEntity;
            page.IsVisited = link.LinkVisited = true;
            _currentPageNumber = page.Number;
            SetCurrentPageSource();
        }


        private void buttonLeft_Click(object sender, EventArgs e)
        {
            MoveToLeft();
        }

        private void buttonRigth_Click(object sender, EventArgs e)
        {
            MoveToRight();
        }

        private void panelBottom_Resize(object sender, EventArgs e)
        {
            var pt = new Point();
            if ((panelBottom.Width - tableStatus.Width) > tableLinks.Width)
            {
                pt.X = (panelBottom.Width - tableLinks.Width) / 2;
                pt.Y = tableLinks.Location.Y;
                tableLinks.Location = pt;
                var rect = new Rectangle(tableStatus.Location,tableStatus.Size);
                var rect1 = new Rectangle(tableLinks.Location,tableLinks.Size);
                if(rect.IntersectsWith(rect1))
                    StuckToStatus();
            }
            else
                StuckToStatus();
        }

        private void StuckToStatus()
        {
            var pt = new Point();
            pt.X = tableStatus.Location.X + tableStatus.Width;
            pt.Y = tableStatus.Location.Y;
            tableLinks.Location = pt;
        }

        private void UpdateData()
        {
            if (_listDataSource == null)
            {
                _pageCount = 0;
                _currentPageNumber = -1;
            }
            else
            {
                _pageCount = _listDataSource.Count / CountItemsInPage;
                _pageCount += _listDataSource.Count % CountItemsInPage > 0 ? 1 : 0;
                _pages.Clear();
                for (int i = 0; i < _pageCount; i++)
                {
                    _pages.Add(new PageEntity()
                    {
                        Number = i,
                        IsVisible = true,
                        IsVisited = false
                    });
                }
                _currentPageNumber = 0;
                SetCurrentPageSource();
                UpdateLinks();
                UpdateLink();
            }
            UpdatedStatics();
        }

        private void UpdateLink()
        {
            var page = _pages.Find(p => p.Number == _currentPageNumber);
            if (page != null)
            {
                page.IsVisited = true;
                _listLinkLabel.ForEach(l => 
                {
                    if (l.Tag == page)
                        l.LinkVisited = page.IsVisited;
                });
            }
        }

        private void UpdatedStatics()
        {
            labelCountText.Text = _pageCount.ToString();
            labelCurrentText.Text = (_currentPageNumber + 1).ToString();
        }

        private void MoveToFirstInternal()
        {
            int start = 0;
            for (int i = 0; i < MaxLinks; i++)
            {
                var page = _pages[start];
                var link = _listLinkLabel[i];
                link.Text = page.Name;
                link.LinkVisited = page.IsVisited;
                link.Visible = page.IsVisible;
                link.Tag = page;
                start++;
            }
            UpdatedStatics();
        }


        private void MoveToLasrInternal()
        {
            int start = _pages[_pages.Count-1].Number;
            int max = _pages.Count > MaxLinks ? MaxLinks : _pages.Count;
            for (int i = MaxLinks - 1; i >= 0; i--)
            {
                var page = _pages[start];
                var link = _listLinkLabel[i];
                link.Text = page.Name;
                link.LinkVisited = page.IsVisited;
                link.Visible = page.IsVisible;
                link.Tag = page;
                start--;
            }
            UpdatedStatics();
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            MoveToFirstInternal();
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            MoveToLasrInternal();
        }

        #endregion

    }
}
