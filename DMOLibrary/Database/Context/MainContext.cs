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
using System.Data.Entity;
using System.Linq;
using DMOLibrary.Database.Entity;

namespace DMOLibrary.Database.Context {

    public class MainContext : BaseContext {

        #region Constructors

        private static MainContext _Instance = null;

        static MainContext() {
            System.Data.Entity.Database.SetInitializer<MainContext>(new ContextInitializer());
        }

        private MainContext() {
            // hide the constructor
        }

        public static MainContext Instance {
            get {
                if (_Instance == null) {
                    _Instance = new MainContext();
                }
                return _Instance;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Guild>()
                .HasRequired(r => r.Server)
                .WithMany(s => s.Guilds)
                .HasForeignKey(f => f.ServerId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Tamer>()
                .HasRequired(r => r.Guild)
                .WithMany(s => s.Tamers)
                .HasForeignKey(f => f.GuildId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Digimon>()
                .HasRequired(r => r.Tamer)
                .WithMany(s => s.Digimons)
                .HasForeignKey(f => f.TamerId)
                .WillCascadeOnDelete(true);
        }

        #endregion Constructors

        #region Database sets

        public DbSet<Server> Servers {
            get;
            set;
        }

        public DbSet<Guild> Guilds {
            get;
            set;
        }

        public DbSet<Tamer> Tamers {
            get;
            set;
        }

        public DbSet<Digimon> Digimons {
            get;
            set;
        }

        public DbSet<DigimonType> DigimonTypes {
            get;
            set;
        }

        public DbSet<TamerType> TamerTypes {
            get;
            set;
        }

        #endregion Databases

        #region Digimon operations

        public Digimon FindRandomDigimon(Guild guild, int minlvl) {
            return Digimons.Where(e => e.Tamer.Guild.Id == guild.Id && e.Level >= minlvl)
                .OrderBy(c => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public Digimon FindRandomDigimon(Tamer tamer, int minlvl) {
            return Digimons.Where(e => e.Tamer.Id == tamer.Id && e.Level >= minlvl)
                .OrderBy(c => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        #endregion

        #region TamerType operations

        public TamerType FindTamerTypeByCode(int code) {
            return TamerTypes.FirstOrDefault(e => e.Code == code);
        }

        #endregion TamerType operations

        #region DigimonType operations

        public DigimonType FindRandomDigimonType() {
            return DigimonTypes.OrderBy(c => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public List<DigimonType> FindDigimonTypesByName(string name) {
            return DigimonTypes.Where(e => e.Name == name).ToList();
        }

        public List<DigimonType> FindDigimonTypesByKoreanName(string name) {
            return DigimonTypes.Where(e => e.NameKorean == name).ToList();
        }

        public DigimonType FindDigimonTypeByCode(int code) {
            return DigimonTypes.FirstOrDefault(e => e.Code == code);
        }

        public List<DigimonType> FindDigimonTypesBySearchGDMO(string search) {
            return DigimonTypes.Where(e => e.SearchGDMO == search).ToList();
        }

        public List<DigimonType> FindDigimonTypesBySearchKDMO(string search) {
            return DigimonTypes.Where(e => e.SearchKDMO == search).ToList();
        }

        public DigimonType FindDigimonTypeBySearchGDMO(string search) {
            return DigimonTypes.Where(e => e.SearchGDMO == search).FirstOrDefault();
        }

        public DigimonType FindDigimonTypeBySearchKDMO(string search) {
            return DigimonTypes.Where(e => e.SearchKDMO == search).FirstOrDefault();
        }

        public void AddOrUpdateDigimonType(DigimonType type, bool isKorean) {
            DigimonType ordinal = DigimonTypes.FirstOrDefault(e => e.Code == type.Code);
            if (ordinal == null) {
                if (isKorean) {
                    type.NameKorean = type.Name;
                    type.SearchKDMO = PrepareDigimonSearch(type.Name);
                } else {
                    type.SearchGDMO = PrepareDigimonSearch(type.Name);
                }
                DigimonTypes.Add(type);
                return;
            }

            if (isKorean) {
                ordinal.NameKorean = type.Name;
                ordinal.SearchKDMO = PrepareDigimonSearch(type.Name);
            } else {
                ordinal.SearchGDMO = PrepareDigimonSearch(type.Name);
            }
        }

        public static String PrepareDigimonSearch(string name) {
            if (name == null) {
                return null;
            }
            return name
                .Replace(" ", String.Empty)
                .Replace("(", String.Empty)
                .Replace(")", String.Empty)
                .Replace("[", String.Empty)
                .Replace("]", String.Empty)
                .Replace("-", String.Empty)
                .ToLower();
        }

        #endregion
    }
}