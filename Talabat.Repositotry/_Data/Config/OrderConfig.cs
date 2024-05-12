using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Repositotry._Data.Config
{
	public class OrderConfig : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.Property(O=>O.Status).HasConversion(OStatus=>OStatus.ToString() , OStatus=> (OrderStatus) Enum.Parse(typeof(OrderStatus), OStatus));

			builder.Property(O => O.SubTotal).HasColumnType("decimal(18,2)");

			//to map Address With Order IN ONE TABLE
			builder.OwnsOne(o => o.ShippingAddress, SA => SA.WithOwner());
			builder.HasOne(O => O.DeliveryMethod)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);

		}
	}
}
