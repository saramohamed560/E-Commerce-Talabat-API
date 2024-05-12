using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Repositotry._Data.Config
{
	public class DeliveryMethodConfig : IEntityTypeConfiguration<DeliveryMethod>
	{
		public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
		{
			builder.Property(D => D.Cost).HasColumnType("decimal(18,2)");
		}
	}
}
