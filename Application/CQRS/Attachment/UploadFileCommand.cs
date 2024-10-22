using Application.DTOs;
using Application.Shared;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.CQRS.Attachment;

public sealed record UploadFileCommand(AttachmentDto attachment) : IRequest<Result>;

public sealed class UploadFileCommandHandler() : IRequestHandler<UploadFileCommand, Result>
{
    
    private const int MaxFileSize = 50 * 1024 * 1024; // File size: 50MB
    private const int ChunkSize = 5 * 1024 * 1024; // Chunk size: 5MB
    private const int Throttle = 200; // throttle rate in ms

    
    public async Task<Result> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var stream = request.attachment.DStream;
        const string filesDirectory = "../Files/";
        const string filePath = filesDirectory + "a.mp4";
        
        // Handle missing upload directory
        if (!Directory.Exists(filesDirectory))
        {
            Directory.CreateDirectory(filesDirectory);
        }

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            byte[] buffer = new byte[ChunkSize];
            int bytesRead;
            long totalBytesRead = 0;
            
            while ((bytesRead = await stream.ReadAsync(buffer, 0, ChunkSize, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalBytesRead += bytesRead;
                
                //Throttle the upload request
                await Task.Delay(Throttle, cancellationToken);

                if (totalBytesRead > MaxFileSize)
                {
                    // TODO: Add proper Error handling
                    throw new Exception("Too many bytes read");
                }
            }
            
            await stream.CopyToAsync(fileStream, cancellationToken);
        }

        return Result.Success();
    }
}