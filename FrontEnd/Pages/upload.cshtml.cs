using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UploadModel : PageModel
{
    private readonly IAmazonS3 _s3Client;

    public UploadModel(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    [BindProperty]
    public IFormFile File { get; set; }

    public string UploadResult { get; private set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (File == null || File.Length == 0)
        {
            UploadResult = "Please select a file.";
            return Page();
        }

        var bucketName = "assemblyai-challenge"; // Replace with your bucket name

        try
        {
            using (var stream = File.OpenReadStream())
            {
                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(stream, bucketName, $"{Guid.NewGuid().ToString()}.mp3");
            }

            UploadResult = "File uploaded successfully!";
        }
        catch (Exception ex)
        {
            UploadResult = $"An error occurred: {ex.Message}";
        }

        return Page();
    }
}
