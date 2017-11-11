using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;

namespace YGOPRODraft
{
	public class YGOPROCard
	{
		#region Public Fields

		public static int ygoproIsExtraDeck = 0x1 | 0x80 | 0x40 | 0x2000 | 0x800000;
		public int m_card_atk;
		public String m_card_attr;
		public int m_card_def;
		public String m_card_desc;
		public bool m_card_extra;
		public Image m_card_image;
		public int m_card_level;
		public String m_card_name;
		public String m_card_race;
		public String m_card_type;
		public Rectangle m_drawing_rect;
		public String m_rarity;
		public int m_ygopro_id;

		#endregion Public Fields

		#region Private Fields

		private static readonly Dictionary<int, string> CARD_ATTR_MAP = new Dictionary<int, string>
		{
		  {1 , "EARTH"},
{2 , "WATER"},
{4 , "FIRE"},
{8 , "WIND"},
{16 , "LIGHT"},
{32 , "DARK"},
{64 , "DIVINE"}
		};

		private static readonly Dictionary<int, string> CARD_RACE_MAP = new Dictionary<int, string>
		{
		  {1 , "Warrior"},
{2 , "Spellcaster"},
{4 , "Fairy"},
{8 , "Fiend"},
{16 , "Zombie"},
{32 , "Machine"},
{64 , "Aqua"},
{128 , "Pyro"},
{256 , "Rock"},
{512 , "Winged - Beast"},
{1024 , "Plant"},
{2048 , "Insect"},
{4096 , "Thunder"},
{8192 , "Dragon"},
{16384 , "Beast"},
{32768 , "Beast - Warrior"},
{65536 , "Dinosaur"},
{131072 , "Fish"},
{262144 , "Sea Serpent"},
{524288 , "Reptile"},
{1048576 , "Psychic"},
{2097152 , "Divine - Beast"},
{4194304 , "Creator God"},
{8388608 , "Wyrm / Genryu"},
		};

		private static readonly Dictionary<int, string> CARD_TYPE_MAP = new Dictionary<int, string>
		{
		{2,"Normal Spell Card"},
{4,"Normal Trap Card"},
{17,"Normal Monster"},
{33,"Effect Monster"},
{65,"Fusion Monster"},
{97,"Fusion / Effect Monster"},
{129,"Ritual Monster"},
{130,"Ritual Spell"},
{161,"Ritual / Effect Monster"},
{545,"Spirit Monster"},
{1057,"Union Monster"},
{2081,"Gemini Monster"},
{4113,"Tuner / Normal Monster"},
{4129,"Tuner / Effect Monster"},
{8193,"Synchro Monster"},
{8225,"Synchro / Effect Monster"},
{12321,"Synchro / Tuner / Effect Monster"},
{16401,"Token"},
{65538,"Quick-Play Spell Card"},
{131074," Continuous Spell Card"},
{131076," Continuous Trap Card"},
{262146," Equip Spell Card"},
{524290," Field Spell Card"},
{1048580," Counter Trap Card"},
{2097185," Flip Effect Monster"},
{4194337," Toon Monster"},
{8388609,"Xyz Monster"},
{8388641,"Xyz / Effect Monster"},
{16777233 ," Normal Pendulum"},
{16777249 ," Effect Pendulum"},
{16777280 ," Fusion Pendulum"},
{16777312 ," Fusion Effect Pendulum"},
{16777344 ," Ritual Pendulum"},
{16777472 ," Ritual Effect Pendulum"},
{16781329 ," Normal Tuner Pendulum"},
{16781345 ," Effect Tuner Pendulum"},
{16781376 ," Fusion Effect Tuner Pendulum"},
{16781408 ," Fusion Tuner Effect Pendulum"},
{16781568 ," Ritual Tuner Effect Pendulum"},
{16785408 ," Synchro Pendulum"},
{16785441 ," Synchro Effect Pendulum"},
{16789504 ," Synchro Tuner Pendulum"},
{16789536 ," Synchro Effect Tuner Pendulum"},
{25165824 ," Xyz Pendulum"},
{25165856 ," Xyz Pendulum Effect"}
};

		#endregion Private Fields

		#region Public Constructors

		public YGOPROCard()
		{
		}

		#endregion Public Constructors

		#region Public Methods

		public static List<YGOPROCard> query_ygopro_ids(List<YGOJSONStruct> json_list, String ygopro_path)
		{
			List<YGOPROCard> cards = new List<YGOPROCard>();
			List<int> ids = new List<int>();
			try
			{
				using (SQLiteConnection connect = new SQLiteConnection("Data Source=" + ygopro_path))
				{
					connect.Open();
					foreach (YGOJSONStruct json_struct in json_list)
					{
						String new_name = json_struct.name.Replace("'", "''");
						using (SQLiteCommand fmd = connect.CreateCommand())
						{
							fmd.CommandText = "SELECT id from texts where name is '" + new_name + "'";
							fmd.CommandType = CommandType.Text;

							SQLiteDataReader r = fmd.ExecuteReader();
							try
							{
								if (r.Read())
								{
									int a = r.GetInt32(0);
									ids.Add(a);
								}
								else
								{
									fmd.CommandText = "SELECT id from texts where name like '%" + new_name + "%'";
									fmd.CommandType = CommandType.Text;

									r = fmd.ExecuteReader();
									if (r.Read())
									{
										int a = r.GetInt32(0);
										//ids.Add(a);
										ids.Add(a);
									}
								}
							}
							catch
							{
							}
						}
					}
				}
			}
			catch
			{
			}

			for (int idx = 0; idx < ids.Count; idx++)
			{
				int id = ids[idx];
				YGOPROCard card = QueryFromID(id, ygopro_path);
				YGOJSONStruct json_struct = json_list[idx];
				card.m_rarity = json_struct.rarity;
				cards.Add(card);
			}

			return cards;
		}

