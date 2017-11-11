using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace YGOPRODraft
{
	public class DraftDeckNotFoundException : Exception
	{
		public DraftDeckNotFoundException()
		{
		}

		public DraftDeckNotFoundException(string message)
        : base(message)
		{
		}

		public DraftDeckNotFoundException(string message, Exception inner)
        : base(message, inner)
		{
		}
	}

	public class EmptyDraftDeckException : Exception
	{
		public EmptyDraftDeckException()
		{
		}

		public EmptyDraftDeckException(string message)
		: base(message)
		{
		}

		public EmptyDraftDeckException(string message, Exception inner)
		: base(message, inner)
		{
		}
	}
	
	public class FileUtilities
	{
		public static Random rng = new Random();

		/// <summary>
		/// Extracts the current Draft Deck for Battle Pack 1 (Epic Dawn) from the LOTD save data.
		/// </summary>
		/// <param name="savegame">byte array containing the raw savedata</param>
		/// <returns>byte array in LOTD Deck format</returns>
		public static byte[] ExtractBattlePack1FromSaveData(byte[] savegame)
		{
			
			//search for "Battle Pack Epic Dawn"
			byte[] search_pattern = new byte[] { 0x42 ,0x00 ,0x61 ,0x00 ,0x74 ,0x00 ,0x74 ,0x00 ,0x6C ,0x00 ,0x65 ,0x00 ,0x20 ,0x00
				,0x50 ,0x00 ,0x61 ,0x00 ,0x63 ,0x00 ,0x6B ,0x00 ,0x3A ,0x00 ,0x20 ,0x00 ,0x45 ,0x00 ,0x70 ,0x00 ,0x69 ,0x00 ,0x63 ,0x00 ,0x20 ,0x00 ,0x44 ,0x00 ,0x61 ,0x00 ,0x77 ,0x00 ,0x6E };

			var locations = savegame.Locate(search_pattern);

			//Take the first one (The second one is the chosen draft cards, I think. Haven't verified that yet.)
			if (locations.Length == 0)
			{
				throw new DraftDeckNotFoundException("Error: Couldn't find the location of the Draft Deck in your savegame.dat!");
			}

			//Offset until the number of main deck cards starts
			int offset_after_string = 23;
			int byte_offset = locations[0] + search_pattern.Length + offset_after_string;
			int number_of_main_deck_cards = (((int)savegame[byte_offset + 1]) << 8) + savegame[byte_offset];
			byte_offset += 2;
			int number_of_extra_deck_cards = (((int)savegame[byte_offset + 1]) << 8) + savegame[byte_offset];
			byte_offset += 2;
			int number_of_side_deck_cards = (((int)savegame[byte_offset + 1]) << 8) + savegame[byte_offset];
			byte_offset += 2;
			int start_deck_offset = byte_offset;
			if (number_of_main_deck_cards == 0)
			{
				throw new EmptyDraftDeckException("Error: Draft Deck is empty - please play at least one duel after drafting, otherwise the deck doesn't save!");
			}

			List<byte> ydc_deck_format = new List<byte>();
			long header_byte = 25740;
			byte[] ydc_bytes = new byte[0];

			using (var MemWriter = new MemoryStream())
			{
				using (var Writer = new BinaryWriter(MemWriter))
				{
					//I don't know what the first 8 bytes do, just write what they contain by default
					Writer.Write(header_byte);

					//Write main deck
					Writer.Write((Int16)(number_of_main_deck_cards));
					for (; byte_offset < start_deck_offset + (number_of_main_deck_cards * 2); byte_offset += 2)
					{
						Writer.Write(savegame[byte_offset]);
						Writer.Write(savegame[byte_offset + 1]);
					}

					//Write extra deck
					Writer.Write((Int16)(number_of_extra_deck_cards));
					for (; byte_offset < start_deck_offset + ((number_of_main_deck_cards + number_of_extra_deck_cards) * 2); byte_offset += 2)
					{
						Writer.Write(savegame[byte_offset]);
						Writer.Write(savegame[byte_offset + 1]);
					}

					//Write side deck
					Writer.Write((Int16)(number_of_side_deck_cards));
					for (; byte_offset < start_deck_offset + ((number_of_main_deck_cards + number_of_extra_deck_cards + number_of_side_deck_cards) * 2); byte_offset += 2)
					{
						Writer.Write(savegame[byte_offset]);
						Writer.Write(savegame[byte_offset + 1]);
					}
				}
				ydc_bytes = MemWriter.ToArray();
			}

			return ydc_bytes;
		}

		/// <summary>
		/// Writes a binary battle pack file out of a list of lists of cards. 
		/// </summary>
		/// <param name="filename">Output filename for the created bin file</param>
		/// <param name="num_categories">Number of "categories" in the pack. For Battle Packs, this *has* to be 5.</param>
		/// <param name="list_of_card_lists">List of card packs/decks/whatever</param>
		/// <param name="card_name_to_LOTD_ID">Dictionary to map between card name and LOTD card ID/number</param>
		/// <returns>string containing log messages</returns>
		public static string WriteBattlePackBinFile(string filename, long num_categories, List<List<YGOPROCard>> list_of_card_lists, Dictionary<string, string> card_name_to_LOTD_ID)
		{
			string ret = "";
			long base_offset_cards = num_categories * 8 + 8;
			long actual_offset = base_offset_cards;
			if (list_of_card_lists.Count != num_categories)
			{
				ret += ("Note: If not equal to 5 packs - will divide them to be 5 randomly!") + Environment.NewLine;
				var rnd = new Random();
				//Make a single list out of all of them
				List<YGOPROCard> full_list = new List<YGOPROCard>();
				foreach (List<YGOPROCard> single_list in list_of_card_lists)
				{
					full_list.AddRange(single_list);
				}

				full_list.Shuffle();

				list_of_card_lists = new List<List<YGOPROCard>>();
				list_of_card_lists.Add(new List<YGOPROCard>());
				list_of_card_lists.Add(new List<YGOPROCard>());
				list_of_card_lists.Add(new List<YGOPROCard>());
				list_of_card_lists.Add(new List<YGOPROCard>());
				list_of_card_lists.Add(new List<YGOPROCard>());

				int num_of_all_cards = full_list.Count;

				for (int i = 0; i < num_of_all_cards; i++)
				{
					list_of_card_lists[i % (int)num_categories].Add(full_list[i]);
				}
				
			}

			using (var Writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write)))
			{
				Writer.Write(num_categories);
				Writer.Write(actual_offset);
				actual_offset += (long)(list_of_card_lists[0].Count * 2 + 2);
				Writer.Write(actual_offset);
				actual_offset += (long)(list_of_card_lists[1].Count * 2 + 2);
				Writer.Write(actual_offset);
				actual_offset += (long)(list_of_card_lists[2].Count * 2 + 2);
				Writer.Write(actual_offset);
				actual_offset += (long)(list_of_card_lists[3].Count * 2 + 2);
				Writer.Write(actual_offset);

				//Write cards now.
				foreach (var category in list_of_card_lists)
				{
					Writer.Write((UInt16)category.Count);
					foreach (var card in category)
					{
						try
						{
							if (Int32.Parse(card_name_to_LOTD_ID[card.m_card_name]) == 65565)
							{
								ret += ("Error: Card ID is set to 65565 - this means card_map.csv has an error for card " + card.m_card_name + ". (Adding Pot of Greed instead)") + Environment.NewLine;
								Writer.Write((Int16)4844);
							}
							else
							{
								Writer.Write(Int16.Parse(card_name_to_LOTD_ID[card.m_card_name]));
							}
						}
						catch (Exception ex)
						{
							ret += ("Error: couldn't find card " + card.m_card_name + " in database - maybe LOTD does not support it? (Adding Pot of Greed instead)") + Environment.NewLine;
							Writer.Write((Int16)4844);
						}
					}
				}
			}

			return ret;
		}


		/// <summary>
		/// Reads raw bytes for LOTD/.ydc decks
		/// </summary>
		/// <param name="fileName">file location of LOTD deck</param>
		/// <returns>byte array containing .ydc data</returns>
		public static byte[] parseCardListMainExtraSideFromYDCFile(string fileName)
		{
			return File.ReadAllBytes(fileName);
		}

		/// <summary>
		/// Parses a ydk file into a single list, containing all cards of main/extra/side deck
		/// </summary>
		/// <param name="fileName">YGOPRO deck filename</param>
		/// <param name="card_db_filename">cards.db location/filename</param>
		/// <param name="cards_not_available_filename">location of cards_not_available.txt</param>
		/// <returns>single list all cards in the deck as YGOPROCard</returns>
		public static List<YGOPROCard> parseCardListFromYDKFile(string fileName, string card_db_filename, string cards_not_available_filename)
		{
			List<YGOPROCard> cards = new List<YGOPROCard>();

			var cards_as_main_extra_side = parseCardListMainExtraSideFromYDKFile(fileName, card_db_filename, cards_not_available_filename);

			foreach(var deck in cards_as_main_extra_side)
			{
				cards.AddRange(deck);
			}

			return cards;
		}

		/// <summary>
		/// Reads a pack .json file, returning a list of YGOPROCard
		/// </summary>
		/// <param name="JSONFileName">file location of the .json file</param>
		/// <returns></returns>
		public static List<YGOPROCard> cardsFromJSON(string JSONFileName, string card_db_filename, bool use_rarity_simulation)
		{
			String json = System.IO.File.ReadAllText(JSONFileName);

			//TODO: read JSON
			List<YGOJSONStruct> json_list = JsonConvert.DeserializeObject<List<YGOJSONStruct>>(json);
			List<string> card_names = new List<string>();
			foreach (YGOJSONStruct json_struct in json_list)
			{
				card_names.Add(json_struct.name);
			}

			List<YGOPROCard> cards = YGOPROCard.query_ygopro_ids(json_list, card_db_filename);

			if (use_rarity_simulation)
			{
				List<YGOPROCard> rarity_adjusted_cards = new List<YGOPROCard>();

				foreach (YGOPROCard card_obj in cards)
				{
					if (card_obj.m_rarity.ToLower().Contains("common"))
					{
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
					}
					else if (card_obj.m_rarity.ToLower().Contains("secret rare"))
					{
						rarity_adjusted_cards.Add(card_obj);
					}
					else if (card_obj.m_rarity.ToLower().Contains("ultra rare"))
					{
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
					}
					else if (card_obj.m_rarity.ToLower().Contains("super rare"))
					{
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
					}
					else if (card_obj.m_rarity.ToLower().Contains("rare"))
					{
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
						rarity_adjusted_cards.Add(card_obj);
					}
					else
					{
						rarity_adjusted_cards.Add(card_obj);
					}
				}
				return rarity_adjusted_cards;
			}

			return cards;
		}


		/// <summary>
		/// Parses a deck list containing main/extra/side decks from a YGOPRO file. Uses YGOPRO cards.cdb with SQLITE to lookup card data.
		/// Uses cards_not_available_filename to prevent cards which do not exist in LOTD from being added
		/// </summary>
		/// <param name="fileName">YGOPRO deck filename</param>
		/// <param name="card_db_filename">cards.db location/filename</param>
		/// <param name="cards_not_available_filename">location of cards_not_available.txt</param>
		/// <returns>List of main/extra/side decks as YGOPROCard</returns>
		private static List<List<YGOPROCard>> parseCardListMainExtraSideFromYDKFile(string fileName, string card_db_filename, string cards_not_available_filename)
		{
			List<YGOPROCard> cards = new List<YGOPROCard>();
			List<List<YGOPROCard>> decks = new List<List<YGOPROCard>>();

			decks.Add(new List<YGOPROCard>());
			decks.Add(new List<YGOPROCard>());
			decks.Add(new List<YGOPROCard>());

			int deck_idx = 0;

			try
			{
				List<String> lines = new List<string>();
				lines.AddRange(File.ReadAllLines(fileName));

				String rarity = "Common";
				foreach (String line in lines)
				{
					String card = line.TrimEnd(new char[] { '\n', '\r', '\t', ' ' });
					YGOPROCard card_obj = null;
					if (line.Contains("!side") || line.Contains("#extra"))
					{
						deck_idx++;
						continue;
					}
					else if (line.Contains("#"))
					{
						//Comment
						continue;
					}
					else
					{
						int id = Int32.Parse(card);
						card_obj = YGOPROCard.getFromIDAndSetRarity(rarity, id, card_db_filename);
						decks[deck_idx].Add(card_obj);
					}
				}
			}
			catch (Exception ex)
			{
				//I don't care about anything
			}
			var card_names_not_available = File.ReadAllLines(cards_not_available_filename);
			decks[0].RemoveAll(card => card_names_not_available.Contains(card.m_card_name));
			decks[1].RemoveAll(card => card_names_not_available.Contains(card.m_card_name));
			decks[2].RemoveAll(card => card_names_not_available.Contains(card.m_card_name));
			return decks;
		}

		/// <summary>
		/// Reads number_of_random_decks .ydc (LOTD) / .ydk (YGOPRO)  randomly chosen decks from a given folder and returns both lists as out parameters.
		/// </summary>
		/// <param name="folder">Folder to look for decks</param>
		/// <param name="card_db_filename">cards.cdb file location (from YGOPRO to look up card data)</param>
		/// <param name="cards_not_available_filename">cards_not_available.txt location (to disable cards not available in LOTD)</param>
		/// <param name="number_of_random_decks">number of decks which will randomly be chosen</param>
		/// <param name="ydk_extension">extension for YGOPRO decks</param>
		/// <param name="ydc_extension">extension for LOTD decks</param>
		/// <param name="checked_items">used to filter which files to use</param> 
		/// <param name="list_of_all_decks_ydk">out List of main/extra/side YGOPROCard for YGOPRO decks</param>
		/// <param name="list_of_ydc_decks">out List of raw bytes for LOTD decks</param>
		public static void GetRandomDecksFromFolder(string folder, string card_db_filename, string cards_not_available_filename,
			int number_of_random_decks, string ydk_extension,
			string ydc_extension, Dictionary<string, bool> checked_items, out List<List<List<YGOPROCard>>> list_of_all_decks_ydk, out List<byte[]> list_of_ydc_decks)
		{
			list_of_all_decks_ydk = new List<List<List<YGOPROCard>>>();
			string[] files = Directory.GetFiles(folder);
			list_of_ydc_decks = new List<byte[]>();
			string[] random_files = files.OrderBy(x => rng.Next()).ToArray();

			
			int deck_counter = 0;
			foreach (string filename in random_files)
			{

				if (checked_items[filename.Split('\\')[1]] == false)
				{
					continue;
				}

				if (filename.Contains(ydk_extension))
				{
					list_of_all_decks_ydk.Add(parseCardListMainExtraSideFromYDKFile(filename, card_db_filename, cards_not_available_filename));
					deck_counter++;
				}
				if (filename.Contains(ydc_extension))
				{
					list_of_ydc_decks.Add(parseCardListMainExtraSideFromYDCFile(filename));
					deck_counter++;
				}


				if (deck_counter >= number_of_random_decks)
				{
					break;
				}
			}
		}

		/// <summary>
		/// Returns the content of a .csv file as a Dictionary. (To look up card names for their respective LOTD card number/ID)
		/// </summary>
		/// <param name="pathToCSV">path to the .csv file</param>
		/// <returns></returns>
		public static Dictionary<string, string> GetCardIDToLOTDMapFromCSV(string pathToCSV)
		{
			var lines = File.ReadLines(pathToCSV);
			Dictionary<string, string> card_name_to_LOTD_ID = new Dictionary<string, string>();

			foreach (var line in lines)
			{
				string[] split_content = line.Split(';');
				card_name_to_LOTD_ID[split_content[0]] = split_content[1];
			}

			return card_name_to_LOTD_ID;
		}

		/// <summary>
		/// Simply switches key/value in the Dictionary.
		/// </summary>
		/// <param name="originalDictionary"></param>
		/// <returns></returns>
		public static Dictionary<string, string> ReverseDict(Dictionary<string, string> originalDictionary)
		{
			Dictionary<string, string> new_dict = new Dictionary<string, string>();

			foreach(var kp in originalDictionary)
			{
				if(kp.Value != "65565")
				{
					new_dict.Add(kp.Value, kp.Key);
				}
			}
			return new_dict;
		}



		/// <summary>
		/// Converts a YDC/LOTD binary deck file to lists (main/extra/side) of YGOPROCards
		/// </summary>
		/// <param name="ydc_binary">all bytes containing the ydc binary</param>
		/// <param name="LOTD_ID_to_card_name">dictionary mapping from LOTD ID to card name</param>
		/// <param name="card_db_filename">cards.cdb file location (from YGOPRO to look up card data)</param>
		/// <returns></returns>
		public static List<List<YGOPROCard>> YDCToYGOPRODeck(byte[] ydc_binary, Dictionary<string, string> LOTD_ID_to_card_name, string card_db_filename)
		{
			List<List<YGOPROCard>> cards = new List<List<YGOPROCard>>();
			//cards.Add(new List<YGOPROCard>());
			//cards.Add(new List<YGOPROCard>());
			//cards.Add(new List<YGOPROCard>());

			List<string> card_names = new List<string>();



			using (var MemReader = new MemoryStream(ydc_binary))
			{
				using (var Reader = new BinaryReader(MemReader))
				{
					//Read LOTD IDs from ydc_binary

					//Read Header
					Reader.ReadInt64();

					//Iterate over main/extra/side
					for(int deck_index = 0; deck_index < 3; deck_index++)
					{
						int num_main_cards = Reader.ReadInt16();

						for (int i = 0; i < num_main_cards; i++)
						{
							try
							{
								//cards[deck_index].Add()
								card_names.Add(LOTD_ID_to_card_name[Reader.ReadInt16().ToString()]);
							}
							catch (Exception ex)
							{

							}
						}

						cards.Add(YGOPROCard.query_ygopro_ids_from_names(card_names, card_db_filename));

					}
					

				}
					
			}

			return cards;
		}
		/// <summary>
		/// Writes a given main/extra/side YGOPRO deck list to a binary .ydc file, readable by LOTD.
		/// </summary>
		/// <param name="filename">output file name for the .ydc file</param>
		/// <param name="list_of_card_lists">List containing main/extra/side decks in YGORPOCard format</param>
		/// <param name="card_name_to_LOTD_ID">Dictionary used for mapping the card name to LOTD numbers</param>
		/// <returns>a string containing all Log/Error output (yeah, it's ugly, but whatever)</returns>
		public static string WriteYDCDeckFile(string filename, List<List<YGOPROCard>> list_of_card_lists, Dictionary<string, string> card_name_to_LOTD_ID)
		{
			long header_byte = 25740;
			string ret = "";
			using (var Writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write)))
			{
				//I don't know what the first 8 bytes do, just write what they contain by default
				Writer.Write(header_byte);
				int[] default_card_ids = { 4844, 11385, 4844 };
				for(int deck_index = 0; deck_index < 3; deck_index++)
				{
					//Write deck
					Writer.Write((Int16)(list_of_card_lists[deck_index].Count));

				

					foreach (YGOPROCard card in list_of_card_lists[deck_index])
					{
						try
						{
							if (Int32.Parse(card_name_to_LOTD_ID[card.m_card_name]) == 65565)
							{
								ret += ("Error: Card ID is set to 65565 - this means card_map.csv has an error for card " + card.m_card_name + ". (Adding Pot of Greed instead)") + Environment.NewLine;
								Writer.Write((Int16)default_card_ids[deck_index]);
							}
							else
							{
								Writer.Write(Int16.Parse(card_name_to_LOTD_ID[card.m_card_name]));
							}
						}
						catch (Exception ex)
						{
							ret += ("Error: couldn't find card " + card.m_card_name + " in database - maybe LOTD does not support it? (Adding Pot of Greed instead)") + Environment.NewLine;
							Writer.Write((Int16)default_card_ids[deck_index]);
						}
					}
				}
				
			}

			return ret;
		}
	}
}
