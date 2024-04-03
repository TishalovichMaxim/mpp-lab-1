using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using AssemblyScannerLib;
using AssemblyViewer.ViewModel.Command;

namespace AssemblyViewer.ViewModel;

public class AssemblyViewerViewModel : INotifyPropertyChanged
{
    public string? AssemblyFileName = null;

    public ICommand ChooseAssemblyCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly AssemblyScanner assemblyScanner = new AssemblyScanner();

    public Dictionary<string, NamespaceInfo> AssemblyData = new();

    public AssemblyViewerViewModel()
    {
        ChooseAssemblyCommand = new ChooseAssemblyCommand(this);
    }

    public void LoadAssemblyData(string assemblyPath)
    {
        AssemblyData = assemblyScanner.Scan(assemblyPath);
        if (PropertyChanged != null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("AssemblyData"));
        }
    }
}
