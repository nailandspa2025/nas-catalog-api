using BuildingBlocks.EventBus;
using BuildingBlocks.EventBus.Events;
using Catalog.Application.Features.UserStores.Commands.SentUserToStore;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;

namespace Catalog.Api.Consumer
{
    public class SentUserToStoreEventConsumerDefinition : ConsumerDefinition<SentUserToStoreEventConsumer>
    {
        public SentUserToStoreEventConsumerDefinition(IOptions<AwsBusOptions> options)
        {
            // override the default endpoint name, for whatever reason
            EndpointName = options.Value.Topic.IdentityUserToStore;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<SentUserToStoreEventConsumer> consumerConfigurator)
        {
            // endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
    public class SentUserToStoreEventConsumer : IConsumer<UserToStoreEvent>
    {
        private readonly ILogger<SentUserToStoreEventConsumer> _logger;
        private readonly IMediator _mediator;

        public SentUserToStoreEventConsumer(ILogger<SentUserToStoreEventConsumer> logger, IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<UserToStoreEvent> context)
        {
            _logger.LogInformation("Consume Message");

            var data = context.Message;

            var command = new SentUserToStoreCommand
            {
                UserId = data.UserId,
                StoreIds = data.StoreIds
            };
            await _mediator.Send(command);

            await Task.CompletedTask;
        }
    }
}

