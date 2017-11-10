using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGOPRODraft
{
	public class Vortex
	{
		public static string PackGame(Constants strings)
		{
			try
			{
				string new_dat_file = Path.Combine(strings.PATCHED_YGODATA_OUT_FOLDER, strings.LOTD_DAT_FILENAME);
				string new_toc_file = Path.Combine(strings.PATCHED_YGODATA_OUT_FOLDER, strings.LOTD_TOC_FILENAME);

				if (File.Exists(new_dat_file))
					File.Delete(new_dat_file);
				if (File.Exists(new_toc_file))
					File.Delete(new_toc_file);

				List<FileNames> Files = new List<FileNames>();
				string[] FilesToPack;
				Files = Utilities.ParseTocFile(Path.Combine(Properties.Settings.Default.LOTDPath, strings.LOTD_TOC_FILENAME));
				FilesToPack = Directory.GetFiles($"{strings.YGO_DATA_WORKING_FOLDER}", "*.*", SearchOption.AllDirectories);

				File.AppendAllText(new_toc_file, "UT\n");

				using (var Writer = new BinaryWriter(File.Open(new_dat_file, FileMode.Append, FileAccess.Write)))
				{
					foreach (var Item in Files)
					{
						var CurrentFileName = FilesToPack?.First(File => File.Contains(Item.FileName));

						//Utilities.Log($"Packing File: {CurrentFileName}.", Utilities.Event.Information);
						var CurrentFileNameLength = Utilities.DecToHex(CurrentFileName
							.Split(new[] { strings.YGO_DATA_WORKING_FOLDER }, StringSplitOptions.None).Last().TrimStart('\\').Length.ToString());
						var CurrentFileSize = Utilities.DecToHex(new FileInfo($"{CurrentFileName}").Length.ToString());

						while (CurrentFileSize.Length != 12)
							CurrentFileSize = CurrentFileSize.Insert(0, " ");
						while (CurrentFileNameLength.Length != 2)
							CurrentFileNameLength = CurrentFileNameLength.Insert(0, " ");

						var Reader = new BinaryReader(File.Open(CurrentFileName, FileMode.Open, FileAccess.Read));
						var NewSize = new FileInfo(CurrentFileName).Length;
						while (NewSize % 4 != 0)
							NewSize = NewSize + 1;

						var BufferSize = NewSize - new FileInfo(CurrentFileName).Length;
						Writer.Write(Reader.ReadBytes((int)new FileInfo(CurrentFileName).Length));

						if (BufferSize > 0)
							while (BufferSize != 0)
							{
								Writer.Write(new byte[] { 00 });
								BufferSize = BufferSize - 1;
							}

						File.AppendAllText(new_toc_file,
							$"{CurrentFileSize} {CurrentFileNameLength} {CurrentFileName.Split(new[] { strings.YGO_DATA_WORKING_FOLDER + "\\" }, StringSplitOptions.None).Last()}\n");
					}
				}
			}
			catch (Exception ex)
			{
				return "Error: Couldn't pack files. " + ex;
			}
			return "Sucessfully finished Packing Files.";
		}
	}
}
