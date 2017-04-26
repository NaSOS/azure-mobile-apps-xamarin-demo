using System;
using Microsoft.WindowsAzure.MobileServices;

namespace GlobalAzureBootcamp2017
{
	/// <summary>
	/// Base model that includes the default EasyTable fields
	/// </summary>
	public abstract class BaseModel
	{
		public string Id { get; set; }

		[UpdatedAt]
		public DateTime? UpdatedAt { get; set; }

		[CreatedAt]
		public DateTime? CreatedAt { get; set; }

		[Deleted]
		public bool Deleted { get; set; }

		[Version]
		public string Version { get; set; }
	}
}
