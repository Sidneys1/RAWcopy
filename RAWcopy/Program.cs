using System;
using System.IO;

namespace RAWcopy {
    internal class Program {
        [STAThread]
        private static void Main() {
            string[] autoFile = null;

            if (File.Exists(".\\RAWcopy_auto.cfg"))
                autoFile = File.ReadAllLines(".\\RAWcopy_auto.cfg");

            #region Copy From File

            string copyFile;
            if (autoFile == null) {
                Console.Write("Enter path of file to copy from: ");
                copyFile = Console.ReadLine();
            }
            else {
                copyFile = autoFile[0];
            }

            copyFile = copyFile?.Replace("\"", "");

            try {
                ValidateFile(copyFile);
            }
            catch (FileNotFoundException ex) {
                Console.WriteLine("Error! {0} Aborting... (Press Enter to Exit)", ex.Message);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("File validated!");

            #endregion

            FileStream stream;

            try {
                stream = File.OpenRead(copyFile);
            }
            catch (Exception ex) {
                Console.WriteLine("Error! {0} Aborting! (Press Enter to Exit)", ex.Message);
                Console.ReadLine();
                return;
            }

            #region Bytes

            long numBytes;

            if (autoFile == null) {
                Console.WriteLine();
                Console.Write("Number of Bytes to copy (0 to copy all): ");
                numBytes = long.Parse(Console.ReadLine()??"0");
            }
            else {
                numBytes = long.Parse(autoFile[1]);
            }

            if (numBytes >= stream.Length || numBytes <= 0) {
                Console.WriteLine("Number of Bytes automatically set to {0}.", stream.Length);
                numBytes = stream.Length;
            }

            #endregion

            var copy = new byte[numBytes];

            var read = stream.Read(copy, 0, (int) numBytes);

            stream.Close();

            Console.WriteLine("Read {0} bytes and closed stream.", read);


            if (numBytes < 1024) {
                Console.WriteLine("Less than 1MB. Displaying data:");
                for (var i = 0; i < copy.Length; i++) {
                    Console.Write("{0:X2} ", copy[i]);
                    if ((i + 1) % 16 == 0)
                        Console.WriteLine();
                }
            }


            Console.WriteLine();
            Console.WriteLine("Successfully copied {0} bytes!", numBytes);
            Console.WriteLine();

            #region Paste File

            string pasteFile;

            if (autoFile == null) {
                Console.Write("Enter path of file to copy to: ");
                pasteFile = Console.ReadLine();
            }
            else {
                pasteFile = autoFile[2];
            }

            pasteFile = pasteFile?.Replace("\"", "");

            try {
                ValidateFile(pasteFile);
            }
            catch (FileNotFoundException ex) {
                Console.WriteLine("Error! {0} Aborting... (Press Enter to Exit)", ex.Message);
                Console.ReadLine();
                return;
            }

            #endregion

            FileStream paste;

            try {
                paste = File.OpenWrite(pasteFile);
            }
            catch (Exception ex) {
                Console.WriteLine("Error! {0} Aborting! (Press Enter to Exit)", ex.Message);
                Console.ReadLine();
                return;
            }

            paste.Write(copy, 0, (int) numBytes);

            paste.Flush();
            paste.Close();

            Console.WriteLine("{0} Bytes pasted! Complete!", numBytes);

            Console.ReadLine();
        }

        private static void ValidateFile(string copyFile) {
            if (!File.Exists(copyFile))
                throw new FileNotFoundException($"File \"{copyFile}\" does not exist.");
        }
    }
}