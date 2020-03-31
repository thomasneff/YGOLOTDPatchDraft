using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGOPRODraft
{
	public class Constants
	{
		public String LOTD_DAT_FILENAME = "YGO_2020.dat";
		public String LOTD_TOC_FILENAME = "YGO_2020.toc";
		public String LOTD_SAVE_FILENAME = "savegame.dat";
		public String CSV_MAP_FILENAME = "card_map_2020.csv";
		public String CARD_DB_FILENAME = "cards.cdb";
		public String BATTLEPACK_1_FILENAME = "bpack_BattlePack1.bin";
		public String YGODATA_PACKS = "packs.zib";
		public String YGODATA_DECKS = "decks.zib";
		public String ADD_PACKS_FOLDER = "PUT_DRAFT_DECKS_PACKS_HERE";
		public String YGO_DATA_WORKING_FOLDER = "YGO_DATA";
		public String UNPACKED_SUFFIX = "_UNPACKED";
		public String PATCHED_YGODATA_OUT_FOLDER = "PATCHED_YGODATA_OUT";
		public String CARDS_NOT_AVAILABLE = "list_of_not_available_cards.txt";
		public String DECK_DATABASE = "DECK_DATABASE";
		public String AI_DECK_DRAFT_FILE_EU = "bp1_draft_eu";
		public String AI_SEALED_DECK_FILE = "bp1_sealed_us";
		public int MAX_AI_DRAFT_INDEX = 10;
		public int MIN_AI_DRAFT_INDEX = 1;
		public long BATTLEPACK_NUM_CATEGORIES = 5;
		public String AI_DECK_DRAFT_FILE_US = "bp1_draft_us";
		public String YDK_EXTENSION = ".ydk";
		public String YDC_EXTENSION = ".ydc";
		public String JSON_EXTENSION = ".json";
		public String EXTRACT_DECK_PREFIX = "extracted_deck_";
	}
}
