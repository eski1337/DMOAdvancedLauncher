﻿// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ======================================================================

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Pages;
using System.Runtime.InteropServices;

namespace AdvancedLauncher.Windows {
    public partial class MainWindow : Window {
        public static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(MainWindow));  
        private const int SC_CLOSE = 0xF060;
        private const int MF_ENABLED = 0x0000;
        private const int MF_GRAYED = 0x0001;
        private const int MF_DISABLED = 0x0002;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);
        private IntPtr hWnd = IntPtr.Zero;

        private bool IsCloseLocked = true;

        private Settings SettingsWindow = null;
        private About AboutWindow = null;
        private Gallery GalleryTab = null;
        private Personalization PersonalizationTab = null;
        private Community CommunityTab = null;
        private int currentTab = 0;

        public MainWindow() {
            // test commit
            InitializeComponent();
            if (!LayoutRoot.Children.Contains(Logger.Instance)) {
                LayoutRoot.Children.Add(Logger.Instance);
            }
            AdvancedLauncher.Service.UpdateChecker.Check();
            LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
            LauncherEnv.Settings.ProfileChanged += ReloadTabs;
            LauncherEnv.Settings.ProfileLocked += OnProfileLocked;
            LauncherEnv.Settings.ClosingLocked += OnClosingLocked;
            LauncherEnv.Settings.FileSystemLocked += OnFileSystemLocked;
            this.Closing += (s, e) => { e.Cancel = IsCloseLocked; };
            MainMenu.AboutClick += OnAboutClick;
            MainMenu.SettingsClick += OnSettingsClick;
            MainMenu.LoggerClick += OnLoggerClick;
            ReloadTabs();
            try { App.splash.Close(TimeSpan.FromSeconds(1)); } catch { }
#if DEBUG
            this.Title += " (development build " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
#endif
            LOGGER.Info(this.Title + " initialized");
        }

        private void OnFileSystemLocked(bool IsLocked) {
            if (IsLocked) {
                //Выбираем первую вкладку и отключаем персонализацию (на всякий случай)
                NavControl.SelectedIndex = 0;
                NavPersonalization.IsEnabled = false;
            } else {
                //Включаем персонализации обратно если игра определена
                if (LauncherEnv.Settings.CurrentProfile.GameEnv.CheckGame()) {
                    NavPersonalization.IsEnabled = true;
                }
            }
        }

        private void OnClosingLocked(bool IsLocked) {
            if (hWnd == IntPtr.Zero)
                hWnd = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle;
            //Заблокировать закрытие окна
            IsCloseLocked = IsLocked;
            //Отключим кнопку "Х"
            EnableMenuItem(GetSystemMenu(hWnd, false), SC_CLOSE, IsLocked ? MF_DISABLED | MF_GRAYED : MF_ENABLED);
        }

        private void OnProfileLocked(bool IsLocked) {
            MainMenu.IsChangeEnabled = !IsLocked;
        }

        public void ReloadTabs() {
            //Выбираем первую вкладку и отключаем все модули
            NavControl.SelectedIndex = 0;
            NavGallery.IsEnabled = false;
            NavCommunity.IsEnabled = false;
            NavPersonalization.IsEnabled = false;

            //Если доступен веб-профиль, включаем вкладку сообщества
            if (LauncherEnv.Settings.CurrentProfile.DMOProfile.IsWebAvailable) {
                NavCommunity.IsEnabled = true;
            }

            //Если путь до игры верен, включаем вкладку галереи и персонализации
            if (LauncherEnv.Settings.CurrentProfile.GameEnv.CheckGame()) {
                NavGallery.IsEnabled = true;
                NavPersonalization.IsEnabled = true;
            }
        }

        private void OnTabChanged(object sender, SelectionChangedEventArgs e) {
            //Prevent handling over changing inside tab item
            if (currentTab == NavControl.SelectedIndex) {
                return;
            }

            switch (NavControl.SelectedIndex) {
                case 0: {
                        MainPage.Activate();
                        break;
                    }
                case 1: {
                        if (GalleryTab == null) {
                            GalleryTab = new Gallery();
                            NavGallery.Content = GalleryTab;
                        }
                        GalleryTab.Activate();
                        break;
                    }
                case 2: {
                        if (CommunityTab == null) {
                            CommunityTab = new Community();
                            NavCommunity.Content = CommunityTab;
                        }
                        CommunityTab.Activate();
                        break;
                    }
                case 3: {
                        if (PersonalizationTab == null) {
                            PersonalizationTab = new Personalization();
                            NavPersonalization.Content = PersonalizationTab;
                        }
                        PersonalizationTab.Activate();
                        break;
                    }
            }

            currentTab = NavControl.SelectedIndex;
            ((TabItem)NavControl.Items[currentTab]).Focus();
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e) {
            if (SettingsWindow == null) {
                SettingsWindow = new Settings();
                LayoutRoot.Children.Add(SettingsWindow);
            }
            SettingsWindow.Show(true);
        }

        private void OnAboutClick(object sender, RoutedEventArgs e) {
            if (AboutWindow == null) {
                AboutWindow = new About();
                LayoutRoot.Children.Add(AboutWindow);
            }
            AboutWindow.Show(true);
        }

        private void OnLoggerClick(object sender, RoutedEventArgs e) {
            if (!LayoutRoot.Children.Contains(Logger.Instance)) {
                LayoutRoot.Children.Add(Logger.Instance);
            }
            Logger.Instance.Show(true);
        }
    }
}
