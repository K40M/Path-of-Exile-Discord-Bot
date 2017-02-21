using Discord;
using Discord.Commands;
using Discord.Audio;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Net;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace PathOfExileBot
{
    class PoEBot
    {
        DiscordClient client;
        CommandService commands;
        Channel voiceChannel;
        IAudioClient voiceClient;
        List<Build> builds;
        List<Map> maps;
        string filePath = Environment.CurrentDirectory + "\\Files\\";
        private const string poeWikiMapUrl = "http://pathofexile.gamepedia.com/Map";

        public PoEBot()
        {
            Random r = new Random();
            builds = new List<Build>();
            maps = new List<Map>();
            GetMapData();
            LoadJsonData();
            UniqueBaseItems();

            //CONFIG
            //Should probably move this to a config file or something
            bool differentOuputChannel = true;
            string outPutChannel = "poe_channel";

            client = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });
            
            client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;
            });

            client.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
            });

            commands = client.GetService<CommandService>();

            #region Commands

            #region MiscCommands

            commands.CreateCommand("Base").Parameter("Item", ParameterType.Multiple).Do(async (e) =>
            {
                Channel outputChannel = differentOuputChannel ? e.Server.TextChannels.Where(x => x.Name == outPutChannel).First() : e.Channel;
                string argument = "";
                string output = "";
                for (int i = 0; i < e.Args.Length; i++)
                {
                    argument += e.Args[i] + " ";
                }
                foreach(Map a in maps)
                {
                    if(a.uniqueBases.Contains(argument.Trim().ToLower()))
                        output += a.Name + "\n";
                }
                await outputChannel.SendMessage(output);
            });

            commands.CreateCommand("Bases").Do(async (e) =>
            {
                Channel outputChannel = differentOuputChannel ? e.Server.TextChannels.Where(x => x.Name == outPutChannel).First() : e.Channel;
                string output = "";
                maps = maps.OrderBy(x => x.Name).ToList();
                for (int i = 0; i < maps.Count / 3; i++)
                {
                    if(maps[i].uniqueBases.Count >0)
                        output += maps[i].ToString() + "\n";
                }
                await outputChannel.SendMessage(output);
                output = "";
                for (int i = (maps.Count / 3); i < (maps.Count / 3) * 2; i++)
                {
                    if (maps[i].uniqueBases.Count > 0)
                        output += maps[i].ToString() + "\n";
                }
                await outputChannel.SendMessage(output);
                output = "";
                for (int i = (maps.Count / 3) * 2; i < maps.Count; i++)
                {
                    if (maps[i].uniqueBases.Count > 0)
                        output += maps[i].ToString() + "\n";
                }
                await outputChannel.SendMessage(output);
            });

            commands.CreateCommand("checkbases").Do(async (e) =>
            {
                Channel outputChannel = differentOuputChannel ? e.Server.TextChannels.Where(x => x.Name == outPutChannel).First() : e.Channel;
                List<string> uniqueBases = new List<string>() { "Blue_Pearl_Amulet", "Bone_Helmet", "Crystal_Belt", "Gripped_Gloves", "Marble_Amulet", "Opal_Ring", "Spiked_Gloves", "Steel_Ring", "Vanguard_Belt", "Fingerless_Silk_Gloves", "Two-Toned_Boots_(Cold_and_Lightning_Resistance)", "Two-Toned_Boots_(Fire_and_Lightning_Resistance)", "Two-Toned_Boots_(Fire_and_Cold_Resistance)", "Blue_Pearl_Amulet" };

                string output = "";
                foreach(string a in uniqueBases)
                {
                    bool exists = false;
                    foreach (var item in maps)
                    {
                        if (item.uniqueBases.Contains(a.Replace('_',' ').ToLower()))
                        {
                            exists = true;
                            output += a + " = " + exists + "\n";
                            break;
                        }
                    }                  
                }

                await outputChannel.SendMessage(output);
            });

            commands.CreateCommand("Map").Parameter("Map", ParameterType.Multiple).Do(async (e) =>
            {
                Channel outputChannel = differentOuputChannel ? e.Server.TextChannels.Where(x => x.Name == outPutChannel).First() : e.Channel;
                var argument = "";
                for (int i = 0; i < e.Args.Length; i++)
                {
                    argument += e.Args[i] + " ";
                }
                argument += "Map";
                foreach (Map map in maps)
                {
                    if(map.Name.ToLower() == argument.ToLower().Trim())
                    {
                        await outputChannel.SendMessage(map.ToString());
                        return;
                    }
                }
            });

            commands.CreateCommand("Maps").Do(async (e) =>
            {
                Channel outputChannel = differentOuputChannel ? e.Server.TextChannels.Where(x => x.Name == outPutChannel).First() : e.Channel;
                string output = "";
                maps = maps.OrderBy(o => o.Name).ToList();

                for (int i = 0; i < maps.Count /3; i++)
                {
                    output += maps[i].ToString() + "\n";
                }
                await outputChannel.SendMessage(output);
                output = "";

                for (int i = (maps.Count/3); i < (maps.Count /3) *2; i++)
                {
                    output += maps[i].ToString() + "\n";
                }
                await outputChannel.SendMessage(output);
                output = "";

                for (int i = (maps.Count / 3) *2; i < maps.Count; i++)
                {
                    output += maps[i].ToString() + "\n";
                }
                await outputChannel.SendMessage(output);
            });

            commands.CreateCommand("Maps").Parameter("Layout", ParameterType.Optional).Parameter("Level", ParameterType.Optional).Do(async (e) =>
            {
                maps = maps.OrderBy(q => q.Name).ToList();
                Channel outputChannel = differentOuputChannel ? e.Server.TextChannels.Where(x => x.Name == outPutChannel).First() : e.Channel;
                string output = "";
                int a;
                var arguments = e.Args;

                if(int.TryParse(arguments[0], out a) && arguments[1] != "")
                        maps.Where(m => m.LayoutType.ToLower() == arguments[1].ToLower()).ToList()
                            .Where(m => m.Level.ToLower() == arguments[0].ToLower()).ToList()
                            .ForEach(x => output += x.ToString() + "\n");

                else if(arguments[0] != "" && arguments[1] != "")
                    maps.Where(m => m.LayoutType.ToLower() == e.GetArg("Layout").ToLower()).ToList()
                        .Where(m => m.Level.ToLower() == e.GetArg("Level").ToLower()).ToList()
                        .ForEach(x => output += x.ToString() + "\n");

                else if (int.TryParse(arguments[0], out a))
                    maps.Where(m => m.Level.ToLower() == arguments[0].ToLower()).ToList()
                        .ForEach(x => output += x.ToString() + "\n");

                else if (arguments[0] != "")
                    maps.Where(m => m.LayoutType.ToLower() == e.GetArg("Layout").ToLower()).ToList()
                        .ForEach(x => output += x.ToString() + "\n");

                await outputChannel.SendMessage(output);
            });

            #endregion

            #region BuildCommands
            commands.CreateCommand("CoC").Alias(new string[] { "cock" }).Parameter("Amount", ParameterType.Optional).Do(async (e) =>
            {
                List<ActiveSkill> a = CoC(int.Parse(e.GetArg("Amount")));
                string returnMessage = "Your CoC skills are | ";
                foreach (ActiveSkill skill in a)
                {
                    returnMessage += skill.name + " | ";
                }

                await e.Channel.SendMessage(returnMessage);
            });

            commands.CreateCommand("Reroll").Do(async (e) =>
            {
                string[] options = new string[] { "You may not reroll.", "NO!", "Try again.", "YES!", "You may reroll once.", "You may reroll twice.", "Bitch please deal with it", "Fuck off, I got better things to do" };
                await e.Channel.SendTTSMessage((options[r.Next(0, options.Length)]));
            });

            commands.CreateCommand("Roll").Parameter("NumberOfKeystones", ParameterType.Optional).Alias(new string[] { "Rollebol", "Barrelroll" }).Description("Generates a random build for your character.")
                .Do(async (e) =>
                {
                    int b;
                    var a = int.TryParse(e.GetArg("NumberOfKeystones"), out b);
                    if (b > 23)
                        b = 23;
                    if (b <= 0)
                        b = 0;
                    JoinChannel(e.User.VoiceChannel);
                    voiceClient = await client.GetService<AudioService>().Join(voiceChannel);
                    SendAudio(filePath + "ROLL.mp3");
                    
                    await e.Channel.SendMessage(Roll(e.User.Name, b));
                });

            commands.CreateCommand("RollSilent").Parameter("NumberOfKeystones", ParameterType.Optional).Do(async (e) =>{
                int b;
                var a = int.TryParse(e.GetArg("NumberOfKeystones"), out b);
                if (b > 23)
                    b = 23;
                if (b <= 0)
                    b = 0;
                await e.Channel.SendMessage(Roll(e.User.Name, b));
            });

            commands.CreateCommand("Builds").Parameter("buildtype", ParameterType.Optional).Do(async (e) =>
            {
                Channel outputChannel = differentOuputChannel ? e.Server.TextChannels.Where(x => x.Name == outPutChannel).First() : e.Channel;
                var arguments = e.Args;
                string messageToSend = "";
                BuildType type;
                List<Build> rightBuilds;

                if(e.GetArg("buildtype") != "")
                {
                    if (arguments[0] == "1" || arguments[0].ToLower() == "life")
                        type = BuildType.Life;
                    else if (arguments[0] == "2" || arguments[0].ToLower() == "lowlife" || arguments[0].ToLower() == "low life")
                        type = BuildType.LowLife;
                    else if (arguments[0] == "3" || arguments[0].ToLower() == "ci")
                        type = BuildType.CI;
                    else type = BuildType.NoIdea;
                    rightBuilds = builds.Where(b => b.type == type).ToList();
                }
                else
                {
                    rightBuilds = builds;
                }

                foreach (Build build in rightBuilds)
                {
                    messageToSend += build.buildName + "\n" +
                                     build.type + "\n" +
                                     "<" + ShortenURL(build.treeURL) +">" + "\n" +
                                     build.description + "\n" + "----------------------------------------------------------------------------------------------------" + "\n";
                }

                await outputChannel.SendMessage(messageToSend);
            });


            commands.CreateCommand("addbuild").Parameter("Name", ParameterType.Required)
                                              .Parameter("buildURL", ParameterType.Required)
                                              .Parameter("buildtype", ParameterType.Required)
                                              .Parameter("Description", ParameterType.Multiple)
                                              .Do(async (e) =>
            {
                await e.Channel.Messages.Last().Delete();
                var arguments = e.Args;
                if (arguments[0] == "" || arguments[1] == "" || arguments[2] == "" ||
                    Uri.IsWellFormedUriString(arguments[1], UriKind.RelativeOrAbsolute) == false)
                {
                    await e.Channel.SendMessage("You did something wrong!");
                }
                else if (builds.Any(b => b.buildName.ToLower() == e.GetArg("Name").ToLower()))
                    await e.Channel.SendMessage("A build with this name already exist");
                else
                {
                    Console.WriteLine(arguments.Length);
                    BuildType type;
                    if (arguments[2] == "1" || arguments[2].ToLower() == "life")
                        type = BuildType.Life;
                    else if (arguments[2] == "2" || arguments[2].ToLower() == "lowlife" || arguments[3].ToLower() == "low life")
                        type = BuildType.LowLife;
                    else if (arguments[2] == "3" || arguments[2].ToLower() == "ci")
                        type = BuildType.CI;
                    else type = BuildType.NoIdea;

                    string description = "";
                    if (arguments.Length > 4)
                    {
                        for (int i = 3; i < arguments.Length; i++)
                        {
                            description += arguments[i] + " ";
                        }
                    }
                    else description = arguments[3];

                    if (arguments[1][0] != 'h')
                        builds.Add(new Build(arguments[0], "https://" + arguments[1], type, arguments[3]));
                    else
                        builds.Add(new Build(arguments[0], arguments[1], type, description));
                    SaveJsonData();
                    await e.Channel.SendMessage("Saved build " + e.GetArg("Name") + " succesfully");
                }
            });

            commands.CreateCommand("RemoveBuild").Parameter("buildname").Do(async (e) =>
            {
                if(builds.Any(b => b.buildName.ToLower() == e.GetArg("buildname").ToLower()))
                {
                    if (e.User.ServerPermissions.ManageServer)
                    {
                        builds.RemoveAll(b => b.buildName.ToLower() == e.GetArg("buildname").ToLower());
                        SaveJsonData();
                        LoadJsonData();
                        await e.Channel.SendMessage("Removed the build " + e.GetArg("buildname"));
                    }
                    else await e.Channel.SendMessage("You are not an admin");
                }
                else
                    await e.Channel.SendMessage("This build doesn't exist");
            });

            #endregion

            #endregion

            //Change BOTTOKEN to your bot token
            client.ExecuteAndWait(async () =>
            {
                await client.Connect("BOTTOKEN", TokenType.Bot);
            });
        }

        //Fills up the skill gems with tags from the wiki
        //Cleave gets tagged with Attack, AOE, Melee etc. 
        private async void GetGemTags()
        {
            Console.WriteLine(Data.skills.Count);
            foreach (var gem in Data.skills)
            {
                HtmlDocument htmlDoc = new HtmlDocument();

                await Task.Run(() =>
                {
                    htmlDoc.Load(GetHtmlStream("http://pathofexile.gamepedia.com/" + gem.name));
                });

                if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Any())
                    throw new Exception("Errors: " + htmlDoc.ParseErrors);
                var tags = htmlDoc.DocumentNode.SelectSingleNode("//span[@class ='group']");
                var nodes = tags.SelectNodes("a");
                foreach (var item in nodes)
                {
                    gem.tags.Add(item.InnerText);
                }
            }
        }

        private void LoadJsonData()
        {
            //If we dont currently have a builds file we create one and load the JSON data.
            if (!File.Exists(filePath + "Builds.json"))
            {
                File.Create(filePath + "Builds.json");
                LoadJsonData();
            }
            if(File.Exists(filePath + "Builds.json"))
            {
                var text = File.ReadAllText(filePath + "Builds.JSON");
                if(text != "")
                    builds = JsonConvert.DeserializeObject<List<Build>>(text);
            }
        }

        private string Roll(string userName, int numberOfKeystones)
        {
            Random r = new Random();
            string displayKeystones;
            if (numberOfKeystones > 23)
                numberOfKeystones = 23;
            if (numberOfKeystones <= 0)
                numberOfKeystones = 0;

            displayKeystones = numberOfKeystones <= 0 ? displayKeystones = "" : "Keystones: ";

            //Ascendancy
            Ascendancy ascendancy = Data.Ascendancies[r.Next(0, Data.Ascendancies.Count)];
            List<SkillType> types = ascendancy.skilltype;

            //Vaal Skill
            string vaalName = "";
            ActiveSkill gem = GetRandomSkill(types);

            if (gem.vaal == true)
                vaalName = "Vaal " + gem.name + " " + GetWikiLink("Vaal " + gem.name) + "\n";

            //Keystones
            List<string> keystones = GetKeystones(numberOfKeystones);
            foreach (string s in keystones)
            {
                displayKeystones += "\n" + s + " " + GetWikiLink(s);
            }

            if (gem.name == "Cast On Critical Strike Support")
            {
                List<ActiveSkill> skills = CoC(1);
                string cocSkill = "Your CoC skill is: " + "\n";
                foreach (ActiveSkill s in skills)
                {
                    cocSkill += s.name + GetWikiLink(s.name) + "\n";
                }

                return userName + "\n" + cocSkill + " " + GetWikiLink("Cast On Critical Strike Support") + "\n" + ascendancy.name + " (" + ascendancy.baseClass.ToString() + ") " + GetWikiLink(ascendancy.name) +
                    "\n" + vaalName +
                    "\n" + displayKeystones;
            }
            else
            {
                return userName + "\n" + gem.name + " " + GetWikiLink(gem.name) + "\n" + ascendancy.name + " (" + ascendancy.baseClass.ToString() + ") " + GetWikiLink(ascendancy.name) +
                    "\n" + vaalName +
                    "\n" + displayKeystones;
            }
        }

        private void SaveJsonData()
        {
            var json = JsonConvert.SerializeObject(builds, Formatting.Indented);
            File.WriteAllText(filePath + "Builds.JSON", json);
        }

        private async void GetMapData()
        {
            List<Map> mapDataList = new List<Map>();
            HtmlDocument htmlDoc = new HtmlDocument();

            await Task.Run(() =>
            {
                htmlDoc.Load(GetHtmlStream(poeWikiMapUrl));
            });

            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Any())
                throw new Exception("Errors: " + htmlDoc.ParseErrors);

            if (htmlDoc.DocumentNode != null)
            {
                HtmlNodeCollection mapRows = htmlDoc.DocumentNode.SelectNodes(@"//div[@id='global-wrapper']/div[@id='pageWrapper']/div[@id='content']/div[@id='bodyContent']/div[@id='mw-content-text']/table[2]/tr");

                for (int i = 0; i < mapRows.Count; i++)
                {
                    if (i == 0) // skip headers
                        continue;
                    try
                    {
                        Map data = GetMapFromHtmlNode(mapRows[i]);
                        mapDataList.Add(data);
                    }
                    catch
                    {

                    }
                }
            }
            maps = mapDataList;
        }

        private Stream GetHtmlStream(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
                return response.GetResponseStream();

            return Stream.Null;
        }

        private async void UniqueBaseItems()
        {
            List<string> uniqueBases = new List<string>() { "Blue_Pearl_Amulet", "Bone_Helmet", "Crystal_Belt", "Gripped_Gloves", "Marble_Amulet", "Opal_Ring", "Spiked_Gloves", "Steel_Ring", "Vanguard_Belt", "Fingerless_Silk_Gloves","Two-Toned_Boots_(Cold_and_Lightning_Resistance)", "Two-Toned_Boots_(Fire_and_Lightning_Resistance)", "Two-Toned_Boots_(Fire_and_Cold_Resistance)", "Blue_Pearl_Amulet"};

            HtmlDocument htmlDoc = new HtmlDocument();

            for (int i = 0; i < uniqueBases.Count; i++)
            {
                await Task.Run(() =>
                {
                    htmlDoc.Load(GetHtmlStream("http://pathofexile.gamepedia.com/" + uniqueBases[i]));
                });

                if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Any())
                    throw new Exception("Errors: " + htmlDoc.ParseErrors);

                HtmlNodeCollection mapRows = htmlDoc.DocumentNode.SelectNodes(@"//div[@id='global-wrapper']/div[@id='pageWrapper']/div[@id='content']/div[@id='bodyContent']/div[@id='mw-content-text']/ul");
                foreach (var row in mapRows)
                {
                    for (int j = 0; j < row.ChildNodes.Count; j++)
                    {
                        HtmlNode subNode = row.ChildNodes[j];
                        if (subNode.Name == "li")
                        {
                            HtmlNodeCollection nameNode = subNode.SelectSingleNode("span").ChildNodes;

                            foreach(Map map in maps)
                            {
                                if (map.Name == nameNode[1].InnerText)
                                {
                                    map.AddBase(uniqueBases[i]);
                                }
                            }                         
                        }
                    }
                }
            }          
        }

        private Map GetMapFromHtmlNode(HtmlNode node)
        {
            Map data = new Map();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                HtmlNode subNode = node.ChildNodes[i];
                if (subNode.Name == "td")
                {
                    switch (i)
                    {
                        case 1:
                            HtmlNode nameNode = subNode.SelectSingleNode("span/a[2]");
                            data.Name = nameNode.InnerText;
                            break;
                        case 3:
                            data.Level = subNode.InnerText.Trim();
                            break;
                        case 5:
                            data.Tier = subNode.InnerText;
                            break;
                        case 7:
                            data.Unique = subNode.InnerHtml.Contains("yes");
                            break;
                        case 9:
                            data.LayoutType = subNode.InnerText.Trim();
                            break;
                        case 11:
                            data.BossDifficulty = subNode.InnerText;
                            break;
                        case 13:
                            data.LayoutSet = subNode.InnerText;
                            break;
                        case 15:
                            List<string> uniqueBosses = new List<string>();
                            subNode.ChildNodes.ToList().ForEach(n =>
                            {
                                if (n.Name == "a")
                                    uniqueBosses.Add(n.InnerText);
                            });
                            data.UniqueBoss = string.Join(",", uniqueBosses);
                            break;

                        case 17:
                            data.NumberOfBosses = subNode.InnerText;
                            break;
                    }
                }
            }

            return data;
        }

        //Returns a random skill of a certain type
        private ActiveSkill GetRandomSkill(List<SkillType> types)
        {
            List<ActiveSkill> gems = new List<ActiveSkill>();
            ActiveSkill gem;

            foreach (ActiveSkill skill in Data.skills)
            {
                if (types.Contains(skill.type))
                    gems.Add(skill);
            }

            Random r = new Random();
            gem = gems[r.Next(0, gems.Count)];
            return gem;
        }

        //Returns a tinyURL based of the given url
        private string ShortenURL(string url)
        {
            Uri address = new Uri("http://tinyurl.com/api-create.php?url=" + url);
            WebClient client = new WebClient();
            string tinyUrl = client.DownloadString(address);
            return tinyUrl;
        }

        /// <summary>
        /// Returns a link in chat to a wiki page
        /// </summary>
        /// <param name="item">The name of the item you want returned</param>
        /// <returns>Returns a link in chat to a wiki page</returns>
        private string GetWikiLink(string item)
        {
            return "<http://pathofexile.gamepedia.com/" + item.Replace(" ", "_") + ">";
        }

        //Logs a message to the console
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        /// <summary>
        /// Joins a voiceChannel
        /// </summary>
        /// <param name="channelToJoin">The channel to join</param>
        private void JoinChannel(Channel channelToJoin)
        {
            var voiceChannels = client.Servers.First().VoiceChannels;

            foreach (var channel in voiceChannels)
            {
                if (channel == channelToJoin)
                    voiceChannel = channel;
            }
        }

        /// <summary>
        /// Plays an mp3 file in the current voiceChannel
        /// </summary>
        /// <param name="filePath">Path for mp3 file</param>
        private void SendAudio(string filePath)
        {
            voiceClient.Wait();
            var channelCount = client.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
            var OutFormat = new WaveFormat(48000, 16, channelCount); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
            using (var MP3Reader = new Mp3FileReader(filePath)) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
            using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
            {
                resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                byte[] buffer = new byte[blockSize];
                int byteCount;

                while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                {
                    if (byteCount < blockSize)
                    {
                        // Incomplete Frame
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }
                    voiceClient.Send(buffer, 0, blockSize); // Send the buffer to Discord
                }
            }
        }

        /// <summary>
        /// Returns a list of keystones
        /// </summary>
        /// <param name="amount">The amount of keystones you want</param>
        /// <returns>Returns a list of keystones</returns>
        private List<string> GetKeystones(int amount)
        {
            Random r = new Random();
            List<string> a = new List<string>();

            for (int i = 0; i < amount; i++)
            {
                string keystone = Data.keyStones[r.Next(0, Data.keyStones.Count)];
                if (!a.Contains(keystone))
                    a.Add(keystone);
                else
                {
                    i--;
                }
            }
            return a;
        }

        /// <summary>
        /// Returns a list of Cast on Crit skills
        /// </summary>
        /// <param name="amountOfSkills">The amount of Cast on Crit skills you want</param>
        /// <returns>Returns a list of Cast on Crit skills</returns>
        private List<ActiveSkill> CoC(int amountOfSkills)
        {
            List<string> notValidSkills = Data.notValidSkillList;

            List<ActiveSkill> a = new List<ActiveSkill>();
            Random r = new Random();
            int amount = (amountOfSkills < 1) ? 1 : (amountOfSkills > 4) ? 4 : amountOfSkills;

            for (int i = 0; i < amount; i++)
            {
                ActiveSkill activeSkill = Data.skills[r.Next(0, Data.skills.Count)];
                if (!a.Contains(activeSkill) && !notValidSkills.Contains(activeSkill.name))
                {
                    if (activeSkill.type == SkillType.MeleeSpell || activeSkill.type == SkillType.RangedSpell)
                        a.Add(activeSkill);
                    else
                        i--;
                }
                else
                {
                    i--;
                }
            }
            return a;
        }
    }
}
