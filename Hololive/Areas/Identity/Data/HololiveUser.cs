using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hololive.Areas.Identity.Data;

// Add profile data for application users by adding properties to the HololiveUser class
public class HololiveUser : IdentityUser
{
    [PersonalData]
    public string CustomerName { get; set; }

    [PersonalData]
    public string CustomerAddress { get; set; }

    [PersonalData]
    public int CustomerAge { get; set; }

    public DateTime CustomerDOB { get; set; }
}

