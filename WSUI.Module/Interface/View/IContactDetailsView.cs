using OF.Module.Interface.ViewModel;

namespace OF.Module.Interface.View
{
    public interface IContactDetailsView
    {
        IContactDetailsViewModel Model { get; set; }

        double ActualHeight { get; }
        double ActualFileHeight { get; }
    }
}