using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            var rounded = Math.Truncate(interest * 100) / 100;
            rounded.Should().Be(49.86m, "Interest should be calculated correctly");
        }


        [Fact]
        public void Should_Calculate_Interest_For_Two_Days()
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
            var rounded = Math.Truncate(interest * 100) / 100;
            rounded.Should().Be(49.86m, "Interest should calculated correctly");
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
            var summedTransactionsPerDay = transactions.GroupBy(a => a.BookingDate.ToShortDateString()).ToList();

            decimal aggregatedInterest = 0;

            var currentTransaction = summedTransactionsPerDay.Take(1).First();
            summedTransactionsPerDay.RemoveAt(0);

            while(summedTransactionsPerDay.Any())
            {
                var nextTransaction = summedTransactionsPerDay.Take(1).First();
                summedTransactionsPerDay.RemoveAt(0);
                aggregatedInterest += Calculate(currentTransaction.ToList(), nextTransaction.First().BookingDate.DayOfYear);
                currentTransaction = nextTransaction;
            }

            aggregatedInterest += Calculate(currentTransaction.ToList(), 365);

            return aggregatedInterest;
        }

        private decimal Calculate(List<Transaction> currentTransactions, int v)
        {
            var totalAmount = currentTransactions.Sum(a => a.Amount);

            var numberOfDaysInPeriod = v - currentTransactions.First().BookingDate.DayOfYear;

            return ((totalAmount * _interestRate / 100 / 365) * numberOfDaysInPeriod);
        }
    }
}
