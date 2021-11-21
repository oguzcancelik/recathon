namespace SpotifyGateway.Models.Resources.Abstractions
{
    public interface IErrorResources : IResources
    {
        string UserNotFound { get; set; }
    }
}