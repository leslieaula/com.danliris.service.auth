namespace Com.DanLiris.Service.Auth.Lib.Models
{
    public class AccountProfile
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }

        public int AccountId { get; set; }
        public virtual Account Account { get; set; }
    }
}
