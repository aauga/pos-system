namespace Application.Common.Attributes;

public class SummaryAttribute : Attribute
{
    public SummaryAttribute(string summary)
    {
        Summary = summary;
    }

    public string Summary { get; set; }
}