		public static List<YGOPROCard> query_ygopro_ids_from_names(List<string> card_names, String ygopro_path)
		{
			List<YGOPROCard> cards = new List<YGOPROCard>();
			List<int> ids = new List<int>();
			try
			{
				using (SQLiteConnection connect = new SQLiteConnection("Data Source=" + ygopro_path))
				{
					connect.Open();
					foreach (string name in card_names)
					{
						String new_name = name.Replace("'", "''");
						using (SQLiteCommand fmd = connect.CreateCommand())
						{
							fmd.CommandText = "SELECT id from texts where name is '" + new_name + "'";
							fmd.CommandType = CommandType.Text;

							SQLiteDataReader r = fmd.ExecuteReader();
							try
							{
								if (r.Read())
								{
									int a = r.GetInt32(0);
									ids.Add(a);
								}
								else
								{
									fmd.CommandText = "SELECT id from texts where name like '%" + new_name + "%'";
									fmd.CommandType = CommandType.Text;

									r = fmd.ExecuteReader();
									if (r.Read())
									{
										int a = r.GetInt32(0);
										//ids.Add(a);
										ids.Add(a);
									}
								}
							}
							catch
							{
							}
						}
					}
				}
			}
			catch
			{
			}

			for (int idx = 0; idx < ids.Count; idx++)
			{
				int id = ids[idx];
				YGOPROCard card = QueryFromID(id, ygopro_path);
				cards.Add(card);
			}

			return cards;
		}

		public static YGOPROCard QueryFromID(int id, String ygopro_path)
		{
			YGOPROCard ret_card = new YGOPROCard();
			ret_card.m_ygopro_id = id;
			try
			{
				using (SQLiteConnection connect = new SQLiteConnection("Data Source=" + ygopro_path))
				{
					connect.Open();

					using (SQLiteCommand fmd = connect.CreateCommand())
					{
						fmd.CommandText = "SELECT name, desc from texts where id = " + id;
						fmd.CommandType = CommandType.Text;

						SQLiteDataReader r = fmd.ExecuteReader();
						try
						{
							if (r.Read())
							{
								ret_card.m_card_name = r.GetString(0);
								ret_card.m_card_desc = r.GetString(1);
							}
						}
						catch
						{
						}
					}

					using (SQLiteCommand fmd = connect.CreateCommand())
					{
						fmd.CommandText = "SELECT type, atk, def, level, race, attribute from datas where id = " + id;
						fmd.CommandType = CommandType.Text;

						SQLiteDataReader r = fmd.ExecuteReader();
						try
						{
							if (r.Read())
							{
								if (CARD_TYPE_MAP.ContainsKey(r.GetInt32(0)))
									ret_card.m_card_type = CARD_TYPE_MAP[r.GetInt32(0)];
								else
									ret_card.m_card_type = "UNKNOWN";

								ret_card.m_card_atk = r.GetInt32(1);
								ret_card.m_card_def = r.GetInt32(2);
								ret_card.m_card_level = r.GetInt32(3);
								int test = r.GetInt32(0);
								ret_card.m_card_extra = (r.GetInt32(0) & YGOPROCard.ygoproIsExtraDeck) > 1;
								if (CARD_RACE_MAP.ContainsKey(r.GetInt32(4)))
									ret_card.m_card_race = CARD_RACE_MAP[r.GetInt32(4)];
								else
									ret_card.m_card_race = "UNKNOWN";

								if (CARD_ATTR_MAP.ContainsKey(r.GetInt32(5)))
									ret_card.m_card_attr = CARD_ATTR_MAP[r.GetInt32(5)];
								else
									ret_card.m_card_attr = "UNKNOWN";
							}
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}

			//get img from ygopro path
			//String trimmed = ygopro_path.TrimEnd("cards.cdb".ToCharArray());
			//ret_card.m_card_image = Image.FromFile(trimmed +"\\pics\\" + id.ToString() + ".jpg");

			return ret_card;
		}

		public bool isInRect(int x, int y)
		{
			return (x >= m_drawing_rect.X && x <= m_drawing_rect.X + m_drawing_rect.Width && y >= m_drawing_rect.Y && y <= m_drawing_rect.Y + m_drawing_rect.Height);
		}

		public static YGOPROCard getFromIDAndSetRarity(String rarity, int id, string card_cdb_path)
		{
			YGOPROCard card;

			card = YGOPROCard.QueryFromID(id, card_cdb_path);
			card.m_rarity = rarity;
			return card;
		}
		#endregion Public Methods
	}
}