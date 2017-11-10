using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGOPRODraft
{
	public class Cyclone
	{
		public static string PackZibFile(string zib_unpacked_folder, string zib_destination)
		{
			try
			{

			
				var FileNamesToReadInOrder = new List<string>();
				if (File.Exists($"{zib_unpacked_folder}\\Index.zib"))
					FileNamesToReadInOrder = File.ReadAllLines($"{zib_unpacked_folder}\\Index.zib").ToList();

				using (var Writer = new BinaryWriter(File.Open(zib_destination, FileMode.OpenOrCreate, FileAccess.Write)))
				{
					uint CurrentOffset = (uint)(Directory.GetFiles(zib_unpacked_folder).Length - 1) * 64 + 16; //First should be Number of Files * 64 + 16.
					uint DeckHeaderSize = 8; //8 Bytes before Main-Deck Size.
					bool initial = true;
					foreach (var FileToPack in FileNamesToReadInOrder)
					{
						uint OffSet = CurrentOffset;
						var CurrentFileSize = new FileInfo($"{zib_unpacked_folder}\\{FileToPack}").Length;


						//TODO: possibly need to add 1 to the initial offset if it breaks
						if (initial)
						{
							OffSet = OffSet + 1;
							initial = false;
						}
						Writer.Write(SwapBytes(OffSet));
						//Writer.Write(0x00);
						//Writer.Write(SwapBytes(OffSet));
						//Writer.Write(0x00);
						uint filesize = (uint)new FileInfo($"{zib_unpacked_folder}\\{FileToPack}").Length;
						Writer.Write(SwapBytes(filesize));

						Writer.Write(Encoding.ASCII.GetBytes(FileToPack));
						//EDIT: Need to pad with zeroes to 64 byte chunks.
						int currentLength = 4 + 4 + Encoding.ASCII.GetBytes(FileToPack).Length;
						var currentLengthPadded = 64 * ((currentLength + 63) / 64);
						var fill = new byte[currentLengthPadded - currentLength];
						Writer.Write(fill);

						//EDIT: CurrentFileSize should be padded to 16 bytes - the game requires all stuff to be padded to 16 bytes.
						CurrentFileSize = 16 * ((CurrentFileSize + 15) / 16);
						CurrentOffset += Convert.ToUInt32(CurrentFileSize);

					}

					//Also add 16 bytes of padding here.
					Writer.Write(new byte[16]);

					foreach (var FileToPack in FileNamesToReadInOrder)
					{
						byte[] bytes = File.ReadAllBytes($"{zib_unpacked_folder}\\{FileToPack}");
						int numBytesWithPadding = 16 * ((bytes.Length + 15) / 16);
						Writer.Write(bytes);

						//Pad to multiples of 16 bytes, aligned.
						var fill = new byte[numBytesWithPadding - bytes.Length];
						Writer.Write(fill);
					}

					//TODO: need to add padding to 16 bytes

				}
			}
			catch (Exception ex)
			{
				return "Error: couldn't pack " + zib_unpacked_folder + ". Maybe your unpacked folder contents are corrupt. " + ex;
			}
			return "Successfully packed " + zib_destination;
		}
		public static uint SwapBytes(uint x)
		{
			// swap adjacent 16-bit blocks
			x = (x >> 16) | (x << 16);
			// swap adjacent 8-bit blocks
			return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
		}
	}
}
