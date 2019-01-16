using System;
using System.Security.Principal;
using System.Threading;

namespace ACorns.Hawkeye
{
	internal class CustomIdentity : IIdentity
	{
		private string authenticationType;
		private bool isAuthenticated;
		private string name;

		public CustomIdentity()
		{
			if ((Thread.CurrentPrincipal != null) && (Thread.CurrentPrincipal.Identity != null))
			{
				this.name = Thread.CurrentPrincipal.Identity.Name;
				this.authenticationType = Thread.CurrentPrincipal.Identity.AuthenticationType;
			}
			this.isAuthenticated = true;
		}

		public string AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
			set
			{
				this.authenticationType = value;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return this.isAuthenticated;
			}
			set
			{
				this.isAuthenticated = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}
	}
}
