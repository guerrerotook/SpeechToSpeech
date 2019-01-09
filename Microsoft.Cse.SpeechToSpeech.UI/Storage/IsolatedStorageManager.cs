namespace Microsoft.Cse.SpeechToSpeech.UI.Storage
{
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Text;

    public class IsolatedStorageManager
    {
        public static void SaveKeyToIsolatedStorage(string fileName, string key)
        {
            if (fileName != null && key != null)
            {
                using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User |
                    IsolatedStorageScope.Assembly,
                    null,
                    null))
                {
                    using (var stream = new IsolatedStorageFileStream(fileName, FileMode.Create, isolatedStorage))
                    {
                        using (var writer = new StreamWriter(stream, Encoding.UTF8))
                        {
                            writer.WriteLine(key);
                        }
                    }
                }
            }
        }

        public static string GetValueFromIsolatedStorage(string fileName)
        {
            string result = null;
            using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetStore(
                IsolatedStorageScope.User |
                IsolatedStorageScope.Assembly,
                null,
                null))
            {
                if (isolatedStorage.FileExists(fileName))
                {
                    using (var stream = new IsolatedStorageFileStream(fileName, FileMode.Open, isolatedStorage))
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            result = reader.ReadLine();
                        }
                    }
                }
            }

            return result;
        }
    }
}
