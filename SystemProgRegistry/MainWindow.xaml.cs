using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;



namespace SystemProgRegistry
{


    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
           
            InitializeComponent();
            this.DataContext = this;
            CreateKeys();
            GetValues();
            themes.Add("Light");
            themes.Add("Dark");
            themes.Add("Custom");
            sizeFont.Add(15);
            sizeFont.Add(18);
            sizeFont.Add(20);
            sizeFont.Add(22);
            sizeFont.Add(25);
            sizeFont.Add(30);
            languages.Add("English");
            languages.Add("Russian");
           

            PropertyChanged += (sender, args) =>
            {


                if (args.PropertyName == nameof(SelectedTheme))
                {
                    ChangeTheme();
                }
                else if (args.PropertyName == nameof(SelectedSize))
                {
                    ChangeSize();
                }
                else if (args.PropertyName == nameof(SelectedLanguage))
                {
                    ChangeLanguage();
                }

            };
        }
        public void CreateKeys()
        {
            RegistryKey key = Registry.CurrentUser;

            if (key.OpenSubKey("SystemProgRegistryKey") == null)
            {
                RegistryKey regKey = key.CreateSubKey("SystemProgRegistryKey", true);

                RegistryKey themeKey = regKey.CreateSubKey("ThemeKey", true);
                themeKey.SetValue("Light", 1);
                themeKey.SetValue("Dark", 0);
                themeKey.Close();
                RegistryKey sizeKey = regKey.CreateSubKey("SizeKey", true);
                sizeKey.SetValue("Size15", 1, RegistryValueKind.DWord);
                sizeKey.SetValue("Size18", 0, RegistryValueKind.DWord);
                sizeKey.SetValue("Size20", 0, RegistryValueKind.DWord);
                sizeKey.SetValue("Size22", 0, RegistryValueKind.DWord);
                sizeKey.SetValue("Size25", 0, RegistryValueKind.DWord);
                sizeKey.SetValue("Size30", 0, RegistryValueKind.DWord);
                sizeKey.Close();

                RegistryKey languageKey = regKey.CreateSubKey("LanguageKey", true);
                languageKey.SetValue("English", 1);
                languageKey.SetValue("Russian", 0);
                languageKey.Close();
                regKey.Close();
                key.Close();

            }
        }
        public void GetValues()
        {
            RegistryKey key = Registry.CurrentUser;
            RegistryKey systemKey = key.OpenSubKey("SystemProgRegistryKey");
            int resTH = (int)systemKey.OpenSubKey("ThemeKey").GetValue("Light", null);
            if (resTH == 0)
            {
                BackgroundBrush = Brushes.Black;
                FontBrush = Brushes.White;
                SelectedTheme = "Dark";
            }
            else if (resTH == 1)
            {
                BackgroundBrush = Brushes.White;
                FontBrush = Brushes.Black;
                SelectedTheme = "Light";
            }

            string[] keysNames = systemKey.OpenSubKey("SizeKey").GetValueNames();
            double size = 0;
            foreach (var item in keysNames)
            {
                int resFS = (int)systemKey.OpenSubKey("SizeKey").GetValue(item, null);
                if (resFS == 1)
                {
                    size = Double.Parse(item.Remove(0, 4));
                    break;
                }
            }
            SelectedSize = size;
            TextSize = SelectedSize;
            int resL = (int)systemKey.OpenSubKey("LanguageKey").GetValue("Russian", null);
            if (resL == 1)
            {
                SelectedLanguage = "Russian";
                Properties.ResourceService.Current.ChangeCulture("ru-RU");

            }
            else
            {
                SelectedLanguage = "English";
                Properties.ResourceService.Current.ChangeCulture("en-US");

            }
        }

        public void ChangeLanguage()
        {
            RegistryKey key = Registry.CurrentUser;
            RegistryKey languageKey = key.OpenSubKey("SystemProgRegistryKey", true).OpenSubKey("LanguageKey", true);
            if (selectedLanguage == "Russian")
            {
                Properties.ResourceService.Current.ChangeCulture("ru-RU");
                languageKey.SetValue("Russian", 1);
                languageKey.SetValue("English", 0);
               
            
            }
            else if (selectedLanguage == "English")
            {
                Properties.ResourceService.Current.ChangeCulture("en-US");
      

                languageKey.SetValue("Russian", 0);
                languageKey.SetValue("English", 1);
                
            }
        }

        public void ChangeSize()
        {
            RegistryKey key = Registry.CurrentUser;
            RegistryKey sizeKey = key.OpenSubKey("SystemProgRegistryKey", true).OpenSubKey("SizeKey", true);

            TextSize = SelectedSize;
            string[] keysNames = sizeKey.GetValueNames();

            foreach (var item in keysNames)
            {
                int resFS = (int)sizeKey.GetValue(item, null);
                if (resFS == 1)
                {
                    sizeKey.SetValue(item, 0);
                    break;
                }
            }
            sizeKey.SetValue(("Size" + TextSize), 1);

        }
        public void ChangeTheme()
        {
            RegistryKey key = Registry.CurrentUser;
            RegistryKey themes = key.OpenSubKey("SystemProgRegistryKey", true).OpenSubKey("ThemeKey", true);

            if (selectedTheme == "Light")
            {


                themes.SetValue("Dark", 0);
                themes.SetValue("Light", 1);
                FontBrush = Brushes.Black;
                BackgroundBrush = Brushes.White;

            }
            if (selectedTheme == "Dark")
            {

                themes.SetValue("Dark", 1);
                themes.SetValue("Light", 0);
                FontBrush = Brushes.White;
                BackgroundBrush = Brushes.Black;

            }
            if (selectedTheme == "Custom")
            {
                int res = (int)Registry.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "AppsUseLightTheme", null);
                if (res == 1)
                {
                    themes.SetValue("Dark", 0);
                    themes.SetValue("Light", 1);
                    FontBrush = Brushes.Black;
                    BackgroundBrush = Brushes.White;
                }
                else
                {
                    themes.SetValue("Dark", 1);
                    themes.SetValue("Light", 0);
                    FontBrush = Brushes.White;
                    BackgroundBrush = Brushes.Black;


                }

            }


            themes.Close();
            key.Close();

        }

        private string selectedTheme;
        private string selectedLanguage;
        private double selectedSize;

        private Brush fontBrush;
        private Brush backgroundBrush;

        private double textSize;

        public string SelectedTheme { get { return selectedTheme; } set { if (value != selectedTheme) { selectedTheme = value; OnPropertyChanged(); } } }
        public string SelectedLanguage { get { return selectedLanguage; } set { if (value != selectedLanguage) { selectedLanguage = value; OnPropertyChanged(); } } }
        public double SelectedSize { get { return selectedSize; } set { if (value != selectedSize) { selectedSize = value; OnPropertyChanged(); } } }

        public Brush FontBrush { get { return fontBrush; } set { if (value != fontBrush) { fontBrush = value; OnPropertyChanged(); } } }
        public Brush BackgroundBrush { get { return backgroundBrush; } set { if (value != backgroundBrush) { backgroundBrush = value; OnPropertyChanged(); } } }


        public double TextSize { get { return textSize; } set { if (value != textSize) { textSize = value; OnPropertyChanged(); } } }



        ICollection<string> themes = new ObservableCollection<string>();
        public IEnumerable<string> Themes => themes;

        ICollection<string> languages = new ObservableCollection<string>();
        public IEnumerable<string> Languages => languages;

        ICollection<double> sizeFont = new ObservableCollection<double>();
        public IEnumerable<double> SizeFont => sizeFont;




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
    namespace SystemProgRegistry.Properties
    {

        using System.Globalization;
        using System.ComponentModel;
        using System.Runtime.CompilerServices;
   

        public class ResourceService : INotifyPropertyChanged
        {
            #region singleton members

            private static readonly ResourceService _current = new ResourceService();
            public static ResourceService Current
            {
                get { return _current; }
            }
            #endregion

            readonly Resources _resources = new Resources();

        public Resources Resources
        {
            get { return this._resources; }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            {
                var handler = this.PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            public void ChangeCulture(string name)
            {
                Resources.Culture = CultureInfo.GetCultureInfo(name);
                this.RaisePropertyChanged("Resources");
            }
        
    }
}

