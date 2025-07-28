using CadastroDePessoas.Application.Behaviors;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace CadastroDePessoas.Application.Testes.Behaviors
{
    public class ValidationBehaviorTests
    {
        private readonly Mock<IValidator<TestRequest>> _validatorMock;
        private readonly Mock<RequestHandlerDelegate<TestResponse>> _nextMock;
        private readonly ValidationBehavior<TestRequest, TestResponse> _behavior;

        public ValidationBehaviorTests()
        {
            _validatorMock = new Mock<IValidator<TestRequest>>();
            _nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();
            
            var validators = new List<IValidator<TestRequest>> { _validatorMock.Object };
            
            _behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        }

        [Fact]
        public async Task Handle_QuandoValidacaoSucesso_DeveChamarNext()
        {
            // Arrange
            var request = new TestRequest { Name = "Valid Name" };
            var expectedResponse = new TestResponse { Result = true };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _nextMock
                .Setup(n => n.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResponse, result);
            _nextMock.Verify(n => n.Invoke(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoValidacaoFalha_DeveLancarValidationException()
        {
            // Arrange
            var request = new TestRequest { Name = "" };
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name cannot be empty")
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _behavior.Handle(request, _nextMock.Object, CancellationToken.None));

            Assert.NotNull(exception);
            Assert.Contains("Name cannot be empty", exception.Message);
            _nextMock.Verify(n => n.Invoke(It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Fact]
        public async Task Handle_SemValidadores_DeveChamarNext()
        {
            // Arrange
            var request = new TestRequest { Name = "Valid Name" };
            var expectedResponse = new TestResponse { Result = true };

            // Criar behavior sem validadores
            var behavior = new ValidationBehavior<TestRequest, TestResponse>(new List<IValidator<TestRequest>>());

            _nextMock
                .Setup(n => n.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResponse, result);
            _nextMock.Verify(n => n.Invoke(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task Handle_ComMultiplosValidadores_DeveChamarTodosOsValidadores()
        {
            // Arrange
            var request = new TestRequest { Name = "Valid Name" };
            var expectedResponse = new TestResponse { Result = true };

            var validator1Mock = new Mock<IValidator<TestRequest>>();
            var validator2Mock = new Mock<IValidator<TestRequest>>();

            validator1Mock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            validator2Mock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var validators = new List<IValidator<TestRequest>>
            {
                validator1Mock.Object,
                validator2Mock.Object
            };

            var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

            _nextMock
                .Setup(n => n.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResponse, result);
            validator1Mock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
            validator2Mock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
            _nextMock.Verify(n => n.Invoke(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task Handle_ComMultiplosValidadoresUmFalha_DeveLancarValidationException()
        {
            // Arrange
            var request = new TestRequest { Name = "Invalid Name" };

            var validator1Mock = new Mock<IValidator<TestRequest>>();
            var validator2Mock = new Mock<IValidator<TestRequest>>();

            validator1Mock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is invalid")
            };

            validator2Mock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            var validators = new List<IValidator<TestRequest>>
            {
                validator1Mock.Object,
                validator2Mock.Object
            };

            var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                behavior.Handle(request, _nextMock.Object, CancellationToken.None));

            Assert.NotNull(exception);
            Assert.Contains("Name is invalid", exception.Message);
            validator1Mock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
            validator2Mock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
            _nextMock.Verify(n => n.Invoke(It.IsAny<CancellationToken>()), Times.Never);
        }

        // Classes de teste
        public class TestRequest : IRequest<TestResponse>
        {
            public string? Name { get; set; }
        }

        public class TestResponse
        {
            public bool Result { get; set; }
        }
    }
}