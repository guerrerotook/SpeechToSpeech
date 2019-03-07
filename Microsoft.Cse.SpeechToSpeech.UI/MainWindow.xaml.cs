using Microsoft.Cse.SpeechToSpeech.UI.Model.WavValidation;
using Microsoft.Cse.SpeechToSpeech.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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
                    List<WavError> errors = speechViewModel.CheckWavFileFormat(fileDialog.FileName);

                    if (errors.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var error in errors)
                        {
                            sb.AppendFormat("ErrorType: {0} Message: {1}", error.Format, error.Message);
                            sb.Append(Environment.NewLine);
                        }

                        sb.Append("Currently, only WAV / PCM with 16-bit samples, 16 kHz sample rate, and a single channel (Mono) is supported.");

                        MessageBox.Show(sb.ToString(), "Errors found(s) in the wav file", MessageBoxButton.OK);
                    }
                    else
                    {
                        speechViewModel.WavInputFilename = fileDialog.FileName;
                    }
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

        private void OnAudioOutputTranslationClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Choose the audio file folder";
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.speechViewModel.AudioOutputFolder = dialog.SelectedPath;
                audioOutputPathAudio.Text = dialog.SelectedPath;
            }
        }
    }
}
