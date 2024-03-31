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
        StepId id = StepId.Create(Guid.NewGuid().ToString());
        Mock<IStep> mockStep = new();
        mockStep.Setup(m => m.Id).Returns(id);
        Mock<IWorkflow> workflowMock = new();
        workflowMock.Setup(m => m.Steps).Returns(new List<IStep>() { mockStep.Object });

        _stepValidatorHandlerMock
            .Setup(m => m.ValidateAsync(It.IsAny<IStep>()))
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
        StepId id = StepId.Create(Guid.NewGuid().ToString());
        Mock<IStep> mockStep = new();
        mockStep.Setup(m => m.Id).Returns(id);
        Mock<IWorkflow> workflowMock = new();
        workflowMock.Setup(m => m.Steps).Returns(new List<IStep>() { mockStep.Object });

        List<WorkflowStepErrorValidation> validationErrors = [new(mockStep.Object, "error")];

        _stepValidatorHandlerMock
            .Setup(m => m.ValidateAsync(It.IsAny<IStep>()))
            .ReturnsAsync(validationErrors);

        // Act
        WorkflowValidatorResult result = await _validator.ValidateAsync(workflowMock.Object);

        // Assert
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().BeEquivalentTo(validationErrors);
    }
}
