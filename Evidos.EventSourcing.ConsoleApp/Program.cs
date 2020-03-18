using Evidos.EventSourcing.Domain.Core.User;
using Evidos.EventSourcing.Domain.PubSub.Abstractions;
using Evidos.EventSourcing.Domain.Storage.Abstractions;
using Evidos.EventSourcing.Domain.Storage.EventStore;
using Evidos.EventSourcing.Domain.Storage.EventStore.Abstractions;
using Evidos.EventSourcing.EventHandling.Handlers;
using Evidos.EventSourcing.EventHandling.Handlers.Abstractions;
using Evidos.EventSourcing.EventHandling.PubSub;
using Evidos.EventSourcing.EventHandling.Services;
using Evidos.EventSourcing.EventHandling.Services.Abstractions;
using Evidos.EventSourcing.Query.Abstractions;
using Evidos.EventSourcing.Query.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.EventSourcing.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {  
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
   
            serviceProvider.GetService<ConsoleApplication>().Run();
        }
        
        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            
            services.AddTransient<ConsoleApplication>();
            
            services.AddTransient<IPublisher, DomainEventHub>();
            services.AddTransient<ISubscriber, DomainEventHub>();
            
            services.AddSingleton<IEventStore, EventStore.InMemory.EventStore>();
            services.AddSingleton<IAggregateRepository<User>, EventStoreAggregateRepository<User>>();
            services.AddSingleton<IEntityRepository<Query.Entities.User>, EntityStore<Query.Entities.User>>();

            services.AddTransient<IDomainEventHandler<UserSubmittedEvent>, UserHandler>();
            services.AddTransient<IDomainEventHandler<UserUpdatedEvent>, UserHandler>();
            services.AddTransient<IDomainEventHandler<UserDeletedEvent>, UserHandler>();
            services.AddTransient<IDomainEventHandler<UserVerifiedEvent>, UserHandler>();

            services.AddTransient<IUserWriter, UserWriter>();
            services.AddTransient<IUserReader, UserReader>();

            return services;
        }
    }
}
