using FluentValidation;
using requests;

namespace HttpTriggerCSharpSample.Validators
{
    public class RequestValidator : AbstractValidator<TriggerRequest<TriggerPipelineRequestBody>>
    {
        public RequestValidator()
        {
            RuleFor(x => x.TriggerData.Environment.SubscriptionName).NotEmpty();
            RuleFor(x => x.TriggerData.Environment.ResourceGroupName).NotEmpty().WithMessage("Please specify ResourceGroupName");
        }
    }
}