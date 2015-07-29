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

namespace AdvancedLauncher.SDK.Model.Events {

    /// <summary>
    /// DMO File System Write status change event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void WriteStatusChangedEventHandler(object sender, WriteDirectoryEventArgs e);

    /// <summary>
    /// Write directory status change event args
    /// </summary>
    public class WriteDirectoryEventArgs : EventArgs {

        /// <summary>
        /// Current file number
        /// </summary>
        public int FileNumber {
            get;
            private set;
        }

        /// <summary>
        /// Files count
        /// </summary>
        public int FileCount {
            get;
            private set;
        }

        public WriteDirectoryEventArgs(int FileNumber, int FileCount) {
            this.FileNumber = FileNumber;
            this.FileCount = FileCount;
        }
    }
}