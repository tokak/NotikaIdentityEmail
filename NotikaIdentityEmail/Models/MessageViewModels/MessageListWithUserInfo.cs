namespace NotikaIdentityEmail.Models.MessageViewModels
{
    public class MessageListWithUserInfo
    {
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public string MessageDetail { get; set; }
        public DateTime SendDate { get; set; }
    }
}
