using System;

namespace DDxHub.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class DataIP
    {
        public string ip { get; set; }
    }

}
