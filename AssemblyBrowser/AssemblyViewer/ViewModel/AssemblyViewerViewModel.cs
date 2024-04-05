using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AssemblyScannerLib;
using AssemblyViewer.ViewModel.Command;

namespace AssemblyViewer.ViewModel;

public class AssemblyViewerViewModel : INotifyPropertyChanged
{
    private readonly TreeViewProcessor _treeViewProcessor = new();

    private readonly AssemblyScanner _assemblyScanner = new();

    private string _assemblyFileName;

    public string? AssemblyFileName
    {
        get
        {
            return _assemblyFileName;
        }
        set
        {
            if (value == null)
            {
                return;
            }

            _assemblyFileName = value;

            Dictionary<string, NamespaceInfo> _assemblyData;

            try
            {
                _assemblyData = _assemblyScanner.Scan(_assemblyFileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Chosen file isn't an assembly file...",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                return;
            }

            TreeViewItems = _treeViewProcessor.Process(_assemblyData);
        }
    }

    public ICommand ChooseAssemblyCommand { get; }

    public List<TreeViewItem> _treeViewItems = new();

    public List<TreeViewItem> TreeViewItems
    {
        get
        {
            return _treeViewItems;
        }
        set
        {
            _treeViewItems = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TreeViewItems"));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Dictionary<string, NamespaceInfo> AssemblyData = new();

    public AssemblyViewerViewModel()
    {
        ChooseAssemblyCommand = new ChooseAssemblyCommand(this);
    }
}

