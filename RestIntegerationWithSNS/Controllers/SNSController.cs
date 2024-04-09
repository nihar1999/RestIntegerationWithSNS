﻿using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestIntegerationWithSNS.Model;
using System.Net;
using System.Runtime.CompilerServices;

namespace RestIntegerationWithSNS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SNSController : ControllerBase
    {
        private readonly ILogger<SNSController> _logger;
        public SNSController(ILogger<SNSController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<string> Index([FromBody] string request)
        {
            SNSMessageModel snsMessage = JsonConvert.DeserializeObject<SNSMessageModel>(request);
            var headers = Request.Headers;
            var headerValue = Request.Headers["x-amz-sns-message-type"];
            Console.WriteLine($"Header value in request is : {headerValue}");

            //Confirm the subscription
            if (headerValue == "SubscriptionConfirmation")
            {

                string confirmationUrl = snsMessage.SubscribeURL;
                // Log the confirmation URL
                Console.WriteLine($"Received confirmation message. Confirmation URL: {confirmationUrl}");
                //Send a request to the confirmation URL to confirm the subscription
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(confirmationUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Subscription confirmed successfully
                        Console.WriteLine($"Subscription confirmation Done. HTTP Status Code: {response.StatusCode}");
                        return "Subscription confirmed";
                    }
                    else
                    {
                        // Handle the case where subscription confirmation failed
                        
                        Console.WriteLine($"Subscription confirmation failed. HTTP Status Code: {response.StatusCode}");
                        return "Subscription confirmation failed";
                    }
                }
            }
            //Notification 
            else
            {
                Console.WriteLine("The recieved message : {0}", snsMessage.Message);
                return snsMessage.Message;
            }

        }
    }
}