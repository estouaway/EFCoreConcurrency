﻿// <auto-generated />
using System;
using EFCoreConcurrency.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EFCoreConcurrency.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20210805153250_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5");

            modelBuilder.Entity("ConcurrencyControl.Models.ConcurrentWithRowVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("Timestamp")
                        .HasColumnType("BLOB");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<long>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ConcurrentWithRowVersion");
                });

            modelBuilder.Entity("ConcurrencyControl.Models.ConcurrentWithToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<long>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ConcurrentWithToken");
                });

            modelBuilder.Entity("ConcurrencyControl.Models.NotConcurrent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<long>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("NotConcurrent");
                });
#pragma warning restore 612, 618
        }
    }
}