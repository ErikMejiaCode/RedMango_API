namespace RedMango_API.Models.Dto
{
    public class OrderHeaderUpdateDTO
    {
        public int OrderHeaderId { get; set; }
        public string PickupName { get; set; }
        public string PickupPhoneNumber { get; set; }
        public string PickupEmail { get; set; }

        public DateTime OrderDate { get; set; }
        public string StripedPaymentIntentId { get; set; }
        public string Status { get; set; }
    }
}