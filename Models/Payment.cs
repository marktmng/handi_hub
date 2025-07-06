namespace DotnetAPI.Models
{
    public class Payment
    {
        public int? PaymentId { get; set; }
        public int OrderId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }

    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; }
    }
}
