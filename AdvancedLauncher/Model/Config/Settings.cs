﻿// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2015 Ilya Egorov (goldrenard@gmail.com)

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
using System.ComponentModel;
using AdvancedLauncher.SDK.Model.Config;

namespace AdvancedLauncher.Model.Config {

    public class Settings : ISettings, INotifyPropertyChanged {

        public string LanguageFile {
            get; set;
        }

        public string AppTheme {
            get; set;
        }

        public string ThemeAccent {
            get; set;
        }

        public Settings() {
            // default constructor
        }

        public Settings(ISettings source) {
            this.LanguageFile = source.LanguageFile;
            this.AppTheme = source.AppTheme;
            this.ThemeAccent = source.ThemeAccent;
        }

        public void MergeConfig(ISettings source) {
            this.LanguageFile = source.LanguageFile;
            this.AppTheme = source.AppTheme;
            this.ThemeAccent = source.ThemeAccent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}