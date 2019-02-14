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
        private TranslationSpeechViewModel speechViewModel;
        private TextTranslationSpeechViewModel textTranslationViewModel;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnMainWindowsLoaded;
        }

        private void OnMainWindowsLoaded(object sender, RoutedEventArgs e)
        {
            speechViewModel = new TranslationSpeechViewModel();
            speechTab.DataContext = speechViewModel;

            textTranslationViewModel = new TextTranslationSpeechViewModel();
            textTranslationTab.DataContext = textTranslationViewModel;
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
                    speechViewModel.WavInputFilename = fileDialog.FileName;
                }
            }
        }

        private void OnTextTranslationStartClick(object sender, RoutedEventArgs e)
        {
            var task = textTranslationViewModel.Translate();
        }

        private async void OnStartClick(object sender, RoutedEventArgs e)
        {
            Button target = (Button)sender;
            target.IsEnabled = false;
            try
            {
                if (speechViewModel.IsRecognizerRunning)
                {
                    target.Content = "Stopping...";
                    await speechViewModel.StopRecognizer();
                    target.Content = "Start";
                }
                else
                {
                    target.Content = "Starting...";
                    await speechViewModel.StartRecognizer();
                    target.Content = "Stop";
                }
            }
            finally
            {
                target.IsEnabled = true;
            }
        }

        private void OnAudioOutputClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Choose the audio file folder";
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textTranslationViewModel.AudioOutputFolder = dialog.SelectedPath;
                audioOutputPath.Text = dialog.SelectedPath;
            }
        }
    }
}
