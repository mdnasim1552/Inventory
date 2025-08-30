using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Inventory.Data
{
    public class TwilioService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioPhoneNumber;

        public TwilioService(IConfiguration configuration)
        {
            var twilioConfig = configuration.GetSection("Twilio");
            _accountSid = twilioConfig["AccountSid"];
            _authToken = twilioConfig["AuthToken"];
            _twilioPhoneNumber = twilioConfig["PhoneNumber"];
        }

        public string SendSms(string toPhoneNumber, string message)
        {
            TwilioClient.Init(_accountSid, _authToken);

            var messageResource = MessageResource.Create(
                to: new PhoneNumber(toPhoneNumber),
                from: new PhoneNumber(_twilioPhoneNumber),
                body: message
            );

            return messageResource.Sid; // Return the message SID as confirmation
        }
    }
}
