﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Services.Orders.Infrastructure;

#nullable disable

namespace Services.Orders.Infrastructure.Migrations
{
    [DbContext(typeof(OrderDbContext))]
    [Migration("20231213135841_ChangedRelatioshipShippingMethodCountriesToManyToMany")]
    partial class ChangedRelatioshipShippingMethodCountriesToManyToMany
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CountryShippingMethod", b =>
                {
                    b.Property<string>("CountriesName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ShippingMethodName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CountriesName", "ShippingMethodName");

                    b.HasIndex("ShippingMethodName");

                    b.ToTable("CountryShippingMethod");
                });

            modelBuilder.Entity("Services.Orders.Domain.ClientDiscountCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UsageDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("Code");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("ClientDiscountCodes");
                });

            modelBuilder.Entity("Services.Orders.Domain.ClientShippingData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ShippingFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShippingLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ClientShippingData");
                });

            modelBuilder.Entity("Services.Orders.Domain.Country", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Name");

                    b.ToTable("ShippingCountries");
                });

            modelBuilder.Entity("Services.Orders.Domain.DiscountCode", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("AssociatedClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MaxUsage")
                        .HasColumnType("int");

                    b.Property<int>("MaxUsagePerClient")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfUses")
                        .HasColumnType("int");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime2");

                    b.Property<double>("Value")
                        .HasMaxLength(4)
                        .HasColumnType("float");

                    b.HasKey("Code");

                    b.ToTable("DiscountCodes");
                });

            modelBuilder.Entity("Services.Orders.Domain.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CurrentOrderStatusId")
                        .HasColumnType("int");

                    b.Property<string>("DiscountCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HistoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentMethodName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("ShippingFees")
                        .HasMaxLength(2000)
                        .HasColumnType("float");

                    b.Property<string>("ShippingFirstName")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("ShippingLastName")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("ShippingMethod")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ShippingMethodName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("SubTotal")
                        .HasColumnType("float");

                    b.Property<double>("TaxApplied")
                        .HasColumnType("float");

                    b.Property<double>("Total")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CurrentOrderStatusId")
                        .IsUnique();

                    b.HasIndex("HistoryId")
                        .IsUnique();

                    b.HasIndex("PaymentMethodName");

                    b.HasIndex("ShippingMethodName");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Services.Orders.Domain.OrderHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("OrderHistory");
                });

            modelBuilder.Entity("Services.Orders.Domain.OrderHistoryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ChangeDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderHistoryId")
                        .HasColumnType("int");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OrderStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderHistoryId");

                    b.ToTable("OrderHistoryItems");
                });

            modelBuilder.Entity("Services.Orders.Domain.OrderItem", b =>
                {
                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("OrderId", "ProductId");

                    b.ToTable("OrderItem");
                });

            modelBuilder.Entity("Services.Orders.Domain.OrderStatus", b =>
                {
                    b.Property<string>("Alias")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Alias");

                    b.ToTable("OrderStatus");
                });

            modelBuilder.Entity("Services.Orders.Domain.PaymentMethod", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Name");

                    b.ToTable("PaymentMethods");
                });

            modelBuilder.Entity("Services.Orders.Domain.ShippingMethod", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("ApplicableFees")
                        .HasColumnType("float");

                    b.HasKey("Name");

                    b.ToTable("ShippingMethods");
                });

            modelBuilder.Entity("CountryShippingMethod", b =>
                {
                    b.HasOne("Services.Orders.Domain.Country", null)
                        .WithMany()
                        .HasForeignKey("CountriesName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Services.Orders.Domain.ShippingMethod", null)
                        .WithMany()
                        .HasForeignKey("ShippingMethodName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Services.Orders.Domain.ClientDiscountCode", b =>
                {
                    b.HasOne("Services.Orders.Domain.DiscountCode", "DiscountCode")
                        .WithMany()
                        .HasForeignKey("Code")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("Services.Orders.Domain.Order", "Order")
                        .WithOne("DiscountCodeApplied")
                        .HasForeignKey("Services.Orders.Domain.ClientDiscountCode", "OrderId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("DiscountCode");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Services.Orders.Domain.ClientShippingData", b =>
                {
                    b.OwnsOne("Services.Orders.Domain.Address", "ShippingAddress", b1 =>
                        {
                            b1.Property<Guid>("ClientShippingDataId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("CountryName")
                                .IsRequired()
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("ExtraDetails")
                                .HasMaxLength(1000)
                                .HasColumnType("nvarchar(1000)");

                            b1.Property<string>("HouseNumber")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.Property<double>("Latitude")
                                .HasColumnType("float");

                            b1.Property<string>("Locality")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("nvarchar(1000)");

                            b1.Property<double>("Longitude")
                                .HasColumnType("float");

                            b1.Property<string>("MajorDivision")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("nvarchar(1000)");

                            b1.Property<string>("MajorDivisionCode")
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.Property<string>("MinorDivision")
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("nvarchar(1000)");

                            b1.Property<string>("StreetNumber")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.HasKey("ClientShippingDataId");

                            b1.HasIndex("CountryName");

                            b1.ToTable("ClientShippingData");

                            b1.WithOwner()
                                .HasForeignKey("ClientShippingDataId");

                            b1.HasOne("Services.Orders.Domain.Country", "Country")
                                .WithMany()
                                .HasForeignKey("CountryName")
                                .OnDelete(DeleteBehavior.ClientNoAction)
                                .IsRequired();

                            b1.Navigation("Country");
                        });

                    b.Navigation("ShippingAddress")
                        .IsRequired();
                });

            modelBuilder.Entity("Services.Orders.Domain.Order", b =>
                {
                    b.HasOne("Services.Orders.Domain.OrderHistoryItem", "CurrentOrderStatus")
                        .WithOne()
                        .HasForeignKey("Services.Orders.Domain.Order", "CurrentOrderStatusId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("Services.Orders.Domain.OrderHistory", "History")
                        .WithOne("Order")
                        .HasForeignKey("Services.Orders.Domain.Order", "HistoryId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("Services.Orders.Domain.PaymentMethod", "PaymentMethod")
                        .WithMany()
                        .HasForeignKey("PaymentMethodName")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("Services.Orders.Domain.ShippingMethod", "Shipping")
                        .WithMany()
                        .HasForeignKey("ShippingMethodName")
                        .OnDelete(DeleteBehavior.ClientNoAction);

                    b.OwnsOne("Services.Orders.Domain.Address", "ShippingAddress", b1 =>
                        {
                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("CountryName")
                                .IsRequired()
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("ExtraDetails")
                                .HasMaxLength(1000)
                                .HasColumnType("nvarchar(1000)");

                            b1.Property<string>("HouseNumber")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.Property<double>("Latitude")
                                .HasColumnType("float");

                            b1.Property<string>("Locality")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("nvarchar(1000)");

                            b1.Property<double>("Longitude")
                                .HasColumnType("float");

                            b1.Property<string>("MajorDivision")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("nvarchar(1000)");

                            b1.Property<string>("MajorDivisionCode")
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.Property<string>("MinorDivision")
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("nvarchar(1000)");

                            b1.Property<string>("StreetNumber")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("N/A");

                            b1.HasKey("OrderId");

                            b1.HasIndex("CountryName");

                            b1.ToTable("Orders");

                            b1.HasOne("Services.Orders.Domain.Country", "Country")
                                .WithMany()
                                .HasForeignKey("CountryName")
                                .OnDelete(DeleteBehavior.ClientNoAction)
                                .IsRequired();

                            b1.WithOwner()
                                .HasForeignKey("OrderId");

                            b1.Navigation("Country");
                        });

                    b.Navigation("CurrentOrderStatus");

                    b.Navigation("History");

                    b.Navigation("PaymentMethod");

                    b.Navigation("Shipping");

                    b.Navigation("ShippingAddress")
                        .IsRequired();
                });

            modelBuilder.Entity("Services.Orders.Domain.OrderHistoryItem", b =>
                {
                    b.HasOne("Services.Orders.Domain.OrderHistory", "OrderHistory")
                        .WithMany("Statuses")
                        .HasForeignKey("OrderHistoryId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("OrderHistory");
                });

            modelBuilder.Entity("Services.Orders.Domain.OrderItem", b =>
                {
                    b.HasOne("Services.Orders.Domain.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Services.Orders.Domain.Order", b =>
                {
                    b.Navigation("DiscountCodeApplied");

                    b.Navigation("Items");
                });

            modelBuilder.Entity("Services.Orders.Domain.OrderHistory", b =>
                {
                    b.Navigation("Order")
                        .IsRequired();

                    b.Navigation("Statuses");
                });
#pragma warning restore 612, 618
        }
    }
}
