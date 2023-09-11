using System.ComponentModel;
using System;
using System.Linq;

namespace Todo_App.Domain.ValueObjects;
public class Status : ValueObject
{
    static Status()
    {
    }

    private Status()
    {
    }

    private Status(string status)
    {
        ForDeletionStatus = status;
    }

    public static Status From(string status)
    {
        var forDeletionStatus = new Status { ForDeletionStatus = status };

        if (!SupportedStatus.Contains(forDeletionStatus))
        {
            throw new UnsupportedColourException(forDeletionStatus);
        }

        return forDeletionStatus;
    }

    public static Status Yes => new("Yes");
    public static Status No => new("No");
    public static Status NA => new("Yes");

    public string ForDeletionStatus { get; private set; } = "No";

    public static implicit operator string(Status status)
    {
        return status.ToString();
    }

    public static explicit operator Status(string status)
    {
        return From(status);
    }

    public override string ToString()
    {
        return ForDeletionStatus;
    }

    protected static IEnumerable<Status> SupportedStatus
    {
        get
        {
            yield return Yes;
            yield return No;
            yield return NA;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ForDeletionStatus;
    }
}
