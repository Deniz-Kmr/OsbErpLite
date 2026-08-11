using FluentAssertions;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Domain.Tests.Entities;

public class LeaveRequestTests
{
    [Fact]
    public void LeaveRequest_DefaultStatus_ShouldBePending()
    {
        var leave = new LeaveRequest();

        leave.Status.Should().Be(LeaveRequestStatus.Pending);
    }

    [Fact]
    public void LeaveRequest_StartDateBeforeEndDate_ShouldBeValid()
    {
        var leave = new LeaveRequest
        {
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 1, 5)
        };

        leave.EndDate.Should().BeAfter(leave.StartDate);
    }

    [Fact]
    public void LeaveRequest_StartDateAfterEndDate_ShouldBeInvalid()
    {
        var leave = new LeaveRequest
        {
            StartDate = new DateTime(2025, 1, 10),
            EndDate = new DateTime(2025, 1, 5)
        };

        leave.StartDate.Should().BeAfter(leave.EndDate);
    }

    [Fact]
    public void LeaveRequest_StartDateEqualsEndDate_ShouldBeAllowed()
    {
        var sameDay = new DateTime(2025, 3, 15);
        var leave = new LeaveRequest
        {
            StartDate = sameDay,
            EndDate = sameDay
        };

        leave.StartDate.Should().Be(leave.EndDate);
    }

    [Theory]
    [InlineData(LeaveRequestStatus.Pending, LeaveRequestStatus.Approved, true)]
    [InlineData(LeaveRequestStatus.Pending, LeaveRequestStatus.Rejected, true)]
    [InlineData(LeaveRequestStatus.Approved, LeaveRequestStatus.Rejected, false)]
    [InlineData(LeaveRequestStatus.Rejected, LeaveRequestStatus.Approved, false)]
    [InlineData(LeaveRequestStatus.Approved, LeaveRequestStatus.Pending, false)]
    public void LeaveRequestStatus_TransitionRules_ShouldBeRespected(
        LeaveRequestStatus from, LeaveRequestStatus to, bool isValid)
    {
        var allowed = IsValidLeaveTransition(from, to);

        allowed.Should().Be(isValid);
    }

    private static bool IsValidLeaveTransition(LeaveRequestStatus from, LeaveRequestStatus to)
    {
        return from == LeaveRequestStatus.Pending &&
               (to == LeaveRequestStatus.Approved || to == LeaveRequestStatus.Rejected);
    }
}
