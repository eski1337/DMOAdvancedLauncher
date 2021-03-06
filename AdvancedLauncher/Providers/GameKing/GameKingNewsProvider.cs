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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;
using AdvancedLauncher.SDK.Model.Web;
using AdvancedLauncher.Tools;
using HtmlAgilityPack;

namespace AdvancedLauncher.Providers.GameKing {

    public class GameKingNewsProvider : AbstractNewsProvider {
        private static string STR_URL_NEW_PAGE = "http://dmo.gameking.com{0}";
        private static string STR_DATE_FORMAT_REGEX = "(\\d{2})(-)(\\d{2})(-)(\\d{2})";

        public GameKingNewsProvider(ILogManager logManager) : base(logManager) {
        }

        public override List<NewsItem> GetNews() {
            LogManager.Info("Getting JoyMax news...");

            HtmlDocument doc = new HtmlDocument();
            List<NewsItem> news = new List<NewsItem>();

            HtmlNodeCollection newsNode = null;
            int tryCount = 5;
            while (newsNode == null && tryCount > 0) {
                string html = WebClientEx.DownloadContent(LogManager, "http://dmo.gameking.com/Main/Main.aspx", 5000);
                doc.LoadHtml(html);
                newsNode = doc.DocumentNode.SelectNodes("//div[@class='news-list']/ul/li");
                tryCount--;
            }

            if (newsNode == null) {
                return null;
            }

            HtmlNode newsWrap = newsNode[0];
            HtmlNodeCollection newsList = doc.DocumentNode.SelectNodes("//div[@class='news-list']/ul/li");
            NewsItem item;

            if (newsList != null) {
                for (int i = 0; i <= newsList.Count - 1; i++) {
                    item = new NewsItem();
                    item.Mode = newsWrap.SelectNodes("//div[@class='lead']/span[contains(@class, 'mode')]")[i].InnerText;
                    item.Subject = System.Web.HttpUtility.HtmlDecode(newsWrap.SelectNodes("//div[@class='lead']/span[@class='subj']")[i].InnerText);
                    item.Date = newsWrap.SelectNodes("//div[@class='lead']/span[@class='date']")[i].InnerText;

                    Regex r = new Regex(STR_DATE_FORMAT_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match m = r.Match(item.Date);
                    if (m.Success) {
                        item.Date = m.Groups[1].ToString() + "-" + m.Groups[3].ToString() + "-20" + m.Groups[5].ToString();
                    } else {
                        item.Date = null;
                    }
                    foreach (HtmlAttribute atr in newsWrap.SelectNodes("//div[@class='view']/div[@class='btn-right']/span[@class='read-more']/a")[i].Attributes) {
                        if (atr.Name == "href") {
                            item.Url = string.Format(STR_URL_NEW_PAGE, atr.Value);
                            break;
                        }
                    }
                    item.Content = System.Web.HttpUtility.HtmlDecode(newsWrap.SelectNodes("//div[@class='view']/div[@class='memo']")[i].InnerText);
                    item.Content = item.Content.Trim().Replace("\r\n\r\n", "\r\n").Replace("\t", "");
                    news.Add(item);
                }
            }

            if (news.Count == 0) {
                return null;
            }
            return news;
        }
    }
}