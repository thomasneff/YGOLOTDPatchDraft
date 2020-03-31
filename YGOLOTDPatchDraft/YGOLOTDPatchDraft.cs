using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace YGOPRODraft
{
	public partial class YGOLOTDPatchDraft : Form
	{
		#region Private Fields

		private static Random rng = new Random();
		private Dictionary<string, bool> ChosenDecks = new Dictionary<string, bool>();
		private Dictionary<string, bool> ChosenPacks = new Dictionary<string, bool>();
		private Constants CONSTANTS = new Constants();
		private Properties.Settings programSettings = Properties.Settings.Default;
		private TempSettings TEMP_SETTINGS = new TempSettings();
		private bool dont_update_list = false;

		#endregion Private Fields

		#region Public Constructors

		public YGOLOTDPatchDraft()
		{
			InitializeComponent();
		}

		#endregion Public Constructors

		#region Private Methods

		private void btnCopyToGameDirectory_Click(object sender, EventArgs e)
		{
			if (!TEMP_SETTINGS.CanCopyToGame)
			{
				LogOut("Error: Can not copy game files - make sure " + CONSTANTS.LOTD_DAT_FILENAME + " and " + CONSTANTS.LOTD_TOC_FILENAME + " exist!");
				return;
			}

			LogOut("Copying game files, please be patient... (I hope you have a backup, lol)", Color.Blue);

			//Copy it to the game installations folder
			File.Copy(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.LOTD_DAT_FILENAME),
				Path.Combine(programSettings.LOTDPath, CONSTANTS.LOTD_DAT_FILENAME), true);
			//Copy it to the game installations folder
			File.Copy(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.LOTD_TOC_FILENAME),
				Path.Combine(programSettings.LOTDPath, CONSTANTS.LOTD_TOC_FILENAME), true);

			LogOut("Successfully copied game files!");
		}

		private void btnExtractGameFiles_Click(object sender, EventArgs e)
		{
			if (!TEMP_SETTINGS.CanExtractGame)
			{
				CheckSettings();
				LogOut("Error: Can not extract game files - try again and choose a correct YGO: LOTD installation path!");
				return;
			}

			LogOut("Extracting... please be patient!", Color.Blue);
			LogOut(Onomatopaira.ExtractYGODATA(CONSTANTS));
			TEMP_SETTINGS.CanUnpackPacks = true;
			TEMP_SETTINGS.CanUnpackDecks = true;
			TEMP_SETTINGS.CanPackGame = true;
		}

		private void btnExtractSaveGameDeck_Click(object sender, EventArgs e)
		{
			if (!TEMP_SETTINGS.CanExtractSave)
			{
				CheckSavePath();
			}

			if (!TEMP_SETTINGS.CanExtractSave)
			{
				LogOut("Error: Can't find savegame.dat in your selected path - try again!");
				return;
			}

			byte[] savegame = File.ReadAllBytes(Path.Combine(programSettings.LOTDSavePath));
			byte[] ydc_bytes = new byte[0];
			try
			{
				ydc_bytes = FileUtilities.ExtractBattlePack1FromSaveData(savegame);
			}
			catch (Exception ex)
			{
				LogOut(ex.Message);
			}

			if (ydc_bytes.Length == 0)
			{
				LogOut("Error: Something went wrong while reading the draft deck from savegame.dat, sorry!");
				return;
			}

			string deck_name = CONSTANTS.EXTRACT_DECK_PREFIX + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + CONSTANTS.YDC_EXTENSION;
			File.WriteAllBytes(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, deck_name), ydc_bytes);
			File.WriteAllBytes(Path.Combine(CONSTANTS.DECK_DATABASE, deck_name), ydc_bytes);

			LogOut("Successfully added the extracted deck " + deck_name + " to the deck database! (" + CONSTANTS.DECK_DATABASE + ")", Color.Blue);
		}

		private void btnPackGame_Click(object sender, EventArgs e)
		{
			if (!TEMP_SETTINGS.CanPackGame)
			{
				LogOut("Error: Can not pack game - make sure the YGO_DATA folder exists!");
				return;
			}

			LogOut("Packing game, please be patient...", Color.Blue);
			LogOut(Vortex.PackGame(CONSTANTS));

			TEMP_SETTINGS.CanCopyToGame = true;
		}

		private void btnPatchAIDraftDecks_Click(object sender, EventArgs e)
		{
			if (!TEMP_SETTINGS.CanPatchDecks)
			{
				LogOut("Error: Can not pack decks folder - unpack decks.zib again!");
				return;
			}

			Random rng = new Random();
			//Load all decks in DECK_DATABASE
			List<List<List<YGOPROCard>>> list_of_all_decks_ydk;
			List<byte[]> list_of_ydc_decks;

			//Get lists of random decks
			FileUtilities.GetRandomDecksFromFolder(CONSTANTS.DECK_DATABASE, CONSTANTS.CARD_DB_FILENAME, CONSTANTS.CARDS_NOT_AVAILABLE,
				CONSTANTS.MAX_AI_DRAFT_INDEX * 2, CONSTANTS.YDK_EXTENSION,
				CONSTANTS.YDC_EXTENSION, ChosenDecks, out list_of_all_decks_ydk, out list_of_ydc_decks);

			int max_num_decks = CONSTANTS.MAX_AI_DRAFT_INDEX * 2;
			//We now have all the decks. Now patch 20 decks.
			int number_of_decks = (list_of_all_decks_ydk.Count + list_of_ydc_decks.Count);

			if (number_of_decks == 0)
			{
				LogOut("Error: No decks found! Please add some decks to patch AI Draft decks!");
				return;
			}

			if (number_of_decks < CONSTANTS.MAX_AI_DRAFT_INDEX * 2)
			{
				LogOut("Note: Less than 20 decks detected, will repeat some!", Color.Magenta);
				int diff = CONSTANTS.MAX_AI_DRAFT_INDEX * 2 - number_of_decks;
				if (list_of_all_decks_ydk.Count != 0)
				{
					for (int i = 0; i < diff; i++)
					{
						list_of_all_decks_ydk.Add(list_of_all_decks_ydk[rng.Next(list_of_all_decks_ydk.Count)]);
					}
				}
				if (list_of_ydc_decks.Count != 0)
				{
					for (int i = 0; i < diff; i++)
					{
						list_of_ydc_decks.Add(list_of_ydc_decks[rng.Next(list_of_ydc_decks.Count)]);
					}
				}
			}

			var card_name_to_LOTD_ID = FileUtilities.GetCardIDToLOTDMapFromCSV(CONSTANTS.CSV_MAP_FILENAME);

			for (int deck_idx = CONSTANTS.MIN_AI_DRAFT_INDEX - 1; deck_idx < CONSTANTS.MAX_AI_DRAFT_INDEX; deck_idx++)
			{
				int random_index = 2 * deck_idx;
				string bpack_deck_name = CONSTANTS.AI_DECK_DRAFT_FILE_EU + (deck_idx + 1).ToString("D3") + CONSTANTS.YDC_EXTENSION;

				if (random_index < list_of_all_decks_ydk.Count)
				{
					//Take YDK Deck
					int list_idx = random_index;

					LogOut(FileUtilities.WriteYDCDeckFile(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, bpack_deck_name),
						list_of_all_decks_ydk[list_idx], card_name_to_LOTD_ID));
				}
				else
				{
					//Take YDC Deck
					int list_idx = random_index - list_of_all_decks_ydk.Count;

					//Write raw binary
					File.WriteAllBytes(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, bpack_deck_name), list_of_ydc_decks[list_idx]);
				}

				random_index = 2 * deck_idx + 1;
				bpack_deck_name = CONSTANTS.AI_DECK_DRAFT_FILE_US + (deck_idx + 1).ToString("D3") + CONSTANTS.YDC_EXTENSION;
				string sealed_deck_name = CONSTANTS.AI_SEALED_DECK_FILE + (deck_idx + 1).ToString("D3") + CONSTANTS.YDC_EXTENSION;

				if (random_index < list_of_all_decks_ydk.Count)
				{
					//Take YDK Deck
					int list_idx = random_index;

					LogOut(FileUtilities.WriteYDCDeckFile(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, bpack_deck_name),
						list_of_all_decks_ydk[list_idx], card_name_to_LOTD_ID));

					LogOut(FileUtilities.WriteYDCDeckFile(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, sealed_deck_name),
						list_of_all_decks_ydk[list_idx], card_name_to_LOTD_ID));
				}
				else
				{
					//Take YDC Deck
					int list_idx = random_index - list_of_all_decks_ydk.Count;

					File.WriteAllBytes(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, bpack_deck_name), list_of_ydc_decks[list_idx]);

					File.WriteAllBytes(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, sealed_deck_name), list_of_ydc_decks[list_idx]);
				}
			}

			//At this point, we should have all decks in our working folder.

			//Now we copy them to the unpacked folder.
			for (int deck_idx = CONSTANTS.MIN_AI_DRAFT_INDEX - 1; deck_idx < CONSTANTS.MAX_AI_DRAFT_INDEX; deck_idx++)
			{
				string bpack_deck_name = CONSTANTS.AI_DECK_DRAFT_FILE_EU + (deck_idx + 1).ToString("D3") + CONSTANTS.YDC_EXTENSION;
				File.Copy(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, bpack_deck_name),
					Path.Combine(CONSTANTS.YGODATA_DECKS + CONSTANTS.UNPACKED_SUFFIX, bpack_deck_name), true);

				bpack_deck_name = CONSTANTS.AI_DECK_DRAFT_FILE_US + (deck_idx + 1).ToString("D3") + CONSTANTS.YDC_EXTENSION;
				File.Copy(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, bpack_deck_name),
					Path.Combine(CONSTANTS.YGODATA_DECKS + CONSTANTS.UNPACKED_SUFFIX, bpack_deck_name), true);

				string sealed_deck_name = CONSTANTS.AI_SEALED_DECK_FILE + (deck_idx + 1).ToString("D3") + CONSTANTS.YDC_EXTENSION;
				File.Copy(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, sealed_deck_name),
					Path.Combine(CONSTANTS.YGODATA_DECKS + CONSTANTS.UNPACKED_SUFFIX, sealed_deck_name), true);
			}

			LogOut("Copied shuffled draft/sealed decks to working folder!", Color.Blue);

			//Pack decks.zib
			LogOut(Cyclone.PackZibFile(CONSTANTS.YGODATA_DECKS + CONSTANTS.UNPACKED_SUFFIX,
			Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.YGODATA_DECKS)));

			//Copy it to the YGODATA folder
			File.Copy(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.YGODATA_DECKS),
				Path.Combine(CONSTANTS.YGO_DATA_WORKING_FOLDER, CONSTANTS.YGODATA_DECKS), true);

			LogOut("Copied " + CONSTANTS.YGODATA_DECKS + " to " + CONSTANTS.YGO_DATA_WORKING_FOLDER + ".", Color.Blue);
		}

		private void btnPatchBattlePack_Click(object sender, EventArgs e)
		{
			if (!TEMP_SETTINGS.CanPatchPacks)
			{
				LogOut("Error: Can not pack packs folder - unpack packs.zib again!");
				return;
			}

			//Read the 5 .ydk decks, patch the file, pack packs.zib again, copy it to working directory
			LogOut("Reading .json/.ydk files, please be patient...", Color.Blue);
			string[] files = Directory.GetFiles(CONSTANTS.ADD_PACKS_FOLDER);
			List<List<YGOPROCard>> list_of_card_lists = new List<List<YGOPROCard>>();

			var card_name_to_LOTD_ID = FileUtilities.GetCardIDToLOTDMapFromCSV(CONSTANTS.CSV_MAP_FILENAME);
			var LOTD_ID_to_card_name = FileUtilities.ReverseDict(card_name_to_LOTD_ID);

			foreach (string filename in files)
			{
				if (ChosenPacks[filename.Split('\\')[1]] == false)
				{
					continue;
				}

				List<YGOPROCard> cards = new List<YGOPROCard>();
				if (filename.Contains(CONSTANTS.YDK_EXTENSION))
				{
					cards = FileUtilities.parseCardListFromYDKFile(filename, CONSTANTS.CARD_DB_FILENAME, CONSTANTS.CARDS_NOT_AVAILABLE);
				}
				else if (filename.Contains(CONSTANTS.JSON_EXTENSION))
				{
					cards = FileUtilities.cardsFromJSON(filename, CONSTANTS.CARD_DB_FILENAME, chkJSONRarity.Checked);
				}
				else if (filename.Contains(CONSTANTS.YDC_EXTENSION))
				{
					try
					{
						byte[] ydc_binary = FileUtilities.parseCardListMainExtraSideFromYDCFile(filename);
						var list_of_main_extra_side = FileUtilities.YDCToYGOPRODeck(ydc_binary, LOTD_ID_to_card_name,
							CONSTANTS.CARD_DB_FILENAME);
						foreach (var deck in list_of_main_extra_side)
						{
							cards.AddRange(deck);
						}
					}
					catch (Exception ex)
					{
						LogOut("Error: couldn't read " + filename + " for pack data!");
						continue;
					}
				}
				else
				{
					LogOut("Error: didn't recognize extension of " + filename + " as a valid draft pack file!");
					continue;
				}

				list_of_card_lists.Add(cards);
			}

			if (list_of_card_lists.Count == 0)
			{
				LogOut("Error: no packs selected - please select at least 1!");
				return;
			}


			LogOut(FileUtilities.WriteBattlePackBinFile(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.BATTLEPACK_1_FILENAME),
				CONSTANTS.BATTLEPACK_NUM_CATEGORIES, list_of_card_lists, card_name_to_LOTD_ID));

			LogOut("Created " + Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.BATTLEPACK_1_FILENAME) + " successfully!");
			//Copy it to the working folder
			File.Copy(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.BATTLEPACK_1_FILENAME),
				Path.Combine(CONSTANTS.YGODATA_PACKS + CONSTANTS.UNPACKED_SUFFIX, CONSTANTS.BATTLEPACK_1_FILENAME), true);

			LogOut("Copied the battlepack to the unpacked folder.", Color.Blue);

			LogOut(Cyclone.PackZibFile(CONSTANTS.YGODATA_PACKS + CONSTANTS.UNPACKED_SUFFIX,
				Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.YGODATA_PACKS)));

			//Copy it to the YGODATA folder
			File.Copy(Path.Combine(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.YGODATA_PACKS),
				Path.Combine(CONSTANTS.YGO_DATA_WORKING_FOLDER, CONSTANTS.YGODATA_PACKS), true);

			LogOut("Copied " + CONSTANTS.YGODATA_PACKS + " to " + CONSTANTS.YGO_DATA_WORKING_FOLDER + ".", Color.Blue);
		}

		//private void add
		private void btnPatchPackCopy_Click(object sender, EventArgs e)
		{
			btnPatchAIDraftDecks_Click(sender, e);
			btnPatchBattlePack_Click(sender, e);
			btnPackGame_Click(sender, e);
			btnCopyToGameDirectory_Click(sender, e);
		}

		private void btnUnpackDecksAndPacks_Click(object sender, EventArgs e)
		{
			if (!TEMP_SETTINGS.CanUnpackDecks || !TEMP_SETTINGS.CanUnpackPacks)
			{
				LogOut("Error: Can not unpack decks/packs - extract YGO: LOTD game files again!");
				return;
			}

			TEMP_SETTINGS.CanPatchDecks = true;
			TEMP_SETTINGS.CanPatchPacks = true;

			LogOut(Relinquished.UnpackDecksAndPacks(CONSTANTS));
		}

		private bool CheckPathContainsFile(string path, string file)
		{
			return File.Exists(Path.Combine(path, file));
		}

		private void CheckSavePath()
		{
			//Check if SavePath is set AND the LOTD savegame.dat can be found.
			if (programSettings.LOTDSavePath == ""
				|| !File.Exists(programSettings.LOTDSavePath))
			{
				//Path is wrong or not set, ask for re-enter
				OpenFileDialog fbd = new OpenFileDialog();
				fbd.InitialDirectory = Environment.CurrentDirectory;
				fbd.Title = "Choose your Yu-Gi-Oh: Legacy of the Duelist savegame.dat! (Should be in Steam userdata/remote.)";
				fbd.ShowDialog();
				programSettings.LOTDSavePath = fbd.FileName;
				if (programSettings.LOTDSavePath == ""
				|| !CheckPathContainsFile(programSettings.LOTDSavePath, CONSTANTS.LOTD_SAVE_FILENAME))
				{
					TEMP_SETTINGS.CanExtractSave = true;
				}
			}
			else
			{
				LogOut("Yu-Gi-Oh: LOTD Save-Path found: " + programSettings.LOTDPath);
				TEMP_SETTINGS.CanExtractSave = true;
			}
			programSettings.Save();
		}

		private void CheckSettings()
		{
			//Check if LOTDPath is set AND the LOTD .exe / .dat / .toc can be found.
			if (programSettings.LOTDPath == ""
				|| !CheckPathContainsFile(programSettings.LOTDPath, CONSTANTS.LOTD_DAT_FILENAME)
				|| !CheckPathContainsFile(programSettings.LOTDPath, CONSTANTS.LOTD_TOC_FILENAME))
			{
				//Path is wrong or not set, ask for re-enter
				FolderBrowserDialog fbd = new FolderBrowserDialog();
				fbd.SelectedPath = Environment.CurrentDirectory;
				fbd.Description = "Choose your Yu-Gi-Oh: Legacy of the Duelist Link Evolution installation path! (Contains YuGiOh.exe, YGO_2020.dat and YGO_2020.toc)";
				fbd.ShowDialog();
				programSettings.LOTDPath = fbd.SelectedPath;
				if (programSettings.LOTDPath == ""
				|| !CheckPathContainsFile(programSettings.LOTDPath, CONSTANTS.LOTD_DAT_FILENAME)
				|| !CheckPathContainsFile(programSettings.LOTDPath, CONSTANTS.LOTD_TOC_FILENAME))
				{
					TEMP_SETTINGS.CanExtractGame = true;
				}
			}
			else
			{
				LogOut("Yu-Gi-Oh: LOTD Path found: " + programSettings.LOTDPath);
				TEMP_SETTINGS.CanExtractGame = true;
			}
			programSettings.Save();

			//Check if SavePath is set AND the LOTD savegame.dat can be found.
			if (programSettings.LOTDSavePath == ""
				|| !File.Exists(programSettings.LOTDSavePath))
			{
			}
			else
			{
				LogOut("Yu-Gi-Oh: LOTD Save-Path found: " + programSettings.LOTDPath);
				TEMP_SETTINGS.CanExtractSave = true;
			}

			//Check if card_map.csv is there
			if (!CheckPathContainsFile("", CONSTANTS.CSV_MAP_FILENAME))
			{
				MessageBox.Show("Error: " + CONSTANTS.CSV_MAP_FILENAME + " is missing, but we definitely need that :(\nRedownload this tool, as that file should come with it!");
				Application.Exit();
			}
			else
			{
				LogOut(CONSTANTS.CSV_MAP_FILENAME + " found!");
			}

			//Check if cards.cdb is there
			if (!CheckPathContainsFile("", CONSTANTS.CARD_DB_FILENAME))
			{
				MessageBox.Show("Error: " + CONSTANTS.CARD_DB_FILENAME + " is missing, but we definitely need that :(\nGet it from YGOPRO/redownload this tool and put it in the same folder as this program.");
				Application.Exit();
			}
			else
			{
				LogOut(CONSTANTS.CARD_DB_FILENAME + " found!");
			}
			
			//Check if PUT_YOUR_DECKS_HERE folder is there, otherwise create it
			if (!Directory.Exists(CONSTANTS.ADD_PACKS_FOLDER))
			{
				Directory.CreateDirectory(CONSTANTS.ADD_PACKS_FOLDER);
				LogOut("Created folder: " + CONSTANTS.ADD_PACKS_FOLDER);
			}
			else
			{
				LogOut("Folder " + CONSTANTS.ADD_PACKS_FOLDER + " found!");
			}

			//Check if DECK_DATABASE folder is there, otherwise create it
			if (!Directory.Exists(CONSTANTS.DECK_DATABASE))
			{
				Directory.CreateDirectory(CONSTANTS.DECK_DATABASE);
				LogOut("Created folder: " + CONSTANTS.DECK_DATABASE + " (Make sure that you put some .ydk/.ydc decks in there for AI deck shuffle!)");
			}
			else
			{
				LogOut("Folder " + CONSTANTS.DECK_DATABASE + " found!");
			}

			//Check if YGO_DATA working folder is there
			if (!Directory.Exists(CONSTANTS.YGO_DATA_WORKING_FOLDER))
			{
				//Directory.CreateDirectory(CONST_STRINGS.YGO_DATA_WORKING_FOLDER);
				LogOut("Error: Folder: " + CONSTANTS.YGO_DATA_WORKING_FOLDER + " not found - extract game data first!");
			}
			else
			{
				LogOut("Folder " + CONSTANTS.YGO_DATA_WORKING_FOLDER + " found!");
			}

			//Check if YGO_DATA working folder contains packs.zib
			if (!CheckPathContainsFile(CONSTANTS.YGO_DATA_WORKING_FOLDER, CONSTANTS.YGODATA_PACKS))
			{
				//Directory.CreateDirectory(CONST_STRINGS.YGO_DATA_WORKING_FOLDER);
				LogOut("Error: File: " + Path.Combine(CONSTANTS.YGO_DATA_WORKING_FOLDER, CONSTANTS.YGODATA_PACKS) + " not found - extract game data first!");
			}
			else
			{
				TEMP_SETTINGS.CanUnpackPacks = true;
				LogOut("File: " + Path.Combine(CONSTANTS.YGO_DATA_WORKING_FOLDER, CONSTANTS.YGODATA_PACKS) + " found!");
			}

			//Check if YGO_DATA working folder contains decks.zib
			if (!CheckPathContainsFile(CONSTANTS.YGO_DATA_WORKING_FOLDER, CONSTANTS.YGODATA_DECKS))
			{
				//Directory.CreateDirectory(CONST_STRINGS.YGO_DATA_WORKING_FOLDER);
				LogOut("Error: File: " + Path.Combine(CONSTANTS.YGO_DATA_WORKING_FOLDER, CONSTANTS.YGODATA_DECKS) + " not found - extract game data first!");
			}
			else
			{
				TEMP_SETTINGS.CanUnpackDecks = true;
				LogOut("File " + Path.Combine(CONSTANTS.YGO_DATA_WORKING_FOLDER, CONSTANTS.YGODATA_DECKS) + " found!");
			}

			if (TEMP_SETTINGS.CanUnpackDecks && TEMP_SETTINGS.CanUnpackPacks)
			{
				TEMP_SETTINGS.CanPackGame = true;
			}

			//Check if packs.zib working folder is there
			if (!Directory.Exists(CONSTANTS.YGODATA_PACKS + CONSTANTS.UNPACKED_SUFFIX))
			{
				//Directory.CreateDirectory(CONST_STRINGS.YGO_DATA_WORKING_FOLDER);
				LogOut("Error: Folder: " + CONSTANTS.YGODATA_PACKS + CONSTANTS.UNPACKED_SUFFIX + " not found - unpack " + CONSTANTS.YGODATA_PACKS + " first!");
			}
			else
			{
				TEMP_SETTINGS.CanPatchPacks = true;
				LogOut("Folder " + CONSTANTS.YGODATA_PACKS + CONSTANTS.UNPACKED_SUFFIX + " found!");
			}

			//Check if decks.zib working folder is there
			if (!Directory.Exists(CONSTANTS.YGODATA_DECKS + CONSTANTS.UNPACKED_SUFFIX))
			{
				//Directory.CreateDirectory(CONST_STRINGS.YGO_DATA_WORKING_FOLDER);

				LogOut("Error: Folder: " + CONSTANTS.YGODATA_DECKS + CONSTANTS.UNPACKED_SUFFIX + " not found - unpack " + CONSTANTS.YGODATA_DECKS + " first!");
			}
			else
			{
				TEMP_SETTINGS.CanPatchDecks = true;
				LogOut("Folder " + CONSTANTS.YGODATA_DECKS + CONSTANTS.UNPACKED_SUFFIX + " found!");
			}
			//Check if patched output working folder is there, otherwise create it
			if (!Directory.Exists(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER))
			{
				Directory.CreateDirectory(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER);
				LogOut("Created folder: " + CONSTANTS.PATCHED_YGODATA_OUT_FOLDER);
			}
			else
			{
				LogOut("Folder " + CONSTANTS.PATCHED_YGODATA_OUT_FOLDER + " found!");
			}

			//Check if patched output working folder is there, otherwise create it
			if (!CheckPathContainsFile(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.LOTD_DAT_FILENAME)
				|| !CheckPathContainsFile(CONSTANTS.PATCHED_YGODATA_OUT_FOLDER, CONSTANTS.LOTD_TOC_FILENAME))
			{
				//Directory.CreateDirectory(CONST_STRINGS.YGO_DATA_WORKING_FOLDER);
				LogOut("Error: Folder/Patched data files in: " + CONSTANTS.PATCHED_YGODATA_OUT_FOLDER + " not found - patch the modified files first!");
			}
			else
			{
				TEMP_SETTINGS.CanCopyToGame = true;
				LogOut("Patched deck/pack files in " + CONSTANTS.PATCHED_YGODATA_OUT_FOLDER + " found!");
			}

			//Check ChosenPacks/ChosenDecks
			if (programSettings.ChosenPacksDictionaryJSON != "")
			{
				ChosenPacks = JsonConvert.DeserializeObject<Dictionary<string, bool>>(programSettings.ChosenPacksDictionaryJSON);
			}

			UpdateChosenList(chkListBoxPacks, ChosenPacks, CONSTANTS.ADD_PACKS_FOLDER, "", false);
			programSettings.ChosenPacksDictionaryJSON = JsonConvert.SerializeObject(ChosenPacks);

			//Check ChosenPacks/ChosenDecks
			if (programSettings.ChosenDecksDictionaryJSON != "")
			{
				ChosenDecks = JsonConvert.DeserializeObject<Dictionary<string, bool>>(programSettings.ChosenDecksDictionaryJSON);
			}

			UpdateChosenList(chkListBoxDecks, ChosenDecks, CONSTANTS.DECK_DATABASE, "", false);
			programSettings.ChosenDecksDictionaryJSON = JsonConvert.SerializeObject(ChosenDecks);

			chkOnlyShowChosenDecks.Checked = programSettings.ShowOnlyCheckedDecks;
			chkOnlyShowChosenPacks.Checked = programSettings.ShowOnlyCheckedPacks;
			chkJSONRarity.Checked = programSettings.SimulateRarity;

		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			programSettings.Save();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			CheckSettings();
		}

		private void LogOut(string text, Color? c = null)
		{
			if (text == "")
			{
				return;
			}

			if (text.ToLower().Contains("error"))
			{
				c = Color.Red;
			}
			else if (c == null)
			{
				c = Color.Green;
			}

			txtDebugOut.SelectionStart = txtDebugOut.TextLength;
			txtDebugOut.SelectionLength = 0;

			txtDebugOut.SelectionColor = (Color)c;
			if (!text.EndsWith(Environment.NewLine))
			{
				txtDebugOut.AppendText(text + "\n");
			}
			else
			{
				txtDebugOut.AppendText(text);
			}

			txtDebugOut.SelectionColor = txtDebugOut.ForeColor;
			txtDebugOut.Update();
		}

		/// <summary>
		/// Updates a CheckedListBox based on items in a folder and a dictionary. Also updates the dictonary.
		/// </summary>
		/// <param name="clb">CheckedListBox to update</param>
		/// <param name="checked_items">Dictionary containing checked/unchecked items</param>
		/// <param name="folder_path">Path to query files from</param>
		/// <param name="filter_string">Only filenames matching the filter_string are added</param>
		/// <param name="only_show_chosen">Only show files which are ticked</param>
		private void UpdateChosenList(CheckedListBox clb, Dictionary<string, bool> checked_items, string folder_path, string filter_string, bool only_show_chosen)
		{
			clb.Enabled = false;
			clb.Hide();
			string[] files = Directory.GetFiles(folder_path);

			int old_index = clb.SelectedIndex;

			string old_item = "";
			string old_top_item = "";

			if (clb.SelectedItem != null)
				old_item = clb.SelectedItem.ToString();

			if (clb.Items.Count != 0)
				old_top_item = clb.Items[clb.TopIndex].ToString();



			clb.Items.Clear();

			List<string> filtered_filenames = new List<string>();

			//Filter queried files
			if (filter_string != "")
			{
				foreach (string file in files)
				{
					if (file.ToLower().Contains(filter_string.ToLower()))
					{
						filtered_filenames.Add(file);
					}
				}
			}
			else
			{
				filtered_filenames.AddRange(files);
			}

			// We change the selected index here without wanting to fire the event handler again
			dont_update_list = true;
			int new_top_index = 0;

			//For each item, add the checked state if found in Dictionary
			foreach (string file in filtered_filenames)
			{
				string file_split = file.Split('\\')[1];
				if (checked_items.ContainsKey(file_split))
				{
					if (only_show_chosen)
					{
						if (checked_items[file_split] == true)
						{
							clb.Items.Add(file_split, checked_items[file_split]);
							if (file_split == old_item)
							{
								clb.SelectedIndex = clb.Items.Count - 1;
							}

							if (file_split == old_top_item)
							{
								new_top_index = clb.Items.Count - 1;
							}
						}
					}
					else
					{
						clb.Items.Add(file_split, checked_items[file_split]);
						if (file_split == old_item)
						{
							clb.SelectedIndex = clb.Items.Count - 1;
						}

						if (file_split == old_top_item)
						{
							new_top_index = clb.Items.Count - 1;
						}
					}
					
				}
				else
				{
					//Dictionary entry is not there -> file is new. Add entry
					checked_items.Add(file_split, false);
					if (only_show_chosen == false)
					{
						clb.Items.Add(file_split, false);
						
					}
				}

				
			}
			//	clb.Update();
			dont_update_list = true;
			clb.TopIndex = new_top_index;
			dont_update_list = false;
			clb.Enabled = true;
			clb.Show();
		}

		#endregion Private Methods

		#region Public Classes

		public class TempSettings
		{
			#region Public Fields

			public bool CanCopyToGame = false;
			public bool CanExtractGame = false;
			public bool CanExtractSave = false;
			public bool CanPackGame = false;
			public bool CanPatchDecks = false;
			public bool CanPatchPacks = false;
			public bool CanUnpackDecks = false;
			public bool CanUnpackPacks = false;

			#endregion Public Fields
		}

		#endregion Public Classes

		private void chkAllDecks_CheckedChanged(object sender, EventArgs e)
		{
			var keys = ChosenDecks.Keys.ToList();
			foreach (var key in keys)
			{
				if (key.ToLower().Contains(txtFilterDecks.Text.ToLower()))
				{
					ChosenDecks[key] = chkAllDecks.Checked;
				}
			}

			UpdateChosenList(chkListBoxDecks, ChosenDecks, CONSTANTS.DECK_DATABASE, txtFilterDecks.Text, chkOnlyShowChosenDecks.Checked);

			//Save programSettings
			programSettings.ChosenDecksDictionaryJSON = JsonConvert.SerializeObject(ChosenDecks);
			programSettings.Save();
		}

		private void chkAllPacks_CheckedChanged(object sender, EventArgs e)
		{
			var keys = ChosenPacks.Keys.ToList();
			foreach (var key in keys)
			{
				if (key.ToLower().Contains(txtFilterPacks.Text.ToLower()))
				{
					ChosenPacks[key] = chkAllPacks.Checked;
				}
			}

			UpdateChosenList(chkListBoxPacks, ChosenPacks, CONSTANTS.ADD_PACKS_FOLDER, txtFilterPacks.Text, chkOnlyShowChosenPacks.Checked);

			//Save programSettings
			programSettings.ChosenPacksDictionaryJSON = JsonConvert.SerializeObject(ChosenPacks);
			programSettings.Save();
		}

		private void chkJSONRarity_CheckedChanged(object sender, EventArgs e)
		{
			programSettings.SimulateRarity = chkJSONRarity.Checked;
			programSettings.Save();
		}

		private void chkListBoxDecks_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (dont_update_list == true)
			{
				dont_update_list = false;
				return;
			}

			if (chkListBoxDecks.SelectedItem == null)
			{
				return;
			}
			//Add change to dictionary
			ChosenDecks[chkListBoxDecks.SelectedItem.ToString()] = chkListBoxDecks.GetItemChecked(chkListBoxDecks.SelectedIndex);

			UpdateChosenList(chkListBoxDecks, ChosenDecks, CONSTANTS.DECK_DATABASE, txtFilterDecks.Text, chkOnlyShowChosenDecks.Checked);

			//Save programSettings
			programSettings.ChosenDecksDictionaryJSON = JsonConvert.SerializeObject(ChosenDecks);
			programSettings.Save();
		}

		private void chkListBoxPacks_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (dont_update_list == true)
			{
				dont_update_list = false;
				return;
			}

			if (chkListBoxPacks.SelectedItem == null)
			{
				return;
			}
			//Add change to dictionary
			ChosenPacks[chkListBoxPacks.SelectedItem.ToString()] = chkListBoxPacks.GetItemChecked(chkListBoxPacks.SelectedIndex);

			UpdateChosenList(chkListBoxPacks, ChosenPacks, CONSTANTS.ADD_PACKS_FOLDER, txtFilterPacks.Text, chkOnlyShowChosenPacks.Checked);

			//Save programSettings
			programSettings.ChosenPacksDictionaryJSON = JsonConvert.SerializeObject(ChosenPacks);
			programSettings.Save();
		}

		private void chkOnlyShowChosenDecks_CheckedChanged(object sender, EventArgs e)
		{
			UpdateChosenList(chkListBoxDecks, ChosenDecks, CONSTANTS.DECK_DATABASE, txtFilterDecks.Text, chkOnlyShowChosenDecks.Checked);
			programSettings.ShowOnlyCheckedDecks = chkOnlyShowChosenDecks.Checked;
			programSettings.Save();
		}

		private void chkOnlyShowChosenPacks_CheckedChanged(object sender, EventArgs e)
		{
			UpdateChosenList(chkListBoxPacks, ChosenPacks, CONSTANTS.ADD_PACKS_FOLDER, txtFilterPacks.Text, chkOnlyShowChosenPacks.Checked);
			programSettings.ShowOnlyCheckedPacks = chkOnlyShowChosenPacks.Checked;
			programSettings.Save();
		}

		private void txtFilterDecks_TextChanged(object sender, EventArgs e)
		{
			UpdateChosenList(chkListBoxDecks, ChosenDecks, CONSTANTS.DECK_DATABASE, txtFilterDecks.Text, chkOnlyShowChosenDecks.Checked);
		}

		private void txtFilterPacks_TextChanged(object sender, EventArgs e)
		{
			UpdateChosenList(chkListBoxPacks, ChosenPacks, CONSTANTS.ADD_PACKS_FOLDER, txtFilterPacks.Text, chkOnlyShowChosenPacks.Checked);
		}
	}
}

public static class Extensions
{
	#region Public Fields

	public static Random rng = new Random();

	#endregion Public Fields

	#region Private Fields

	private static readonly int[] Empty = new int[0];

	#endregion Private Fields

	#region Public Methods

	public static int[] Locate(this byte[] self, byte[] candidate)
	{
		if (IsEmptyLocate(self, candidate))
			return Empty;

		var list = new List<int>();

		for (int i = 0; i < self.Length; i++)
		{
			if (!IsMatch(self, i, candidate))
				continue;

			list.Add(i);
		}

		return list.Count == 0 ? Empty : list.ToArray();
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	#endregion Public Methods

	#region Private Methods

	private static bool IsEmptyLocate(byte[] array, byte[] candidate)
	{
		return array == null
			|| candidate == null
			|| array.Length == 0
			|| candidate.Length == 0
			|| candidate.Length > array.Length;
	}

	private static bool IsMatch(byte[] array, int position, byte[] candidate)
	{
		if (candidate.Length > (array.Length - position))
			return false;

		for (int i = 0; i < candidate.Length; i++)
			if (array[position + i] != candidate[i])
				return false;

		return true;
	}

	#endregion Private Methods
}