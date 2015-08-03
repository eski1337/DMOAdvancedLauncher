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
using System.Windows.Threading;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.UI.Extension;
using MahApps.Metro.Controls;
using Ninject;

namespace AdvancedLauncher.UI.Windows {

    public partial class Splashscreen : MetroWindow {
        private bool IsClosed = false;

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set; // This injection will force bootstrap environment for this window (require color theme)
        }

        public Splashscreen() {
            InitializeComponent();
            this.Closing += (s, e) => {
                IsClosed = true;
            };
        }

        public void SetProgress(string title) {
            if (IsClosed) {
                return;
            }
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action<string>((t) => {
                    this.Title = t;
                    DispatcherHelper.DoEvents();
                }), title);
        }
    }
}