namespace SingleSignOnAPI
{
    public class VM_CmdPostData
    {
        public string hostName { get; set; }
        public string userName { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
