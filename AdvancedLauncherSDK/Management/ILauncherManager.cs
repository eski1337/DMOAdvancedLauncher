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
using System.Collections;
using System.Collections.Generic;
using AdvancedLauncher.SDK.Management.Execution;
using AdvancedLauncher.SDK.Model.Config;

namespace AdvancedLauncher.SDK.Management {

    public interface ILauncherManager : IManager, IEnumerable, IEnumerable<ILauncher> {

        ILauncher Default {
            get;
        }

        void RegisterLauncher(ILauncher launcher);

        bool UnRegisterLauncher(string name);

        bool UnRegisterLauncher(ILauncher launcher);

        ILauncher GetLauncher(Profile profile);

        ILauncher findByMnemonic(string name);

        T findByType<T>(Type type) where T : ILauncher;
    }
}