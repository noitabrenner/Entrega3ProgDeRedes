using System.Threading.Tasks;

namespace FileManager.Interfaces
{
    public interface IFileStreamHandler
    {
        Task<byte[]> ReadDataAsync(string path, long offset, int length);

        Task WriteDataAsync(string fileName, byte[] data);
    }
}