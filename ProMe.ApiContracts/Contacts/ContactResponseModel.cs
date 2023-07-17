using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.ApiContracts.Contacts;
public sealed class ContactResponseModel
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}
