using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Auth.Service.Business.Logging
{
	public static class EventIds
	{
		public static class Information
		{
			public static EventId ServerStarted { get; } = new EventId(100, nameof(ServerStarted));
		}

		public static class Warning
		{
			public static EventId WarningEvent { get; } = new EventId(200, nameof(WarningEvent));
		}

		public static class Error
		{
			public static EventId DatabaseError { get; } = new EventId(301, nameof(DatabaseError));

			public static EventId UserManagementError { get; } = new EventId(302, nameof(UserManagementError));
		}

		public static class Critical
		{
			public static EventId UnhandledException { get; } = new EventId(300, nameof(UnhandledException));
		}
	}
}
