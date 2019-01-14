using Microsoft.Cse.SpeechToSpeech.UI.Speech;
using Microsoft.Cse.SpeechToSpeech.UI.Storage;
using Microsoft.Cse.SpeechToSpeech.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.Cse.SpeechToSpeech.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnMainWindowsLoaded;
        }

        private void OnMainWindowsLoaded(object sender, RoutedEventArgs e)
        {
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;
        }

        private void OnInputButtonClick(object sender, RoutedEventArgs e)
        {
            EnableButtons();

            
        }

        private void EnableButtons()
        {
            Dispatcher.Invoke(() =>
            {
                startButton.IsEnabled = true;
                radioGroup.IsEnabled = true;
                optionPanel.IsEnabled = true;
            });
        }

        private void OnSelectFileButtonClick(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FileDialog fileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileNameTextBox.Text = fileDialog.FileName;
                }
            }
        }

        private void OnSaveKeysClick(object sender, RoutedEventArgs e)
        {
            
        }

        private async void OnStartClick(object sender, RoutedEventArgs e)
        {
            Button target = (Button)sender;
            if (viewModel.IsRecoznizingRunning)
            {
                target.Content = "Start";
                await viewModel.StopRecognizer();
            }
            else
            {
                target.Content = "Stop";
                await viewModel.StartRecognizer();
            }
        }
    }
}
