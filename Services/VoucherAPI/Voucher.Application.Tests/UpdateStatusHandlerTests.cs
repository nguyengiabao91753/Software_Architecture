using Xunit;
using Moq;
using MassTransit;
using Voucher.Application.Features.UpdateStatus;
using Voucher.Application.Abstractions;
using Voucher.Application.Dtos;
using Voucher.Shared.Events;

namespace Voucher.Application.Tests;

public class UpdateStatusHandlerTests
{
    private readonly Mock<IVoucherRepository> _mockRepository;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly UpdateStatusHandler _handler;

    public UpdateStatusHandlerTests()
    {
        _mockRepository = new Mock<IVoucherRepository>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _handler = new UpdateStatusHandler(_mockRepository.Object, _mockPublishEndpoint.Object);
    }

    [Fact]
    public async Task Handle_WithValidVoucherId_ShouldUpdateStatusAndPublishEvent()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var newStatus = "Inactive";
        var command = new UpdateStatusCommand(voucherId, newStatus);

        var voucherDto = new VoucherDto(
            VoucherId: voucherId,
            VoucherCode: "SUMMER2024",
            Description: "Summer discount",
            DiscountType: "Percentage",
            DiscountValue: 20m,
            StartDate: DateTime.UtcNow.AddMonths(-1),
            EndDate: DateTime.UtcNow.AddMonths(1),
            Quantity: 100,
            UsedCount: 50,
            Status: newStatus
        );

        _mockRepository
            .Setup(r => r.UpdateStatusAsync(voucherId, newStatus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucherDto);

        _mockPublishEndpoint
            .Setup(p => p.Publish(It.IsAny<VoucherStatusUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        _mockRepository.Verify(
            r => r.UpdateStatusAsync(voucherId, newStatus, It.IsAny<CancellationToken>()),
            Times.Once,
            "Repository.UpdateStatusAsync should be called once"
        );

        _mockRepository.Verify(
            r => r.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()),
            Times.Once,
            "Repository.GetByIdAsync should be called to retrieve updated voucher"
        );

        _mockPublishEndpoint.Verify(
            p => p.Publish(It.IsAny<VoucherStatusUpdatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "PublishEndpoint.Publish should be called once"
        );
    }

    [Fact]
    public async Task Handle_WhenVoucherNotFound_ShouldReturnFalse()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var newStatus = "Inactive";
        var command = new UpdateStatusCommand(voucherId, newStatus);

        _mockRepository
            .Setup(r => r.UpdateStatusAsync(voucherId, newStatus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VoucherDto)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);

        _mockPublishEndpoint.Verify(
            p => p.Publish(It.IsAny<VoucherStatusUpdatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "PublishEndpoint should not be called when voucher is not found"
        );
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ShouldReturnFalseAndNotPublish()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var newStatus = "Inactive";
        var command = new UpdateStatusCommand(voucherId, newStatus);

        _mockRepository
            .Setup(r => r.UpdateStatusAsync(voucherId, newStatus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);

        _mockRepository.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "Repository.GetByIdAsync should not be called when update fails"
        );

        _mockPublishEndpoint.Verify(
            p => p.Publish(It.IsAny<VoucherStatusUpdatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "PublishEndpoint should not be called when update fails"
        );
    }

    [Theory]
    [InlineData("Active")]
    [InlineData("Inactive")]
    [InlineData("Expired")]
    [InlineData("Suspended")]
    public async Task Handle_WithVariousStatuses_ShouldUpdateCorrectly(string newStatus)
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var command = new UpdateStatusCommand(voucherId, newStatus);

        var voucherDto = new VoucherDto(
            VoucherId: voucherId,
            VoucherCode: "TEST2024",
            Description: "Test voucher",
            DiscountType: "Percentage",
            DiscountValue: 15m,
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddMonths(3),
            Quantity: 100,
            UsedCount: 0,
            Status: newStatus
        );

        _mockRepository
            .Setup(r => r.UpdateStatusAsync(voucherId, newStatus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucherDto);

        _mockPublishEndpoint
            .Setup(p => p.Publish(It.IsAny<VoucherStatusUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateStatusAsync(voucherId, newStatus, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPublishThrowsException_ShouldThrowException()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var newStatus = "Inactive";
        var command = new UpdateStatusCommand(voucherId, newStatus);

        var voucherDto = new VoucherDto(
            VoucherId: voucherId,
            VoucherCode: "FAILTEST",
            Description: "Test",
            DiscountType: "Percentage",
            DiscountValue: 10m,
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddMonths(1),
            Quantity: 100,
            UsedCount: 0,
            Status: newStatus
        );

        _mockRepository
            .Setup(r => r.UpdateStatusAsync(voucherId, newStatus, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucherDto);

        _mockPublishEndpoint
            .Setup(p => p.Publish(It.IsAny<VoucherStatusUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("RabbitMQ connection failed"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        // Repository should still be updated
        _mockRepository.Verify(r => r.UpdateStatusAsync(voucherId, newStatus, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExpiredVoucher_ShouldUpdateStatusToExpired()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var command = new UpdateStatusCommand(voucherId, "Expired");

        var voucherDto = new VoucherDto(
            VoucherId: voucherId,
            VoucherCode: "EXPIRED2024",
            Description: "Expired voucher",
            DiscountType: "Percentage",
            DiscountValue: 10m,
            StartDate: DateTime.UtcNow.AddMonths(-3),
            EndDate: DateTime.UtcNow.AddDays(-1), // Already ended
            Quantity: 100,
            UsedCount: 100,
            Status: "Expired"
        );

        _mockRepository
            .Setup(r => r.UpdateStatusAsync(voucherId, "Expired", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucherDto);

        _mockPublishEndpoint
            .Setup(p => p.Publish(It.IsAny<VoucherStatusUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateStatusAsync(voucherId, "Expired", It.IsAny<CancellationToken>()), Times.Once);
    }
}
