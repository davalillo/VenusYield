﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace VenusYield
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            try
            {
                using (StreamReader file = File.OpenText(systemPath + @"\VenusYield.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Settings mySettings = (Settings)serializer.Deserialize(file, typeof(Settings));

                    tbBSCAddress.Text = mySettings.BSCaddress;
                    tblTelegramBot.Text = mySettings.TelegramBot;
                    tbTelegramChatID.Text = mySettings.TelegramChatID.ToString();
                    nBorrowUnder.Text = mySettings.BorrowUnder.ToString();
                    nBorrowOver.Text = mySettings.BorrowOver.ToString();
                    nRefreshMins.Text = mySettings.RefreshRate.ToString();
                }
            }
            catch (Exception ex) { Console.WriteLine("{0}", ex.Message.ToString()); }
        }

        private void lGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/J1Mtonic/VenusYield");
        }

        private void lVenusCommunity_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://community.venus.io");
        }

        private void lJ1MtonicTwitter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://twitter.com/J1Mtonic");
        }

        private void bGetID_Click(object sender, EventArgs e)
        {
            if (tblTelegramBot.Text != "")
            {
                string telegramGetUpdates = "https://api.telegram.org/bot" + tblTelegramBot.Text + "/getUpdates";
                Telegram telegramMsg = new Telegram();
                WebClient webclient = new WebClient();

                try
                {
                    telegramMsg = Telegram.FromJson(webclient.DownloadString(telegramGetUpdates));
                    tbTelegramChatID.Text = telegramMsg.Result[0].Message.Chat.Id.ToString();
                }
                catch (Exception ex) { Console.WriteLine("{0}", ex.Message.ToString()); }
            }
            else MessageBox.Show("'Telegram Bot' cannot be empty!");
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings mySettings = new Settings
            {
                BSCaddress = tbBSCAddress.Text,
                TelegramBot = tblTelegramBot.Text,
                TelegramChatID = UInt32.TryParse(tbTelegramChatID.Text, out var f) ? f : default,
                BorrowUnder = UInt32.TryParse(nBorrowUnder.Text, out f) ? f : default,
                BorrowOver = UInt32.TryParse(nBorrowOver.Text, out f) ? f : default,
                RefreshRate = UInt32.TryParse(nRefreshMins.Text, out f) ? f : default,
            };

            if (mySettings.BorrowUnder >= mySettings.BorrowOver)
            {
                MessageBox.Show("Under must be smaller than Over...");
            }

            var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            using (StreamWriter file = File.CreateText(systemPath + @"\VenusYield.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, mySettings);
            }
        }
    }
    public class Settings
    {
        public string BSCaddress { get; set; }
        public string TelegramBot { get; set; }
        public UInt32 TelegramChatID { get; set; }
        public UInt32 BorrowUnder { get; set; }
        public UInt32 BorrowOver { get; set; }
        public UInt32 RefreshRate { get; set; }
    }        

    public partial class Telegram
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("result")]
        public List<Result> Result { get; set; }

        public static Telegram FromJson(string json) => JsonConvert.DeserializeObject<Telegram>(json);
    }

    public partial class Result
    {
        [JsonProperty("update_id")]
        public long UpdateId { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("message_id")]
        public long MessageId { get; set; }

        [JsonProperty("from")]
        public From From { get; set; }

        [JsonProperty("chat")]
        public Chat Chat { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("entities")]
        public List<Entity> Entities { get; set; }
    }

    public partial class Chat
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Entity
    {
        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("length")]
        public long Length { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class From
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("language_code")]
        public string LanguageCode { get; set; }
    }
}
