using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.Phone.Controls;
using Spico.Recorder;
using PocketSphinxRntComp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Navigation;
using System.Threading;
using System.ComponentModel;
using System.Windows.Media;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;

namespace Spico
{
    /// <summary>
    /// PocketSphinx implementation for Windows Phone
    /// pure code; no MVVM and all in 1 code behind file
    /// 
    /// Orginially created by Toine de Boer, Enbyin (NL)
    /// Implementation for color blind test with speech recognition created by Hamdan Prakoso, ID
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {

      

 
        #region Fields

        private SpeechRecognizer speechRecognizer;

        private WasapiAudioRecorder audioRecorder;

        


        public static int numberOfFailed;

        public static bool isSpeechActived;

        public class Answers
        {
            public static int countRightAnswer { get; set; }
            public static int absolutelyNormal { get; set; }
            public static int possibleProtan { get; set; }
            public static int possibleDeutan { get; set; }
            public List<string> answerPlate { get; set; }

        }

        public class countFinalAnswer
        {
            public int numberRightAnswer { get; set; }
            public int numberPossibleNormal { get; set; }
            public int numberPossibleProtan { get; set; }
            public int numberPossibleDeutan { get; set; }
        }

        #endregion

        #region Constructor & Loaded event

        public MainPage()
        {

            InitializeComponent();
            mainPivot.IsLocked = true;
            BackKeyPress += OnBackKeyPress;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("SpeechConsent"))
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                Boolean speechConsent = (Boolean)settings["SpeechConsent"];
                if (speechConsent == true)
                {
                    MainPage.isSpeechActived = true;
                    resultText.IsHitTestVisible = false;
                    BitmapImage bm = new BitmapImage(new Uri(@"Assets/Microphone-64.png", UriKind.RelativeOrAbsolute));
                    micType.Source = bm;
                }
                else
                {
                    isSpeechActived = false;
                    BitmapImage bm = new BitmapImage(new Uri(@"Assets/Sent-96.png", UriKind.RelativeOrAbsolute));
                    micType.Source = bm;
                }
            } else
            {
                MainPage.isSpeechActived = true;

                resultText.IsHitTestVisible = false;
                BitmapImage bm = new BitmapImage(new Uri(@"Assets/Microphone-64.png", UriKind.RelativeOrAbsolute));
                micType.Source = bm;
            }

            AudioContainer.SphinxSpeechRecognizer = new SpeechRecognizer();
            speechRecognizer = AudioContainer.SphinxSpeechRecognizer;

            speechRecognizer.resultFound += speechRecognizer_resultFound;

