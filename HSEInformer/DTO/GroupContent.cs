using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace HSEInformer.DTO
{
    class GroupContent
    {
        [JsonProperty("isAdministrator")]
        public bool IsAdministrator { get; set; }

        [JsonProperty("posts")]
        public List<Post> Posts { get; set; }

        [JsonProperty("members")]
        public List<User> Members { get; set; }

    }
}