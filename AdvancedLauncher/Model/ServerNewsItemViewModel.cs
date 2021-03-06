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
using System.Windows;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;

namespace AdvancedLauncher.Model {

    public class ServerNewsItemViewModel : AbstractItemViewModel<NewsItem> {

        public ServerNewsItemViewModel(ILanguageManager LanguageManager) : base(LanguageManager) {
        }

        protected override void OnLanguageChanged(object sender, SDK.Model.Events.BaseEventArgs e) {
            NotifyPropertyChanged("Type");
            NotifyPropertyChanged("LocalizedDate");
        }

        private string _Title;

        public string Title {
            get {
                return _Title;
            }
            set {
                if (value != _Title) {
                    _Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private DateTime? _Date;

        public DateTime? Date {
            get {
                return _Date;
            }
            set {
                if (value != _Date) {
                    _Date = value;
                    NotifyPropertyChanged("Date");
                    NotifyPropertyChanged("LocalizedDate");
                }
            }
        }

        public string LocalizedDate {
            get {
                if (_Date == null) {
                    return string.Empty;
                }
                return _Date.Value.ToLocalTime().ToShortDateString();
            }
        }

        private string _Content;

        public string Content {
            get {
                return _Content;
            }
            set {
                if (value != _Content) {
                    _Content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        private string _Type;

        public string Type {
            get {
                return LanguageManager.Model[_Type];
            }
        }

        public string TypeName {
            get {
                return _Type;
            }
            set {
                if (value != _Type) {
                    _Type = value;
                    NotifyPropertyChanged("Type");
                    NotifyPropertyChanged("TypeName");
                }
            }
        }

        private Rect _ImgVB;

        public Rect ImgVB {
            get {
                return _ImgVB;
            }
            set {
                if (value != _ImgVB) {
                    _ImgVB = value;
                    NotifyPropertyChanged("ImgVB");
                }
            }
        }

        private string _Link;

        public string Link {
            get {
                return _Link;
            }
            set {
                if (value != _Link) {
                    _Link = value;
                    NotifyPropertyChanged("Link");
                }
            }
        }
    }
}