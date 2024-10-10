﻿// <auto-generated />
using System;
using DZ_ChatApp_vSem6.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DZ_ChatApp_vSem6.Migrations
{
    [DbContext(typeof(ChatUdpContext))]
    partial class ChatUdpContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("DZ_ChatApp_vSem6.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("FromUserId")
                        .HasColumnType("int")
                        .HasColumnName("MessageFromUser");

                    b.Property<string>("Text")
                        .HasColumnType("longtext")
                        .HasColumnName("Text");

                    b.Property<int?>("ToUserId")
                        .HasColumnType("int")
                        .HasColumnName("MessageToUser");

                    b.Property<bool>("isReceived")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("isReceived");

                    b.HasKey("Id")
                        .HasName("message_pkey");

                    b.HasIndex("FromUserId");

                    b.HasIndex("ToUserId");

                    b.ToTable("Message", (string)null);
                });

            modelBuilder.Entity("DZ_ChatApp_vSem6.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("Name");

                    b.HasKey("Id")
                        .HasName("users_pkey");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("DZ_ChatApp_vSem6.Models.Message", b =>
                {
                    b.HasOne("DZ_ChatApp_vSem6.Models.User", "FromUser")
                        .WithMany("MessagesFromUser")
                        .HasForeignKey("FromUserId")
                        .HasConstraintName("message_FromUserId_fkey");

                    b.HasOne("DZ_ChatApp_vSem6.Models.User", "ToUser")
                        .WithMany("MessagesToUser")
                        .HasForeignKey("ToUserId")
                        .HasConstraintName("message_ToUserId_fkey");

                    b.Navigation("FromUser");

                    b.Navigation("ToUser");
                });

            modelBuilder.Entity("DZ_ChatApp_vSem6.Models.User", b =>
                {
                    b.Navigation("MessagesFromUser");

                    b.Navigation("MessagesToUser");
                });
#pragma warning restore 612, 618
        }
    }
}
