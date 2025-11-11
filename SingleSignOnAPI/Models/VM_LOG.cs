namespace SingleSignOnAPI.Models
{
    public class VM_LOG
    {
        public string Status { get; set; }
        public string System { get; set; }
        public string User { get; set; }
        public string IP_Address { get; set; }
        public string? Device { get; set; }
        public string? Message { get; set; }
        //public DateTime DateTime { get; set; }
    }
}
