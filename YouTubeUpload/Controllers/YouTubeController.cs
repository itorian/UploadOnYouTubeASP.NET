using Google.Apis.Auth.OAuth2;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmptyWebForm.Controllers
{
    public class YouTubeController : Controller
    {
        public static string statusMessage = "Booting";
        public static string totalSize = "0";
        public static string totalSent = "0";
        public static string videoId = "";

        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> YouTubeUpload()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                totalSize = Request.Files[i].ContentLength.ToString();
                Stream fileStream = file.InputStream;
                await Run(fileStream);
            }

            return Json(new { error = false, message = "Video uploaded." });
        }

        public async Task Run(Stream fileStream)
        {
            string CLIENT_ID = "xxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.apps.googleusercontent.com";  // Replace with your client id
            string CLIENT_SECRET = "xxxxxxxxxxxxxxxxxxxxxxxx";  // Replace with your secret

            var youtubeService = AuthenticateOauth(CLIENT_ID, CLIENT_SECRET, "SingleUser");

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = "Default Video Title " + new Guid();
            video.Snippet.Description = "Default Video Description";
            video.Snippet.Tags = new string[] { "tag1", "tag2" };
            video.Snippet.CategoryId = "22";
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted"; // or "private" or "public"

            const int KB = 0x400;
            var minimumChunkSize = 50 * KB;

            using (fileStream)
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;
                videosInsertRequest.ChunkSize = minimumChunkSize * 8;

                await videosInsertRequest.UploadAsync();
            }
        }

        void videosInsertRequest_ProgressChanged(IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Starting:
                    Debug.WriteLine("Starting");
                    new Task(() => { UpdateUIAsync(progress, "starting"); }).Start();
                    break;
                case UploadStatus.Uploading:
                    Debug.WriteLine(progress.BytesSent + " bytes sent. Please wait.");
                    new Task(() => { UpdateUIAsync(progress, "uploading"); }).Start();
                    break;
                case UploadStatus.Completed:
                    Debug.WriteLine("Upload completed on YouTube.");
                    new Task(() => { UpdateUIAsync(progress, "completed"); }).Start();
                    break;
                case UploadStatus.Failed:
                    Debug.WriteLine("An error prevented the upload from completing.\n" + progress.Exception);
                    new Task(() => { UpdateUIAsync(progress, "failed"); }).Start();
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            videoId = video.Id;
            Debug.WriteLine("Video id " + video.Id + " was successfully uploaded.");
            new Task(() => { UpdateUIAsync(video, "done"); }).Start();
        }

        public static YouTubeService AuthenticateOauth(string clientId, string clientSecret, string userName)
        {

            string[] scopes = new string[] { YouTubeService.Scope.Youtube,
                                             YouTubeService.Scope.YoutubeForceSsl,
                                             YouTubeService.Scope.Youtubepartner,
                                             YouTubeService.Scope.YoutubepartnerChannelAudit,
                                             YouTubeService.Scope.YoutubeReadonly,
                                             YouTubeService.Scope.YoutubeUpload};

            try
            {
                // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret }
                                                                                             , scopes
                                                                                             , userName
                                                                                             , CancellationToken.None
                                                                                             , new FileDataStore("Daimto.YouTube.Auth.Store")).Result;

                YouTubeService service = new YouTubeService(new YouTubeService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "YouTube Data API Sample"
                });

                return service;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return null;
            }
        }

        private void UpdateUIAsync(object obj, string type)
        {
            if (type == "starting")
            {
                IUploadProgress progress = (IUploadProgress)obj;
                totalSent = progress.BytesSent.ToString();
                statusMessage = "Upload Starting";
            }

            if (type == "uploading")
            {
                IUploadProgress progress = (IUploadProgress)obj;
                totalSent = progress.BytesSent.ToString();
                statusMessage = "Video uploading";
            }

            if (type == "completed")
            {
                IUploadProgress progress = (IUploadProgress)obj;
                totalSent = progress.BytesSent.ToString();
                statusMessage = "Completed";
            }

            if (type == "failed")
            {
                statusMessage = "Completed";
            }

            if (type == "done")
            {
                Video video = (Video)obj;
                statusMessage = "Done";
            }
        }

        [HttpGet]
        public JsonResult GetYouTubeUploadingStatus()
        {
            Debug.WriteLine("Ajax call received...");
            return Json(new { statusMessage = statusMessage, totalSent = totalSent, totalSize = totalSize }, JsonRequestBehavior.AllowGet);
        }
    }
}