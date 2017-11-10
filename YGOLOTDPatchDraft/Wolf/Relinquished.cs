using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YGOPRODraft
{
	public class Relinquished
	{

		public static string UnpackDecksAndPacks(YGOPRODraft.Constants strings)
		{
			string ret = "";

			ret += UnpackZib(strings.YGODATA_PACKS + strings.UNPACKED_SUFFIX, Path.Combine(strings.YGO_DATA_WORKING_FOLDER, strings.YGODATA_PACKS)) + Environment.NewLine;
			ret += UnpackZib(strings.YGODATA_DECKS + strings.UNPACKED_SUFFIX, Path.Combine(strings.YGO_DATA_WORKING_FOLDER, strings.YGODATA_DECKS)) + Environment.NewLine;
			return ret;
		}


		private static string UnpackZib(string zib_path, string old_zib_path)
		{
			try
			{ 

				var ZibFileName = new FileInfo(old_zib_path).Name;

				if (Directory.Exists(zib_path) || File.Exists($"{zib_path}/Index.zib"))
					return "Error: unpacked files already exist at " + zib_path + ". Please delete before unpacking again!";

				Directory.CreateDirectory(zib_path);
				File.Create($"{zib_path}/Index.zib").Close();
				File.SetAttributes($"{zib_path}/Index.zib", File.GetAttributes($"{zib_path}/Index.zib") | FileAttributes.Hidden);

				long DataStartOffset = 0x0;
				int OffsetReadSize = 0x0, SizeReadSize = 0x0, FileNameReadSize = 0x0; //These Should Add Up To 64.

				switch (ZibFileName)
				{
					case "cardcropHD400.jpg.zib":
						OffsetReadSize = 8;
						SizeReadSize = 8;
						FileNameReadSize = 48;
						DataStartOffset = 0x69F10;
						break;

					case "cardcropHD401.jpg.zib":
						OffsetReadSize = 8;
						SizeReadSize = 8;
						FileNameReadSize = 48;
						DataStartOffset = 0xC810;
						break;

					case "busts.zib":
						OffsetReadSize = 4;
						SizeReadSize = 4;
						FileNameReadSize = 56;
						DataStartOffset = 0x2390;
						break;

					case "decks.zib":
						OffsetReadSize = 4;
						SizeReadSize = 4;
						FileNameReadSize = 56;
						DataStartOffset = 0x8650;
						break;

					case "packs.zib":
						OffsetReadSize = 4;
						SizeReadSize = 4;
						FileNameReadSize = 56;
						DataStartOffset = 0x750;
						break;
				}
				using (var IndexWriter = new StreamWriter(File.Open($"{zib_path}/Index.zib", FileMode.Open, FileAccess.Write)))
				{
					using (var Reader = new BinaryReader(File.Open(old_zib_path, FileMode.Open, FileAccess.Read)))
					{
						while (Reader.BaseStream.Position + 64 <= DataStartOffset)
						{
							var CurrentChunk = Reader.ReadBytes(64); //40 In HEX is 64 in DEC
							var CurrentStartOffset = Utilities.HexToDec(CurrentChunk.Take(OffsetReadSize).ToArray());
							CurrentChunk = CurrentChunk.Skip(OffsetReadSize).ToArray();
							var CurrentFileSize = Utilities.HexToDec(CurrentChunk.Take(SizeReadSize).ToArray(), false);



							CurrentChunk = CurrentChunk.Skip(SizeReadSize).ToArray();
							var CurrentFileName = Utilities.GetText(CurrentChunk.Take(FileNameReadSize).ToArray());

							//Start Offset Is WRONG In ZIB For Some Reason, or maybe I am...
							if (CurrentFileName == "adriangecko_neutral.png")
								CurrentStartOffset = 0x2390;

							//This also seems to be wrong in the decks zib file.
							if (CurrentFileName == "1classic_01a_yugimuto.ydc")
								CurrentStartOffset = 0x8650;


							//This also seems to be wrong in the packs zib file.
							if (CurrentFileName == "bpack_BattlePack1.bin")
								CurrentStartOffset = 0x750;

							//Utilities.Log($"Exporting {CurrentFileName} ({CurrentFileSize} Bytes)", Utilities.Event.Information);

							var SnapBack = Reader.BaseStream.Position;
							Reader.BaseStream.Position = CurrentStartOffset;
							using (var Writer = new BinaryWriter(File.Open($"{zib_path}/" + CurrentFileName, FileMode.Create, FileAccess.Write)))
							{
								Writer.Write(Reader.ReadBytes(CurrentFileSize));
								Writer.Close();
								IndexWriter.Write(CurrentFileName + "\n");
							}
							Reader.BaseStream.Position = SnapBack;
						}
					}
				}
			}
			catch (Exception ex)
			{
				return "Error: Couldn't extract " + zib_path + ". Try deleting and extracting game files again. " + ex;
			}
			return "Successfully extracted " + zib_path;
		}

	}
}
