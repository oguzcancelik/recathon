using FluentValidation;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Models.Requests;

namespace SpotifyGateway.Infrastructure.Validations
{
    public class GeneratePlaylistRequestValidator : AbstractValidator<GeneratePlaylistRequest>
    {
        public GeneratePlaylistRequestValidator()
        {
            RuleFor(x => x.PlaylistId)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(ErrorConstants.PlaylistIdNotFound);

            RuleFor(x => x.PlaylistId)
                .Must(x => !x.Contains('-'))
                .WithMessage(ErrorConstants.InvalidPlaylistId);

            RuleFor(x => x.EncryptedToken)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(ErrorConstants.InvalidRequest);
        }
    }
}