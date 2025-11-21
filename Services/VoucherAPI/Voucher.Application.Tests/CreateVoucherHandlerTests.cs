using Xunit;
using Moq;
using MassTransit;
using Voucher.Application.Features.CreateVoucher;
using Voucher.Application.Abstractions;
using Voucher.Application.Dtos;
using Voucher.Shared.Events;

namespace Voucher.Application.Tests;

public class CreateVoucherHandlerTests
{
    private readonly Mock<IVoucherRepository> _mockRepository;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly CreateVoucherHandler _handler;

    public CreateVoucherHandlerTests()
    {
        _mockRepository = new Mock<IVoucherRepository>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _handler = new CreateVoucherHandler(_mockRepository.Object, _mockPublishEndpoint.Object);
    }

    [Fact]
    public async Task Handle_WithValidVoucher_ShouldCreateVoucherAndPublishEvent()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var voucherDto = new VoucherDto(
            VoucherId: Guid.Empty,
            VoucherCode: "SUMMER2024",
            Description: "Summer discount voucher",
            DiscountType: "Percentage",
            DiscountValue: 20m,
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddMonths(1),
            Quantity: 100,
            UsedCount: 0,
            Status: "Active"
        );

        var command = new CreateVoucherCommand(voucherDto);

        _mockRepository
            .Setup(r => r.CreateAsync(voucherDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucherId);

        _mockPublishEndpoint
            .Setup(p => p.Publish(It.IsAny<VoucherCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(voucherId, result);

        _mockRepository.Verify(
            r => r.CreateAsync(voucherDto, It.IsAny<CancellationToken>()),
            Times.Once,
            "Repository.CreateAsync should be called once"
        );

        _mockPublishEndpoint.Verify(
            p => p.Publish(It.IsAny<VoucherCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "PublishEndpoint.Publish should be called once"
        );
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var voucherDto = new VoucherDto(
            VoucherId: Guid.Empty,
            VoucherCode: "INVALID",
            Description: "Test",
            DiscountType: "Percentage",
            DiscountValue: 10m,
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddMonths(1),
            Quantity: 100,
            UsedCount: 0,
            Status: "Active"
        );

        var command = new CreateVoucherCommand(voucherDto);

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<VoucherDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockPublishEndpoint.Verify(
            p => p.Publish(It.IsAny<VoucherCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "PublishEndpoint.Publish should not be called when repository fails"
        );
    }

    [Theory]
    [InlineData("NEWYEAR2024", "New Year Voucher", "Percentage", 15, 50)]
    [InlineData("BLACKFRIDAY", "Black Friday Deal", "FixedAmount", 50, 200)]
    [InlineData("EASTER2024", "Easter Special", "Percentage", 25, 75)]
    public async Task Handle_WithVariousVoucherTypes_ShouldSucceed(
        string code, string description, string discountType, decimal discountValue, int quantity)
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var voucherDto = new VoucherDto(
            VoucherId: Guid.Empty,
            VoucherCode: code,
            Description: description,
            DiscountType: discountType,
            DiscountValue: discountValue,
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddMonths(3),
            Quantity: quantity,
            UsedCount: 0,
            Status: "Active"
        );

        var command = new CreateVoucherCommand(voucherDto);

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<VoucherDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucherId);

        _mockPublishEndpoint
            .Setup(p => p.Publish(It.IsAny<VoucherCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(voucherId, result);
        _mockRepository.Verify(r => r.CreateAsync(voucherDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPublishFails_ShouldThrowException()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var voucherDto = new VoucherDto(
            VoucherId: Guid.Empty,
            VoucherCode: "TESTFAIL",
            Description: "Test",
            DiscountType: "Percentage",
            DiscountValue: 10m,
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddMonths(1),
            Quantity: 100,
            UsedCount: 0,
            Status: "Active"
        );

        var command = new CreateVoucherCommand(voucherDto);

        _mockRepository
            .Setup(r => r.CreateAsync(voucherDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucherId);

        _mockPublishEndpoint
            .Setup(p => p.Publish(It.IsAny<VoucherCreatedEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("RabbitMQ connection failed"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        // Repository should still be called
        _mockRepository.Verify(r => r.CreateAsync(voucherDto, It.IsAny<CancellationToken>()), Times.Once);
    }
}
