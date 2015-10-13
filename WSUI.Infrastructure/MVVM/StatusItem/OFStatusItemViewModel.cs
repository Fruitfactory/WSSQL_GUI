using OF.Core.Core.MVVM;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Enums;

namespace OF.Infrastructure.MVVM.StatusItem
{
    public class OFStatusItemViewModel : OFDataViewModel
    {
        public OFStatusItemViewModel(OFStatusItem item )
        :base(item)
        {
            UpdateValue(item);
        }

        public string Name
        {
            get { return Get(() => Name); }
            set { Set(() => Name, value); }
        }

        public int Count
        {
            get { return Get(() => Count); }
            set { Set(() => Count, value); }
        }

        public int Processing
        {
            get { return Get(() => Processing); }
            set { Set(() => Processing, value); }
        }

        public PstReaderStatus Status
        {
            get { return Get(() => Status); }
            set { Set(() => Status, value); }
        }

        public double Value
        {
            get { return Get(() => Value); }
            set { Set(() => Value, value); }
        }

        public string Folder
        {
            get { return Get(() => Folder); }
            set { Set(() => Folder, value); }
        }

        public override void Update(object item)
        {
            var data = item as OFStatusItem;
            if (data == null)
                return;

            Name = data.Name;
            Count = data.Count;
            Processing = data.Processing;
            Status = data.Status;
            Folder = data.Folder;
            UpdateValue(data);
        }

        private void UpdateValue(OFStatusItem item)
        {
            if (item.Count == 0)
            {
                Value = 100;
                return;
            }
            Value = (((double)item.Processing)/((double)item.Count))*100;
        }
    }

}