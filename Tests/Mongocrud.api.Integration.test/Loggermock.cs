

using SharpCompress.Common;
using System.Diagnostics;

namespace Mongocrud.api.Integration.test
{
    public class Loggermock : IDisposable
    {
        private readonly string Filepath;

        public Loggermock() 
        {
            Filepath = Path.Combine(Directory.GetCurrentDirectory(), "text.txt");
            File.Create(Filepath).Dispose();


        }

        public void Dispose()
        {
            if (File.Exists(Filepath))
            {
                Debug.WriteLine("File exists. Deleting...");

                // Delete the file
                File.Delete(Filepath);

                Debug.WriteLine("File deleted.");
            }
            else
            {
                Debug.WriteLine("File does not exist.");
            }
        }
    }
}
