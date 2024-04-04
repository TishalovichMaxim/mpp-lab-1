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
    }
}