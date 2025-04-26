using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;

namespace POS1.Data
{
    public class Supplier
    {

        public int Id { get; set; }  

        public int TenantId { get; set; }  

        public string Name { get; set; } 

        public string Address { get; set; } 

        public string ContactAgent { get; set; }

        public string Phone { get; set; } 

        public string? Email { get; set; }

        // Navigation property for the Tenant
        public Tenant Tenant { get; set; }


    }
}