            speechRecognizer.resultFinalizedBySilence += speechRecognizer_resultFinalizedBySilence;




        }


        private void OnBackKeyPress(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            var mm = MessageBox.Show("Apakah Anda yakin akan keluar dari aplikasi ini ?", "Keluar", MessageBoxButton.OKCancel);
            if (mm == MessageBoxResult.OK)
            {
                Application.Current.Terminate();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                var index = NavigationContext.QueryString["item"];
                var indexParsed = int.Parse(index);
                mainPivot.SelectedIndex = indexParsed;
            }
        }
        #endregion


        #region SpeechRecognizer & Audio

        public void FilterResult(string recognizedSpeech)
        {
            if (mainPivot.SelectedIndex == 0)
            {
                if (recognizedSpeech.Contains("one two") || recognizedSpeech.Contains("dua belas") || recognizedSpeech.Contains("twelve")) {
                    resultText.Text = "12";
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            } else if (mainPivot.SelectedIndex == 1)
            {
                if (recognizedSpeech.Contains("eight") || recognizedSpeech.Contains("delapan") || recognizedSpeech.Contains("three") || recognizedSpeech.Contains("tiga"))
                {
                    if (recognizedSpeech.Contains("eight") || recognizedSpeech.Contains("delapan"))
                    {
                        resultText.Text = "8";

                    }
                    else
                    {
                        resultText.Text = "3";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            } else if (mainPivot.SelectedIndex == 2)
            {
                if (recognizedSpeech.Contains("six") || recognizedSpeech.Contains("enam") || recognizedSpeech.Contains("five") || recognizedSpeech.Contains("lima"))
                {
                    if (recognizedSpeech.Contains("six") || recognizedSpeech.Contains("enam"))
                    {
                        resultText.Text = "6";

                    }
                    else
                    {
                        resultText.Text = "5";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            } else if (mainPivot.SelectedIndex == 3)
            {
                if (recognizedSpeech.Contains("two nine") || recognizedSpeech.Contains("dua puluh sembilan") || recognizedSpeech.Contains("seven zero") || recognizedSpeech.Contains("tujuh puluh"))
                {
                    if (recognizedSpeech.Contains("two nine") || recognizedSpeech.Contains("dua puluh sembilan"))
                    {
                        resultText.Text = "29";

                    }
                    else
                    {
                        resultText.Text = "70";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 4)
            {
                if (recognizedSpeech.Contains("five seven") || recognizedSpeech.Contains("lima puluh tujuh") || recognizedSpeech.Contains("three five") || recognizedSpeech.Contains("tiga puluh lima"))
                {
                    if (recognizedSpeech.Contains("five seven") || recognizedSpeech.Contains("lima puluh tujuh"))
                    {
                        resultText.Text = "57";

                    }
                    else
                    {
                        resultText.Text = "35";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 5)
            {
                if (recognizedSpeech.Contains("five") || recognizedSpeech.Contains("lima") || recognizedSpeech.Contains("two") || recognizedSpeech.Contains("dua"))
                {
                    if (recognizedSpeech.Contains("five") || recognizedSpeech.Contains("lima"))
                    {
                        resultText.Text = "5";

                    }
                    else
                    {
                        resultText.Text = "2";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 6)
            {
                if (recognizedSpeech.Contains("three") || recognizedSpeech.Contains("tiga") || recognizedSpeech.Contains("five") || recognizedSpeech.Contains("lima"))
                {
                    if (recognizedSpeech.Contains("three") || recognizedSpeech.Contains("tiga"))
                    {
                        resultText.Text = "3";

                    }
                    else
                    {
                        resultText.Text = "5";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 7)
            {
                if (recognizedSpeech.Contains("one five") || recognizedSpeech.Contains("lima belas") || recognizedSpeech.Contains("one seven") || recognizedSpeech.Contains("tujuh belas"))
                {
                    if (recognizedSpeech.Contains("one five") || recognizedSpeech.Contains("lima belas"))
                    {
                        resultText.Text = "15";

                    }
                    else
                    {
                        resultText.Text = "17";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 8)
            {
                if (recognizedSpeech.Contains("seven four") || recognizedSpeech.Contains("tujuh puluh empat") || recognizedSpeech.Contains("two one") || recognizedSpeech.Contains("dua puluh satu"))
                {
                    if (recognizedSpeech.Contains("seven four") || recognizedSpeech.Contains("tujuh puluh empat"))
                    {
                        resultText.Text = "74";

                    }
                    else
                    {
                        resultText.Text = "21";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 9)
            {
                if (recognizedSpeech.Contains("two") || recognizedSpeech.Contains("dua") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("two") || recognizedSpeech.Contains("dua"))
                    {
                        resultText.Text = "2";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 10)
            {
                if (recognizedSpeech.Contains("six") || recognizedSpeech.Contains("enam") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("six") || recognizedSpeech.Contains("enam"))
                    {
                        resultText.Text = "6";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 11)
            {
                if (recognizedSpeech.Contains("nine seven") || recognizedSpeech.Contains("sembilan puluh tujuh") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("nine seven") || recognizedSpeech.Contains("sembilan puluh tujuh"))
                    {
                        resultText.Text = "97";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 12)
            {
                if (recognizedSpeech.Contains("four five") || recognizedSpeech.Contains("empat puluh lima") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("four five") || recognizedSpeech.Contains("empat puluh lima"))
                    {
                        resultText.Text = "45";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 13)
            {
                if (recognizedSpeech.Contains("five") || recognizedSpeech.Contains("lima") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("five") || recognizedSpeech.Contains("lima"))
                    {
                        resultText.Text = "5";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 14)
            {
                if (recognizedSpeech.Contains("seven") || recognizedSpeech.Contains("tujuh") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("seven") || recognizedSpeech.Contains("tujuh"))
                    {
                        resultText.Text = "7";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 15)
            {
                if (recognizedSpeech.Contains("one six") || recognizedSpeech.Contains("enam belas") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("one six") || recognizedSpeech.Contains("enam belas"))
                    {
                        resultText.Text = "16";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 16)
            {
                if (recognizedSpeech.Contains("seven three") || recognizedSpeech.Contains("tujuh puluh tiga") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("seven three") || recognizedSpeech.Contains("tujuh puluh tiga"))
                    {
                        resultText.Text = "73";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 17)
            {
                if (recognizedSpeech.Contains("five") || recognizedSpeech.Contains("lima") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("five") || recognizedSpeech.Contains("lima"))
                    {
                        resultText.Text = "5";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 18)
            {
                if (recognizedSpeech.Contains("two") || recognizedSpeech.Contains("dua") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("two") || recognizedSpeech.Contains("dua"))
                    {
                        resultText.Text = "2";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 19)
            {
                if (recognizedSpeech.Contains("four five") || recognizedSpeech.Contains("empat puluh lima") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("four five") || recognizedSpeech.Contains("empat puluh lima"))
                    {
                        resultText.Text = "45";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 20)
            {
                if (recognizedSpeech.Contains("seven three") || recognizedSpeech.Contains("tujuh puluh tiga") || recognizedSpeech.Contains("zero") || recognizedSpeech.Contains("tidak ada"))
                {
                    if (recognizedSpeech.Contains("seven three") || recognizedSpeech.Contains("tujuh puluh tiga"))
                    {
                        resultText.Text = "73";

                    }
                    else
                    {
                        resultText.Text = "tidak ada";

                    }
                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 21)
            {
                if (recognizedSpeech.Contains("two six") || recognizedSpeech.Contains("dua puluh enam") || recognizedSpeech.Contains("two") || recognizedSpeech.Contains("dua") || recognizedSpeech.Contains("six") || recognizedSpeech.Contains("enam"))
                {
                    if (recognizedSpeech.Contains("two six") || recognizedSpeech.Contains("dua puluh enam"))
                    {
                        resultText.Text = "26";

                    } else if ((recognizedSpeech.Contains("two") && !recognizedSpeech.Contains("six")) || (recognizedSpeech.Contains("dua") && !recognizedSpeech.Contains("enam")))
                    {
                        resultText.Text = "2";
                    } else if ((recognizedSpeech.Contains("six") && !recognizedSpeech.Contains("two")) || (recognizedSpeech.Contains("enam") && !recognizedSpeech.Contains("dua")))
                    {
                        resultText.Text = "6";
                    }

                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 22)
            {
                if (recognizedSpeech.Contains("four two") || recognizedSpeech.Contains("empat puluh dua") || recognizedSpeech.Contains("four") || recognizedSpeech.Contains("empat") || recognizedSpeech.Contains("two") || recognizedSpeech.Contains("dua"))
                {
                    if (recognizedSpeech.Contains("four two") || recognizedSpeech.Contains("empat puluh dua"))
                    {
                        resultText.Text = "42";

                    }
                    else if ((recognizedSpeech.Contains("two") && !recognizedSpeech.Contains("four")) || (recognizedSpeech.Contains("dua") && !recognizedSpeech.Contains("empat")))
                    {
                        resultText.Text = "2";
                    }
                    else if ((recognizedSpeech.Contains("four") && !recognizedSpeech.Contains("two")) || (recognizedSpeech.Contains("empat") && !recognizedSpeech.Contains("dua")))
                    {
                        resultText.Text = "4";
                    }

                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 23)
            {
                if (recognizedSpeech.Contains("three five") || recognizedSpeech.Contains("tiga puluh lima") || recognizedSpeech.Contains("three") || recognizedSpeech.Contains("tiga") || recognizedSpeech.Contains("five") || recognizedSpeech.Contains("lima"))
                {
                    if (recognizedSpeech.Contains("three five") || recognizedSpeech.Contains("tiga puluh lima"))
                    {
                        resultText.Text = "35";

                    }
                    else if ((recognizedSpeech.Contains("three") && !recognizedSpeech.Contains("five")) || (recognizedSpeech.Contains("tiga") && !recognizedSpeech.Contains("lima")))
                    {
                        resultText.Text = "3";
                    }
                    else if ((recognizedSpeech.Contains("five") && !recognizedSpeech.Contains("three")) || (recognizedSpeech.Contains("lima") && !recognizedSpeech.Contains("tiga")))
                    {
                        resultText.Text = "5";
                    }

                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }
            else if (mainPivot.SelectedIndex == 24)
            {
                if (recognizedSpeech.Contains("nine six") || recognizedSpeech.Contains("sembilan puluh enam") || recognizedSpeech.Contains("nine") || recognizedSpeech.Contains("sembilan") || recognizedSpeech.Contains("six") || recognizedSpeech.Contains("enam"))
                {
                    if (recognizedSpeech.Contains("nine six") || recognizedSpeech.Contains("sembilan puluh enam"))
                    {
                        resultText.Text = "96";

                    }
                    else if ((recognizedSpeech.Contains("nine") && !recognizedSpeech.Contains("six")) || (recognizedSpeech.Contains("sembilan") && !recognizedSpeech.Contains("enam")))
                    {
                        resultText.Text = "9";
                    }
                    else if ((recognizedSpeech.Contains("six") && !recognizedSpeech.Contains("nine")) || (recognizedSpeech.Contains("enam") && !recognizedSpeech.Contains("sembilan")))
                    {
                        resultText.Text = "6";
                    }

                }
                else
                {
                    resultText.Text = "Gagal mengenal angka";
                }
            }

        }
        
        #region FoundText
        private void FoundText()
        {
            speechRecognizer.StopProcessing();
            StopNativeRecorder();
            //speechRecognizer.CleanPocketSphinx();


            usingSpeech();


            //MainMessageBlock.Text = recognizedText;
        }

        #endregion

        #region SpeechRecognizer Methods (PocketSphinx) - Continuous

        private void StartSpeechRecognizerProcessing()
        {

            string result = string.Empty;

            try
            {
                if (speechRecognizer.IsProcessing())
                {
                    result = "PocketSphinx already started";
                }
                else
                {
                    result = speechRecognizer.StartProcessing();
                }


            }
            catch
            {
                result = "Starting PocketSphinx processing failed";
            }

            Debug.WriteLine(result);
        }

        private void StopSpeechRecognizerProcessing()
        {
            string result = string.Empty;

            try
            {
                result = speechRecognizer.StopProcessing();


            }
            catch
            {
                result = "Stopping PocketSphinx processing failed";
            }

            Debug.WriteLine(result);
        }

        void speechRecognizer_resultFinalizedBySilence(string finalResult)
        {
            Debug.WriteLine("final result found: {0}", finalResult);
            FilterResult(finalResult);
            FoundText();
        }

        void speechRecognizer_resultFound(string result)
        {
            Debug.WriteLine("result found: {0}", result);

        }

        #endregion


        #region Recording Methods

        private void InitializeAudioRecorder()
        {
            AudioContainer.AudioRecorder = new WasapiAudioRecorder();
            audioRecorder = AudioContainer.AudioRecorder;
            audioRecorder.AudioReported += audioRecorder_BufferReady;

            Debug.WriteLine("AudioRecorder Initialized");
        }

        private void StartNativeRecorder()
        {
            audioRecorder.StartRecording();
            micType.IsHitTestVisible = false;

            if (mainPivot.SelectedIndex == 0)
            {
                plate1.Visibility = Visibility.Collapsed;
                speech1.Visibility = Visibility.Visible;
                textSpeech1.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 1)
            {
                plate2.Visibility = Visibility.Collapsed;
                speech2.Visibility = Visibility.Visible;
                textSpeech2.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 2)
            {
                plate3.Visibility = Visibility.Collapsed;
                speech3.Visibility = Visibility.Visible;
                textSpeech3.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 3)
            {
                plate4.Visibility = Visibility.Collapsed;
                speech4.Visibility = Visibility.Visible;
                textSpeech4.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 4)
            {
                plate5.Visibility = Visibility.Collapsed;
                speech5.Visibility = Visibility.Visible;
                textSpeech5.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 5)
            {
                plate6.Visibility = Visibility.Collapsed;
                speech6.Visibility = Visibility.Visible;
                textSpeech6.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 6)
            {
                plate7.Visibility = Visibility.Collapsed;
                speech7.Visibility = Visibility.Visible;
                textSpeech7.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 7)
            {
                plate8.Visibility = Visibility.Collapsed;
                speech8.Visibility = Visibility.Visible;
                textSpeech8.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 8)
            {
                plate9.Visibility = Visibility.Collapsed;
                speech9.Visibility = Visibility.Visible;
                textSpeech9.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 9)
            {
                plate10.Visibility = Visibility.Collapsed;
                speech10.Visibility = Visibility.Visible;
                textSpeech10.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 10)
            {
                plate11.Visibility = Visibility.Collapsed;
                speech11.Visibility = Visibility.Visible;
                textSpeech11.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 11)
            {
                plate12.Visibility = Visibility.Collapsed;
                speech12.Visibility = Visibility.Visible;
                textSpeech12.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 12)
            {
                plate13.Visibility = Visibility.Collapsed;
                speech13.Visibility = Visibility.Visible;
                textSpeech13.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 13)
            {
                plate14.Visibility = Visibility.Collapsed;
                speech14.Visibility = Visibility.Visible;
                textSpeech14.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 14)
            {
                plate15.Visibility = Visibility.Collapsed;
                speech15.Visibility = Visibility.Visible;
                textSpeech15.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 15)
            {
                plate16.Visibility = Visibility.Collapsed;
                speech16.Visibility = Visibility.Visible;
                textSpeech16.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 16)
            {
                plate17.Visibility = Visibility.Collapsed;
                speech17.Visibility = Visibility.Visible;
                textSpeech17.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 17)
            {
                plate18.Visibility = Visibility.Collapsed;
                speech18.Visibility = Visibility.Visible;
                textSpeech18.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 18)
            {
                plate19.Visibility = Visibility.Collapsed;
                speech19.Visibility = Visibility.Visible;
                textSpeech19.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 19)
            {
                plate20.Visibility = Visibility.Collapsed;
                speech20.Visibility = Visibility.Visible;
                textSpeech20.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 20)
            {
                plate21.Visibility = Visibility.Collapsed;
                speech21.Visibility = Visibility.Visible;
                textSpeech21.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 21)
            {
                plate22.Visibility = Visibility.Collapsed;
                speech22.Visibility = Visibility.Visible;
                textSpeech22.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 22)
            {
                plate23.Visibility = Visibility.Collapsed;
                speech23.Visibility = Visibility.Visible;
                textSpeech23.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 23)
            {
                plate24.Visibility = Visibility.Collapsed;
                speech24.Visibility = Visibility.Visible;
                textSpeech24.Visibility = Visibility.Visible;
            }
            else if (mainPivot.SelectedIndex == 24)
            {
                plate25.Visibility = Visibility.Collapsed;
                speech25.Visibility = Visibility.Visible;
                textSpeech25.Visibility = Visibility.Visible;
            }

            playSound_SpeechOn();
        }

        private void StopNativeRecorder()
        {
            audioRecorder.StopRecording();
            if (mainPivot.SelectedIndex == 0)
            {
                plate1.Visibility = Visibility.Visible;
                speech1.Visibility = Visibility.Collapsed;
                textSpeech1.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 1)
            {
                plate2.Visibility = Visibility.Visible;
                speech2.Visibility = Visibility.Collapsed;
                textSpeech2.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 2)
            {
                plate3.Visibility = Visibility.Visible;
                speech3.Visibility = Visibility.Collapsed;
                textSpeech3.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 3)
            {
                plate4.Visibility = Visibility.Visible;
                speech4.Visibility = Visibility.Collapsed;
                textSpeech4.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 4)
            {
                plate4.Visibility = Visibility.Visible;
                speech4.Visibility = Visibility.Collapsed;
                textSpeech4.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 5)
            {
                plate6.Visibility = Visibility.Visible;
                speech6.Visibility = Visibility.Collapsed;
                textSpeech6.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 6)
            {
                plate7.Visibility = Visibility.Visible;
                speech7.Visibility = Visibility.Collapsed;
                textSpeech7.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 7)
            {
                plate8.Visibility = Visibility.Visible;
                speech8.Visibility = Visibility.Collapsed;
                textSpeech8.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 8)
            {
                plate9.Visibility = Visibility.Visible;
                speech9.Visibility = Visibility.Collapsed;
                textSpeech9.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 9)
            {
                plate10.Visibility = Visibility.Visible;
                speech10.Visibility = Visibility.Collapsed;
                textSpeech10.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 10)
            {
                plate11.Visibility = Visibility.Visible;
                speech11.Visibility = Visibility.Collapsed;
                textSpeech11.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 11)
            {
                plate12.Visibility = Visibility.Visible;
                speech12.Visibility = Visibility.Collapsed;
                textSpeech12.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 12)
            {
                plate13.Visibility = Visibility.Visible;
                speech13.Visibility = Visibility.Collapsed;
                textSpeech13.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 13)
            {
                plate14.Visibility = Visibility.Visible;
                speech14.Visibility = Visibility.Collapsed;
                textSpeech14.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 14)
            {
                plate15.Visibility = Visibility.Visible;
                speech15.Visibility = Visibility.Collapsed;
                textSpeech15.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 15)
            {
                plate16.Visibility = Visibility.Visible;
                speech16.Visibility = Visibility.Collapsed;
                textSpeech16.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 16)
            {
                plate17.Visibility = Visibility.Visible;
                speech17.Visibility = Visibility.Collapsed;
                textSpeech17.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 17)
            {
                plate18.Visibility = Visibility.Visible;
                speech18.Visibility = Visibility.Collapsed;
                textSpeech18.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 18)
            {
                plate19.Visibility = Visibility.Visible;
                speech19.Visibility = Visibility.Collapsed;
                textSpeech19.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 19)
            {
                plate20.Visibility = Visibility.Visible;
                speech20.Visibility = Visibility.Collapsed;
                textSpeech20.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 20)
            {
                plate21.Visibility = Visibility.Visible;
                speech21.Visibility = Visibility.Collapsed;
                textSpeech21.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 21)
            {
                plate22.Visibility = Visibility.Visible;
                speech22.Visibility = Visibility.Collapsed;
                textSpeech22.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 22)
            {
                plate23.Visibility = Visibility.Visible;
                speech23.Visibility = Visibility.Collapsed;
                textSpeech23.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 23)
            {
                plate24.Visibility = Visibility.Visible;
                speech24.Visibility = Visibility.Collapsed;
                textSpeech24.Visibility = Visibility.Collapsed;
            }
            else if (mainPivot.SelectedIndex == 24)
            {
                plate25.Visibility = Visibility.Visible;
                speech25.Visibility = Visibility.Collapsed;
                textSpeech25.Visibility = Visibility.Collapsed;
            }
            playSound_SpeechOff();
            micType.IsHitTestVisible = true;

        }

        void audioRecorder_BufferReady(object sender, AudioDataEventArgs e)
        {
            int registerResult = 0;
            try
            {
                registerResult = speechRecognizer.RegisterAudioBytes(e.Data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                StopNativeRecorder();
                StopSpeechRecognizerProcessing();
                Debug.WriteLine("all stoped because of error");
                //StateMessageBlock.Text = "all stoped because of error";
            }

            // incoming raw sound
            //Debug.WriteLine("{0} bytes of raw audio recieved, {1} frames processed at PocketSphinx", e.Data.Length, registerResult);
        }
        #endregion
        #endregion





        #region No using speech
        private void notUsingSpeech()
        {
           

            if (mainPivot.SelectedIndex == 0)
            {
                
                if (resultText.Text == "12")
                {
                    playSound_RightAnswer();
                    Answers.countRightAnswer++;
                    MainPage.numberOfFailed = 0;
                    
                    

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=1", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=0", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 1)
            {

                
                if (resultText.Text == "8" || resultText.Text == "3")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "8")
                    {
                        Answers.countRightAnswer++;
                        

                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=2", UriKind.RelativeOrAbsolute));


                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=1", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 2)
            {
               
                if (resultText.Text == "6" ||  resultText.Text == "5")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "enam" || resultText.Text == "6")
                    {
                        Answers.countRightAnswer++;
                        


                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=3", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=2", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 3)
            {
                
                if (resultText.Text == "29" || resultText.Text == "70")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "29")
                    {
                        Answers.countRightAnswer++;
                        

                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=4", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=3", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 4)
            {
               
                if (resultText.Text == "57" || resultText.Text == "35")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "57")
                    {
                        Answers.countRightAnswer++;
                        

                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=5", UriKind.RelativeOrAbsolute));


                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=4", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 5)
            {
                
                if (resultText.Text == "5" || resultText.Text == "2")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "5")
                    {
                        Answers.countRightAnswer++;
                        

                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=6", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=5", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 6)
            {
                
                if (resultText.Text == "3" || resultText.Text == "5")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "3")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=7", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=6", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 7)
            {
               
                if (resultText.Text == "15" || resultText.Text == "17")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "15")
                    {
                        Answers.countRightAnswer++;
                        

                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=8", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=7", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 8)
            {
               
                if (resultText.Text == "74" || resultText.Text == "21")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "74")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=9", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=8", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 9)
            {
                
                if (resultText.Text == "2" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "2")
                    {
                        Answers.countRightAnswer++;
                        

                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=10", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=9", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 10)
            {
               
                if (resultText.Text == "6" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "6")
                    {
                        Answers.countRightAnswer++;
                        

                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=11", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=10", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 11)
            {
               
                if (resultText.Text == "97" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "97")
                    {
                        Answers.countRightAnswer++;
                        

                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=12", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=11", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 12)
            {
                
                if (resultText.Text == "45" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "45")
                    {
                        Answers.countRightAnswer++;
                        

                    }else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=13", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=12", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 13)
            {
              
                if (resultText.Text == "5" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer(); 
                    if (resultText.Text == "5")
                    {
                        Answers.countRightAnswer++;
                        

                    } else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=14", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=13", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 14)
            {
               
                if (resultText.Text == "7" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "7")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=15", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=14", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 15)
            {
                
                if (resultText.Text == "16" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "16")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=16", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=15", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 16)
            {
               
                if (resultText.Text == "73" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "73")
                    {
                        Answers.countRightAnswer++;
                        


                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=17", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=16", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 17)
            {

                if (resultText.Text == "" || resultText.Text == "5")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=18", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=17", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 18)
            {
                
                if (resultText.Text == "" || resultText.Text == "2")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "")
                    {
                        Answers.countRightAnswer++;
                        


                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=19", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=18", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 19)
            {
              
                if (resultText.Text == "" || resultText.Text == "45")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "")
                    {
                        Answers.countRightAnswer++;
                        


                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=20", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=19", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 20)
            {

                if (resultText.Text == "" || resultText.Text == "73")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=21", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=20", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 21)
            {
               
                if (resultText.Text == "26" || resultText.Text == "6" || resultText.Text == "2")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "26")
                    {
                        Answers.absolutelyNormal++;
                        

                    }
                    else if (resultText.Text == "6")
                    {
                        Answers.possibleProtan++;


                    }
                    else if (resultText.Text == "2")
                    {
                        Answers.possibleDeutan++;


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=22", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=21", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 22)
            {
               
                if (resultText.Text == "42" || resultText.Text == "2" || resultText.Text == "4")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "42")
                    {
                        Answers.absolutelyNormal++;
                        

                    }
                    else if (resultText.Text == "2")
                    {
                        Answers.possibleProtan++;


                    }
                    else if (resultText.Text == "4")
                    {
                        Answers.possibleDeutan++;


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=23", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=22", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 23)
            {
                
                if (resultText.Text == "35" || resultText.Text == "5" || resultText.Text == "3")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "35")
                    {
                        Answers.absolutelyNormal++;
                        

                    }
                    else if (resultText.Text == "5")
                    {
                        Answers.possibleProtan++;


                    }
                    else if (resultText.Text == "3")
                    {
                        Answers.possibleDeutan++;


                    }
                    MainPage.numberOfFailed = 0;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=24", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=23", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 24)
            {
                
                if (resultText.Text == "96" || resultText.Text == "6" || resultText.Text == "9")
                {
                    playSound_RightAnswer();

                    countFinalAnswer finalAnswer = new countFinalAnswer();

                    if (resultText.Text == "96")
                    {
                        Answers.absolutelyNormal++;
                        

                    }
                    else if (resultText.Text == "6")
                    {
                        Answers.possibleProtan++;


                    }
                    else if (resultText.Text == "9")
                    {
                        Answers.possibleDeutan++;


                    }
                    MainPage.numberOfFailed = 0;


                    finalAnswer.numberRightAnswer = Answers.countRightAnswer;
                    finalAnswer.numberPossibleNormal = Answers.absolutelyNormal;
                    finalAnswer.numberPossibleProtan = Answers.possibleProtan;
                    finalAnswer.numberPossibleDeutan = Answers.possibleDeutan;
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=25", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=24", UriKind.RelativeOrAbsolute));

                }
            }
        }
        #endregion

        private void fallBackScenario()
        {
            isSpeechActived = false;
            BitmapImage bm = new BitmapImage(new Uri(@"Assets/Sent-96.png", UriKind.RelativeOrAbsolute));
            micType.Source = bm;
            resultText.Text = "";
            resultText.Focus();

            resultText.IsHitTestVisible = true;
        }

        #region using speech
        private void usingSpeech()
        {

            if (mainPivot.SelectedIndex == 0)
            {
                
                if (resultText.Text == "12")
                {
                    Answers.countRightAnswer++;
                    MainPage.numberOfFailed = 0;
                    isSpeechActived = true;
                    playSound_RightAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=1", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if(numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 1)
            {
                if (resultText.Text == "8" || resultText.Text == "3")
                {
                    if (resultText.Text == "8")
                    {
                        Answers.countRightAnswer++;
                        isSpeechActived = true;



                    }
                    else
                    {


                    }
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=2", UriKind.RelativeOrAbsolute));


                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)
                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 2)
            {
               if (resultText.Text == "6" || resultText.Text == "5")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "6")
                    {
                        Answers.countRightAnswer++;

                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=3", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 3)
            {
                if (resultText.Text == "29" || resultText.Text == "70")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "29")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=4", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 4)
            {
                if (resultText.Text == "57" || resultText.Text == "35")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "57")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=5", UriKind.RelativeOrAbsolute));


                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 5)
            {
                if (resultText.Text == "5" || resultText.Text == "2")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "5")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=6", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 6)
            {
                if (resultText.Text == "3" || resultText.Text == "5")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "3")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=7", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 7)
            {
                if (resultText.Text == "15" || resultText.Text == "17")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "15")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=8", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 8)
            {
                if (resultText.Text == "74" || resultText.Text == "21")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "74")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=9", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 9)
            {
                if (resultText.Text == "2" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "2")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=10", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 10)
            {
                if (resultText.Text == "6" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "6")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=11", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 11)
            {
                if (resultText.Text == "97" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "97")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=12", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 12)
            {
                if (resultText.Text == "45" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "45")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=13", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 13)
            {
                if (resultText.Text == "5" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "5")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=14", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 14)
            {
                 if (resultText.Text == "7" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "7")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=15", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 15)
            {
                 if (resultText.Text == "16" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "16")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=16", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 16)
            {
                if (resultText.Text == "73" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "73")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=17", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 17)
            {
                if (resultText.Text == "tidak ada" || resultText.Text == "5")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=18", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 18)
            {
                if (resultText.Text == "tidak ada" || resultText.Text == "2")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;
                        

                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=19", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 19)
            {
               if (resultText.Text == "tidak ada" || resultText.Text == "45")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;
                        


                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=20", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 20)
            {
                if (resultText.Text == "tidak ada" || resultText.Text == "73")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;
                        


                    }
                    else
                    {


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=21", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 21)
            {
                if (resultText.Text == "26" || resultText.Text == "6" || resultText.Text == "2")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "26")
                    {
                        Answers.absolutelyNormal++;
                        

                    }
                    else if (resultText.Text == "6")
                    {
                        Answers.possibleProtan++;


                    }
                    else if (resultText.Text == "2")
                    {
                        Answers.possibleDeutan++;


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=22", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 22)
            {
                if (resultText.Text == "42" || resultText.Text == "2" || resultText.Text == "4")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "empat puluh dua")
                    {
                        Answers.absolutelyNormal++;
                        

                    }
                    else if (resultText.Text == "dua")
                    {
                        Answers.possibleProtan++;


                    }
                    else if (resultText.Text == "empat")
                    {
                        Answers.possibleDeutan++;


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=23", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 23)
            {
              if (resultText.Text == "35" || resultText.Text == "5" || resultText.Text == "3")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    if (resultText.Text == "tiga puluh lima")
                    {
                        Answers.absolutelyNormal++;
                        

                    }
                    else if (resultText.Text == "lima")
                    {
                        Answers.possibleProtan++;


                    }
                    else if (resultText.Text == "tiga")
                    {
                        Answers.possibleDeutan++;


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=24", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    playSound_WrongAnswer();

                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
            else if (mainPivot.SelectedIndex == 24)
            {
                if (resultText.Text == "96" || resultText.Text == "6" || resultText.Text == "9")
                {
                    isSpeechActived = true;
                    MainPage.numberOfFailed = 0;
                    playSound_RightAnswer();

                    countFinalAnswer finalAnswer = new countFinalAnswer();

                    if (resultText.Text == "sembilan puluh enam")
                    {
                        Answers.absolutelyNormal++;
                        

                    }
                    else if (resultText.Text == "enam")
                    {
                        Answers.possibleProtan++;


                    }
                    else if (resultText.Text == "sembilan")
                    {
                        Answers.possibleDeutan++;


                    }

                    finalAnswer.numberRightAnswer = Answers.countRightAnswer;
                    finalAnswer.numberPossibleNormal = Answers.absolutelyNormal;
                    finalAnswer.numberPossibleProtan = Answers.possibleProtan;
                    finalAnswer.numberPossibleDeutan = Answers.possibleDeutan;
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=25", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    stateMessage.Text = "Coba lagi";
                    MainPage.numberOfFailed++;
                    if (numberOfFailed == 3)

                    {
                        fallBackScenario();
                    }
                }
            }
        }
        #endregion

        #region SoundEffect
        public void playSound_SpeechOn()
        {
            Stream stream = TitleContainer.OpenStream("Assets/Sound/beep_short_on.wav");
            SoundEffect effect = SoundEffect.FromStream(stream);
            FrameworkDispatcher.Update();
            effect.Play();
        }

        public void playSound_SpeechOff()
        {
            Stream stream = TitleContainer.OpenStream("Assets/Sound/beep_short_off.wav");
            SoundEffect effect = SoundEffect.FromStream(stream);
            FrameworkDispatcher.Update();
            effect.Play();
        }

        public void playSound_RightAnswer()
        {
            Stream stream = TitleContainer.OpenStream("Assets/Sound/music_marimba_chord.wav");
            SoundEffect effect = SoundEffect.FromStream(stream);
            FrameworkDispatcher.Update();
            effect.Play();
        }

        public void playSound_WrongAnswer()
        {
            Stream stream = TitleContainer.OpenStream("Assets/Sound/Error-sound-effect.wav");
            SoundEffect effect = SoundEffect.FromStream(stream);
            FrameworkDispatcher.Update();
            effect.Play();
        }
        #endregion
       

       

      
        private void mic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(isSpeechActived== true)
            {
                try
                {
                    InitializeAudioRecorder();

                    // Start processes
                    StartSpeechRecognizerProcessing();
                    StartNativeRecorder();

                }
                catch (Exception ex)
                {
                    stateMessage.Text = ex.Message;

                }
            } else
            {
                notUsingSpeech();
            }
        }
           

        public System.Windows.Media.Color ConvertStringToColor(String hex)
        {
            //remove the # at the front
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }

        private void retest_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=0", UriKind.RelativeOrAbsolute));
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=26", UriKind.RelativeOrAbsolute));

        }

        private void goToNextPage(object sender, RoutedEventArgs e, int selectedIndex)
        {
            mainPivot.SelectedIndex = selectedIndex;

        }

        private void mainPivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(mainPivot.SelectedIndex == 0)
            {
                Answers.countRightAnswer = 0;
                Answers.absolutelyNormal = 0;
                Answers.possibleDeutan = 0;
                Answers.possibleProtan = 0;
            }
            if (mainPivot.SelectedIndex == 25 || mainPivot.SelectedIndex == 26)
            {
                speechRow.Visibility = Visibility.Collapsed;

            }

            if(mainPivot.SelectedIndex == 25)
            {
                if (Answers.countRightAnswer >= 17)
                {
                    resultGreet.Text = "SELAMAT";
                    var successColor = ConvertStringToColor("#6CA8D1");
                    SolidColorBrush success = new SolidColorBrush(successColor);
                    resultGreet.Foreground = success;
                    resultExplanation.Text = "Hasil tes menunjukkan penglihatan Anda masih dalam keadaan NORMAL.";
                    resultAdvice.Text = "Tetap jaga kesehatan mata Anda dengan mengonsumsi sumber vitamin A seperti wortel dan buah-buahan lainnya";

                }
                else if (Answers.countRightAnswer <= 13 && Answers.possibleProtan > 2)
                {
                    resultGreet.Text = "MAAF";
                    var failColor = ConvertStringToColor("#FD5711");
                    SolidColorBrush fail = new SolidColorBrush(failColor);
                    resultGreet.Foreground = fail;
                    resultExplanation.Text = "Hasil tes menunjukkan penglihatan Anda mengalami gangguan PROTANOPIA. Penglihatan Anda kurang sensitif terhadap warna merah.";
                    resultAdvice.Text = "Segera periksakan mata Anda ke dokter dan mintalah terapi atau pengobatan yang sesuai untuk mengurangi dampak.";

                }
                else if (Answers.countRightAnswer <= 13 && Answers.possibleDeutan > 2)
                {
                    resultGreet.Text = "MAAF";
                    var failColor = ConvertStringToColor("#FD5711");
                    SolidColorBrush fail = new SolidColorBrush(failColor);
                    resultGreet.Foreground = fail;
                    resultExplanation.Text = "Hasil tes menunjukkan penglihatan Anda mengalami gangguan DEUTERANOPIA. Penglihatan Anda kurang sensitif terhadap warna hijau.";
                    resultAdvice.Text = "Segera periksakan mata Anda ke dokter dan mintalah terapi atau pengobatan yang sesuai untuk mengurangi dampak.";

                }
                else if (Answers.countRightAnswer <= 13 && (Answers.possibleProtan <= 2 && Answers.absolutelyNormal >= 2))
                {
                    resultGreet.Text = "MAAF";
                    var failColor = ConvertStringToColor("#FD5711");
                    SolidColorBrush fail = new SolidColorBrush(failColor);
                    resultGreet.Foreground = fail;
                    resultExplanation.Text = "Hasil tes menunjukkan penglihatan Anda mengalami gangguan PROTANOMALIA. Penglihatan Anda sedikit kurang peka terhadap warna merah.";
                    resultAdvice.Text = "Segera periksakan mata Anda ke dokter dan mintalah terapi atau pengobatan yang sesuai untuk mengurangi dampak.";

                }
                else if (Answers.countRightAnswer <= 13 && (Answers.possibleDeutan <= 2 && Answers.absolutelyNormal >= 2))
                {
                    resultGreet.Text = "MAAF";
                    var failColor = ConvertStringToColor("#FD5711");
                    SolidColorBrush fail = new SolidColorBrush(failColor);
                    resultGreet.Foreground = fail;
                    resultExplanation.Text = "Hasil tes menunjukkan penglihatan Anda mengalami gangguan DETERANOMALIA. Penglihatan Anda sedikit kurang peka terhadap warna hijau.";
                    resultAdvice.Text = "Segera periksakan mata Anda ke dokter dan mintalah terapi atau pengobatan yang sesuai untuk mengurangi dampak.";

                }
                else if ((Answers.countRightAnswer > 13 && Answers.countRightAnswer < 17) && Answers.absolutelyNormal == 4)
                {
                    resultGreet.Text = "SELAMAT";
                    var successColor = ConvertStringToColor("#6CA8D1");
                    SolidColorBrush success = new SolidColorBrush(successColor);
                    resultGreet.Foreground = success;
                    resultExplanation.Text = "Hasil tes menunjukkan penglihatan Anda masih dalam keadaan NORMAL.";
                    resultAdvice.Text = "Tetap jaga kesehatan mata Anda dengan mengonsumsi sumber vitamin A seperti wortel dan buah-buahan lainnya";
                }

            }

            if (mainPivot.SelectedIndex == 26)
            {
                retestGrid.Visibility = Visibility.Visible;
                //resultPlate1.Text = Answers.answerPlate[0];
                //resultPlate2.Text = Answers.answerPlate[1];
                //resultPlate3.Text = Answers.answerPlate[2];
                //resultPlate4.Text = Answers.answerPlate[3];
                //resultPlate5.Text = Answers.answerPlate[4];
                //resultPlate6.Text = Answers.answerPlate[5];
                //resultPlate7.Text = Answers.answerPlate[6];
                //resultPlate8.Text = Answers.answerPlate[7];
                //resultPlate9.Text = Answers.answerPlate[8];
                //resultPlate10.Text = Answers.answerPlate[9];
                //resultPlate11.Text = Answers.answerPlate[10];
                //resultPlate12.Text = Answers.answerPlate[11];
                //resultPlate13.Text = Answers.answerPlate[12];
                //resultPlate14.Text = Answers.answerPlate[13];
                //resultPlate15.Text = Answers.answerPlate[14];
                //resultPlate16.Text = Answers.answerPlate[15];
                //resultPlate17.Text = Answers.answerPlate[16];
                //resultPlate18.Text = Answers.answerPlate[17];
                //resultPlate19.Text = Answers.answerPlate[18];
                //resultPlate20.Text = Answers.answerPlate[19];
                //resultPlate21.Text = Answers.answerPlate[20];
                //resultPlate22.Text = Answers.answerPlate[21];
                //resultPlate23.Text = Answers.answerPlate[22];
                //resultPlate24.Text = Answers.answerPlate[23];
                //resultPlate25.Text = Answers.answerPlate[24];


            }


        }

        private void settingIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.RelativeOrAbsolute));

        }

        private void helpIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Help.xaml", UriKind.RelativeOrAbsolute));

        }
    }
}
