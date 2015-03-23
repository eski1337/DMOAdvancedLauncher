﻿// ======================================================================
// DMOLibrary
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;

namespace DMOLibrary.Profiles {

    public abstract class AbstractWebProfile {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(AbstractWebProfile));
        protected DownloadStatus downloadStatus = new DownloadStatus();

        #region EVENTS

        /* Complete results:
         * 0 - all good
         * 1 - can't connect to database
         * 2 - web access error
         * 404 - guild not found
         * 3 - can't get guild info
         * 4 - web page is not supported or guild not found
         * */

        protected System.Windows.Threading.Dispatcher OwnerDispatcher;

        public delegate void DownloadHandler(object sender);

        public delegate void DownloadCompleteHandler(object sender, DMODownloadResultCode code, Guild result);

        public delegate void DownloadStatusChangedHandler(object sender, DownloadStatus status);

        public event DownloadHandler DownloadStarted;

        public event DownloadCompleteHandler DownloadCompleted;

        public event DownloadStatusChangedHandler StatusChanged;

        protected virtual void OnStarted() {
            LOGGER.Info("GuildInfo obtaining started.");
            if (DownloadStarted != null) {
                if (OwnerDispatcher != null && !OwnerDispatcher.CheckAccess()) {
                    OwnerDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadHandler((sender) => {
                        DownloadStarted(sender);
                    }), this);
                } else {
                    DownloadStarted(this);
                }
            }
        }

        protected virtual void OnCompleted(DMODownloadResultCode code, Guild result) {
            LOGGER.Info(String.Format("GuildInfo obtaining completed: code={0}, result={1}", code, result));
            if (DownloadCompleted != null) {
                if (OwnerDispatcher != null && !OwnerDispatcher.CheckAccess()) {
                    OwnerDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadCompleteHandler((sender, code_, result_) => {
                        DownloadCompleted(sender, code_, result_);
                    }), this, code, result);
                } else
                    DownloadCompleted(this, code, result);
            }
        }

        protected virtual void OnStatusChanged(DMODownloadStatusCode code, string info, int p, int pm) {
            LOGGER.Info(String.Format("GuildInfo obtaining status changed: code={0}, info={1}, p={2}, pm={3}", code, info, p, pm));
            downloadStatus.Code = code;
            downloadStatus.Info = info;
            downloadStatus.Progress = p;
            downloadStatus.MaxProgress = pm;

            if (StatusChanged != null) {
                if (OwnerDispatcher != null && !OwnerDispatcher.CheckAccess()) {
                    OwnerDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadStatusChangedHandler((sender, status_) => {
                        StatusChanged(sender, status_);
                    }), this, downloadStatus);
                } else
                    StatusChanged(this, downloadStatus);
            }
        }

        #endregion EVENTS

        public void GetGuildAsync(System.Windows.Threading.Dispatcher ownerDispatcher, Server server, string guildName, bool isDetailed) {
            this.OwnerDispatcher = ownerDispatcher;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s1, e2) => {
                GetGuild(server, guildName, isDetailed);
            };
            bw.RunWorkerAsync();
        }

        public void SetDispatcher(System.Windows.Threading.Dispatcher ownerDispatcher) {
            this.OwnerDispatcher = ownerDispatcher;
        }

        public abstract List<DigimonType> GetDigimonTypes();

        public abstract Guild GetGuild(Server server, string guildName, bool isDetailed);

        protected abstract bool GetGuildInfo(ref Guild guild, bool isDetailed);

        protected abstract List<Digimon> GetDigimons(Tamer tamer, bool isDetailed);

        protected abstract bool GetStarterInfo(ref Digimon digimon, Tamer tamer);

        protected abstract bool GetMercenaryInfo(ref Digimon digimon, Tamer tamer);

        public static Guild GetActualGuild(AbstractWebProfile webProfile, Server server, string guildName, bool isDetailed, int actualInterval) {
            bool fetchCurrent = false;
            using (MainContext context = new MainContext()) {
                Guild storedGuild = context.FindGuild(server, guildName);
                if (storedGuild != null && !(isDetailed && !storedGuild.IsDetailed) && storedGuild.UpdateTime != null) {
                    TimeSpan timeDiff = (TimeSpan)(DateTime.Now - storedGuild.UpdateTime);
                    if (timeDiff.Days < actualInterval) {
                        fetchCurrent = true;
                    }
                }
                if (fetchCurrent) {
                    webProfile.OnStarted();
                    webProfile.OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, guildName, 0, 50);
                    storedGuild = context.FetchGuild(server, guildName);
                    webProfile.OnCompleted(DMODownloadResultCode.OK, storedGuild);
                    return storedGuild;
                }
            }
            return webProfile.GetGuild(server, guildName, isDetailed);
        }

        public static void GetActualGuildAsync(System.Windows.Threading.Dispatcher ownerDispatcher, AbstractWebProfile webProfile,
            Server server, string guildName, bool isDetailed, int actualInterval) {
            bool fetchCurrent = false;
            using (MainContext context = new MainContext()) {
                Guild storedGuild = context.FindGuild(server, guildName);
                if (storedGuild != null && !(isDetailed && !storedGuild.IsDetailed) && storedGuild.UpdateTime != null) {
                    TimeSpan timeDiff = (TimeSpan)(DateTime.Now - storedGuild.UpdateTime);
                    if (timeDiff.Days < actualInterval) {
                        fetchCurrent = true;
                    }
                }
            }
            if (fetchCurrent) {
                Task.Factory.StartNew(() => {
                    using (MainContext context = new MainContext()) {
                        webProfile.OnStarted();
                        webProfile.OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, guildName, 0, 50);
                        Guild storedGuild = context.FetchGuild(server, guildName);
                        webProfile.OnCompleted(DMODownloadResultCode.OK, storedGuild);
                    }
                });
                return;
            }
            webProfile.GetGuildAsync(ownerDispatcher, server, guildName, isDetailed);
        }
    }
}