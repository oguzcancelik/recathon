namespace SpotifyGateway.Models.Enums
{
    public enum Operation
    {
        Equal,
        NotEqual,
        More,
        MoreOrEqual,
        Less,
        LessOrEqual,
        In,
        NotIn,
        Contains
    }

    public enum SelectType
    {
        Field,
        Distinct,
        Count,
        Avg,
        Sum
    }

    public enum SortType
    {
        Asc,
        Desc,
        Random
    }
}