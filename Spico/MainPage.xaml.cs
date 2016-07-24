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

        #region Constant values

        private const string WakeupText = "oh mighty computer";

        private string[] DigitValues = new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

        private string[] MenuValues = new string[] { "digits", "forecast" };

        #endregion

        #region Properties

        private RecognizerMode _mode;
        private RecognizerMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    SetRecognizerMode(_mode);
                }
            }
        }

        #endregion

        #region Fields

        private SpeechRecognizer speechRecognizer;

        private WasapiAudioRecorder audioRecorder;

        private enum RecognizerMode { Wakeup, Digits, Menu, Phones };

        private bool isPhonemeRecognitionEnabled;

        public int rightAnswer;
        public int possibleNormal;
        public int countProtan;
        public int countDeutan;
        public string[] isRight;

        public class SpeechRecognizerInitiated
        {
            public static bool isInitiated;
        }

        public static int numberOfFailed;

        public static bool isSpeechActived;

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

                startRecognize();




        }

        

        private async  void waitInitialize()
        {
            await InitialzeSpeechRecognizerForContinuous();

        }

        private void OnBackKeyPress(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            var mm = MessageBox.Show("Apakah Anda yakin akan keluar dari aplikasi ini ?", "Keluar", MessageBoxButton.OKCancel);
            if(mm == MessageBoxResult.OK)
            {
                Application.Current.Terminate();
            }
        }
        #endregion

        #region No using speech
        private void notUsingSpeech()
        {
            

            if (mainPivot.SelectedIndex == 0)
            {
                
                if (resultText.Text == "dua belas" || resultText.Text == "12")
                {
                    playSound_RightAnswer();
                    Answers.countRightAnswer++;
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=1", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=0", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 1)
            {

                
                if (resultText.Text == "delapan" || resultText.Text == "8" || resultText.Text == "tiga" || resultText.Text == "3")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "delapan")
                    {
                        Answers.countRightAnswer++;

                    }

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=2", UriKind.RelativeOrAbsolute));


                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=1", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 2)
            {
               
                if (resultText.Text == "enam" || resultText.Text == "6" || resultText.Text == "lima" || resultText.Text == "5")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "enam" || resultText.Text == "6")
                    {
                        Answers.countRightAnswer++;


                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=3", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=2", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 3)
            {
                
                    if (resultText.Text == "enam" || resultText.Text == "6")
                if (resultText.Text == "dua puluh sembilan" || resultText.Text == "29" || resultText.Text == "tujuh puluh" || resultText.Text == "70")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "dua puluh sembilan" || resultText.Text == "29")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=4", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=3", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 4)
            {
               
                if (resultText.Text == "lima puluh tujuh" || resultText.Text == "57" || resultText.Text == "tiga puluh lima" || resultText.Text == "35")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "lima puluh tujuh" || resultText.Text == "57")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=5", UriKind.RelativeOrAbsolute));


                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=4", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 5)
            {
                
                if (resultText.Text == "lima" || resultText.Text == "5" || resultText.Text == "dua" || resultText.Text == "2")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "lima" || resultText.Text == "5")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=6", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=5", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 6)
            {
                
                if (resultText.Text == "tiga" || resultText.Text == "3" || resultText.Text == "lima" || resultText.Text == "5")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "tiga" || resultText.Text == "3")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=7", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=6", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 7)
            {
               
                if (resultText.Text == "lima belas" || resultText.Text == "15" || resultText.Text == "tujuh belas" || resultText.Text == "17")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "lima belas" || resultText.Text == "15")
                    {
                        Answers.countRightAnswer++;
                    }
                                        this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=8", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=7", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 8)
            {
               
                if (resultText.Text == "tujuh puluh empat" || resultText.Text == "74" || resultText.Text == "dua puluh satu" || resultText.Text == "21")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "tujuh puluh empat" || resultText.Text == "74")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=9", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=8", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 9)
            {
                
                if (resultText.Text == "dua" || resultText.Text == "2" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "dua" || resultText.Text == "2")
                    {
                        Answers.countRightAnswer++;
                    }
                                        this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=10", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=9", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 10)
            {
               
                if (resultText.Text == "enam" || resultText.Text == "6" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "enam" || resultText.Text == "6")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=11", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=10", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 11)
            {
               
                if (resultText.Text == "sembilan puluh tujuh" || resultText.Text == "97" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "sembilan puluh tujuh" || resultText.Text == "97")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=12", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=11", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 12)
            {
                
                if (resultText.Text == "empat puluh lima" || resultText.Text == "45" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "empat puluh lima" || resultText.Text == "45")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=13", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=12", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 13)
            {
              
                if (resultText.Text == "lima" || resultText.Text == "5" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer(); 
                    if (resultText.Text == "lima" || resultText.Text == "5")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=14", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=13", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 14)
            {
               
                if (resultText.Text == "tujuh" || resultText.Text == "7" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "tujuh" || resultText.Text == "7")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=15", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=14", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 15)
            {
                
                if (resultText.Text == "enam belas" || resultText.Text == "16" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "enam belas" || resultText.Text == "16")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=16", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=15", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 16)
            {
               
                if (resultText.Text == "tujuh puluh tiga" || resultText.Text == "73" || resultText.Text == "tidak ada")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "tujuh puluh tiga" || resultText.Text == "73")
                    {
                        Answers.countRightAnswer++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=17", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=16", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 17)
            {
               
                if (resultText.Text == "tidak ada" || resultText.Text == "lima" || resultText.Text == "5")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;

                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=18", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=17", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 18)
            {
                
                if (resultText.Text == "tidak ada" || resultText.Text == "dua" || resultText.Text == "2")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;

                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=19", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=18", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 19)
            {
              
                if (resultText.Text == "tidak ada" || resultText.Text == "empat puluh lima" || resultText.Text == "45")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;

                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=20", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=19", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 20)
            {
              
                if (resultText.Text == "tidak ada" || resultText.Text == "tujuh puluh tiga" || resultText.Text == "73")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;

                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=21", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=20", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 21)
            {
               
                if (resultText.Text == "dua puluh enam" || resultText.Text == "26" || resultText.Text == "enam" || resultText.Text == "6" || resultText.Text == "dua" || resultText.Text == "2")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "dua puluh enam" || resultText.Text == "26")
                    {
                        Answers.absolutelyNormal++;
                    }
                    else if (resultText.Text == "enam" || resultText.Text == "6")
                    {
                        Answers.possibleProtan++;
                    }
                    else if (resultText.Text == "dua" || resultText.Text == "2")
                    {
                        Answers.possibleDeutan++;
                    }
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=22", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=21", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 22)
            {
               
                if (resultText.Text == "empat puluh dua" || resultText.Text == "dua" || resultText.Text == "empat")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "empat puluh dua" || resultText.Text == "42")
                    {
                        Answers.absolutelyNormal++;
                    }
                    else if (resultText.Text == "dua" || resultText.Text == "2")
                    {
                        Answers.possibleProtan++;
                    }
                    else if (resultText.Text == "empat" || resultText.Text == "4")
                    {
                        Answers.possibleDeutan++;
                    }

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=23", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=22", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 23)
            {
                
                if (resultText.Text == "tiga puluh lima" || resultText.Text == "35" || resultText.Text == "lima" || resultText.Text == "5" || resultText.Text == "tiga" || resultText.Text == "3")
                {
                    playSound_RightAnswer();
                    if (resultText.Text == "tiga puluh lima" || resultText.Text == "35")
                    {
                        Answers.absolutelyNormal++;
                    }
                    else if (resultText.Text == "lima" || resultText.Text == "5")
                    {
                        Answers.possibleProtan++;
                    }
                    else if (resultText.Text == "tiga" || resultText.Text == "3")
                    {
                        Answers.possibleDeutan++;
                    }

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=24", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=23", UriKind.RelativeOrAbsolute));

                }
            }
            else if (mainPivot.SelectedIndex == 24)
            {
                
                if (resultText.Text == "sembilan puluh enam" || resultText.Text == "96" || resultText.Text == "enam" || resultText.Text == "6" || resultText.Text == "sembilan" || resultText.Text == "9")
                {
                    playSound_RightAnswer();

                    countFinalAnswer finalAnswer = new countFinalAnswer();

                    if (resultText.Text == "sembilan puluh enam" || resultText.Text == "96")
                    {
                        Answers.absolutelyNormal++;
                    }
                    else if (resultText.Text == "enam" || resultText.Text == "6")
                    {
                        Answers.possibleProtan++;
                    }
                    else if (resultText.Text == "sembilan" || resultText.Text == "9")
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
                
                if (resultText.Text == "dua belas")
                {
                    Answers.countRightAnswer++;
                    MainPage.numberOfFailed = 0;
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=1", UriKind.RelativeOrAbsolute));
                }
                else
                {
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
                if (resultText.Text == "delapan" || resultText.Text == "tiga")
                {
                    if (resultText.Text == "delapan")
                    {
                        Answers.countRightAnswer++;
                        isSpeechActived = true;

                    }

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=2", UriKind.RelativeOrAbsolute));


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
            else if (mainPivot.SelectedIndex == 2)
            {
               if (resultText.Text == "enam" || resultText.Text == "lima")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=3", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "enam")
                    {
                        Answers.countRightAnswer++;


                    }
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
            else if (mainPivot.SelectedIndex == 3)
            {
                if (resultText.Text == "dua puluh sembilan" || resultText.Text == "tujuh puluh")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=4", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "dua puluh sembilan")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 4)
            {
                if (resultText.Text == "lima puluh tujuh" || resultText.Text == "tiga puluh lima")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=5", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "lima puluh tujuh")
                    {
                        Answers.countRightAnswer++;
                    }

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
            else if (mainPivot.SelectedIndex == 5)
            {
                if (resultText.Text == "lima" || resultText.Text == "dua")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=6", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "lima")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 6)
            {
                if (resultText.Text == "tiga" || resultText.Text == "lima")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=7", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "tiga")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 7)
            {
                if (resultText.Text == "lima belas" || resultText.Text == "tujuh belas")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=8", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "lima belas")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 8)
            {
                if (resultText.Text == "tujuh puluh empat" || resultText.Text == "dua puluh satu")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=9", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "tujuh puluh empat")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 9)
            {
                if (resultText.Text == "dua" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=10", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "dua")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 10)
            {
                if (resultText.Text == "enam" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=11", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "enam")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 11)
            {
                if (resultText.Text == "sembilan puluh tujuh" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=12", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "sembilan puluh tujuh")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 12)
            {
                if (resultText.Text == "empat puluh lima" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=13", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "empat puluh lima")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 13)
            {
                if (resultText.Text == "lima" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=14", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "lima")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 14)
            {
                 if (resultText.Text == "tujuh" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=15", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "tujuh")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 15)
            {
                 if (resultText.Text == "enam belas" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=16", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "enam belas")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 16)
            {
                if (resultText.Text == "tujuh puluh tiga" || resultText.Text == "tidak ada")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=17", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "tujuh puluh tiga")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 17)
            {
                if (resultText.Text == "tidak ada" || resultText.Text == "lima")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=18", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 18)
            {
                if (resultText.Text == "tidak ada" || resultText.Text == "dua")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=19", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;
                    }
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
            else if (mainPivot.SelectedIndex == 19)
            {
               if (resultText.Text == "tidak ada" || resultText.Text == "empat puluh lima")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=20", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;

                    }
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
            else if (mainPivot.SelectedIndex == 20)
            {
                if (resultText.Text == "tidak ada" || resultText.Text == "tujuh puluh tiga")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=21", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "tidak ada")
                    {
                        Answers.countRightAnswer++;

                    }
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
                if (resultText.Text == "dua puluh enam" || resultText.Text == "enam" || resultText.Text == "dua")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=22", UriKind.RelativeOrAbsolute));
                    if (resultText.Text == "dua puluh enam")
                    {
                        Answers.absolutelyNormal++;
                    }
                    else if (resultText.Text == "enam")
                    {
                        Answers.possibleProtan++;
                    }
                    else if (resultText.Text == "dua")
                    {
                        Answers.possibleDeutan++;
                    }
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
            else if (mainPivot.SelectedIndex == 22)
            {
                if (resultText.Text == "empat puluh dua" || resultText.Text == "dua" || resultText.Text == "empat")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=23", UriKind.RelativeOrAbsolute));
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
            else if (mainPivot.SelectedIndex == 23)
            {
              if (resultText.Text == "tiga puluh lima" || resultText.Text == "lima" || resultText.Text == "tiga")
                {
                    isSpeechActived = true;

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=24", UriKind.RelativeOrAbsolute));
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
            else if (mainPivot.SelectedIndex == 24)
            {
                if (resultText.Text == "sembilan puluh enam" || resultText.Text == "enam" || resultText.Text == "sembilan")
                {
                    isSpeechActived = true;

                    countFinalAnswer finalAnswer = new countFinalAnswer();

                    this.NavigationService.Navigate(new Uri("/MainPage.xaml?item=25", UriKind.RelativeOrAbsolute));
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

        





        #region Innitial Load methods

        private async void LoadRecognitionAsync(bool phonemeRecognitionEnabled)
        {
            isPhonemeRecognitionEnabled = phonemeRecognitionEnabled;

            ContentPanel.IsHitTestVisible = false;

            plate1.Visibility = Visibility.Collapsed;
            stateMessage.Text = "Sedang memuat komponen speech recognition";


            //// Initializing
            await InitialzeSpeechRecognizerForContinuous();
            SetRecognizerMode(Mode);


            //// UI            
            ContentPanel.IsHitTestVisible = true;
            plate1.Visibility = Visibility.Visible;
            stateMessage.Text = "";

            // Set innitial UI state
            if (!isPhonemeRecognitionEnabled)
            {
                Mode = RecognizerMode.Digits;
            }


        }



        
        #region Common Methods

        private void SetRecognizerMode(RecognizerMode mode)
        {
            try
            {
                string result = string.Empty;
                speechRecognizer.StopProcessing();
                Debug.WriteLine(result);
                result = speechRecognizer.SetSearch(mode.ToString());
                Debug.WriteLine(result);
                speechRecognizer.StartProcessing();
                Debug.WriteLine(result);

            }
            catch (Exception ex)
            {
                stateMessage.Text = ex.Message;
            }

            //ModeMessageBlock.Text = string.Format("running '{0}' mode", mode);
        }

        private void FoundText(string recognizedText)
        {
            resultText.Text = recognizedText;
            StopNativeRecorder();

            usingSpeech();

            //MainMessageBlock.Text = recognizedText;
        }

        #endregion

        #region SpeechRecognizer Methods (PocketSphinx) - Continuous

        private async Task InitialzeSpeechRecognizerForContinuous()
        {
            List<string> initResults = new List<string>();

            try
            {
                AudioContainer.SphinxSpeechRecognizer = new SpeechRecognizer();
                speechRecognizer = AudioContainer.SphinxSpeechRecognizer;

                speechRecognizer.resultFound += speechRecognizer_resultFound;
                speechRecognizer.resultFinalizedBySilence += speechRecognizer_resultFinalizedBySilence;

                if (!isPhonemeRecognitionEnabled)
                {
                    await Task.Run(() =>
                    {
                        var initResult = speechRecognizer.Initialize("\\Assets\\models\\hmm\\en-us", "\\Assets\\models\\dict\\cmu07a.dic");
                        initResults.Add(initResult);
                        initResult = speechRecognizer.AddKeyphraseSearch(RecognizerMode.Wakeup.ToString(), WakeupText);
                        initResults.Add(initResult);
                        initResult = speechRecognizer.AddGrammarSearch(RecognizerMode.Menu.ToString(), "\\Assets\\models\\grammar\\menu.gram");
                        initResults.Add(initResult);
                        initResult = speechRecognizer.AddGrammarSearch(RecognizerMode.Digits.ToString(), "\\Assets\\models\\grammar\\digits.gram");
                        initResults.Add(initResult);
                        initResult = speechRecognizer.AddNgramSearch("forecast", "\\Assets\\models\\lm\\weather.dmp");
                        initResults.Add(initResult);
                        SpeechRecognizerInitiated.isInitiated = true;
                    });
                }
                else
                {
                    await Task.Run(() =>
                    {
                        var initResult = speechRecognizer.InitializePhonemeRecognition("\\Assets\\models\\hmm\\en-us");
                        initResults.Add(initResult);
                        initResult = speechRecognizer.AddPhonesSearch(RecognizerMode.Phones.ToString(), "\\Assets\\models\\lm\\en-us-phone.lm.bin");
                        initResults.Add(initResult);
                    });
                }
            }
            catch (Exception ex)
            {
                var initResult = ex.Message;
                initResults.Add(initResult);
            }

            foreach (var result in initResults)
            {
                Debug.WriteLine(result);
            }
        }

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
            FoundText(finalResult);
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
                plate4.Visibility = Visibility.Collapsed;
                speech4.Visibility = Visibility.Visible;
                textSpeech4.Visibility = Visibility.Visible;
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

        public class Answers
        {
            public static int countRightAnswer { get; set; }
            public string countsRightAnswer { get; set; }

            public static int absolutelyNormal { get; set; }
            public static int possibleProtan { get; set; }
            public static int possibleDeutan { get; set; }
            public string[] isRightAnswer { get; set; }
        }
        
        public class countFinalAnswer
        {
            public int numberRightAnswer { get; set; }
            public int numberPossibleNormal { get; set; }
            public int numberPossibleProtan { get; set; }
            public int numberPossibleDeutan { get; set; }
        }

        private void startRecognize()
        {
            
                _mode = RecognizerMode.Digits;
                LoadRecognitionAsync(false);
           


        }

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
            Answers.countRightAnswer = 0;
            Answers.absolutelyNormal = 0;
            Answers.possibleDeutan = 0;
            Answers.possibleProtan = 0;
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
#endregion