using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AdvProject
{
    class SaveStorage
    {
        private IsolatedStorageFile isolatedStorage;
        Object locker = new object();

        private string pathToText;
        private string folderName;

        public SaveStorage()
        {
            folderName = "ScoresFolder";

            pathToText = String.Format("{0}\\ScoreFile.txt", folderName);

            isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
        }

        public void WriteToStorage(Object Score)
        {
            string scoreSaveToStorage = Score.ToString();

            if (isolatedStorage != null)
            {
                lock (locker)
                {
                    try
                    {
                        if (!isolatedStorage.DirectoryExists(folderName))
                            isolatedStorage.CreateDirectory(folderName);

                        using (IsolatedStorageFileStream isoStorageTxtFile = isolatedStorage.OpenFile(pathToText, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(isoStorageTxtFile))
                            {
                                writer.Write(scoreSaveToStorage);
                                MessageBox.Show("saved to ScoreFile.txt");
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }

        public void ReadFromStorage()
        {
            if (isolatedStorage != null)
            {
                try
                {
                    lock (locker)
                    {
                        using (IsolatedStorageFileStream isoStorageTxtFile = isolatedStorage.OpenFile(pathToText, FileMode.Open, FileAccess.Read))
                        {
                            using (StreamReader reader = new StreamReader(isoStorageTxtFile))
                            {
                                string scoreFromTxtFile = reader.ReadLine();

                                
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }


    }
}
