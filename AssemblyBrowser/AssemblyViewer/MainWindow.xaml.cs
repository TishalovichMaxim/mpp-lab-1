using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using AssemblyViewer.ViewModel;

namespace AssemblyViewer;

public partial class MainWindow : Window
{
    private readonly AssemblyViewerViewModel assemblyViewerViewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = assemblyViewerViewModel;
        //assemblyViewerViewModel.PropertyChanged += OnPropertyChanged;
    }

    //private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    //{
    //    treeView.Items.Clear();

    //    List<TreeViewItem> items = treeViewProcessor.Process(assemblyViewerViewModel.AssemblyData);

    //    foreach (TreeViewItem item in items)
    //    {
    //        treeView.Items.Add(item);
    //    }
    //}
}