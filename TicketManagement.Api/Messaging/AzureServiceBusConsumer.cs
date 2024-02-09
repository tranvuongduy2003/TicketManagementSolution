using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Messaging.IMessaging;
using TicketManagement.Api.Services;

namespace TicketManagement.Api.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string emailTicketQueue;
    private readonly string registerUserQueue;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private ServiceBusProcessor _emailTicketProcessor;
    private ServiceBusProcessor _registerUserProcessor;

    public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
    {
        _emailService = emailService;
        _configuration = configuration;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        
        emailTicketQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailTicketQueue");
        registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _emailTicketProcessor = client.CreateProcessor(emailTicketQueue);
        _registerUserProcessor = client.CreateProcessor(registerUserQueue);
    }

    public async Task Start()
    {
        _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
        _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
        await _registerUserProcessor.StartProcessingAsync();
        
        _emailTicketProcessor.ProcessMessageAsync += OnEmailTicketRequestReceived;
        _emailTicketProcessor.ProcessErrorAsync += ErrorHandler;
        await _emailTicketProcessor.StartProcessingAsync();
    }


    public async Task Stop()
    {
        await _registerUserProcessor.StopProcessingAsync();
        await _registerUserProcessor.DisposeAsync();
        
        await _emailTicketProcessor.StopProcessingAsync();
        await _emailTicketProcessor.DisposeAsync();
    }
    
    private async Task OnEmailTicketRequestReceived(ProcessMessageEventArgs args)
    {
        //this is where you will receive message
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        ValidateStripeResponseDto objMessage = JsonConvert.DeserializeObject<ValidateStripeResponseDto>(body);
        try
        {
            //TODO - try to log email
            await _emailService.ValidateTicketEmailAndLog(objMessage);  
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex) {
            throw;
        }

    }

    private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        string email = JsonConvert.DeserializeObject<string>(body);
        try
        {
            //TODO - try to log email
            await _emailService.RegisterUserEmailAndLog(email);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}