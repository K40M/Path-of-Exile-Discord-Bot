using Discord;
using Discord.Commands;
using Discord.Audio;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        List<Ascendancy> ascendancies;
        List<ActiveSkill> skillGems;
        List<Build> builds;
        string filePath = Environment.CurrentDirectory + "\\Files\\";

        public PoEBot()
        {
            Random r = new Random();
            ascendancies = FillAscendancyList();
            skillGems = FillSkillList();
            builds = new List<Build>();

            LoadJsonData();

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

            #region SoundCommands
            commands.CreateCommand("Physical").Do(async (e) =>
            {
                JoinChannel(e.User.VoiceChannel);
                voiceClient = await client.GetService<AudioService>().Join(voiceChannel);
                SendAudio(filePath + "PHYSICAL.mp3");
            });

            commands.CreateCommand("Divine").Do(async (e) =>
            {
                JoinChannel(e.User.VoiceChannel);
                voiceClient = await client.GetService<AudioService>().Join(voiceChannel);
                SendAudio(filePath + "Kreygasm.mp3");
            });

            commands.CreateCommand("Alive").Do(async (e) =>
            {
                JoinChannel(e.User.VoiceChannel);
                voiceClient = await client.GetService<AudioService>().Join(voiceChannel);
                SendAudio(filePath + "StayinAlive.mp3");
            });

            commands.CreateCommand("Smak").Do(async (e) =>
            {
                if (e.Server.Users.Contains(e.User))
                {
                    JoinChannel(e.User.VoiceChannel);
                    voiceClient = await client.GetService<AudioService>().Join(voiceChannel);
                    await e.Channel.SendMessage("http://i.imgur.com/RkDAORK.gif");
                    SendAudio(filePath + "Smak.mp3");
                }
                else await e.User.PrivateChannel.SendMessage("You need to be in the channel to use this command");
            });

            #endregion

            #region MiscCommands

            commands.CreateCommand("Profiles").Parameter("name", ParameterType.Optional).Do(async (e) =>
            {
                string profileURL = "<https://www.pathofexile.com/account/view-profile/";
                string url = "/characters> " + "\n";
                if (e.GetArg("name") != "")
                    await e.Channel.SendMessage(profileURL + e.GetArg("name") + url);
                else
                {
                    await e.Channel.SendMessage(profileURL + "Zuanartha" + url +
                                                profileURL + "Woetnie" + url +
                                                profileURL + "Skrylax" + url +
                                                profileURL + "Filetus" + url +
                                                profileURL + "SilenceEcho" + url +
                                                profileURL + "Pwnz0rWarr" + url);
                }
            });

            commands.CreateCommand("Wiki").Parameter("ItemName", ParameterType.Unparsed).Do(async (e) =>
            {
                Console.WriteLine("Test with " + e.GetArg("ItemName") + " as arguments");
                await e.Channel.SendMessage(CheckIfExists(e.GetArg("ItemName")));
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
                    string displayKeystones;
                    int b;
                    var a = int.TryParse(e.GetArg("NumberOfKeystones"), out b);
                    if (b > 23)
                        b = 23;
                    if (b <= 0)
                        b = 0;

                    displayKeystones = b <= 0 ? displayKeystones = "" : "Keystones: ";

                    //Ascendancy
                    Ascendancy ascendancy = ascendancies[r.Next(0, ascendancies.Count)];
                    List<SkillType> types = ascendancy.skilltype;

                    //Vaal Skill
                    string vaalName = "";
                    ActiveSkill gem = GetRandomSkill(types);

                    if (gem.vaal == true)
                        vaalName = "Vaal " + gem.name + " " + GetWikiLink("Vaal " + gem.name) + "\n";

                    //Keystones
                    List<string> keystones = GetKeystones(b);
                    foreach (string s in keystones)
                    {
                        displayKeystones += "\n" + s + " " + GetWikiLink(s);
                    }

                    JoinChannel(e.User.VoiceChannel);
                    voiceClient = await client.GetService<AudioService>().Join(voiceChannel);
                    SendAudio(filePath + "ROLL.mp3");

                    if (gem.name == "Cast On Critical Strike Support")
                    {
                        List<ActiveSkill> skills = CoC(1);
                        string cocSkill = "Your CoC skill is: " + "\n";
                        foreach (ActiveSkill s in skills)
                        {
                            cocSkill += s.name + GetWikiLink(s.name) + "\n";
                        }

                        await e.Channel.SendMessage(e.User.Name + "\n" + cocSkill + " " + GetWikiLink("Cast On Critical Strike Support") + "\n" + ascendancy.name + " (" + ascendancy.baseClass.ToString() + ") " + GetWikiLink(ascendancy.name) +
                            "\n" + vaalName +
                            "\n" + displayKeystones);
                    }
                    else
                    {
                        await e.Channel.SendMessage(e.User.Name + "\n" + gem.name + " " + GetWikiLink(gem.name) + "\n" + ascendancy.name + " (" + ascendancy.baseClass.ToString() + ") " + GetWikiLink(ascendancy.name) +
                            "\n" + vaalName +
                            "\n" + displayKeystones);
                    }

                });

            commands.CreateCommand("Builds").Parameter("buildtype", ParameterType.Optional).Do(async (e) =>
            {
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
                                     "<" + build.treeURL +">" + "\n" +
                                     build.description + "----------------------------------------------------------------------------------------------------" + "\n";
                }

                await e.Channel.SendMessage(messageToSend);
            });


            commands.CreateCommand("addbuild").Parameter("Name", ParameterType.Required)
                                              .Parameter("buildURL", ParameterType.Required)
                                              .Parameter("buildtype", ParameterType.Required)
                                              .Parameter("Description", ParameterType.Optional)
                                              .Do(async (e) =>
            {
                var arguments = e.Args;
                if (arguments[0] == "" || arguments[1] == "" || arguments[2] == "" || 
                    Uri.IsWellFormedUriString(arguments[1], UriKind.RelativeOrAbsolute) == false
                    || builds.Any(b => b.buildName == e.GetArg("Name")))
                {
                    await e.Channel.SendMessage("You did something wrong!");
                }
                else
                {
                    BuildType type;
                    if (arguments[2] == "1" || arguments[2].ToLower() == "life")
                        type = BuildType.Life;
                    else if (arguments[2] == "2" || arguments[2].ToLower() == "lowlife" || arguments[3].ToLower() == "low life")
                        type = BuildType.LowLife;
                    else if (arguments[2] == "3" || arguments[2].ToLower() == "ci")
                        type = BuildType.CI;
                    else type = BuildType.NoIdea;

                    if(arguments[1][0] != 'h')
                        builds.Add(new Build(arguments[0], "https://" + arguments[1], type, arguments[3]));
                    else
                        builds.Add(new Build(arguments[0], arguments[1], type, arguments[3]));
                    SaveJsonData();
                    await e.Channel.SendMessage("Saved build " + e.GetArg("Name") + " succesfully");
                }
            });

            commands.CreateCommand("RemoveBuild").Parameter("buildname").Do(async (e) =>
            {
                if(builds.Any(b => b.buildName == e.GetArg("buildname")))
                {
                    if (e.User.ServerPermissions.ManageServer)
                    {
                        builds.RemoveAll(b => b.buildName == e.GetArg("buildname"));
                        await e.Channel.SendMessage("Removed the build " + e.GetArg("buildname"));
                    }
                    else await e.Channel.SendMessage("You are not an admin");
                }
                else
                    await e.Channel.SendMessage("This build doesn't exist");
            });

            commands.CreateCommand("json").Do(async (e) =>
            {
                SaveJsonData();
                await e.Channel.SendMessage("Saved");
            });

            #endregion

            #endregion

            client.ExecuteAndWait(async () =>
            {
                //Old bot with old name
                //await client.Connect("MjY2OTQ5MjEzOTY5OTczMjU4.C1FJTQ.R5ioBOCezKto441jzESn2JOzZaw", TokenType.Bot);
                await client.Connect("MjczOTU3MTA3OTIzOTQzNDI0.C2rF-A.cdG2sZlXXz8d_qQsVerE5GiLmeQ", TokenType.Bot);
            });
        }

        private void LoadJsonData()
        {
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

        private void SaveJsonData()
        {
            var json = JsonConvert.SerializeObject(builds, Formatting.Indented);
            File.WriteAllText(filePath + "Builds.JSON", json);
        }

        //Returns a random skill of a certain type
        ActiveSkill GetRandomSkill(List<SkillType> types)
        {
            List<ActiveSkill> gems = new List<ActiveSkill>();
            ActiveSkill gem;

            foreach (ActiveSkill skill in skillGems)
            {
                if (types.Contains(skill.type))
                    gems.Add(skill);
            }

            Random r = new Random();
            gem = gems[r.Next(0, gems.Count)];
            return gem;
        }

        //Checks if the requested item exists on the wiki
        private string CheckIfExists(string item)
        {
            string itemName = "";
            string[] a = item.ToLower().Replace("'", "%27").Split(new char[] { ' ', '_', '=', '~', '.', '/', '|', '\'' });
            if (a.Length == 1)
                itemName += char.ToUpper(a[0][0]) + a[0].Substring(1);
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (i < a.Length - 1)
                    {
                        if (a[i].ToCharArray().Contains('-'))
                        {
                            string[] ab = a[i].Split('-');
                            itemName += char.ToUpper(ab[0][0]) + ab[0].Substring(1) + "-" + char.ToUpper(ab[1][0]) + ab[1].Substring(1) + "_";
                        }
                        else if (a[i].ToLower() == "and")
                        {
                            itemName += "and_";
                        }
                        else if (a[i].ToLower() == "of")
                        {
                            itemName += "of_";
                        }
                        else if (a[i].ToLower() == "the")
                        {
                            itemName += "the_";
                        }
                        else if (a[i].ToLower() == "when")
                        {
                            itemName += "when_";
                        }
                        else if (a[i] == "-")
                        {
                            itemName += "-";
                        }
                        else
                        {
                            itemName += char.ToUpper(a[i][0]) + a[i].Substring(1) + "_";
                        }
                    }
                    else
                    {
                        itemName += char.ToUpper(a[i][0]) + a[i].Substring(1);
                    }
                }
            }

            HttpWebResponse response;
            try
            {
                Console.WriteLine("checking if " + itemName + " exists");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://pathofexile.gamepedia.com/" + itemName);
                request.Proxy = null;
                request.Method = WebRequestMethods.Http.Head;
                response = (HttpWebResponse)request.GetResponse();
                bool pageExists = response.StatusCode == HttpStatusCode.OK;
                Console.WriteLine(itemName + " " + pageExists);

                response.Close();
                return "<http://pathofexile.gamepedia.com/" + itemName + ">";
            }
            catch (WebException)
            {
                Console.WriteLine("Exception");
                return "Item doesn't exist";
            }

        }

        //Returns a link to the requested item
        private string GetWikiLink(string item)
        {
            return "<http://pathofexile.gamepedia.com/" + item.Replace(" ", "_") + ">";
        }

        //Logs message to the console
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        //Joins a specific voiceChannel.
        private void JoinChannel(Channel channelToJoin)
        {
            var voiceChannels = client.FindServers("Big Boys").FirstOrDefault().VoiceChannels;

            foreach (var channel in voiceChannels)
            {
                if (channel == channelToJoin)
                    voiceChannel = channel;
            }
        }

        //Play audio files to the current Voice Channel
        void SendAudio(string filePath)
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

        //List with available ascendancies
        List<Ascendancy> FillAscendancyList()
        {
            List<Ascendancy> a = new List<Ascendancy>()
            {
                new Ascendancy("Slayer", BaseClass.Duelist, new List<SkillType>() {SkillType.Mines, SkillType.Minions, SkillType.Totems, SkillType.Traps }),
                new Ascendancy("Gladiator", BaseClass.Duelist),
                new Ascendancy("Champion", BaseClass.Duelist),
                new Ascendancy("Assasin",BaseClass.Shadow, new List<SkillType>() {SkillType.Minions }),
                new Ascendancy("Saboteur",BaseClass.Shadow, new List<SkillType>() {SkillType.Totems, SkillType.Minions }),
                new Ascendancy("Trickster", BaseClass.Shadow),
                new Ascendancy("Juggernaut", BaseClass.Marauder),
                new Ascendancy("Berserker", BaseClass.Marauder, new List<SkillType>() {SkillType.Minions }),
                new Ascendancy("Chieftain", BaseClass.Marauder),
                new Ascendancy("Necromancer", BaseClass.Witch),
                new Ascendancy("Elementalist",BaseClass.Witch, new List<SkillType>() {SkillType.Minions }),
                new Ascendancy("Occultist", BaseClass.Witch, new List<SkillType>() {SkillType.Minions }),
                new Ascendancy("Deadeye", BaseClass.Ranger, new List<SkillType>() {SkillType.MeleeAttack, SkillType.MeleeSpell, SkillType.Mines, SkillType.Minions, SkillType.Minions, SkillType.Totems, SkillType.Traps }),
                new Ascendancy("Raider", BaseClass.Ranger, new List<SkillType>() {SkillType.Minions }),
                new Ascendancy("Pathfinder", BaseClass.Ranger, new List<SkillType>() {SkillType.Minions }),
                new Ascendancy("Inquisitor", BaseClass.Templar, new List<SkillType>() {SkillType.Minions }),
                new Ascendancy("Hierophant", BaseClass.Templar, new List<SkillType>() {SkillType.Minions, SkillType.Traps, SkillType.Mines }),
                new Ascendancy("Guardian", BaseClass.Templar),
                new Ascendancy("Ascendant", BaseClass.Scion)
            };
            return a;
        }

        //Get a random keystone
        List<string> GetKeystones(int amount)
        {
            Random r = new Random();
            List<string> a = new List<string>();

            for (int i = 0; i < amount; i++)
            {
                string keystone = keyStones[r.Next(0, keyStones.Count)];
                if (!a.Contains(keystone))
                    a.Add(keystone);
                else
                {
                    i--;
                }
            }
            return a;
        }

        //Returns a list of skills gems
        private List<ActiveSkill> CoC(int amountOfSkills)
        {
            List<string> notValidSkills = notValidSkillList;

            List<ActiveSkill> a = new List<ActiveSkill>();
            Random r = new Random();
            int amount = (amountOfSkills < 1) ? 1 : (amountOfSkills > 4) ? 4 : amountOfSkills;

            for (int i = 0; i < amount; i++)
            {
                ActiveSkill activeSkill = skillGems[r.Next(0, skillGems.Count)];
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

        //Fills the list of skills
        List<ActiveSkill> FillSkillList()
        {
            List<ActiveSkill> a = new List<ActiveSkill>()
            {
                new ActiveSkill("Ancestral Protector", SkillType.Totems, 4),
                new ActiveSkill("Ancestral Warchief", SkillType.Totems, 28),
                new ActiveSkill("Cleave", SkillType.MeleeAttack, 1),
                new ActiveSkill("Dominating Blow", SkillType.MeleeAttack, 28),
                new ActiveSkill("Earthquake", SkillType.MeleeAttack, 28),
                new ActiveSkill("Flame Totem", SkillType.Totems, 4),
                new ActiveSkill("Glacial Hammer", SkillType.MeleeAttack, 1),
                new ActiveSkill("Ground Slam", SkillType.MeleeAttack, 1),
                new ActiveSkill("Heavy Strike", SkillType.MeleeAttack, 1),
                new ActiveSkill("Ice Crash", SkillType.MeleeAttack, 28),
                new ActiveSkill("Infernal Blow", SkillType.MeleeAttack, 1),
                new ActiveSkill("Leap Slam", SkillType.MeleeAttack, 10),
                new ActiveSkill("Molten Strike", SkillType.MeleeAttack, 1),
                new ActiveSkill("Searing Bond", SkillType.Totems, 12),
                new ActiveSkill("Shield Charge", SkillType.MeleeAttack, 10),
                new ActiveSkill("Shockwave Totem", SkillType.Totems, 28),
                new ActiveSkill("Static Strike", SkillType.MeleeAttack, 12),
                new ActiveSkill("Sunder", SkillType.MeleeAttack, 12),
                new ActiveSkill("Sweep", SkillType.MeleeAttack, 12),
                new ActiveSkill("Animate Weapon", SkillType.RangedSpell, 4),
                new ActiveSkill("Barrage", SkillType.RangedAttack, 12),
                new ActiveSkill("Bear Trap", SkillType.Traps, 4),
                new ActiveSkill("Blade Flurry", SkillType.MeleeAttack, 28),
                new ActiveSkill("Blade Vortex", SkillType.MeleeSpell, 12),
                new ActiveSkill("Bladefall", SkillType.RangedSpell, 28),
                new ActiveSkill("Blast Rain", SkillType.RangedAttack, 28),
                new ActiveSkill("Burning Arrow", SkillType.RangedAttack, 1),
                new ActiveSkill("Cyclone", SkillType.MeleeAttack, 28),
                new ActiveSkill("Detonate Dead", SkillType.RangedSpell, 4),
                new ActiveSkill("Double Strike", SkillType.MeleeAttack, 1),
                new ActiveSkill("Dual Strike", SkillType.MeleeAttack, 1),
                new ActiveSkill("Ethereal Knives", SkillType.RangedSpell, 1),
                new ActiveSkill("Explosive Arrow", SkillType.RangedAttack, 28),
                new ActiveSkill("Fire Trap", SkillType.Traps, 1),
                new ActiveSkill("Flicker Strike", SkillType.MeleeAttack, 10),
                new ActiveSkill("Freeze Mine", SkillType.Traps, 10),
                new ActiveSkill("Frenzy", SkillType.MeleeAttack, 16),
                new ActiveSkill("Frost Blades", SkillType.MeleeAttack, 1),
                new ActiveSkill("Ice Shot", SkillType.RangedAttack, 1),
                new ActiveSkill("Ice Trap", SkillType.Traps, 28),
                new ActiveSkill("Lacerate", SkillType.MeleeAttack, 12),
                new ActiveSkill("Lightning Arrow", SkillType.RangedAttack, 12),
                new ActiveSkill("Lightning Strike", SkillType.MeleeAttack, 12),
                new ActiveSkill("Blink Arrow", SkillType.Minions, 10),
                new ActiveSkill("Mirror Arrow", SkillType.Minions, 10),
                new ActiveSkill("Puncture", SkillType.RangedAttack, 4),
                new ActiveSkill("Rain of Arrows", SkillType.RangedAttack, 12),
                new ActiveSkill("Reave", SkillType.MeleeAttack, 12),
                new ActiveSkill("Shrapnel Shot", SkillType.RangedAttack, 1),
                new ActiveSkill("Siege Ballista", SkillType.Totems, 4),
                new ActiveSkill("Spectral Throw", SkillType.RangedAttack, 1),
                new ActiveSkill("Split Arrow", SkillType.RangedAttack, 1),
                new ActiveSkill("Tornado Shot", SkillType.RangedAttack, 28),
                new ActiveSkill("Viper Strike", SkillType.MeleeAttack, 1),
                new ActiveSkill("Wild Strike", SkillType.MeleeAttack, 28),
                new ActiveSkill("Arc", SkillType.RangedSpell, 12),
                new ActiveSkill("Arctic Breath", SkillType.RangedSpell, 28),
                new ActiveSkill("Ball Lightning", SkillType.RangedSpell, 28),
                new ActiveSkill("Blight", SkillType.MeleeSpell, 1),
                new ActiveSkill("Discharge", SkillType.MeleeSpell, 28),
                new ActiveSkill("Essence Drain", SkillType.RangedSpell, 12),
                new ActiveSkill("Fire Nova Mine", SkillType.Traps, 12),
                new ActiveSkill("Fireball", SkillType.RangedSpell, 1),
                new ActiveSkill("Firestorm", SkillType.RangedSpell, 12),
                new ActiveSkill("Flame Surge", SkillType.RangedSpell, 12),
                new ActiveSkill("Flameblast", SkillType.RangedSpell, 28),
                new ActiveSkill("Freezing Pulse", SkillType.RangedSpell, 1),
                new ActiveSkill("Frost Bomb", SkillType.RangedSpell, 4),
                new ActiveSkill("Frostbolt", SkillType.RangedSpell, 1),
                new ActiveSkill("Glacial Cascade", SkillType.RangedSpell, 28),
                new ActiveSkill("Ice Nova", SkillType.RangedSpell, 12),
                new ActiveSkill("Ice Spear", SkillType.RangedSpell, 12),
                new ActiveSkill("Incinerate", SkillType.RangedSpell, 12),
                new ActiveSkill("Kinetic Blast", SkillType.RangedSpell, 28),
                new ActiveSkill("Lightning Tendrils", SkillType.RangedSpell, 1),
                new ActiveSkill("Lightning Trap", SkillType.Traps, 12),
                new ActiveSkill("Magma Orb", SkillType.RangedSpell, 1),
                new ActiveSkill("Orb of Storms", SkillType.RangedSpell, 4),
                new ActiveSkill("Power Siphon", SkillType.RangedAttack, 12),
                new ActiveSkill("Raise Spectre", SkillType.Minions, 28),
                new ActiveSkill("Raise Zombie", SkillType.Minions, 1),
                new ActiveSkill("Righteous Fire", SkillType.MeleeSpell, 16),
                new ActiveSkill("Scorching Ray", SkillType.RangedSpell, 12),
                new ActiveSkill("Shock Nova", SkillType.RangedSpell, 28),
                new ActiveSkill("Spark", SkillType.RangedSpell, 1),
                new ActiveSkill("Storm Call", SkillType.RangedSpell, 12),
                new ActiveSkill("Summon Raging Spirit", SkillType.Minions, 4),
                new ActiveSkill("Summon Skeletons", SkillType.Minions, 10),
                new ActiveSkill("Vortex", SkillType.RangedSpell, 28),
                new ActiveSkill("Cast On Critical Strike Support", SkillType.RangedSpell, 38)
            };
            return a;
        }

        //List of non valid Cast on Crit skills
        #region Not valid CoC skills
        private List<string> notValidSkillList = new List<string>()
        {
            "Blade Flurry",
            "Blight",
            "Flameblast",
            "Incinerate",
            "Scorching Ray",
            "Wither",
            "Lightning Tendrils",
            "Righteous Fire",
            "Flame Surge",
            "Cast On Critical Strike Support"
        };
        #endregion

        //List of vaal skills
        #region Vaalskills
        public static List<string> vaalSkills = new List<string>()
        {
            "Glacial Hammer",
            "Ground Slam",
            "Burning Arrow",
            "Cyclone",
            "Detonate Dead",
            "Double Strike",
            "Lightning Strike",
            "Rain of Arrows",
            "Reave",
            "Spectral Throw",
            "Arc",
            "Cold Snap",
            "Fireball",
            "Flameblast",
            "Ice Nova",
            "Lightning Trap",
            "Power Siphon",
            "Righteous Fire",
            "Spark",
            "Storm Call",
            "Summon Skeletons"
        };
        #endregion

        //List of Keystones
        #region keyStones
        public static List<string> keyStones = new List<string>()
        {
            "Acrobatics",
            "Ancestral Bond",
            "Arrow Dancing",
            "Avatar of Fire",
            "Blood Magic",
            "Chaos Inoculation",
            "Conduit",
            "Eldritch Battery",
            "Elemental Equilibrium",
            "Elemental Overload",
            "Ghost Reaver",
            "Iron Grip",
            "Iron Reflexes",
            "Mind Over Matter",
            "Minion Instability",
            "Necromantic Aegis",
            "Pain Attunement",
            "Phase Acrobatics",
            "Point Blank",
            "Resolute Technique",
            "Unwavering Stance",
            "Vaal Pact",
            "Zealot's Oath"
        };
        #endregion
    }
}
