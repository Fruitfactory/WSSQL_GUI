using MVCSharp.Core.CommandManager;

namespace WSSQLGUI.Controllers
{
    internal interface IAllFilesSettingsController
    {
        ICommand TestCommand { get; }
    }
}