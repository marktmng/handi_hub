namespace DotnetAPI.Dtos
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string MethodName { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
