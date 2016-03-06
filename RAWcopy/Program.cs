using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAWcopy
{
	class Program
	{
		[STAThread()]
		static void Main(string[] args)
		{
			string[] autoFile = null;

			if (File.Exists(".\\RAWcopy_auto.cfg"))
			{
				autoFile = File.ReadAllLines(".\\RAWcopy_auto.cfg");
			}

			#region Copy From File

			string copyFile = "";
			if (autoFile == null)
			{
				Console.Write("Enter path of file to copy from: ");
				copyFile = Console.ReadLine();
			}
			else
			{
				copyFile = autoFile[0];
			}

			copyFile = copyFile.Replace("\"", "");

			try
			{
				ValidateFile(copyFile);
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine(string.Format("Error! {0} Aborting... (Press Enter to Exit)", ex.Message));
				Console.ReadLine();
				return;
			}

			Console.WriteLine("File validated!");

			#endregion

			FileStream stream = null;

			try
			{
				stream = System.IO.File.OpenRead(copyFile);
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("Error! {0} Aborting! (Press Enter to Exit)", ex.Message));
				Console.ReadLine();
				return;
			}

			#region Bytes

			long numBytes = -1;

			if (autoFile == null)
			{
				Console.WriteLine();
				Console.Write("Number of Bytes to copy (0 to copy all): ");
				numBytes = long.Parse(Console.ReadLine());
			}
			else
				numBytes = long.Parse(autoFile[1]);

			if (numBytes >= stream.Length || numBytes <= 0)
			{
				Console.WriteLine(string.Format("Number of Bytes automatically set to {0}.", stream.Length));
				numBytes = stream.Length;
			} 

			#endregion

			byte[] copy = new byte[numBytes];

			int read = stream.Read(copy, 0, (int)numBytes);

			stream.Close();

			Console.WriteLine(string.Format("Read {0} bytes and closed stream.", read));


			if (numBytes < 1024)
			{
				Console.WriteLine("Less than 1MB. Displaying data:");
				for (int i = 0; i < copy.Length; i++)
				{
					Console.Write(string.Format("{0:X2} ", copy[i]));
					if ((i + 1) % 16 == 0)
						Console.WriteLine();
				}
			}

			
			Console.WriteLine();
			Console.WriteLine(string.Format("Successfully copied {0} bytes!", numBytes));
			Console.WriteLine();

			#region Paste File

			string pasteFile = "";

			if (autoFile == null)
			{
				Console.Write("Enter path of file to copy to: ");
				pasteFile = Console.ReadLine();
			}
			else
				pasteFile = autoFile[2];

			pasteFile = pasteFile.Replace("\"", "");

			try
			{
				ValidateFile(pasteFile);
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine(string.Format("Error! {0} Aborting... (Press Enter to Exit)", ex.Message));
				Console.ReadLine();
				return;
			}

			#endregion

			FileStream paste = null;

			try
			{
				paste = System.IO.File.OpenWrite(pasteFile);
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("Error! {0} Aborting! (Press Enter to Exit)", ex.Message));
				Console.ReadLine();
				return;
			}

			paste.Write(copy, 0, (int)numBytes);

			paste.Flush();
			paste.Close();

			Console.WriteLine(string.Format("{0} Bytes pasted! Complete!", numBytes));

			Console.ReadLine();
		}

		private static void ValidateFile(string copyFile)
		{
			if (!File.Exists(copyFile))
				throw new System.IO.FileNotFoundException(string.Format("File \"{0}\" does not exist.", copyFile));
		}
	}
}
