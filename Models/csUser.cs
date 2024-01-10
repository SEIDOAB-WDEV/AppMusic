using System;
using Microsoft.AspNetCore.Identity;

namespace Models
{
	public class csUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public csUser()
		{
		}
	}
}

