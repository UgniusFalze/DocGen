﻿// <auto-generated />
using System;
using DocsManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DocsManager.Migrations
{
    [DbContext(typeof(DocsManagementContext))]
    [Migration("20240613183108_IsPayed")]
    partial class IsPayed
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DocsManager.Models.Client", b =>
                {
                    b.Property<int>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ClientId"));

                    b.Property<string>("BuyerAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BuyerCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BuyerName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VatCode")
                        .HasColumnType("text");

                    b.HasKey("ClientId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("DocsManager.Models.Invoice", b =>
                {
                    b.Property<int>("InvoiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("InvoiceId"));

                    b.Property<int>("InvoiceClientId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("InvoiceDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("InvoiceUserId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsPayed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<int>("SeriesNumber")
                        .HasColumnType("integer");

                    b.HasKey("InvoiceId");

                    b.HasIndex("InvoiceClientId");

                    b.HasIndex("InvoiceUserId", "SeriesNumber")
                        .IsUnique();

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("DocsManager.Models.InvoiceItem", b =>
                {
                    b.Property<int>("InvoiceItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("InvoiceItemId"));

                    b.Property<int>("InvoiceId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("PriceOfUnit")
                        .HasColumnType("numeric");

                    b.Property<string>("UnitOfMeasurement")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Units")
                        .HasColumnType("integer");

                    b.HasKey("InvoiceItemId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceItems");
                });

            modelBuilder.Entity("DocsManager.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BankNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FreelanceWorkId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PersonalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DocsManager.Models.docs.Template", b =>
                {
                    b.Property<int>("TemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TemplateId"));

                    b.Property<string>("HtmlString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TemplateModel")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TemplateId");

                    b.HasIndex("TemplateModel")
                        .IsUnique();

                    b.ToTable("Templates", "docs");
                });

            modelBuilder.Entity("DocsManager.Models.Invoice", b =>
                {
                    b.HasOne("DocsManager.Models.Client", "InvoiceClient")
                        .WithMany()
                        .HasForeignKey("InvoiceClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DocsManager.Models.User", "InvoiceUser")
                        .WithMany()
                        .HasForeignKey("InvoiceUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InvoiceClient");

                    b.Navigation("InvoiceUser");
                });

            modelBuilder.Entity("DocsManager.Models.InvoiceItem", b =>
                {
                    b.HasOne("DocsManager.Models.Invoice", null)
                        .WithMany("Items")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DocsManager.Models.Invoice", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
