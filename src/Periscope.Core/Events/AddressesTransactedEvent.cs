namespace Periscope.Core.Events
{
    public class AddressesTransactedEvent
    {
        public string[] Addresses { get; set; }

        public static AddressesTransactedEvent Create(string[] addresses)
        {
            return new() {Addresses = addresses};
        }
    }
}
