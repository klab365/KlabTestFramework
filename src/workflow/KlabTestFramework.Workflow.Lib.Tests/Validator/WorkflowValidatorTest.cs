using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Klab.Toolkit.Results;
using KlabTestFramework.Workflow.Lib.Specifications;
using Moq;

namespace KlabTestFramework.Workflow.Lib.Validator.Tests;

public class WorkflowValidatorTests
{
    private readonly WorkflowValidator _validator;
    private readonly Mock<IStepValidatorHandler> _stepValidatorHandlerMock;

    public WorkflowValidatorTests()
    {
        _stepValidatorHandlerMock = new Mock<IStepValidatorHandler>();
        IEnumerable<IStepValidatorHandler> stepValidatorHandlers = new List<IStepValidatorHandler> { _stepValidatorHandlerMock.Object };
        _validator = new WorkflowValidator(stepValidatorHandlers);
    }

    [Fact]
    public async Task ValidateAsyncShouldReturnSuccessResultWhenWorkflowIsValid()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Mock<IStep> mockStep = new();
        Mock<IWorkflow> workflowMock = new();
        workflowMock.Setup(m => m.Steps).Returns(new List<StepContainer>() { new(mockStep.Object) { Id = id } });

        _stepValidatorHandlerMock
            .Setup(m => m.ValidateAsync(It.IsAny<Guid>(), It.IsAny<IStep>()))
            .ReturnsAsync([]);

        // Act
        Result<WorkflowValidatorResult> result = await _validator.ValidateAsync(workflowMock.Object);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsyncShouldReturnFailureResultWhenWorkflowHasValidationErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Mock<IStep> mockStep = new();
        Mock<IWorkflow> workflowMock = new();
        workflowMock.Setup(m => m.Steps).Returns(new List<StepContainer>() { new(mockStep.Object) { Id = id } });

        List<WorkflowStepErrorValidation> validationErrors = [new(id, mockStep.Object, "error")];

        _stepValidatorHandlerMock
            .Setup(m => m.ValidateAsync(It.IsAny<Guid>(), It.IsAny<IStep>()))
            .ReturnsAsync(validationErrors);

        // Act
        WorkflowValidatorResult result = await _validator.ValidateAsync(workflowMock.Object);

        // Assert
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().BeEquivalentTo(validationErrors);
    }
}
