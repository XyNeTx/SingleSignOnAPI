using Microsoft.EntityFrameworkCore;

namespace SingleSignOnAPI
{
    [PrimaryKey(nameof(F_System_Name), nameof(F_UserID), nameof(F_Host_Client), nameof(F_IPAddress),
        nameof(F_Update))]
    public class T_SQL_License
    {
        public string F_System_Name { get; set; }
        public string F_UserID { get; set; }
        public string F_Host_Client { get; set; }
        public string F_IPAddress { get; set; }
        public DateTime F_Update { get; set; }
    }
}
