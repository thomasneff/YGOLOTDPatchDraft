using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YGOPRODraft
{
	public class Onomatopaira
	{
		public static string ExtractYGODATA(YGOPRODraft.Constants strings)
		{
			try
			{

				using (var Reader = new StreamReader(Path.Combine(Properties.Settings.Default.LOTDPath, strings.LOTD_TOC_FILENAME)))
				{
					var DatReader = new BinaryReader(File.Open(Path.Combine(Properties.Settings.Default.LOTDPath, strings.LOTD_DAT_FILENAME), FileMode.Open));
					Reader.ReadLine(); //Dispose First Line.
					char[] delim = new char[] { ' ' };
					while (!Reader.EndOfStream)
					{
						var Line = Reader.ReadLine();
						if (Line == null) continue;

						Line = Line.TrimStart(' '); //Trim Starting Spaces.
						Line = Regex.Replace(Line, @"  +", " ",
							RegexOptions.Compiled); //Remove All Extra Spaces.
						var LineData = Line.Split(delim, 3); //Split Into Chunks.

						//Utilities.Log($"Extracting File: {new FileInfo(LineData[2]).Name} ({LineData[0]} Bytes)", Utilities.Event.Information);

						//Create Item's Folder.
						new FileInfo(Path.Combine(strings.YGO_DATA_WORKING_FOLDER, LineData[2])).Directory.Create();

						//Check Alignment
						var ExtraBytes = Utilities.HexToDec(LineData[0]);
						if (Utilities.HexToDec(LineData[0]) % 4 != 0)
							while (ExtraBytes % 4 != 0)
								ExtraBytes = ExtraBytes + 1;

						//Write File
						using (var FileWriter = new BinaryWriter(File.Open(Path.Combine(strings.YGO_DATA_WORKING_FOLDER, LineData[2]), FileMode.Create, FileAccess.Write)))
						{
							FileWriter.Write(DatReader.ReadBytes(Utilities.HexToDec(LineData[0])));
							FileWriter.Flush();
						}

						//Advance Stream
						DatReader.BaseStream.Position += ExtraBytes - Utilities.HexToDec(LineData[0]);
					}
				}

				return "Successfully extracted YGO_DATA.";
			}
			catch (Exception Ex)
			{
				return "Error: could not extract YGO_DATA :( " +  "\nException: " + Ex;
			}
		}
	}
}
