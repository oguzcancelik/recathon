using FluentValidation;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Models.Requests;

namespace SpotifyGateway.Infrastructure.Validations
{
    public class UpdateSettingsRequestValidator : AbstractValidator<UpdateSettingsRequest>
    {
        public UpdateSettingsRequestValidator()
        {
            RuleFor(x => x.SettingsClass)
                .IsInEnum()
                .WithMessage(ErrorConstants.UnexpectedError);
        }
    }
}