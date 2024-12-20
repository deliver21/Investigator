using Investigator.Services.IServices;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace Investigator.Services
{
    public class FileSaver : IFileSaver
    {
        private string credentialsPath;
        private string folderId;
        private readonly IWebHostEnvironment _webHostEnvironment;
       public FileSaver(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            credentialsPath = configuration.GetSection("Credentials").GetValue<string>("CredentialsPath");
            folderId = configuration.GetSection("Credentials").GetValue<string>("FolderId");
            _webHostEnvironment = webHostEnvironment;
        }
        public string UploadFilesToGoogleDrive(IFormFile file)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[]
                {
                    DriveService.ScopeConstants.DriveFile
                });

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Drive Upload Mate_Code"
                });
                
                var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = file.FileName,
                    Parents = new List<string> { folderId }
                };

                FilesResource.CreateMediaUpload request;
                try
                {
                     request = service.Files.Create(fileMetaData, file.OpenReadStream(), "");
                     request.Fields = "id";
                     request.Upload();
                    

                    var uploadedFile = request.ResponseBody;
                    Console.WriteLine($"File '{fileMetaData.Name}' uploaded with ID: {uploadedFile.Id}");

                    return !string.IsNullOrEmpty(uploadedFile.Id) ? uploadedFile.Id : "";
                }
                catch
                {
                    return "";
                }                
            }
        }
        public async Task<bool> DeleteFileFromGoogleDrive(string fileId)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[]
                {
                    DriveService.ScopeConstants.DriveFile
                });

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Drive Delete Mate_Code"
                });
                try
                {
                    var request = service.Files.Delete(fileId);
                    request.SupportsAllDrives = true;
                    var response = await request.ExecuteAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return false;
                }
            }
            return true;
        }
    }
}
