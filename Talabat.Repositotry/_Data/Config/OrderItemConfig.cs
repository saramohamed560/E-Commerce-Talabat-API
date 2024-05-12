using Microsoft.Data.SqlClient;
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
	public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
	{
		public void Configure(EntityTypeBuilder<OrderItem> builder)
		{
			builder.Property(O => O.Price).HasColumnType("decimal(18,2)");
			builder.OwnsOne(O => O.Product, p => p.WithOwner());
		}
	}
}
