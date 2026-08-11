using FluentAssertions;

namespace OsbErpLite.Domain.Tests.Entities;

public class EmployeeNationalIdTests
{
    [Theory]
    [InlineData("10000000146")]
    [InlineData("20433218148")]
    [InlineData("70013389034")]
    public void ValidTcKimlikNo_ShouldPassValidation(string nationalId)
    {
        var result = BeValidTcKimlikNo(nationalId);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("0123456789")]
    [InlineData("1234567890")]
    [InlineData("12345678901")]
    [InlineData("ABCDEFGHIJK")]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("1234567890123")]
    public void InvalidTcKimlikNo_ShouldFailValidation(string nationalId)
    {
        var result = BeValidTcKimlikNo(nationalId);

        result.Should().BeFalse();
    }

    [Fact]
    public void TcKimlikNo_FirstDigitCannotBeZero()
    {
        var result = BeValidTcKimlikNo("01234567890");

        result.Should().BeFalse();
    }

    [Fact]
    public void TcKimlikNo_MustBeExactly11Digits()
    {
        BeValidTcKimlikNo("1234567890").Should().BeFalse();
        BeValidTcKimlikNo("123456789012").Should().BeFalse();
    }

    [Fact]
    public void TcKimlikNo_NullOrEmpty_ShouldFail()
    {
        BeValidTcKimlikNo(null!).Should().BeFalse();
        BeValidTcKimlikNo("").Should().BeFalse();
    }

    [Fact]
    public void TcKimlikNo_10thDigitAlgorithm_ShouldWork()
    {
        // 10000000146 — bilinen gecerli TC
        // d1=1,d2=0,d3=0,d4=0,d5=0,d6=0,d7=0,d8=0,d9=0
        // oddSum = 1+0+0+0+0 = 1, evenSum = 0+0+0+0 = 0
        // (1*7 - 0) % 10 = 7 % 10 = 7 -> d10 artik bu formule uymali
        // totalSum = 1+0+0+0+0+0+0+0+0+4 = 5 -> d11 = 5 % 10 .. wait
        // Gercek dogrulama: 10000000146 gecerli mi kontrol et
        var result = BeValidTcKimlikNo("10000000146");

        result.Should().BeTrue();
    }

    [Fact]
    public void TcKimlikNo_WrongCheckDigit_ShouldFail()
    {
        // 10000000146 gecerli, son hanesiyi degistirince gecersiz olmali
        BeValidTcKimlikNo("10000000147").Should().BeFalse();
        BeValidTcKimlikNo("10000000145").Should().BeFalse();
    }

    private static bool BeValidTcKimlikNo(string nationalId)
    {
        if (string.IsNullOrEmpty(nationalId) || nationalId.Length != 11)
            return false;

        if (nationalId[0] == '0')
            return false;

        var digits = new int[11];
        for (int i = 0; i < 11; i++)
        {
            if (!char.IsDigit(nationalId[i]))
                return false;
            digits[i] = nationalId[i] - '0';
        }

        int oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        int evenSum = digits[1] + digits[3] + digits[5] + digits[7];
        if ((oddSum * 7 - evenSum) % 10 != digits[9])
            return false;

        int totalSum = 0;
        for (int i = 0; i < 10; i++)
            totalSum += digits[i];
        return totalSum % 10 == digits[10];
    }
}
