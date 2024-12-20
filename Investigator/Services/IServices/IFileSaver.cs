namespace Investigator.Services.IServices
{
    public interface IFileSaver
    {
        string UploadFilesToGoogleDrive(IFormFile file);
        Task<bool> DeleteFileFromGoogleDrive(string fileId);
    }
}
