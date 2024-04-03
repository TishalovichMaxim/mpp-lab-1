using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AssemblyViewer.ViewModel.Command;

internal class ChooseAssemblyCommand : ICommand
{
    private readonly AssemblyViewerViewModel _viewModel;

    public ChooseAssemblyCommand(AssemblyViewerViewModel assemblyViewerViewModel)
    {
        _viewModel = assemblyViewerViewModel;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        Microsoft.Win32.OpenFileDialog dialog = new();

        dialog.Filter = "Assembly files (*.dll, *.exe)| *.dll;*.exe";

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            _viewModel.AssemblyFileName = dialog.FileName;
        }
    }
}
