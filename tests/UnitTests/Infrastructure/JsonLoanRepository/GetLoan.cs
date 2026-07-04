using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace UnitTests.Infrastructure.JsonLoanRepositoryTests;

public class GetLoan
{
    private readonly ILoanRepository _mockLoanRepository;
    private readonly JsonLoanRepository _jsonLoanRepository;
    private readonly IConfiguration _configuration;
    private readonly JsonData _jsonData;

    public GetLoan()
    {
        _mockLoanRepository = Substitute.For<ILoanRepository>();

        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JsonPaths:Authors"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "Authors.json"),
                ["JsonPaths:Books"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "Books.json"),
                ["JsonPaths:BookItems"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "BookItems.json"),
                ["JsonPaths:Patrons"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "Patrons.json"),
                ["JsonPaths:Loans"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "Loans.json")
            })
            .Build();

        _jsonData = new JsonData(_configuration);
        _jsonLoanRepository = new JsonLoanRepository(_jsonData);
    }

    [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns a loan when it exists")]
    public async Task GetLoan_ReturnsLoanWhenItExists()
    {
        // Arrange
        const int loanId = 1;
        var expectedLoan = new Loan { Id = loanId };
        _mockLoanRepository.GetLoan(loanId).Returns(expectedLoan);

        // Act
        var actualLoan = await _jsonLoanRepository.GetLoan(loanId);

        // Assert
        Assert.NotNull(actualLoan);
        Assert.Equal(expectedLoan.Id, actualLoan!.Id);
    }

    [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns null when loan does not exist")]
    public async Task GetLoan_ReturnsNullWhenLoanDoesNotExist()
    {
        // Arrange
        const int loanId = 99; // Usamos un ID que no exista en los JSON
        _mockLoanRepository.GetLoan(loanId).Returns((Loan?)null);

        // Act
        var actualLoan = await _jsonLoanRepository.GetLoan(loanId);

        // Assert
        Assert.Null(actualLoan);
    }
}