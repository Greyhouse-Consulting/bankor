using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Bancor.Core.Tests
{
    public class InterestTests
    {
        [Fact]
        public void Should_Calculate_Interest_For_One_Day()
        {
            // Arrange
            var dayByDayInterestCalculator = new DayByDayInterestCalculator(10);

            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Amount = 2000,
                    BookingDate = new DateTime(2018, 10, 1)
                }
            };

            // Act
            var interest = dayByDayInterestCalculator.Calculate(transactions);

            // Assert
            interest.Should().Be(200, "Interest should calculated correctly");
        }


        [Fact]
        public void Should_Calculate_Interest_For_Two_Day()
        {
            // Arrange
            var dayByDayInterestCalculator = new DayByDayInterestCalculator(10);

            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Amount = 2000,
                    BookingDate = new DateTime(2018, 10, 1),
                },
                new Transaction
                {
                    Amount = 2000,
                    BookingDate = new DateTime(2018, 10, 2),
                }
            };

            // Act
            var interest = dayByDayInterestCalculator.Calculate(transactions);

            // Assert
            interest.Should().Be(210, "Interest should calculated correctly");
        }
    }

    public class DayByDayInterestCalculator
    {
        private readonly decimal _interestRate;

        public DayByDayInterestCalculator(decimal interestRate)
        {
            _interestRate = interestRate;
        }

        public decimal Calculate(List<Transaction> transactions)
        {

            var summedTransactionsPerDay = transactions.GroupBy(a => a.BookingDate.ToShortDateString());

            decimal aggregatedInterest = 0;
            int dayOffset = 0;

            foreach (var dayTransactions in summedTransactionsPerDay.OrderBy(a => a.Key))
            {

                dayOffset = dayTransactions.First().BookingDate.DayOfYear + 1 - dayOffset;

                var daySum = dayTransactions.Sum(x => x.Amount);

                aggregatedInterest += ((daySum * _interestRate / 100 / 365) * dayOffset);
            }

            return aggregatedInterest;
        }
    }
}
