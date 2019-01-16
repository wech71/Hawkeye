using System;
using System.Security.Principal;

namespace ACorns.Hawkeye
{
	internal class CustomPrincipal : IPrincipal
	{
		private string definedRoles = string.Empty;
		public CustomIdentity identity = new CustomIdentity();
		private bool useManualRoles = false;

		public bool IsInRole(string role)
		{
			if (this.useManualRoles)
			{
				return this.definedRoles.Contains(role);
			}
			return true;
		}

		public string DefinedRoles
		{
			get
			{
				return this.definedRoles;
			}
			set
			{
				this.definedRoles = value;
			}
		}

		public IIdentity Identity
		{
			get
			{
				return this.identity;
			}
		}

		public bool UseManualRoles
		{
			get
			{
				return this.useManualRoles;
			}
			set
			{
				this.useManualRoles = value;
			}
		}
	}
}

