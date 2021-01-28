using Newtonsoft.Json;
using System;

namespace Routing.Producer
{
  public class Input
  {
    [JsonProperty("transaction")]
    public AuthorizedTransaction Transaction { get; set; }
  }

  public class AuthorizedTransaction
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("order_id")]
    public Guid OrderId { get; set; }
  }
}
