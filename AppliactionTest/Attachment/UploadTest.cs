using Application.CQRS.Attachment;
using Moq;
using System.IO.Abstractions;
using Application.DTOs;
using Xunit;


namespace AppliactionTest.Attachment;

public class UploadTest
{
    //private readonly Mock<IFileSystem> _fileSystemMock;
    private readonly UploadFileCommandHandler _handler;
    private readonly Mock<IDirectory> _directoryMock;

    public UploadTest()
    {
        _handler = new UploadFileCommandHandler();
    }

    [Fact]
    public async Task UploadFileAsync_WhenStreamEmpty_ThrowsFileNotFoundException()
    {
        var stream = new MemoryStream();
        var attachmentDTO = new AttachmentDto() { DStream = stream };
        var command = new UploadFileCommand(attachmentDTO);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsSuccess);
    }
}